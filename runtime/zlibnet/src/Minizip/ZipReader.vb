Imports System.Collections
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language

Public Class ZipReader
    Implements IEnumerable(Of ZipEntry)
    Implements IDisposable
    ''' <summary>ZipFile handle to read data from.</summary>
    Private _handle As IntPtr = IntPtr.Zero

    ''' <summary>Name of zip file.</summary>
    Private _fileName As String = Nothing

    ''' <summary>Global zip file comment.</summary>
    Private _comment As String = Nothing

    ''' <summary>Current zip entry open for reading.</summary>
    Private _current As ZipEntry = Nothing

    ''' <summary>Initializes a instance of the <see cref="ZipReader"/> class for reading the zip file with the given name.</summary>
    ''' <param name="fileName">The name of zip file that will be read.</param>
    Public Sub New(fileName As String)
        _fileName = fileName
        _handle = Minizip.unzOpen(fileName)
        If _handle = IntPtr.Zero Then
            Dim msg As String = [String].Format("Could not open zip file '{0}'.", fileName)
            Throw New ZipException(msg)
        End If
    End Sub

    ''' <summary>Cleans up the resources used by this zip file.</summary>
    Protected Overrides Sub Finalize()
        Try
            CloseFile()
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    ''' <remarks>Dispose is synonym for Close.</remarks>
    Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
        Close()
    End Sub

    ''' <summary>Closes the zip file and releases any resources.</summary>
    Public Sub Close()
        ' Free unmanaged resources.
        CloseFile()

        ' If base type implements IDisposable we would call it here.

        ' Request the system not call the finalizer method for this object.
        GC.SuppressFinalize(Me)
    End Sub

    ''' <summary>Gets the name of the zip file that was passed to the constructor.</summary>
    Public ReadOnly Property Name() As String
        Get
            Return _fileName
        End Get
    End Property

    ''' <summary>Gets the global comment for the zip file.</summary>
    Public ReadOnly Property Comment() As String
        Get
            If _comment Is Nothing Then
                Dim info As New ZipFileInfo()
                Dim result As Integer = Minizip.unzGetGlobalInfo(_handle, info)
                If result < 0 Then
                    Dim msg As String = [String].Format("Could not read comment from zip file '{0}'.", Name)
                    Throw New ZipException(msg, result)
                End If

                Dim buffer As Byte() = New Byte(info.CommentLength - 1) {}
                result = Minizip.unzGetGlobalComment(_handle, buffer, CUInt(buffer.Length))
                If result < 0 Then
                    Dim msg As String = [String].Format("Could not read comment from zip file '{0}'.", Name)
                    Throw New ZipException(msg, result)
                End If

                'file comment is for some weird reason ANSI, while entry name + comment is OEM...

                _comment = Encoding.[Default].GetString(buffer)
            End If
            Return _comment
        End Get
    End Property

    Private ReadOnly Property Current() As ZipEntry
        Get
            Return _current
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator(Of ZipEntry) Implements IEnumerable(Of ZipEntry).GetEnumerator
        ' Will protect agains most common case, but if someone gets two enumerators up front and uses them,
        ' we wont catch it.
        If _current IsNot Nothing Then
            Throw New InvalidOperationException("Entry already open/enumeration already in progress")
        End If
        Return New ZipEntryEnumerator(Me)
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    ''' <summary>Advances the enumerator to the next element of the collection.
    ''' Sets <see cref="Current"/> to the next zip entry.</summary>
    ''' <returns><c>true</c> if the next entry is not <c>null</c>; otherwise <c>false</c>.</returns>
    Private Function MoveNext() As Boolean

        Dim result As Integer
        If _current Is Nothing Then
            result = Minizip.unzGoToFirstFile(_handle)
        Else
            CloseCurrentEntry()
            result = Minizip.unzGoToNextFile(_handle)
        End If

        If result = ZipReturnCode.EndOfListOfFile Then
            ' no more entries
            _current = Nothing
        ElseIf result < 0 Then
            Throw New ZipException("MoveNext failed.", result)
        Else
            ' entry found
            OpenCurrentEntry()
        End If

        Return (_current IsNot Nothing)
    End Function

    ''' <summary>Move to just before the first entry in the zip directory.</summary>
    Private Sub Reset()
        CloseCurrentEntry()
    End Sub

    Private Sub CloseCurrentEntry()
        If _current IsNot Nothing Then
            Dim result As Integer = Minizip.unzCloseCurrentFile(_handle)
            If result < 0 Then
                Throw New ZipException("Could not close zip entry.", result)
            End If
            _current = Nothing
        End If
    End Sub

    Private Sub OpenCurrentEntry()
        _current = New ZipEntry(_handle)
        Dim result As Integer = Minizip.unzOpenCurrentFile(_handle)
        If result < 0 Then
            _current = Nothing
            Throw New ZipException("Could not open entry for reading.", result)
        End If
    End Sub

    ''' <summary>Uncompress a block of bytes from the current zip entry and writes the data in a given buffer.</summary>
    ''' <param name="buffer">The array to write data into.</param>
    ''' <param name="index">The byte offset in <paramref name="buffer"/> at which to begin writing.</param>
    ''' <param name="count">The maximum number of bytes to read.</param>
    Public Function Read(buffer As Byte(), index As Integer, count As Integer) As Integer
        Using fixedBuff As New FixedArray(buffer)
            Dim bytesRead As Integer = Minizip.unzReadCurrentFile(_handle, fixedBuff(index), CUInt(count))
            If bytesRead < 0 Then
                Throw New ZipException("Error reading zip entry.", bytesRead)
            End If
            Return bytesRead
        End Using
    End Function

    Public Sub Read(writer As Stream)
        Dim i As int = 0
        Dim buff As Byte() = New Byte(4095) {}

        While (i = Me.Read(buff, 0, buff.Length)) > 0
            writer.Write(buff, 0, i)
        End While
    End Sub

    Private Sub CloseFile()
        If _handle <> IntPtr.Zero Then
            Try
                CloseCurrentEntry()
            Finally
                Dim result As Integer = Minizip.unzClose(_handle)
                If result < 0 Then
                    Throw New ZipException("Could not close zip file.", result)
                End If
                _handle = IntPtr.Zero
            End Try
        End If
    End Sub

    Private Class ZipEntryEnumerator
        Implements IEnumerator(Of ZipEntry)
        Private pReader As ZipReader
        Public Sub New(zr As ZipReader)
            pReader = zr
        End Sub
        Public ReadOnly Property Current() As ZipEntry Implements IEnumerator(Of ZipEntry).Current
            Get
                Return pReader._current
            End Get
        End Property
        Public Sub Dispose() Implements IDisposable.Dispose
            pReader.CloseCurrentEntry()
        End Sub
        Private ReadOnly Property IEnumerator_Current() As Object Implements IEnumerator.Current
            Get
                Return Current
            End Get
        End Property
        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            Return pReader.MoveNext()
        End Function
        Public Sub Reset() Implements IEnumerator.Reset
            pReader.Reset()
        End Sub
    End Class
End Class

'public class ZipEntryCollection : List<ZipEntry>
'{
'    public ZipEntryCollection()
'    {
'    }
'    public ZipEntryCollection(IEnumerable<ZipEntry> entries)
'        : base(entries)
'    {
'    }
'}

Imports System.Runtime.Serialization
Imports System.Text
Imports System.Diagnostics
Imports System.IO



Public Class ZipWriter
	Implements IDisposable

	'public CompressionMethod Method = CompressionMethod.Deflated;
	'public CompressionLevel Compression = CompressionLevel.Average;

	'		CompressionMethod _method = CompressionMethod.Deflated;
	'		int _level = (int)CompressionLevel.Default;


	''' <summary>Name of the zip file.</summary>
	Private _fileName As String

	''' <summary>Zip file global comment.</summary>
	Private _comment As String = ""

	''' <summary>Current zip entry open for write.</summary>
	Private _current As ZipEntry = Nothing

	''' <summary>Zip file handle.</summary>
	Private _handle As IntPtr = IntPtr.Zero

	''' <summary>Initializes a new instance fo the <see cref="ZipWriter"/> class with a specified file name.  Any Existing file will be overwritten.</summary>
	''' <param name="fileName">The name of the zip file to create.</param>
	Public Sub New(fileName As String)
		_fileName = fileName

			' false = create new, true = append 
		_handle = Minizip.zipOpen(fileName, False)
		If _handle = IntPtr.Zero Then
			Dim msg As String = [String].Format("Could not open zip file '{0}' for writing.", fileName)
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

	''' <summary>Gets the name of the zip file.</summary>
	Public ReadOnly Property Name() As String
		Get
			Return _fileName
		End Get
	End Property

	''' <summary>Gets and sets the zip file comment.</summary>
	Public Property Comment() As String
		Get
			Return _comment
		End Get
		Set
			_comment = value
		End Set
	End Property

	''' <summary>Creates a new zip entry in the directory and positions the stream to the start of the entry data.</summary>
	''' <param name="entry">The zip entry to be written.</param>
	''' <remarks>Closes the current entry if still active.</remarks>
	Public Sub AddEntry(entry As ZipEntry)
		'Close previous entry (if any).
		'Will trigger write of central dir info for previous file and may throw.
		CloseCurrentEntry()

		Dim info As New ZipFileEntryInfo()
		info.ZipDateTime = entry.ModifiedTime
		info.ExternalFileAttributes = CUInt(entry.GetFileAttributesForZip())

		Dim extra As Byte() = Nothing
		Dim extraLength As UInteger = 0
		If entry.ExtraField IsNot Nothing Then
			extra = entry.ExtraField
			extraLength = CUInt(entry.ExtraField.Length)
		End If

		Dim nameForZip As String = entry.GetNameForZip()

		Dim flagBase As UInteger = 0
		If entry.UTF8Encoding Then
			flagBase = flagBase Or ZipEntryFlag.UTF8
		Else
			If Not nameForZip.IsAscii() Then
				Throw New ArgumentException("Name can only contain Ascii 8 bit characters.")
			End If
			If entry.Comment IsNot Nothing AndAlso Not entry.Comment.IsAscii() Then
				Throw New ArgumentException("Comment can only contain Ascii 8 bit characters.")
			End If
		End If

		Dim encoding__1 As Encoding = If(entry.UTF8Encoding, Encoding.UTF8, Minizip.OEMEncoding)
		Dim name As Byte() = encoding__1.GetBytes(nameForZip & [Char].MinValue)
		' add nullterm
		Dim comment As Byte() = Nothing
		If entry.Comment IsNot Nothing Then
			comment = encoding__1.GetBytes(entry.Comment & [Char].MinValue)
		End If
		' add nullterm
		'null is ok here
		Dim result As Integer = Minizip.zipOpenNewFileInZip4_64(_handle, name, info, extra, extraLength, Nothing, _
			0, comment, CInt(entry.Method), entry.Level, flagBase, entry.Zip64)

		If result < 0 Then
			Throw New ZipException("AddEntry error.", result)
		End If

		_current = entry
	End Sub

    ''' <summary>Compress a block of bytes from the given buffer and writes them into the current zip entry.</summary>
    ''' <param name="buffer">The array to read data from.</param>
    ''' <param name="index">The byte offset in <paramref name="buffer"/> at which to begin reading.</param>
    ''' <param name="count">The maximum number of bytes to write.</param>
    Public Sub Write(buffer As Byte(), index As Integer, count As Integer)
		Using fixedBuffer As New FixedArray(buffer)
			Dim result As Integer = Minizip.zipWriteInFileInZip(_handle, fixedBuffer(index), CUInt(count))
			If result < 0 Then
				Throw New ZipException("Write error.", result)
			End If
		End Using
	End Sub

	Public Sub Write(reader As Stream)
		Dim i As Integer
		Dim buff As Byte() = New Byte(4095) {}
		While (InlineAssignHelper(i, reader.Read(buff, 0, buff.Length))) > 0
			Write(buff, 0, i)
		End While
	End Sub

	Private Sub CloseCurrentEntry()
		If _current IsNot Nothing Then
			Dim result As Integer = Minizip.zipCloseFileInZip(_handle)
			If result < 0 Then
				Throw New ZipException("Could not close entry.", result)
			End If
			_current = Nothing
		End If
	End Sub

	Private Sub CloseFile()
		If _handle <> IntPtr.Zero Then
			Try
				CloseCurrentEntry()
			Finally
				'file comment is for some weird reason ANSI, while entry name + comment is OEM...
				Dim result As Integer = Minizip.zipClose(_handle, _comment)
				If result < 0 Then
					Throw New ZipException("Could not close zip file.", result)
				End If
				_handle = IntPtr.Zero
			End Try
		End If
	End Sub
	Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
		target = value
		Return value
	End Function
End Class

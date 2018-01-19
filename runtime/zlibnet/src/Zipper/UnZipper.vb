Imports System.IO
Imports Microsoft.VisualBasic.Language

Public Class UnZipper
    Public ZipFile As String
    ''' <summary>
    ''' Tipper disse er fra rota??
    ''' ABC\JOG.* ABC\RUN.PCX *.HLP "SPORTS FANS\BASKETBALL.TIF"
    ''' PS: stien kan ikke ha wildcards
    ''' </summary>
    Public ItemList As New List(Of String)()
    Public Recurse As Boolean
    'how does it work? //good def?
    ''' <summary>
    ''' ONly valid for files. Dirs are always created and last writeTime is updated
    ''' </summary>
    Public IfFileExist As enIfFileExist = enIfFileExist.Exception
    'good def?
    Public NoDirectoryNames As Boolean
    Public Destination As String = Nothing
    Private buffer As Byte()

    Public Sub UnZip()
        If Destination Is Nothing Then
            Throw New ArgumentException("Destination is null")
        End If
        If ItemList.Count = 0 Then
            Throw New ArgumentException("ItemList is empty")
        End If
        'if (Filespecs.Count == 0)
        '    Filespecs.Add("*");

        Dim fileSpecs As New FileSpecMatcher(ItemList, Recurse)

        Dim unzippedSomeEntry As Boolean = False

        Using reader As New ZipReader(ZipFile)
            ' buffer to hold temp bytes
            buffer = New Byte(4095) {}

            For Each entry As ZipEntry In reader
                If fileSpecs.MatchSpecs(entry.Name, entry.IsDirectory) Then
                    If entry.IsDirectory Then
                        'FIXME: bør kanskje ha sjekk på om flere filer med samme navn havner på rota og overskriver hverandre?
                        If Not NoDirectoryNames Then
                            Dim dirName As String = CreateUnzippedName(entry)
                            Dim di As New DirectoryInfo(dirName)
                            If Not di.Exists Then
                                di.Create()
                            End If
                            SetLastWriteTimeFixed(di, entry.ModifiedTime)
                        End If
                    Else
                        Dim fileName As String = CreateUnzippedName(entry)
                        Dim fi As New FileInfo(fileName)
                        If Not fi.Directory.Exists Then
                            fi.Directory.Create()
                        End If

                        If fi.Exists Then
                            Select Case IfFileExist
                                Case enIfFileExist.Exception
                                    Throw New ZipException("File already exists: " & fileName)
                                Case enIfFileExist.Skip

                                Case enIfFileExist.Overwrite

                                Case Else
                                    'fall thru
                                    Throw New NotImplementedException("enIfFileExist " & Convert.ToString(IfFileExist))
                            End Select
                        End If

                        Using writer As FileStream = fi.Create()
                            Dim byteCount As int = 0

                            While (byteCount = reader.Read(buffer, 0, buffer.Length)) > 0
                                writer.Write(buffer, 0, byteCount)
                            End While
                        End Using

                        SetLastWriteTimeFixed(fi, entry.ModifiedTime)
                    End If

                    unzippedSomeEntry = True
                End If
            Next
        End Using

        If Not unzippedSomeEntry Then
            Throw New ZipException("No files to unzip")
        End If
    End Sub

    Private Sub SetLastWriteTimeFixed(fsi As FileSystemInfo, dt As DateTime)
        'http://www.codeproject.com/KB/files/csharpfiledate.aspx?msg=2885854#xx2885854xx
        Dim localOffset As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
        fsi.LastWriteTimeUtc = dt - localOffset
    End Sub

    Private Function CreateUnzippedName(entry As ZipEntry) As String
        Dim name As String = Nothing
        If NoDirectoryNames Then
            If entry.IsDirectory Then
                Throw New Exception("NoDirectoryNames but got dir")
            End If
            name = Path.GetFileName(entry.Name)
        Else
            name = entry.Name.TrimStartDirSep()
        End If
        'trim not requred since we dont use Path.Combine, but do it anyways
        'PS: don't use Path.Combine here! if name is absolute, it will override destination!
        'use Path.GetFullPath to normalize path. also it will give error if invalid chars in path
        'FIXME: figure out if other ziputils allow storing relative path's in zip (\..\..\test) and how they handle extraction
        'of such items.
        Dim unzippedName As String = Path.GetFullPath(Destination & Path.DirectorySeparatorChar & name)
        Dim ea As New CreateUnzippedNameEventArgs(unzippedName, entry.IsDirectory)
        OnCreateUnzippedName(ea)
        Return ea.UnzippedName
    End Function

    Private Sub OnCreateUnzippedName(ea As CreateUnzippedNameEventArgs)
        RaiseEvent CreateUnzippedNameEvent(Me, ea)
    End Sub

    Public Event CreateUnzippedNameEvent As CreateUnzippedNameEventHandler

End Class

Public Delegate Sub CreateUnzippedNameEventHandler(sender As Object, ea As CreateUnzippedNameEventArgs)
Public Class CreateUnzippedNameEventArgs
    Inherits EventArgs
    Public Property UnzippedName() As String
        Get
            Return m_UnzippedName
        End Get
        Set
            m_UnzippedName = Value
        End Set
    End Property
    Private m_UnzippedName As String
    Public Property IsDirectory() As Boolean
        Get
            Return m_IsDirectory
        End Get
        Private Set
            m_IsDirectory = Value
        End Set
    End Property
    Private m_IsDirectory As Boolean
    Public Sub New(unzippedName As String, isDirectory As Boolean)
        Me.UnzippedName = unzippedName
        Me.IsDirectory = isDirectory
    End Sub
End Class

Public Enum enIfFileExist
    Overwrite
    Skip
    Exception
End Enum

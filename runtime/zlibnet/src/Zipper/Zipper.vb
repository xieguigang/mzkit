Imports System.IO
Imports Microsoft.VisualBasic.Language

Public Class Zipper

    ''' <summary>
    ''' DynaZip default is false (and so are we)
    ''' </summary>
    Public Recurse As Boolean
    Public ZipFile As String
    ''' <summary>
    ''' More than 64k count zip entries supported in any case.
    ''' More than 4GB data per zip entry only supported with Zip64.
    ''' Zip's larger than 4GB is supporten in any case.
    ''' </summary>
    Public Zip64 As enZip64 = enZip64.Auto
    ''' <summary>
    ''' Use UTF8 for zip entry name/comment
    ''' </summary>
    Public UTF8Encoding As Boolean = True
    ''' <summary>
    ''' List of files, dirs etc FULL PATH. With wildcards.
    ''' 
    ''' Recurse = TRUE – only the beginning of the path specification of the item must match the path
    ''' specification of the filespec for the item to be selected. This allows items within the
    ''' Filespec path and in any of its subdirectories to be selected.
    ''' FALSE – the path specification of the item must match that of the filespec exactly
    ''' for the item to be selected. Items in any subdirectories of the filespec path are not
    ''' selected.
    '''                For example, assume that the filespec is ABC\*.C and the ZIP file contains two
    ''' items, ABC\TEXT.C and ABC\DEF\TEXT.C. If recurseFlag is FALSE, only
    ''' ABC\TEXT.C is selected. If recurseFlag is TRUE, both files are selected
    '''
    '''
    ''' PS: c:\some\dir or c:\some\dir\ will not include any files in dir (or if recursive, files in subdirs).
    ''' PS: If adding c:\some\dir, we (and DZ) will think dir is a file.
    ''' It will include subdirs thou (if recursive), but this is mostly useless.
    ''' Conclusion: it is meaningless to add dirs without file/mask to ItemList (but DynaZip allows it, and so do we).
    '''
    ''' The most logical would be to add *.* automatically if no file/mask specified,
    ''' but keep DZ compat for now. Maybe change later.
    ''' TODO: add recursive options for every item?
    ''' </summary>
    Public ItemList As New List(Of String)()
    ''' <summary>
    ''' Files to store
    ''' </summary>
    Public StoreSuffixes As New List(Of String)()
    'This functionality is more confusing than usefull -> made private
    Private NoDirectoryEntries As Boolean = False
    Public ExcludeFollowing As New List(Of String)()
    Public IncludeOnlyFollowing As New List(Of String)()
    '		public bool DontCheckNames;
    ''' <summary>
    ''' DynaZip default is true (and so are we)
    ''' </summary>
    Public UseTempFile As Boolean = True
    'bad def?
    ''' <summary>
    ''' DynaZip default is Absolute (but why arent't we?)
    ''' </summary>
    Public PathInZip As enPathInZip = enPathInZip.Relative
    'good def? yes
    Public Comment As String

    ' buffer to hold temp bytes
    Private pBuffer As Byte()

    Public Sub Zip()
        If ZipFile Is Nothing Then
            Throw New ArgumentException("ZipFile is null")
        End If
        If ItemList.Count = 0 Then
            Throw New ArgumentException("ItemList is empty")
        End If

        If Path.GetExtension(ZipFile).Length = 0 Then
            ZipFile = Path.ChangeExtension(ZipFile, "zip")
        End If

        Dim realZipFile As String = Nothing
        If UseTempFile Then
            realZipFile = ZipFile
            ZipFile = GetTempFileName(ZipFile)
        End If

        Dim excludes As FileSpecMatcher = Nothing
        If ExcludeFollowing.Count > 0 Then
            excludes = New FileSpecMatcher(ExcludeFollowing, True)
        End If
        Dim includes As FileSpecMatcher = Nothing
        If IncludeOnlyFollowing.Count > 0 Then
            includes = New FileSpecMatcher(IncludeOnlyFollowing, True)
        End If

        pBuffer = New Byte(4095) {}


        '
        '			1) collect files. if we find a file several times its ok, as long as the zipped name is the same, else exception! (typically when 2 items are same dir, but different level and we store relative path)
        '			 * Same with zipped name: if two different files map to same zipped name -> exception (typically when no path is stored + recursive)
        '			 * 
        '			 * 
        '			


        Dim fsEntries As List(Of FileSystemEntry) = CollectFileSystemEntries()

        Try
            Dim addedSomeEntry As Boolean = False

            'hmmm...denne vil adde hvis fila eksisterer? Nei...vi bruker append = 0
            Using writer As New ZipWriter(ZipFile)
                writer.Comment = Me.Comment

                For Each fsEntry As FileSystemEntry In fsEntries
                    If IsIncludeFile(fsEntry.ZippedName, fsEntry.IsDirectory, includes, excludes) Then
                        If fsEntry.IsDirectory Then
                            If Not AddDirEntries Then
                                Throw New Exception("!AddDirEntries but still got dir")
                            End If

                            Dim di As DirectoryInfo = DirectCast(fsEntry.FileSystemInfo, DirectoryInfo)
                            Dim entry As New ZipEntry(fsEntry.ZippedName, True)
                            entry.ModifiedTime = GetLastWriteTimeFixed(di)
                            entry.FileAttributes = di.Attributes
                            entry.UTF8Encoding = Me.UTF8Encoding
                            entry.Zip64 = (Me.Zip64 = enZip64.Yes)
                            entry.Method = CompressionMethod.Stored
                            'DIR
                            writer.AddEntry(entry)
                        Else
                            Dim fi As FileInfo = DirectCast(fsEntry.FileSystemInfo, FileInfo)
                            If Me.Zip64 = enZip64.No AndAlso fi.Length > UInt32.MaxValue Then
                                Throw New NotSupportedException("Files above 4GB only supported with Zip64 enabled or auto")
                            End If
                            Dim entry As New ZipEntry(fsEntry.ZippedName)
                            entry.ModifiedTime = GetLastWriteTimeFixed(fi)
                            entry.FileAttributes = fi.Attributes
                            entry.UTF8Encoding = Me.UTF8Encoding
                            entry.Zip64 = (Me.Zip64 = enZip64.Yes) OrElse (fi.Length > UInt32.MaxValue AndAlso Me.Zip64 = enZip64.Auto)
                            If fi.Length = 0 OrElse IsStoreFile(fsEntry.ZippedName) Then
                                entry.Method = CompressionMethod.Stored
                            End If
                            writer.AddEntry(entry)

                            Using reader As FileStream = fi.OpenRead()
                                Dim byteCount As int = 0
                                While (byteCount = reader.Read(pBuffer, 0, pBuffer.Length)) > 0
                                    writer.Write(pBuffer, 0, byteCount)
                                End While
                            End Using
                        End If

                        addedSomeEntry = True
                    End If
                Next
            End Using

            If Not addedSomeEntry Then
                Throw New ZipException("Nothing to add")
            End If

            If UseTempFile Then
                File.Delete(realZipFile)
                'overwrite
                File.Move(ZipFile, realZipFile)
                ZipFile = realZipFile
            End If
        Catch
            File.Delete(ZipFile)
            Throw
        End Try
    End Sub

    Private Function IsStoreFile(fileName As String) As Boolean
        If fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        For Each suffix As String In StoreSuffixes
            If fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function GetLastWriteTimeFixed(fsi As FileSystemInfo) As DateTime
        'http://www.codeproject.com/KB/files/csharpfiledate.aspx?msg=2885854#xx2885854xx
        Dim localOffset As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
        Return fsi.LastWriteTimeUtc + localOffset
    End Function

    Private Function CollectFileSystemEntries() As List(Of FileSystemEntry)
        Dim htEntries As New Dictionary(Of String, FileSystemEntry)(StringComparer.OrdinalIgnoreCase)

        For Each item As String In ItemList
            Dim itemDirName As String = Path.GetDirectoryName(item)
            Dim baseDi As New DirectoryInfo(itemDirName)

            Dim dirs As New Stack(Of DirectoryInfo)()
            dirs.Push(baseDi)

            Dim itemFileName As String = Path.GetFileName(item)

            While dirs.Count <> 0
                Dim di As DirectoryInfo = dirs.Pop()

                ProcessDir(htEntries, baseDi, di, itemFileName)

                ' problem: vi får masse direntries fra kalalog der fil ligger if recurse.
                ' i dette tilfellet vil vi bare har dirs for filer vi fant!
                ' MEN når man legger til dir slik: dir\*.*, da vil man vel gjerne ha alle dirs...
                ' DynaZip fungerer akkurat likt da, så mulig vi skulle hatt en ny collector som fungerer
                ' mer logisk. Den gammle kan bruke ItemList mens den nye kan bruke files\dirs lists kanskje?

                If Recurse Then
                    For Each subDi As DirectoryInfo In di.GetDirectories()
                        dirs.Push(subDi)
                    Next
                End If
            End While
        Next

        Dim result As New List(Of FileSystemEntry)(htEntries.Values)
        result.Sort()
        Return result
    End Function

    Private ReadOnly Property AddDirEntries() As Boolean
        Get
            Return Not (PathInZip = enPathInZip.None OrElse NoDirectoryEntries)
        End Get
    End Property

    Private Sub ProcessDir(htEntries As Dictionary(Of String, FileSystemEntry), baseDi As DirectoryInfo, di As DirectoryInfo, itemFileName As String)
        Dim addedSomeFiles As Boolean = False

        If AddDirEntries AndAlso di IsNot baseDi Then
            AddFsEntry(htEntries, baseDi, di)
        End If

        ' di.GetFiles("") does work (always returns 0 files), but this is more readable/logical:
        If itemFileName.Length > 0 Then
            'PS: note that GetFiles will get test.srtt if we use *.srt
            'More info: http://www.codeproject.com/Articles/153471/DirectoryInfo-GetFiles-returns-more-files-than-exp
            'I have not added a fix for this because DynaZip works like this too.
            For Each fi As FileInfo In di.GetFiles(itemFileName)
                addedSomeFiles = addedSomeFiles Or AddFsEntry(htEntries, baseDi, fi)
            Next
        End If

        'if (addedSomeFiles && AddDirEntries && di != baseDi)
        '{
        '    AddFsEntry(htEntries, baseDi, di);
        '}
    End Sub

    Private Function AddFsEntry(htEntries As Dictionary(Of String, FileSystemEntry), baseDi As DirectoryInfo, fsi As FileSystemInfo) As Boolean
        Dim zippedName As String = GetPathInZip(baseDi, fsi)
        Dim fsEntry As New FileSystemEntry(zippedName, fsi)
        ' Remove trailing dir sep from key since we want to catch file and dirs with same name clash
        Dim key As String = zippedName.TrimEndDirSep()
        Dim existingEntry As FileSystemEntry = Nothing
        If htEntries.TryGetValue(key, existingEntry) Then
            'don't care about two different dirs added with same name in zip. DZ does the same.
            If fsEntry.IsDirectory AndAlso existingEntry.IsDirectory Then
                'ok. same file added (several times) with same name in zip -> just add it once
            ElseIf fsEntry.IsFile AndAlso existingEntry.IsFile AndAlso existingEntry.FullName.Equals(fsEntry.FullName, StringComparison.OrdinalIgnoreCase) Then
            Else
                'not ok. different files/file+dir added with same name in zip
                Throw New ArgumentException(String.Format("Both {0} '{1}' and {2} '{3}' maps to '{4}' in zip", If(existingEntry.IsDirectory, "dir", "file"), existingEntry.FileSystemInfo.FullName, If(fsEntry.IsDirectory, "dir", "file"), fsEntry.FileSystemInfo.FullName, fsEntry.ZippedName))
            End If
            Return False
        Else
            htEntries.Add(key, fsEntry)
            Return True
        End If
    End Function

    Private Function IsIncludeFile(zippedName As String, isDir As Boolean, includes As FileSpecMatcher, excludes As FileSpecMatcher) As Boolean
        If includes Is Nothing OrElse includes.MatchSpecs(zippedName, isDir) Then
            If excludes Is Nothing OrElse Not excludes.MatchSpecs(zippedName, isDir) Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Function GetTempFileName(zipFile As String) As String
        Dim i As Integer = 0
        While True
            Dim tempFile As String = zipFile & "."
            If i > 0 Then
                tempFile += i
            End If
            tempFile += "tmp"

            If Not File.Exists(tempFile) Then
                Return tempFile
            End If
            i += 1
        End While

        Return Nothing
    End Function

    Private Function GetPathInZip(baseDi As DirectoryInfo, fsi As FileSystemInfo) As String
        Dim name As String = Nothing

        If NoDirectoryEntries AndAlso TypeOf fsi Is DirectoryInfo Then
            Throw New Exception("NoDirectoryEntries but trying to create name for DirectoryInfo")
        End If

        Select Case PathInZip
            'Absolute = relative from root dir
            Case enPathInZip.Absolute
                name = GetRelativeName(fsi.FullName, baseDi.Root.FullName)
                Exit Select
            ' AbsoluteRoot is not supported by Windows Compressed folders! (Will show zip as empty)
            Case enPathInZip.AbsoluteRoot
                name = fsi.FullName
                Exit Select
            Case enPathInZip.Relative
                name = GetRelativeName(fsi.FullName, baseDi.FullName)
                Exit Select
            Case enPathInZip.None
                name = fsi.Name
                Exit Select
            Case Else
                Throw New NotImplementedException("enPathInZip " & Convert.ToString(PathInZip))

        End Select

        If name.Length = 0 Then
            Throw New Exception(String.Format("Zipped name for {0} '{1}' is empty", If(TypeOf fsi Is DirectoryInfo, "dir", "file"), fsi.FullName))
        End If

        'Not really needed, but will give same sorting as DZ, making comparison with DZ easier/possible.
        If TypeOf fsi Is DirectoryInfo Then
            name = name.SetEndDirSep()
        End If

        Return name
    End Function

    Private Function GetRelativeName(full As String, baseDir As String) As String
        Return full.Substring(baseDir.Length).TrimStartDirSep()
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    Private Class FileSystemEntry
        Implements IComparable(Of FileSystemEntry)
        Public ZippedName As String
        Public FileSystemInfo As FileSystemInfo

        Public Sub New(zippedName As String, fsi As FileSystemInfo)
            Me.ZippedName = zippedName
            Me.FileSystemInfo = fsi
        End Sub

        Public ReadOnly Property IsFile() As Boolean
            Get
                Return Not IsDirectory
            End Get
        End Property
        Public ReadOnly Property IsDirectory() As Boolean
            Get
                Return TypeOf FileSystemInfo Is DirectoryInfo
            End Get
        End Property
        Public ReadOnly Property FullName() As String
            Get
                Return FileSystemInfo.FullName
            End Get
        End Property

        Public Function CompareTo(that As FileSystemEntry) As Integer Implements IComparable(Of FileSystemEntry).CompareTo
            Return Me.ZippedName.CompareTo(that.ZippedName)
        End Function
    End Class
End Class

Public Enum enPathInZip
    ''' <summary>
    ''' Relative to item in ItemList
    ''' </summary>
    Relative
    ''' <summary>
    ''' Absolute from first directory on drive (test\a.c)
    ''' This is DynaZip default.
    ''' </summary>
    Absolute
    ''' <summary>
    ''' Absolute from root (C:\test\a.c))
    ''' </summary>
    AbsoluteRoot
    ''' <summary>
    ''' No path stored, all files on root (a.c)
    ''' </summary>
    None
End Enum

Public Enum enZip64
    Yes
    No
    Auto
End Enum


Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports System.Collections
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Win32.GetLastErrorAPI

''' <summary>Support methods for uncompressing zip files.</summary>
''' <remarks>
'''   <para>This unzip package allow extract file from .ZIP file, compatible with PKZip 2.04g WinZip, InfoZip tools and compatible.</para>
'''   <para>Encryption and multi volume ZipFile (span) are not supported.  Old compressions used by old PKZip 1.x are not supported.</para>
'''   <para>Copyright (C) 1998 Gilles Vollant.  http://www.winimage.com/zLibDll/unzip.htm</para>
'''   <para>C# wrapper by Gerry Shaw (gerry_shaw@yahoo.com).  http://www.organicbit.com/zip/</para>
'''   
''' ZipLib = MiniZip part of zlib
''' 
''' </remarks>
Friend NotInheritable Class Minizip
    Private Sub New()
    End Sub
    <DllImport(ZLibDll.Name32, EntryPoint:="setOpenUnicode", ExactSpelling:=True)>
    Private Shared Function setOpenUnicode_32(openUnicode As Integer) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="setOpenUnicode", ExactSpelling:=True)>
    Private Shared Function setOpenUnicode_64(openUnicode As Integer) As Integer
    End Function

    Friend Shared Function setOpenUnicode(openUnicode As Boolean) As Boolean
        Dim oldVal As Integer
        If ZLibDll.Is64 Then
            oldVal = setOpenUnicode_64(If(openUnicode, 1, 0))
        Else
            oldVal = setOpenUnicode_32(If(openUnicode, 1, 0))
        End If
        Return oldVal = 1
    End Function

    Shared Sub New()
        DllLoader.Load()
    End Sub

    '
    '		 Create a zipfile.
    '		 pathname contain on Windows NT a filename like "c:\\zlib\\zlib111.zip" or on an Unix computer "zlib/zlib111.zip".
    '		 if the file pathname exist and append=1, the zip will be created at the end of the file. (useful if the file contain a self extractor code)
    '		 If the zipfile cannot be opened, the return value is NULL.
    '		 Else, the return value is a zipFile Handle, usable with other function of this zip package.
    '	 

    ''' <summary>Create a zip file.</summary>
    <DllImport(ZLibDll.Name32, EntryPoint:="zipOpen64", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function zipOpen_32(fileName As String, append As Integer) As IntPtr
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="zipOpen64", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function zipOpen_64(fileName As String, append As Integer) As IntPtr
    End Function

    Friend Shared Function zipOpen(fileName As String, append As Boolean) As IntPtr
        setOpenUnicode(True)

        If ZLibDll.Is64 Then
            Return zipOpen_64(fileName, If(append, 1, 0))
        Else
            Return zipOpen_32(fileName, If(append, 1, 0))
        End If
    End Function
    '
    '			Open a file in the ZIP for writing.
    '			filename : the filename in zip (if NULL, '-' without quote will be used
    '			*zipfi contain supplemental information
    '			if extrafield_local!=NULL and size_extrafield_local>0, extrafield_local contains the extrafield data the the local header
    '			if extrafield_global!=NULL and size_extrafield_global>0, extrafield_global contains the extrafield data the the local header
    '			if comment != NULL, comment contain the comment string
    '			method contain the compression method (0 for store, Z_DEFLATED for deflate)
    '			level contain the level of compression (can be Z_DEFAULT_COMPRESSION)
    '		

    <DllImport(ZLibDll.Name32, EntryPoint:="zipOpenNewFileInZip4_64", ExactSpelling:=True)>
    Private Shared Function zipOpenNewFileInZip4_64_32(handle As IntPtr, entryName As Byte(), ByRef entryInfoPtr As ZipFileEntryInfo, extraField As Byte(), extraFieldLength As UInteger, extraFieldGlobal As Byte(),
        extraFieldGlobalLength As UInteger, comment As Byte(), method As Integer, level As Integer, raw As Integer, windowBits As Integer,
        memLevel As Integer, strategy As Integer, password As Byte(), crcForCrypting As UInteger, versionMadeBy As UInteger, flagBase As UInteger,
        zip64 As Integer) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="zipOpenNewFileInZip4_64", ExactSpelling:=True)>
    Private Shared Function zipOpenNewFileInZip4_64_64(handle As IntPtr, entryName As Byte(), ByRef entryInfoPtr As ZipFileEntryInfo, extraField As Byte(), extraFieldLength As UInteger, extraFieldGlobal As Byte(),
        extraFieldGlobalLength As UInteger, comment As Byte(), method As Integer, level As Integer, raw As Integer, windowBits As Integer,
        memLevel As Integer, strategy As Integer, password As Byte(), crcForCrypting As UInteger, versionMadeBy As UInteger, flagBase As UInteger,
        zip64 As Integer) As Integer
    End Function


    Public Shared Function zipOpenNewFileInZip4_64(handle As IntPtr, entryName As Byte(), ByRef entryInfoPtr As ZipFileEntryInfo, extraField As Byte(), extraFieldLength As UInteger, extraFieldGlobal As Byte(),
        extraFieldGlobalLength As UInteger, comment As Byte(), method As Integer, level As Integer, flagBase As UInteger, zip64 As Boolean) As Integer
        If ZLibDll.Is64 Then
            Return zipOpenNewFileInZip4_64_64(handle, entryName, entryInfoPtr, extraField, extraFieldLength, extraFieldGlobal,
                extraFieldGlobalLength, comment, method, level, 0, -ZLib.MAX_WBITS,
                ZLib.DEF_MEM_LEVEL, ZLib.Z_DEFAULT_STRATEGY, Nothing, 0, ZLib.VERSIONMADEBY, flagBase,
                If(zip64, 1, 0))
        Else
            Return zipOpenNewFileInZip4_64_32(handle, entryName, entryInfoPtr, extraField, extraFieldLength, extraFieldGlobal,
                extraFieldGlobalLength, comment, method, level, 0, -ZLib.MAX_WBITS,
                ZLib.DEF_MEM_LEVEL, ZLib.Z_DEFAULT_STRATEGY, Nothing, 0, ZLib.VERSIONMADEBY, flagBase,
                If(zip64, 1, 0))
        End If
    End Function



    ''' <summary>Write data to the zip file.</summary>
    <DllImport(ZLibDll.Name32, EntryPoint:="zipWriteInFileInZip", ExactSpelling:=True)>
    Private Shared Function zipWriteInFileInZip_32(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="zipWriteInFileInZip", ExactSpelling:=True)>
    Private Shared Function zipWriteInFileInZip_64(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
    End Function

    Friend Shared Function zipWriteInFileInZip(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
        If ZLibDll.Is64 Then
            Return zipWriteInFileInZip_64(handle, buffer, count)
        Else
            Return zipWriteInFileInZip_32(handle, buffer, count)
        End If
    End Function

    ''' <summary>Close the current entry in the zip file.</summary>
    <DllImport(ZLibDll.Name32, EntryPoint:="zipCloseFileInZip", ExactSpelling:=True)>
    Private Shared Function zipCloseFileInZip_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="zipCloseFileInZip", ExactSpelling:=True)>
    Private Shared Function zipCloseFileInZip_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function zipCloseFileInZip(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return zipCloseFileInZip_64(handle)
        Else
            Return zipCloseFileInZip_32(handle)
        End If
    End Function

    ''' <summary>Close the zip file.</summary>
    ''' //file comment is for some weird reason ANSI, while entry name + comment is OEM...
    <DllImport(ZLibDll.Name32, EntryPoint:="zipClose", ExactSpelling:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function zipClose_32(handle As IntPtr, comment As String) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="zipClose", ExactSpelling:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function zipClose_64(handle As IntPtr, comment As String) As Integer
    End Function

    Friend Shared Function zipClose(handle As IntPtr, comment As String) As Integer
        If ZLibDll.Is64 Then
            Return zipClose_64(handle, comment)
        Else
            Return zipClose_32(handle, comment)
        End If
    End Function

    ''' <summary>Opens a zip file for reading.</summary>
    ''' <param name="fileName">The name of the zip to open.</param>
    ''' <returns>
    '''   <para>A handle usable with other functions of the ZipLib class.</para>
    '''   <para>Otherwise IntPtr.Zero if the zip file could not e opened (file doen not exist or is not valid).</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzOpen64", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function unzOpen_32(fileName As String) As IntPtr
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzOpen64", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function unzOpen_64(fileName As String) As IntPtr
    End Function

    Friend Shared Function unzOpen(fileName As String) As IntPtr
        setOpenUnicode(True)

        If ZLibDll.Is64 Then
            Return unzOpen_64(fileName)
        Else
            Return unzOpen_32(fileName)
        End If
    End Function

    ''' <summary>Closes a zip file opened with unzipOpen.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <remarks>If there are files inside the zip file opened with <see cref="unzOpenCurrentFile"/> these files must be closed with <see cref="unzCloseCurrentFile"/> before call <c>unzClose</c>.</remarks>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise a value less than zero.  See <see cref="ErrorCodes"/> for the specific reason.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzClose", ExactSpelling:=True)>
    Private Shared Function unzClose_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzClose", ExactSpelling:=True)>
    Private Shared Function unzClose_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function unzClose(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return unzClose_64(handle)
        Else
            Return unzClose_32(handle)
        End If
    End Function

    ''' <summary>Get global information about the zip file.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <param name="globalInfoPtr">An address of a <see cref="ZipFileInfo"/> struct to hold the information.  No preparation of the structure is needed.</param>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise a value less than zero.  See <see cref="ErrorCodes"/> for the specific reason.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzGetGlobalInfo", ExactSpelling:=True)>
    Private Shared Function unzGetGlobalInfo_32(handle As IntPtr, ByRef globalInfoPtr As ZipFileInfo) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzGetGlobalInfo", ExactSpelling:=True)>
    Private Shared Function unzGetGlobalInfo_64(handle As IntPtr, ByRef globalInfoPtr As ZipFileInfo) As Integer
    End Function

    Friend Shared Function unzGetGlobalInfo(handle As IntPtr, ByRef globalInfoPtr As ZipFileInfo) As Integer
        If ZLibDll.Is64 Then
            Return unzGetGlobalInfo_64(handle, globalInfoPtr)
        Else
            Return unzGetGlobalInfo_32(handle, globalInfoPtr)
        End If
    End Function

    ''' <summary>Get the comment associated with the entire zip file.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/></param>
    ''' <param name="commentBuffer">The buffer to hold the comment.</param>
    ''' <param name="commentBufferLength">The length of the buffer in bytes (8 bit characters).</param>
    ''' <returns>
    '''   <para>The number of characters in the comment if there was no error.</para>
    '''   <para>Otherwise a value less than zero.  See <see cref="ErrorCodes"/> for the specific reason.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzGetGlobalComment", ExactSpelling:=True)>
    Private Shared Function unzGetGlobalComment_32(handle As IntPtr, commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzGetGlobalComment", ExactSpelling:=True)>
    Private Shared Function unzGetGlobalComment_64(handle As IntPtr, commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
    End Function

    Friend Shared Function unzGetGlobalComment(handle As IntPtr, commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
        If ZLibDll.Is64 Then
            Return unzGetGlobalComment_64(handle, commentBuffer, commentBufferLength)
        Else
            Return unzGetGlobalComment_32(handle, commentBuffer, commentBufferLength)
        End If
    End Function

    ''' <summary>Set the current file of the zip file to the first file.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise a value less than zero.  See <see cref="ErrorCodes"/> for the specific reason.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzGoToFirstFile", ExactSpelling:=True)>
    Private Shared Function unzGoToFirstFile_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzGoToFirstFile", ExactSpelling:=True)>
    Private Shared Function unzGoToFirstFile_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function unzGoToFirstFile(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return unzGoToFirstFile_64(handle)
        Else
            Return unzGoToFirstFile_32(handle)
        End If
    End Function

    ''' <summary>Set the current file of the zip file to the next file.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise ErrorCode.EndOfListOfFile if there are no more entries.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzGoToNextFile", ExactSpelling:=True)>
    Private Shared Function unzGoToNextFile_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzGoToNextFile", ExactSpelling:=True)>
    Private Shared Function unzGoToNextFile_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function unzGoToNextFile(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return unzGoToNextFile_64(handle)
        Else
            Return unzGoToNextFile_32(handle)
        End If
    End Function

    ''' <summary>Get information about the current entry in the zip file.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <param name="entryInfoPtr">A ZipEntryInfo struct to hold information about the entry or null.</param>
    ''' <param name="entryNameBuffer">An array of sbyte characters to hold the entry name or null.</param>
    ''' <param name="entryNameBufferLength">The length of the entryNameBuffer in bytes.</param>
    ''' <param name="extraField">An array to hold the extra field data for the entry or null.</param>
    ''' <param name="extraFieldLength">The length of the extraField array in bytes.</param>
    ''' <param name="commentBuffer">An array of sbyte characters to hold the entry name or null.</param>
    ''' <param name="commentBufferLength">The length of theh commentBuffer in bytes.</param>
    ''' <remarks>
    '''   <para>If entryInfoPtr is not null the structure will contain information about the current file.</para>
    '''   <para>If entryNameBuffer is not null the name of the entry will be copied into it.</para>
    '''   <para>If extraField is not null the extra field data of the entry will be copied into it.</para>
    '''   <para>If commentBuffer is not null the comment of the entry will be copied into it.</para>
    ''' </remarks>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise a value less than zero.  See <see cref="ErrorCodes"/> for the specific reason.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzGetCurrentFileInfo64", ExactSpelling:=True)>
    Private Shared Function unzGetCurrentFileInfo64_32(handle As IntPtr, ByRef entryInfoPtr As ZipEntryInfo64, entryNameBuffer As Byte(), entryNameBufferLength As UInteger, extraField As Byte(), extraFieldLength As UInteger,
        commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzGetCurrentFileInfo64", ExactSpelling:=True)>
    Private Shared Function unzGetCurrentFileInfo64_64(handle As IntPtr, ByRef entryInfoPtr As ZipEntryInfo64, entryNameBuffer As Byte(), entryNameBufferLength As UInteger, extraField As Byte(), extraFieldLength As UInteger,
        commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
    End Function

    Friend Shared Function unzGetCurrentFileInfo64(handle As IntPtr, ByRef entryInfoPtr As ZipEntryInfo64, entryNameBuffer As Byte(), entryNameBufferLength As UInteger, extraField As Byte(), extraFieldLength As UInteger,
        commentBuffer As Byte(), commentBufferLength As UInteger) As Integer
        If ZLibDll.Is64 Then
            Return unzGetCurrentFileInfo64_64(handle, entryInfoPtr, entryNameBuffer, entryNameBufferLength, extraField, extraFieldLength,
                commentBuffer, commentBufferLength)
        Else
            Return unzGetCurrentFileInfo64_32(handle, entryInfoPtr, entryNameBuffer, entryNameBufferLength, extraField, extraFieldLength,
                commentBuffer, commentBufferLength)
        End If

    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function GetOEMCP() As UInteger
    End Function

    Public Shared OEMEncoding As Encoding = Encoding.GetEncoding(CInt(Minizip.GetOEMCP()))

    ''' <summary>Open the zip file entry for reading.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>Otherwise a value from <see cref="ErrorCodes"/>.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzOpenCurrentFile", ExactSpelling:=True)>
    Public Shared Function unzOpenCurrentFile_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzOpenCurrentFile", ExactSpelling:=True)>
    Public Shared Function unzOpenCurrentFile_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function unzOpenCurrentFile(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return unzOpenCurrentFile_64(handle)
        Else
            Return unzOpenCurrentFile_32(handle)
        End If
    End Function

    ''' <summary>Close the file entry opened by <see cref="unzOpenCurrentFile"/>.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <returns>
    '''   <para>Zero if there was no error.</para>
    '''   <para>CrcError if the file was read but the Crc does not match.</para>
    '''   <para>Otherwise a value from <see cref="ErrorCodes"/>.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzCloseCurrentFile", ExactSpelling:=True)>
    Public Shared Function unzCloseCurrentFile_32(handle As IntPtr) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzCloseCurrentFile", ExactSpelling:=True)>
    Public Shared Function unzCloseCurrentFile_64(handle As IntPtr) As Integer
    End Function

    Friend Shared Function unzCloseCurrentFile(handle As IntPtr) As Integer
        If ZLibDll.Is64 Then
            Return unzCloseCurrentFile_64(handle)
        Else
            Return unzCloseCurrentFile_32(handle)
        End If
    End Function

    ''' <summary>Read bytes from the current zip file entry.</summary>
    ''' <param name="handle">The zip file handle opened by <see cref="unzOpenCurrentFile"/>.</param>
    ''' <param name="buffer">Buffer to store the uncompressed data into.</param>
    ''' <param name="count">Number of bytes to write from <paramref name="buffer"/>.</param>
    ''' <returns>
    '''   <para>The number of byte copied if somes bytes are copied.</para>
    '''   <para>Zero if the end of file was reached.</para>
    '''   <para>Less than zero with error code if there is an error.  See <see cref="ErrorCodes"/> for a list of possible error codes.</para>
    ''' </returns>
    <DllImport(ZLibDll.Name32, EntryPoint:="unzReadCurrentFile", ExactSpelling:=True)>
    Private Shared Function unzReadCurrentFile_32(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
    End Function
    <DllImport(ZLibDll.Name64, EntryPoint:="unzReadCurrentFile", ExactSpelling:=True)>
    Private Shared Function unzReadCurrentFile_64(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
    End Function

    Friend Shared Function unzReadCurrentFile(handle As IntPtr, buffer As IntPtr, count As UInteger) As Integer
        If ZLibDll.Is64 Then
            Return unzReadCurrentFile_64(handle, buffer, count)
        Else
            Return unzReadCurrentFile_32(handle, buffer, count)
        End If
    End Function

End Class

Friend NotInheritable Class ZipEntryFlag
    Private Sub New()
    End Sub
    Friend Const UTF8 As UInteger = &H800
    '1 << 11
End Class

''' <summary>Global information about the zip file.</summary>
<StructLayout(LayoutKind.Sequential)>
Friend Structure ZipFileInfo
    ''' <summary>The number of entries in the directory.</summary>
    Public EntryCount As UInt32

    ''' <summary>Length of zip file comment in bytes (8 bit characters).</summary>
    Public CommentLength As UInt32
End Structure

<StructLayout(LayoutKind.Sequential)>
Friend Structure ZipFileEntryInfo
    Public ZipDateTime As ZipDateTimeInfo
    Public DosDate As UInt32
    Public InternalFileAttributes As UInt32
    ' 2 bytes
    Public ExternalFileAttributes As UInt32
    ' 4 bytes
End Structure

''' <summary>Custom ZipLib date time structure.</summary>
<StructLayout(LayoutKind.Sequential)>
Friend Structure ZipDateTimeInfo
    ''' <summary>Seconds after the minute - [0,59]</summary>
    Public Seconds As UInt32

    ''' <summary>Minutes after the hour - [0,59]</summary>
    Public Minutes As UInt32

    ''' <summary>Hours since midnight - [0,23]</summary>
    Public Hours As UInt32

    ''' <summary>Day of the month - [1,31]</summary>
    Public Day As UInt32

    ''' <summary>Months since January - [0,11]</summary>
    Public Month As UInt32

    ''' <summary>Years - [1980..2044]</summary>
    Public Year As UInt32

    ' implicit conversion from DateTime to ZipDateTimeInfo
    Public Shared Widening Operator CType([date] As DateTime) As ZipDateTimeInfo
        Dim d As ZipDateTimeInfo
        d.Seconds = CUInt([date].Second)
        d.Minutes = CUInt([date].Minute)
        d.Hours = CUInt([date].Hour)
        d.Day = CUInt([date].Day)
        d.Month = CUInt([date].Month) - 1
        d.Year = CUInt([date].Year)
        Return d
    End Operator

    Public Shared Widening Operator CType([date] As ZipDateTimeInfo) As DateTime
        Dim dt As New DateTime(CInt([date].Year), CInt([date].Month) + 1, CInt([date].Day), CInt([date].Hours), CInt([date].Minutes), CInt([date].Seconds))
        Return dt
    End Operator

End Structure

''' <summary>Information stored in zip file directory about an entry.</summary>
<StructLayout(LayoutKind.Sequential)>
Friend Structure ZipEntryInfo64
    ' <summary>Version made by (2 bytes).</summary>
    Public Version As UInt32

    ''' <summary>Version needed to extract (2 bytes).</summary>
    Public VersionNeeded As UInt32

    ''' <summary>General purpose bit flag (2 bytes).</summary>
    Public Flag As UInt32

    ''' <summary>Compression method (2 bytes).</summary>
    Public CompressionMethod As UInt32

    ''' <summary>Last mod file date in Dos fmt (4 bytes).</summary>
    Public DosDate As UInt32

    ''' <summary>Crc-32 (4 bytes).</summary>
    Public Crc As UInt32

    ''' <summary>Compressed size (8 bytes).</summary>
    Public CompressedSize As UInt64

    ''' <summary>Uncompressed size (8 bytes).</summary>
    Public UncompressedSize As UInt64

    ''' <summary>Filename length (2 bytes).</summary>
    Public FileNameLength As UInt32

    ''' <summary>Extra field length (2 bytes).</summary>
    Public ExtraFieldLength As UInt32

    ''' <summary>File comment length (2 bytes).</summary>
    Public CommentLength As UInt32

    ''' <summary>Disk number start (2 bytes).</summary>
    Public DiskStartNumber As UInt32

    ''' <summary>Internal file attributes (2 bytes).</summary>
    Public InternalFileAttributes As UInt32

    ''' <summary>External file attributes (4 bytes).</summary>
    Public ExternalFileAttributes As UInt32

    ''' <summary>File modification date of entry.</summary>
    Public ZipDateTime As ZipDateTimeInfo
End Structure


''' <summary>Specifies how the the zip entry should be compressed.</summary>
Public Enum CompressionMethod
    ''' <summary>No compression.</summary>
    Stored = 0

    ''' <summary>Default and only supported compression method.</summary>
    Deflated = 8
End Enum

''' <summary>Type of compression to use for the GZipStream. Currently only Decompress is supported.</summary>
Public Enum CompressionMode
    ''' <summary>Compresses the underlying stream.</summary>
    Compress
    ''' <summary>Decompresses the underlying stream.</summary>
    Decompress
End Enum

''' <summary>List of possible error codes.
''' 
''' </summary>
Friend NotInheritable Class ZipReturnCode
    Private Sub New()
    End Sub
    ''' <summary>No error.</summary>
    Friend Const Ok As Integer = 0

    ''' <summary>Unknown error.</summary>
    Friend Const [Error] As Integer = -1

    ''' <summary>Last entry in directory reached.</summary>
    Friend Const EndOfListOfFile As Integer = -100

    ''' <summary>Parameter error.</summary>
    Friend Const ParameterError As Integer = -102

    ''' <summary>Zip file is invalid.</summary>
    Friend Const BadZipFile As Integer = -103

    ''' <summary>Internal program error.</summary>
    Friend Const InternalError As Integer = -104

    ''' <summary>Crc values do not match.</summary>
    Friend Const CrcError As Integer = -105

    Public Shared Function GetMessage(retCode As Integer) As String
        Select Case retCode
            Case ZipReturnCode.Ok
                Return "No error"
            Case ZipReturnCode.[Error]
                Return "Unknown error"
            Case ZipReturnCode.EndOfListOfFile
                Return "Last entry in directory reached"
            Case ZipReturnCode.ParameterError
                Return "Parameter error"
            Case ZipReturnCode.BadZipFile
                Return "Zip file is invalid"
            Case ZipReturnCode.InternalError
                Return "Internal program error"
            Case ZipReturnCode.CrcError
                Return "Crc values do not match"
            Case Else
                Return "Unknown error: " & retCode
        End Select
    End Function
End Class


''' <summary>Thrown whenever an error occurs during the build.</summary>
<Serializable>
Public Class ZipException
    Inherits ApplicationException

    ''' <summary>Constructs an exception with no descriptive information.</summary>
    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>Constructs an exception with a descriptive message.</summary>
    ''' <param name="message">The error message that explains the reason for the exception.</param>
    Public Sub New(message As [String])
        MyBase.New(message)
    End Sub

    Public Sub New(message As [String], errorCode As Integer)
        MyBase.New(message & " (" & ZipReturnCode.GetMessage(errorCode) & ")")
    End Sub

    ''' <summary>Constructs an exception with a descriptive message and a reference to the instance of the <c>Exception</c> that is the root cause of the this exception.</summary>
    ''' <param name="message">The error message that explains the reason for the exception.</param>
    ''' <param name="innerException">An instance of <c>Exception</c> that is the cause of the current Exception. If <paramref name="innerException"/> is non-null, then the current Exception is raised in a catch block handling <paramref>innerException</paramref>.</param>
    Public Sub New(message As [String], innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    ''' <summary>Initializes a new instance of the BuildException class with serialized data.</summary>
    ''' <param name="info">The object that holds the serialized object data.</param>
    ''' <param name="context">The contextual information about the source or destination.</param>
    Public Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class



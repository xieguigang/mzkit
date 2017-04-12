Imports System.IO
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text


''' <summary>Represents a entry in a zip file.</summary>
Public Class ZipEntry

	Private _name As String = [String].Empty
	Private _crc As UInteger = 0
	Private _compressedLength As Long
	Private _uncompressedLength As Long
	Private _extraField As Byte() = Nothing
	Private _comment As String = [String].Empty
	Private _modifiedTime As DateTime = DateTime.Now
	Private _fileAttributes As FileAttributes
	Private _method As CompressionMethod = CompressionMethod.Deflated
	Private _level As Integer = CInt(CompressionLevel.[Default])
	Private _isDirectory As Boolean
	''' <summary>
	''' Utf8 filename/comment.
	''' Not supported by Windows Compressed Folders
	''' </summary>
	Private _UTF8Encoding As Boolean
	Private _zip64 As Boolean

    ''' <summary>Initializes a instance of the <see cref="ZipEntry"/> class with the given name.</summary>
    ''' <param name="name">The name of entry that will be stored in the directory of the zip file.</param>
    Public Sub New(name As String, isDirectory As Boolean)
        Me.Name = name
        _isDirectory = isDirectory
	End Sub

	''' <summary>Initializes a instance of the <see cref="ZipEntry"/> class with the given name.</summary>
	''' <param name="name">The name of entry that will be stored in the directory of the zip file.</param>
	Public Sub New(name As String)
		Me.New(name, False)
	End Sub


	''' <summary>Creates a new Zip file entry reading values from a zip file.</summary>
	Friend Sub New(handle As IntPtr)
		Dim entryInfo As New ZipEntryInfo64()
		Dim result As Integer = Minizip.unzGetCurrentFileInfo64(handle, entryInfo, Nothing, 0, Nothing, 0, _
			Nothing, 0)
		If result <> 0 Then
			Throw New ZipException("Could not read entry from zip file " & Name, result)
		End If

		_extraField = New Byte(entryInfo.ExtraFieldLength - 1) {}
		Dim entryNameBuffer As Byte() = New Byte(entryInfo.FileNameLength - 1) {}
		Dim commentBuffer As Byte() = New Byte(entryInfo.CommentLength - 1) {}

		result = Minizip.unzGetCurrentFileInfo64(handle, entryInfo, entryNameBuffer, CUInt(entryNameBuffer.Length), _extraField, CUInt(_extraField.Length), _
			commentBuffer, CUInt(commentBuffer.Length))

		If result <> 0 Then
			Throw New ZipException("Could not read entry from zip file " & Name, result)
		End If

		Me._UTF8Encoding = BitFlag.IsSet(entryInfo.Flag, ZipEntryFlag.UTF8)
		Dim encoding__1 As Encoding = If(Me._UTF8Encoding, Encoding.UTF8, Minizip.OEMEncoding)

		_name = encoding__1.GetString(entryNameBuffer)
		'null or empty string if empty buffer?
		_comment = encoding__1.GetString(commentBuffer)
		_crc = entryInfo.Crc
		_compressedLength = CLng(entryInfo.CompressedSize)
		_uncompressedLength = CLng(entryInfo.UncompressedSize)
		_method = CType(entryInfo.CompressionMethod, CompressionMethod)
		_modifiedTime = entryInfo.ZipDateTime
		_fileAttributes = CType(entryInfo.ExternalFileAttributes, FileAttributes)
		_isDirectory = InterpretIsDirectory()
	End Sub

	'private ExtraField[] GetExtraFields(byte[] _extraFields)
	'{
	'    throw new NotImplementedException();
	'}

	'struct ExtraField
	'{
	'    public Int16 Tag;
	'    public byte[] Data;
	'}

	'private byte[] GetUTF8Name(byte[] ExtraField)
	'{
	'    byte[] data = GetTag(ExtraField, 0x7075);
	'    if (data == null)
	'        return null;
	'}

	'private byte[] GetUTF8Comment(byte[] ExtraField)
	'{
	'    byte[] data = GetTag(ExtraField, 0x6375);
	'    return null;
	'    BitConverter.ToInt16(
	'}

	'private byte[] GetTag(byte[] ExtraField, int p)
	'{
	'    throw new NotImplementedException();
	'}

	Private Function InterpretIsDirectory() As Boolean
		Dim winDir As Boolean = ((_fileAttributes And FileAttributes.Directory) <> 0)
		'windows
		Dim otherDir As Boolean = _name.EndsWithDirSep()
		' other os'
		Dim isDir As Boolean = winDir OrElse otherDir
		If isDir Then
			Debug.Assert(Name.Length > 0)
			Debug.Assert(Method = CompressionMethod.Stored)
			Debug.Assert(CompressedLength = 0)
			Debug.Assert(Length = 0)
		End If

		Return isDir
	End Function


	''' <summary>Gets and sets the local file comment for the entry.</summary>
	''' <remarks>
	'''   <para>Currently only Ascii 8 bit characters are supported in comments.</para>
	'''   <para>A comment cannot exceed 65535 bytes.</para>
	''' </remarks>
	Public Property Comment() As String
		Get
			Return _comment
		End Get
		Set
			' null comments are valid
			If value IsNot Nothing Then
				If value.Length > &Hffff Then
					Throw New ArgumentOutOfRangeException("Comment cannot not exceed 65535 characters.")
				End If
			End If
			_comment = value
		End Set
	End Property

	''' <summary>Gets the compressed size of the entry data in bytes, or -1 if not known.</summary>
	Public ReadOnly Property CompressedLength() As Long
		Get
			Return _compressedLength
		End Get
	End Property

	''' <summary>Gets the CRC-32 checksum of the uncompressed entry data.</summary>
	Public ReadOnly Property Crc() As UInteger
		Get
			Return _crc
		End Get
	End Property

	' true = Use UTF8 for name and comment
	Public Property UTF8Encoding() As Boolean
		Get
			Return Me._UTF8Encoding
		End Get
		Set
			Me._UTF8Encoding = value
		End Set
	End Property

	''' <summary>Gets and sets the optional extra field data for the entry.</summary>
	''' <remarks>ExtraField data cannot exceed 65535 bytes.</remarks>
	Public Property ExtraField() As Byte()
		Get
			Return _extraField
		End Get
		Set
			If value.Length > &Hffff Then
				Throw New ArgumentOutOfRangeException("ExtraField cannot not exceed 65535 bytes.")
			End If
			_extraField = value
		End Set
	End Property

	'''// <summary>Gets and sets the default compresion method for zip file entries.  See <see cref="CompressionMethod"/> for a list of possible values.</summary>
	Public Property Method() As CompressionMethod
		Get
			Return _method
		End Get
		Set
			_method = value
		End Set
	End Property

	'''// <summary>Gets and sets the default compresion level for zip file entries.  See <see cref="CompressionMethod"/> for a partial list of values.</summary>
	Public Property Level() As Integer
		Get
			Return _level
		End Get
		Set
			If value < -1 OrElse value > 9 Then
				Throw New ArgumentOutOfRangeException("Level", value, "Level value must be between -1 and 9.")
			End If
			_level = value
		End Set
	End Property

	Public Property Zip64() As Boolean
		Get
			Return Me._zip64
		End Get
		Set
			Me._zip64 = value
		End Set
	End Property

	''' <summary>Gets the size of the uncompressed entry data in in bytes.</summary>
	Public ReadOnly Property Length() As Long
		Get
			Return _uncompressedLength
		End Get
	End Property

	''' <summary>Gets and sets the modification time of the entry.</summary>
	Public Property ModifiedTime() As DateTime
		Get
			Return _modifiedTime
		End Get
		Set
			_modifiedTime = value
		End Set
	End Property

	''' <summary>Gets and sets the name of the entry.</summary>
	''' <remarks>
	'''   <para>Currently only Ascii 8 bit characters are supported in comments.</para>
	'''   <para>A comment cannot exceed 65535 bytes.</para>
	''' </remarks>
	Public Property Name() As String
		Get
			Return _name
		End Get
		Set
			If value Is Nothing Then
				Throw New ArgumentNullException("Name cannot be null.")
			End If
			If value.Length > &Hffff Then
				Throw New ArgumentOutOfRangeException("Name cannot not exceed 65535 characters.")
			End If
			_name = value
		End Set
	End Property

	Friend Function GetNameForZip() As String
		Dim nameForZip As String = _name

		If _isDirectory Then
			nameForZip = nameForZip.SetEndDirSep()
		End If

		' name in zip should always be stored with slashes (for compat)
		Return nameForZip.Replace("\"C, "/"C)
	End Function

	''' <summary>Flag that indicates if this entry is a directory or a file.</summary>
	Public ReadOnly Property IsDirectory() As Boolean
		Get
			Return _isDirectory
		End Get
	End Property

	''' <summary>Gets the compression ratio as a percentage.</summary>
	''' <remarks>Returns -1.0 if unknown.</remarks>
	Public ReadOnly Property Ratio() As Single
		Get
			Dim ratio__1 As Single = -1F
			If Length > 0 Then
				ratio__1 = Convert.ToSingle(Length - CompressedLength) / Length
			End If
			Return ratio__1
		End Get
	End Property

	Friend Function GetFileAttributesForZip() As FileAttributes
		Dim att As FileAttributes = Me._fileAttributes
		If Me._isDirectory Then
			att = att Or FileAttributes.Directory
		End If
		Return att
	End Function

	Public Property FileAttributes() As FileAttributes
		Get
			Return _fileAttributes
		End Get
		Set
			_fileAttributes = value
		End Set
	End Property

	''' <summary>Returns a string representation of the Zip entry.</summary>
	Public Overrides Function ToString() As String
		Return [String].Format("{0} {1}", Name, MyBase.ToString())
	End Function
End Class

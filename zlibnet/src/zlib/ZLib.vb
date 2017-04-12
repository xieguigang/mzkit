Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.IO
Imports System.Reflection

Friend NotInheritable Class ZLib
	Private Sub New()
	End Sub
	Friend Const ZLibVersion As String = "1.2.8"
	Friend Const MAX_WBITS As Integer = 15
	' 32K LZ77 window 
	Friend Const DEF_MEM_LEVEL As Integer = 8
	Friend Const Z_DEFAULT_STRATEGY As Integer = 0
	Friend Const VERSIONMADEBY As UInteger = 0

	Const Z_DEFLATED As Integer = 8
	' The deflate compression method (the only one supported in this version) 


	Shared Sub New()
		DllLoader.Load()
	End Sub


	<DllImport(ZLibDll.Name32, EntryPoint := "inflateInit2_", ExactSpelling := True, CharSet := CharSet.Ansi)> _
	Private Shared Function inflateInit2_32(ByRef strm As z_stream, windowBits As Integer, version As String, stream_size As Integer) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "inflateInit2_", ExactSpelling := True, CharSet := CharSet.Ansi)> _
	Private Shared Function inflateInit2_64(ByRef strm As z_stream, windowBits As Integer, version As String, stream_size As Integer) As Integer
	End Function

	Friend Shared Function inflateInit(ByRef strm As z_stream, windowBits As ZLibOpenType) As Integer
		If ZLibDll.Is64 Then
			Return inflateInit2_64(strm, CInt(windowBits), ZLib.ZLibVersion, Marshal.SizeOf(GetType(z_stream)))
		Else
			Return inflateInit2_32(strm, CInt(windowBits), ZLib.ZLibVersion, Marshal.SizeOf(GetType(z_stream)))
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "deflateInit2_", ExactSpelling := True, CharSet := CharSet.Ansi)> _
	Private Shared Function deflateInit2_32(ByRef strm As z_stream, level As Integer, method As Integer, windowBits As Integer, memLevel As Integer, strategy As Integer, _
		version As String, stream_size As Integer) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "deflateInit2_", ExactSpelling := True, CharSet := CharSet.Ansi)> _
	Private Shared Function deflateInit2_64(ByRef strm As z_stream, level As Integer, method As Integer, windowBits As Integer, memLevel As Integer, strategy As Integer, _
		version As String, stream_size As Integer) As Integer
	End Function

	Friend Shared Function deflateInit(ByRef strm As z_stream, level As CompressionLevel, windowBits As ZLibWriteType) As Integer
		If ZLibDll.Is64 Then
			Return deflateInit2_64(strm, CInt(level), Z_DEFLATED, CInt(windowBits), DEF_MEM_LEVEL, Z_DEFAULT_STRATEGY, _
				ZLibVersion, Marshal.SizeOf(GetType(z_stream)))
		Else
			Return deflateInit2_32(strm, CInt(level), Z_DEFLATED, CInt(windowBits), DEF_MEM_LEVEL, Z_DEFAULT_STRATEGY, _
				ZLibVersion, Marshal.SizeOf(GetType(z_stream)))
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "inflate", ExactSpelling := True)> _
	Private Shared Function inflate_32(ByRef strm As z_stream, flush As ZLibFlush) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "inflate", ExactSpelling := True)> _
	Private Shared Function inflate_64(ByRef strm As z_stream, flush As ZLibFlush) As Integer
	End Function

	Friend Shared Function inflate(ByRef strm As z_stream, flush As ZLibFlush) As Integer
		If ZLibDll.Is64 Then
			Return inflate_64(strm, flush)
		Else
			Return inflate_32(strm, flush)
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "deflate", ExactSpelling := True)> _
	Private Shared Function deflate_32(ByRef strm As z_stream, flush As ZLibFlush) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "deflate", ExactSpelling := True)> _
	Private Shared Function deflate_64(ByRef strm As z_stream, flush As ZLibFlush) As Integer
	End Function

	Friend Shared Function deflate(ByRef strm As z_stream, flush As ZLibFlush) As Integer
		If ZLibDll.Is64 Then
			Return deflate_64(strm, flush)
		Else
			Return deflate_32(strm, flush)
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "inflateEnd", ExactSpelling := True)> _
	Private Shared Function inflateEnd_32(ByRef strm As z_stream) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "inflateEnd", ExactSpelling := True)> _
	Private Shared Function inflateEnd_64(ByRef strm As z_stream) As Integer
	End Function

	Friend Shared Function inflateEnd(ByRef strm As z_stream) As Integer
		If ZLibDll.Is64 Then
			Return inflateEnd_64(strm)
		Else
			Return inflateEnd_32(strm)
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "deflateEnd", ExactSpelling := True)> _
	Private Shared Function deflateEnd_32(ByRef strm As z_stream) As Integer
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "deflateEnd", ExactSpelling := True)> _
	Private Shared Function deflateEnd_64(ByRef strm As z_stream) As Integer
	End Function

	Friend Shared Function deflateEnd(ByRef strm As z_stream) As Integer
		If ZLibDll.Is64 Then
			Return deflateEnd_64(strm)
		Else
			Return deflateEnd_32(strm)
		End If
	End Function

	<DllImport(ZLibDll.Name32, EntryPoint := "crc32", ExactSpelling := True)> _
	Private Shared Function crc32_32(crc As UInteger, buffer As IntPtr, len As UInteger) As UInteger
	End Function
	<DllImport(ZLibDll.Name64, EntryPoint := "crc32", ExactSpelling := True)> _
	Private Shared Function crc32_64(crc As UInteger, buffer As IntPtr, len As UInteger) As UInteger
	End Function

	Friend Shared Function crc32(crc As UInteger, buffer As IntPtr, len As UInteger) As UInteger
		If ZLibDll.Is64 Then
			Return crc32_64(crc, buffer, len)
		Else
			Return crc32_32(crc, buffer, len)
		End If
	End Function
End Class

Enum ZLibFlush
	NoFlush = 0
	'Z_NO_FLUSH
	PartialFlush = 1
	SyncFlush = 2
	FullFlush = 3
	Finish = 4
	' Z_FINISH
End Enum

Enum ZLibCompressionStrategy
	Filtered = 1
	HuffmanOnly = 2
	DefaultStrategy = 0
End Enum

'enum ZLibCompressionMethod
'{
'    Delated = 8
'}

Enum ZLibDataType
	Binary = 0
	Ascii = 1
	Unknown = 2
End Enum

Public Enum ZLibOpenType
	'If a compressed stream with a larger window
	'size is given as input, inflate() will return with the error code
	'Z_DATA_ERROR instead of trying to allocate a larger window.
	Deflate = -15
	' -8..-15
	ZLib = 15
	' 8..15, 0 = use the window size in the zlib header of the compressed stream.
	GZip = 15 + 16
	Both_ZLib_GZip = 15 + 32
End Enum

Public Enum ZLibWriteType
	'If a compressed stream with a larger window
	'size is given as input, inflate() will return with the error code
	'Z_DATA_ERROR instead of trying to allocate a larger window.
	Deflate = -15
	' -8..-15
	ZLib = 15
	' 8..15, 0 = use the window size in the zlib header of the compressed stream.
	GZip = 15 + 16
	'		Both = 15 + 32,
End Enum

Public Enum CompressionLevel
	NoCompression = 0
	BestSpeed = 1
	BestCompression = 9
	' The "real" default is -1. Currently, zlib interpret -1 as 6, but they are free to change the interpretation.
	' The reason for overriding the default and using 5 is I want this library to match DynaZip's default
	' compression ratio and speed, and 5 was the best match (6 was somewhat slower than dynazip default).
	[Default] = 5
	Level0 = 0
	Level1 = 1
	Level2 = 2
	Level3 = 3
	Level4 = 4
	Level5 = 5
	Level6 = 6
	Level7 = 7
	Level8 = 8
	Level9 = 9
End Enum

<StructLayout(LayoutKind.Sequential)> _
Structure z_stream
	Public next_in As IntPtr
	' next input byte 
	Public avail_in As UInteger
	' number of bytes available at next_in 
	Public total_in As UInteger
	' total nb of input bytes read so far 

	Public next_out As IntPtr
	' next output byte should be put there 
	Public avail_out As UInteger
	' remaining free space at next_out 
	Public total_out As UInteger
	' total nb of bytes output so far 

	Private msg As IntPtr
	' last error message, NULL if no error 

	Private state As IntPtr
	' not visible by applications 

	Private zalloc As IntPtr
	' used to allocate the internal state 
	Private zfree As IntPtr
	' used to free the internal state 
	Private opaque As IntPtr
	' private data object passed to zalloc and zfree 

	Public data_type As ZLibDataType
	' best guess about the data type: ascii or binary 
	Public adler As UInteger
	' adler32 value of the uncompressed data 
	Private reserved As UInteger
	' reserved for future use 

	Public ReadOnly Property lasterrormsg() As String
		Get
			Return Marshal.PtrToStringAnsi(msg)
		End Get
	End Property
End Structure

Friend NotInheritable Class ZLibReturnCode
	Private Sub New()
	End Sub
	Public Const Ok As Integer = 0
	Public Const StreamEnd As Integer = 1
	'positive = no error
	Public Const NeedDictionary As Integer = 2
	'positive = no error?
	Public Const Errno As Integer = -1
	Public Const StreamError As Integer = -2
	Public Const DataError As Integer = -3
	'CRC
	Public Const MemoryError As Integer = -4
	Public Const BufferError As Integer = -5
	Public Const VersionError As Integer = -6

	Public Shared Function GetMesage(retCode As Integer) As String
		Select Case retCode
			Case ZLibReturnCode.Ok
				Return "No error"
			Case ZLibReturnCode.StreamEnd
				Return "End of stream reaced"
			Case ZLibReturnCode.NeedDictionary
				Return "A preset dictionary is needed"
			Case ZLibReturnCode.Errno
				'consult error code
				Return "Unknown error " & Marshal.GetLastWin32Error()
			Case ZLibReturnCode.StreamError
				Return "Stream error"
			Case ZLibReturnCode.DataError
				Return "Data was corrupted"
			Case ZLibReturnCode.MemoryError
				Return "Out of memory"
			Case ZLibReturnCode.BufferError
				Return "Not enough room in provided buffer"
			Case ZLibReturnCode.VersionError
				Return "Incompatible zlib library version"
			Case Else
				Return "Unknown error"
		End Select
	End Function
End Class


<Serializable> _
Public Class ZLibException
	Inherits ApplicationException
	Public Sub New(info As SerializationInfo, context As StreamingContext)
		MyBase.New(info, context)
	End Sub

	Public Sub New(errorCode As Integer)

		MyBase.New(GetMsg(errorCode, Nothing))
	End Sub

	Public Sub New(errorCode As Integer, lastStreamError As String)
		MyBase.New(GetMsg(errorCode, lastStreamError))
	End Sub

	Private Shared Function GetMsg(errorCode As Integer, lastStreamError As String) As String
		Dim msg As String = "ZLib error " & errorCode & ": " & ZLibReturnCode.GetMesage(errorCode)
		If lastStreamError IsNot Nothing AndAlso lastStreamError.Length > 0 Then
			msg += " (" & lastStreamError & ")"
		End If
		Return msg
	End Function
End Class

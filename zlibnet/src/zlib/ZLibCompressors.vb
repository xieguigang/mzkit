Imports System.IO

''' <summary>
''' Classes that simplify a common use of compression streams
''' </summary>

Delegate Function CreateStreamDelegate(s As Stream, cm As CompressionMode, leaveOpen As Boolean) As DeflateStream

Public NotInheritable Class DeflateCompressor
	Private Sub New()
	End Sub
	Public Shared Function Compress(source As Stream) As MemoryStream
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Stream) As MemoryStream
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Public Shared Function Compress(source As Byte()) As Byte()
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Byte()) As Byte()
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Private Shared Function CreateStream(s As Stream, cm As CompressionMode, leaveOpen As Boolean) As DeflateStream
		Return New DeflateStream(s, cm, leaveOpen)
	End Function
End Class

Public NotInheritable Class GZipCompressor
	Private Sub New()
	End Sub
	Public Shared Function Compress(source As Stream) As MemoryStream
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Stream) As MemoryStream
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Public Shared Function Compress(source As Byte()) As Byte()
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Byte()) As Byte()
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Private Shared Function CreateStream(s As Stream, cm As CompressionMode, leaveOpen As Boolean) As DeflateStream
		Return New GZipStream(s, cm, leaveOpen)
	End Function
End Class

Public NotInheritable Class ZLibCompressor
	Private Sub New()
	End Sub
	Public Shared Function Compress(source As Stream) As MemoryStream
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Stream) As MemoryStream
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Public Shared Function Compress(source As Byte()) As Byte()
		Return CommonCompressor.Compress(AddressOf CreateStream, source)
	End Function
	Public Shared Function DeCompress(source As Byte()) As Byte()
		Return CommonCompressor.DeCompress(AddressOf CreateStream, source)
	End Function
	Private Shared Function CreateStream(s As Stream, cm As CompressionMode, leaveOpen As Boolean) As DeflateStream
		Return New ZLibStream(s, cm, leaveOpen)
	End Function
End Class

Public NotInheritable Class DynazipCompressor
	Private Sub New()
	End Sub
	Const DZ_DEFLATE_POS As Integer = 46

	Public Shared Function IsDynazip(source As Byte()) As Boolean
		Return source.Length >= 4 AndAlso BitConverter.ToInt32(source, 0) = &H2014b50
	End Function

	Public Shared Function DeCompress(source As Byte()) As Byte()
		If Not IsDynazip(source) Then
			Throw New InvalidDataException("not dynazip header")
		End If
		Using srcStream As New MemoryStream(source, DZ_DEFLATE_POS, source.Length - DZ_DEFLATE_POS)
			Using dstStream As MemoryStream = DeCompress(srcStream)
				Return dstStream.ToArray()
			End Using
		End Using
	End Function

	Private Shared Function DeCompress(source As Stream) As MemoryStream
		Dim dest As New MemoryStream()
		DeCompress(source, dest)
		dest.Position = 0
		Return dest
	End Function

	Private Shared Sub DeCompress(source As Stream, dest As Stream)
		Using zsSource As New DeflateStream(source, CompressionMode.Decompress, True)
			zsSource.CopyTo(dest)
		End Using
	End Sub
End Class

Class CommonCompressor
	Private Shared Sub Compress(sc As CreateStreamDelegate, source As Stream, dest As Stream)
		Using zsDest As DeflateStream = sc(dest, CompressionMode.Compress, True)
			source.CopyTo(zsDest)
		End Using
	End Sub

	Private Shared Sub DeCompress(sc As CreateStreamDelegate, source As Stream, dest As Stream)
		Using zsSource As DeflateStream = sc(source, CompressionMode.Decompress, True)
			zsSource.CopyTo(dest)
		End Using
	End Sub

	Public Shared Function Compress(sc As CreateStreamDelegate, source As Stream) As MemoryStream
		Dim result As New MemoryStream()
		Compress(sc, source, result)
		result.Position = 0
		Return result
	End Function

	Public Shared Function DeCompress(sc As CreateStreamDelegate, source As Stream) As MemoryStream
		Dim result As New MemoryStream()
		DeCompress(sc, source, result)
		result.Position = 0
		Return result
	End Function

	Public Shared Function Compress(sc As CreateStreamDelegate, source As Byte()) As Byte()
		Using srcStream As New MemoryStream(source)
			Using dstStream As MemoryStream = Compress(sc, srcStream)
				Return dstStream.ToArray()
			End Using
		End Using
	End Function

	Public Shared Function DeCompress(sc As CreateStreamDelegate, source As Byte()) As Byte()
		Using srcStream As New MemoryStream(source)
			Using dstStream As MemoryStream = DeCompress(sc, srcStream)
				Return dstStream.ToArray()
			End Using
		End Using
	End Function
End Class

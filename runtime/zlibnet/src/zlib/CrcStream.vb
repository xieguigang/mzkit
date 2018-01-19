Imports System.IO

Public Class CrcStream
    Inherits Stream

    Private pCrcValue As UInteger = 0
	Private pStream As Stream

	Public Sub New(stream As Stream)
		Me.pStream = stream
	End Sub

	Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
		Dim readLen As Integer = pStream.Read(buffer, offset, count)
		Using bufferPtr As New FixedArray(buffer)
			pCrcValue = ZLib.crc32(pCrcValue, bufferPtr(offset), CUInt(readLen))
		End Using
		Return readLen
	End Function

	Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
		pStream.Write(buffer, offset, count)
		Using bufferPtr As New FixedArray(buffer)
			pCrcValue = ZLib.crc32(pCrcValue, bufferPtr(offset), CUInt(count))
		End Using
	End Sub

	Public Overrides Sub Flush()
		Me.pStream.Flush()
	End Sub

	Public ReadOnly Property CRC32() As UInteger
		Get
			Return pCrcValue
		End Get
	End Property

	Public Overrides ReadOnly Property CanRead() As Boolean
		Get
			Return pStream.CanRead
		End Get
	End Property

	Public Overrides ReadOnly Property CanWrite() As Boolean
		Get
			Return pStream.CanWrite
		End Get
	End Property

	Public Overrides ReadOnly Property CanSeek() As Boolean
		Get
			Return (pStream.CanSeek)
		End Get
	End Property

	Public ReadOnly Property BaseStream() As Stream
		Get
			Return (Me.pStream)
		End Get
	End Property

	Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
		Return pStream.Seek(offset, origin)
	End Function

	Public Overrides Sub SetLength(value As Long)
		pStream.SetLength(value)
	End Sub

	Public Overrides ReadOnly Property Length() As Long
		Get
			Return pStream.Length
		End Get
	End Property

	Public Overrides Property Position() As Long
		Get
			Return pStream.Position
		End Get
		Set
			pStream.Position = value
		End Set
	End Property
End Class

Public NotInheritable Class CrcCalculator
	Private Sub New()
	End Sub
	Public Shared Function CaclulateCRC32(buffer As Byte()) As UInteger
		Using bufferPtr As New FixedArray(buffer)
			Return ZLib.crc32(0, bufferPtr, CUInt(buffer.Length))
		End Using
	End Function
End Class

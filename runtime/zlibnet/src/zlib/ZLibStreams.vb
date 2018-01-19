Imports System.IO

''' <summary>Provides methods and properties used to compress and decompress streams.</summary>
Public Class DeflateStream
	Inherits Stream
	'		private const int BufferSize = 16384;

	Private pBytesIn As Long = 0
	Private pBytesOut As Long = 0
	Private pSuccess As Boolean
	'		uint pCrcValue = 0;
	Const WORK_DATA_SIZE As Integer = &H1000
	Private pWorkData As Byte() = New Byte(WORK_DATA_SIZE - 1) {}
	Private pWorkDataPos As Integer = 0

	Private pStream As Stream
	Private pCompMode As CompressionMode
	Private pZstream As New z_stream()
	Private pLeaveOpen As Boolean

	Public Sub New(stream As Stream, mode As CompressionMode)
		Me.New(stream, mode, CompressionLevel.[Default])
	End Sub

	Public Sub New(stream As Stream, mode As CompressionMode, leaveOpen As Boolean)
		Me.New(stream, mode, CompressionLevel.[Default], leaveOpen)
	End Sub

	Public Sub New(stream As Stream, mode As CompressionMode, level As CompressionLevel)
		Me.New(stream, mode, level, False)
	End Sub

	Public Sub New(stream As Stream, compMode As CompressionMode, level As CompressionLevel, leaveOpen As Boolean)
		Me.pLeaveOpen = leaveOpen
		Me.pStream = stream
		Me.pCompMode = compMode

        Dim ret As Integer
		If Me.pCompMode = CompressionMode.Compress Then
			ret = ZLib.deflateInit(pZstream, level, Me.WriteType)
		Else
			ret = ZLib.inflateInit(pZstream, Me.OpenType)
		End If

		If ret <> ZLibReturnCode.Ok Then
			Throw New ZLibException(ret, pZstream.lasterrormsg)
		End If

		pSuccess = True
	End Sub

	''' <summary>GZipStream destructor. Cleans all allocated resources.</summary>
	Protected Overrides Sub Finalize()
		Try
			Me.Dispose(False)
		Finally
			MyBase.Finalize()
		End Try
	End Sub

	''' <summary>
	''' Stream.Close() ->   this.Dispose(true); + GC.SuppressFinalize(this);
	''' Stream.Dispose() ->  this.Close();
	''' </summary>
	''' <param name="disposing"></param>
	Protected Overrides Sub Dispose(disposing As Boolean)
		Try
			Try
				If disposing Then
					'managed stuff
					If Me.pStream IsNot Nothing Then
						'managed stuff
						If Me.pCompMode = CompressionMode.Compress AndAlso pSuccess Then
								'								this.pStream.Flush();
							Flush()
						End If
						If Not pLeaveOpen Then
							Me.pStream.Close()
						End If
						Me.pStream = Nothing
					End If
				End If
			Finally
				'unmanaged stuff
				FreeUnmanagedResources()

			End Try
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	' Finished, free the resources used.
	Private Sub FreeUnmanagedResources()
		If Me.pCompMode = CompressionMode.Compress Then
			ZLib.deflateEnd(pZstream)
		Else
			ZLib.inflateEnd(pZstream)
		End If
	End Sub

	Protected Overridable ReadOnly Property OpenType() As ZLibOpenType
		Get
			Return ZLibOpenType.Deflate
		End Get
	End Property
	Protected Overridable ReadOnly Property WriteType() As ZLibWriteType
		Get
			Return ZLibWriteType.Deflate
		End Get
	End Property

    ''' <summary>Reads a number of decompressed bytes into the specified byte array.</summary>
    ''' <param name="buffer">The array used to store decompressed bytes.</param>
    ''' <param name="offset">The location in the array to begin reading.</param>
    ''' <param name="count">The number of bytes decompressed.</param>
    ''' <returns>The number of bytes that were decompressed into the byte array. If the end of the stream has been reached, zero or the number of bytes read is returned.</returns>
    Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
		If pCompMode = CompressionMode.Compress Then
			Throw New NotSupportedException("Can't read on a compress stream!")
		End If

		Dim readLen As Integer = 0
		If pWorkDataPos <> -1 Then
			Using workDataPtr As New FixedArray(pWorkData)
				Using bufferPtr As New FixedArray(buffer)
					pZstream.next_in = workDataPtr(pWorkDataPos)
					pZstream.next_out = bufferPtr(offset)
					pZstream.avail_out = CUInt(count)

					While pZstream.avail_out <> 0
						If pZstream.avail_in = 0 Then
							pWorkDataPos = 0
							pZstream.next_in = workDataPtr
							pZstream.avail_in = CUInt(pStream.Read(pWorkData, 0, WORK_DATA_SIZE))
							pBytesIn += pZstream.avail_in
						End If

						Dim inCount As UInteger = pZstream.avail_in
						Dim outCount As UInteger = pZstream.avail_out

						Dim zlibError As Integer = ZLib.inflate(pZstream, ZLibFlush.NoFlush)
						' flush method for inflate has no effect
						pWorkDataPos += CInt(inCount - pZstream.avail_in)
						readLen += CInt(outCount - pZstream.avail_out)

						If zlibError = ZLibReturnCode.StreamEnd Then
							pWorkDataPos = -1
							' magic for StreamEnd
							Exit While
						ElseIf zlibError <> ZLibReturnCode.Ok Then
							pSuccess = False
							Throw New ZLibException(zlibError, pZstream.lasterrormsg)
						End If
					End While

                    pBytesOut += readLen
				End Using

			End Using
		End If
		Return readLen
	End Function

    ''' <summary>This property is not supported and always throws a NotSupportedException.</summary>
    ''' <param name="buffer">The array used to store compressed bytes.</param>
    ''' <param name="offset">The location in the array to begin reading.</param>
    ''' <param name="count">The number of bytes compressed.</param>
    Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
		If pCompMode = CompressionMode.Decompress Then
			Throw New NotSupportedException("Can't write on a decompression stream!")
		End If

		pBytesIn += count

		Using writePtr As New FixedArray(pWorkData)
			Using bufferPtr As New FixedArray(buffer)
				pZstream.next_in = bufferPtr(offset)
				pZstream.avail_in = CUInt(count)
				pZstream.next_out = writePtr(pWorkDataPos)
				pZstream.avail_out = CUInt(WORK_DATA_SIZE - pWorkDataPos)

				'				pCrcValue = crc32(pCrcValue, &bufferPtr[offset], (uint)count);

				While pZstream.avail_in <> 0
					If pZstream.avail_out = 0 Then
						'rar logikk, men det betyr vel bare at den kun skriver hvis buffer ble fyllt helt,
						'dvs halvfyllt buffer vil kun skrives ved flush
						pStream.Write(pWorkData, 0, CInt(WORK_DATA_SIZE))
						pBytesOut += WORK_DATA_SIZE
						pWorkDataPos = 0
						pZstream.next_out = writePtr
						pZstream.avail_out = WORK_DATA_SIZE
					End If

					Dim outCount As UInteger = pZstream.avail_out

					Dim zlibError As Integer = ZLib.deflate(pZstream, ZLibFlush.NoFlush)

					pWorkDataPos += CInt(outCount - pZstream.avail_out)

					If zlibError <> ZLibReturnCode.Ok Then
						pSuccess = False
						Throw New ZLibException(zlibError, pZstream.lasterrormsg)

					End If
				End While
			End Using
		End Using
	End Sub

	''' <summary>Flushes the contents of the internal buffer of the current GZipStream object to the underlying stream.</summary>
	Public Overrides Sub Flush()
		If pCompMode = CompressionMode.Decompress Then
			Throw New NotSupportedException("Can't flush a decompression stream.")
		End If

		Using workDataPtr As New FixedArray(pWorkData)
			pZstream.next_in = IntPtr.Zero
			pZstream.avail_in = 0
			pZstream.next_out = workDataPtr(pWorkDataPos)
			pZstream.avail_out = CUInt(WORK_DATA_SIZE - pWorkDataPos)

			Dim zlibError As Integer = ZLibReturnCode.Ok
			While zlibError <> ZLibReturnCode.StreamEnd
				If pZstream.avail_out <> 0 Then
					Dim outCount As UInteger = pZstream.avail_out
					zlibError = ZLib.deflate(pZstream, ZLibFlush.Finish)

					pWorkDataPos += CInt(outCount - pZstream.avail_out)
							'ok. will break loop
					If zlibError = ZLibReturnCode.StreamEnd Then
					ElseIf zlibError <> ZLibReturnCode.Ok Then
						pSuccess = False
						Throw New ZLibException(zlibError, pZstream.lasterrormsg)
					End If
				End If

				pStream.Write(pWorkData, 0, pWorkDataPos)
				pBytesOut += pWorkDataPos
				pWorkDataPos = 0
				pZstream.next_out = workDataPtr
				pZstream.avail_out = WORK_DATA_SIZE
			End While
		End Using

		Me.pStream.Flush()
	End Sub


	'public uint CRC32
	'{
	'    get
	'    {
	'        return pCrcValue;
	'    }
	'}

	Public ReadOnly Property TotalIn() As Long
		Get
			Return Me.pBytesIn
		End Get
	End Property

	Public ReadOnly Property TotalOut() As Long
		Get
			Return Me.pBytesOut
		End Get
	End Property

	' The compression ratio obtained (same for compression/decompression).
	Public ReadOnly Property CompressionRatio() As Double
		Get
			If pCompMode = CompressionMode.Compress Then
				Return (If((pBytesIn = 0), 0.0, (100.0 - (CDbl(pBytesOut) * 100.0 / CDbl(pBytesIn)))))
			Else
				Return (If((pBytesOut = 0), 0.0, (100.0 - (CDbl(pBytesIn) * 100.0 / CDbl(pBytesOut)))))
			End If
		End Get
	End Property

	''' <summary>Gets a value indicating whether the stream supports reading while decompressing a file.</summary>
	Public Overrides ReadOnly Property CanRead() As Boolean
		Get
			Return pCompMode = CompressionMode.Decompress AndAlso pStream.CanRead
		End Get
	End Property

	''' <summary>Gets a value indicating whether the stream supports writing.</summary>
	Public Overrides ReadOnly Property CanWrite() As Boolean
		Get
			Return pCompMode = CompressionMode.Compress AndAlso pStream.CanWrite
		End Get
	End Property

	''' <summary>Gets a value indicating whether the stream supports seeking.</summary>
	Public Overrides ReadOnly Property CanSeek() As Boolean
		Get
			Return (False)
		End Get
	End Property

	''' <summary>Gets a reference to the underlying stream.</summary>
	Public ReadOnly Property BaseStream() As Stream
		Get
			Return (Me.pStream)
		End Get
	End Property

	''' <summary>This property is not supported and always throws a NotSupportedException.</summary>
	''' <param name="offset">The location in the stream.</param>
	''' <param name="origin">One of the SeekOrigin values.</param>
	''' <returns>A long value.</returns>
	Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
		Throw New NotSupportedException("Seek not supported")
	End Function

	''' <summary>This property is not supported and always throws a NotSupportedException.</summary>
	''' <param name="value">The length of the stream.</param>
	Public Overrides Sub SetLength(value As Long)
		Throw New NotSupportedException("SetLength not supported")
	End Sub

	''' <summary>This property is not supported and always throws a NotSupportedException.</summary>
	Public Overrides ReadOnly Property Length() As Long
		Get
			Throw New NotSupportedException("Length not supported.")
		End Get
	End Property

	''' <summary>This property is not supported and always throws a NotSupportedException.</summary>
	Public Overrides Property Position() As Long
		Get
			Throw New NotSupportedException("Position not supported.")
		End Get
		Set
			Throw New NotSupportedException("Position not supported.")
		End Set
	End Property
End Class

''' <summary>
''' hdr(?) + adler32 et end.
''' wraps a deflate stream
''' </summary>
Public Class ZLibStream
	Inherits DeflateStream
	Public Sub New(stream As Stream, mode As CompressionMode)
		MyBase.New(stream, mode)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, leaveOpen As Boolean)
		MyBase.New(stream, mode, leaveOpen)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, level As CompressionLevel)
		MyBase.New(stream, mode, level)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, level As CompressionLevel, leaveOpen As Boolean)
		MyBase.New(stream, mode, level, leaveOpen)
	End Sub

	Protected Overrides ReadOnly Property OpenType() As ZLibOpenType
		Get
			Return ZLibOpenType.ZLib
		End Get
	End Property
	Protected Overrides ReadOnly Property WriteType() As ZLibWriteType
		Get
			Return ZLibWriteType.ZLib
		End Get
	End Property
End Class

''' <summary>
''' Saved to file (.gz) can be opened with zip utils.
''' Have hdr + crc32 at end.
''' Wraps a deflate stream
''' </summary>
Public Class GZipStream
	Inherits DeflateStream
	Public Sub New(stream As Stream, mode As CompressionMode)
		MyBase.New(stream, mode)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, leaveOpen As Boolean)
		MyBase.New(stream, mode, leaveOpen)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, level As CompressionLevel)
		MyBase.New(stream, mode, level)
	End Sub
	Public Sub New(stream As Stream, mode As CompressionMode, level As CompressionLevel, leaveOpen As Boolean)
		MyBase.New(stream, mode, level, leaveOpen)
	End Sub

	Protected Overrides ReadOnly Property OpenType() As ZLibOpenType
		Get
			Return ZLibOpenType.GZip
		End Get
	End Property
	Protected Overrides ReadOnly Property WriteType() As ZLibWriteType
		Get
			Return ZLibWriteType.GZip
		End Get
	End Property
End Class


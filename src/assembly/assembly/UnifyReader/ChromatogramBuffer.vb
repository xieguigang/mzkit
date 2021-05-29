Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Namespace DataReader

    Public Module ChromatogramBuffer

        ''' <summary>
        ''' get data buffer of a <see cref="Chromatogram"/> data object.
        ''' </summary>
        ''' <param name="chr"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetBytes(chr As Chromatogram) As Byte()
            Using buffer As New MemoryStream, bin As New BinaryDataWriter(buffer) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Call bin.Write(chr.length)
                Call bin.Write(chr.scan_time)
                Call bin.Write(chr.TIC)
                Call bin.Write(chr.BPC)
                Call bin.Flush()

                Return buffer.ToArray
            End Using
        End Function

        Public Function FromBuffer(buffer As Stream) As Chromatogram
            Using bin As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.BigEndian}
                Dim n As Integer = bin.ReadInt32
                Dim scan_time As Double() = bin.ReadDoubles(n)
                Dim TiC As Double() = bin.ReadDoubles(n)
                Dim BpC As Double() = bin.ReadDoubles(n)

                Return New Chromatogram With {
                    .scan_time = scan_time,
                    .BPC = BpC,
                    .TIC = TiC
                }
            End Using
        End Function

        Public Function MeasureSize(len As Integer) As Long
            ' scan_time double
            ' TIC double
            ' BPC double
            ' count integer
            Dim size As Long = len * 8 * 3 + 4
            Return size
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function MeasureSize(chr As Chromatogram) As Long
            Return MeasureSize(chr.length)
        End Function

    End Module
End Namespace
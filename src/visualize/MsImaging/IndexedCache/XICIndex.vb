Imports System.IO
Imports Microsoft.VisualBasic.Data.IO

Namespace IndexedCache

    Public Class XICIndex

        Public Property mz As Double()
        Public Property offset As Long()
        Public Property width As Integer
        Public Property height As Integer
        ''' <summary>
        ''' the file name of the upstream source file
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        Public Property tolerance As String

        Public Const MagicHeader As String = "BioNovoGene/MSI"

        Public Shared Sub WriteIndexFile(cache As XICWriter, file As Stream)
            Dim mz As Double() = cache.offsets.Keys.ToArray
            Dim nfeatures As Integer = mz.Length

            Using out As New BinaryDataWriter(file) With {.ByteOrder = ByteOrder.BigEndian}
                Call out.Write(MagicHeader, BinaryStringFormat.NoPrefixOrTermination)
                ' write meta data
                Call out.Write(nfeatures)
                Call out.Write(cache.width)
                Call out.Write(cache.height)
                Call out.Write(cache.src, BinaryStringFormat.ZeroTerminated)
                Call out.Write(cache.tolerance.GetScript, BinaryStringFormat.ZeroTerminated)
                Call out.Write(Now.ToString, BinaryStringFormat.ZeroTerminated)
                Call out.Write(CByte(0))
                Call out.Write(mz)
                Call out.Flush()

                Dim offsetPos As Long = out.Position

                ' write placeholder
                Call out.Write(mz.Select(Function(any) 0&).ToArray)
                Call out.Write(CByte(0))
                Call out.Flush()

                Dim offsets As New Dictionary(Of Double, Long)

                Call cache.Dispose()

                Using cachefile As New BinaryDataReader(cache.cache.Open(FileMode.Open))
                    For Each mzi As Double In mz
                        Dim offset As Long = cache.offsets(mzi).position
                        Dim nlen As Integer = cache.length(mzi)
                        Dim size As Integer = XICWriter.delta * nlen
                        ' [x,y] intensity
                        Dim bytes = cachefile.ReadBytes(size)
                        Dim x As Integer() = New Integer(nlen - 1) {}
                        Dim y As Integer() = New Integer(nlen - 1) {}
                        Dim intensity As Double() = New Double(nlen - 1) {}

                        Using ms As New MemoryStream(bytes), temp As New BinaryDataReader(ms)
                            For i As Integer = 0 To intensity.Length - 1
                                Dim xy = temp.ReadInt32s(2)

                                x(i) = xy(0)
                                y(i) = xy(1)
                                intensity(i) = temp.ReadDouble
                            Next
                        End Using

                        Erase bytes

                        offsets(mzi) = out.Position

                        Call out.Write(mzi)
                        Call out.Write(nlen)
                        Call out.Write(intensity)
                        Call out.Write(x)
                        Call out.Write(y)
                        Call out.Write(CByte(0))
                        Call out.Flush()
                    Next
                End Using

                out.Seek(offsetPos, SeekOrigin.Begin)
                out.Write(mz.Select(Function(mzi) offsets(mzi)).ToArray)
                out.Flush()
            End Using
        End Sub

    End Class
End Namespace
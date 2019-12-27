Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Emit.Marshal
Imports SMRUCC.MassSpectrum.Math.Spectra
Imports ANSI = Microsoft.VisualBasic.Text.ASCII

Namespace ASCII.MSN

    Public Class MsnList

        Public Property id As String
        Public Property msn As String
        Public Property type As String
        Public Property ionMode As String
        Public Property c As String
        Public Property instrument As String
        Public Property activation As String
        Public Property mz_range As DoubleRange
        Public Property ms2 As ms2()

        Public Overrides Function ToString() As String
            Return id
        End Function

        Friend Shared Function GetMsnList(file As String) As MsnList()
            Dim ions As MsnList() = file _
                .IterateAllLines _
                .SkipWhile(Function(l) l.First = "#"c) _
                .Split(Function(line) line.First = ">"c, DelimiterLocation.NextFirst) _
                .Where(Function(b) Not b.IsNullOrEmpty) _
                .Select(AddressOf MsnList) _
                .ToArray

            Return ions
        End Function

        Private Shared Function MsnList(block As String()) As MsnList
            Dim i As Pointer(Of String) = block(Scan0).Split(ANSI.TAB)
            Dim ms2 As ms2() = block _
                .Skip(1) _
                .Select(Function(l)
                            Dim mzInto As Double() = l _
                                .Split(ANSI.TAB) _
                                .Select(Function(c) Val(c)) _
                                .ToArray
                            Dim fragment As New ms2 With {
                                .mz = mzInto(Scan0),
                                .intensity = mzInto(1),
                                .quantity = .intensity
                            }

                            Return fragment
                        End Function) _
                .ToArray

            Return New MsnList With {
                .id = ++i,
                .msn = ++i,
                .type = ++i,
                .ionMode = ++i,
                .c = ++i,
                .instrument = ++i,
                .activation = ++i,
                .mz_range = (++i) _
                    .Split("-"c) _
                    .Select(AddressOf Val) _
                    .ToArray,
                .ms2 = ms2
            }
        End Function

    End Class
End Namespace
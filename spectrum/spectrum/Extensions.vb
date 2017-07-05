Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Assembly

Public Module Extensions

    ''' <summary>
    ''' Creates plot data from matrix
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SpectrumFromMatrix(matrix As IEnumerable(Of LibraryMatrix)) As IEnumerable(Of spectrumData)
        Dim groups = matrix.GroupBy(Function(l) l.Name)

        For Each group As IGrouping(Of String, LibraryMatrix) In groups
            Yield New spectrumData With {
                .name = group.Key,
                .data = group _
                    .Select(Function(l)
                                Return New MSSignal With {
                                    .x = l.ProductMz,
                                    .y = l.LibraryIntensity
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function
End Module

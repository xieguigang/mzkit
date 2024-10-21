
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the package tools for the DIA spectrum data annotation
''' </summary>
<Package("DIA")>
Public Module DIASpectrumAnnotations

    ''' <summary>
    ''' make the spectrum set decompose into multiple spectrum groups via the NMF method
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="n">
    ''' the number of the target spectrum to decomposed, 
    ''' this number should be query from the DDA experiment 
    ''' database.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("dia_nmf")>
    Public Function dia_nmf(<RRawVectorArgument> spectrum As Object, n As Integer,
                            Optional maxItrs As Integer = 1000,
                            Optional tolerance As Double = 0.001,
                            Optional eps As Double = 0.0001,
                            Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(spectrum, env, suppress:=True)

        If pull.isError Then
            pull = pipeline.TryCreatePipeline(Of LibraryMatrix)(spectrum, env)

            If pull.isError Then
                Return pull.getError
            End If

            Dim libs = pull _
                .populates(Of LibraryMatrix)(env) _
                .ToArray

            pull = pipeline.CreateFromPopulator(
                Iterator Function() As IEnumerable(Of PeakMs2)
                    For Each mat As LibraryMatrix In libs
                        Yield New PeakMs2 With {
                            .lib_guid = mat.name,
                            .mzInto = mat.Array,
                            .mz = mat.parentMz
                        }
                    Next
                End Function)
        End If

        Dim groups As NamedCollection(Of PeakMs2)() = pull _
            .populates(Of PeakMs2)(env) _
            .DecomposeSpectrum(n,
                               maxItrs:=maxItrs,
                               tolerance:=tolerance,
                               eps:=eps) _
            .ToArray
        Dim list As New list With {
            .slots = groups _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return CObj(a.value)
                              End Function)
        }
        Dim sum As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each group In groups
            sum.slots(group.name) = New LibraryMatrix(
                name:=group.name,
                spectrum:=group _
                    .description _
                    .LoadJSON(Of ms2())
            )
        Next

        Call list.setAttribute("sum_spectrum", sum)

        Return list
    End Function

End Module

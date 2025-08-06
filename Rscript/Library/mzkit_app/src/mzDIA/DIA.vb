#Region "Microsoft.VisualBasic::a0b87120910e3263e1f9db7e573349bc, Rscript\Library\mzkit_app\src\mzDIA\DIA.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 133
    '    Code Lines: 97 (72.93%)
    ' Comment Lines: 18 (13.53%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (13.53%)
    '     File Size: 5.12 KB


    ' Module DIASpectrumAnnotations
    ' 
    '     Function: dia_nmf, pullMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' the package tools for the DIA spectrum data annotation
''' </summary>
<Package("DIA")>
Public Module DIASpectrumAnnotations

    Private Function pullMatrix(pull As pipeline, env As Environment) As pipeline
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

        Return pull
    End Function

    ''' <summary>
    ''' make the spectrum set decompose into multiple spectrum groups via the NMF method
    ''' </summary>
    ''' <param name="spectrum">
    ''' a set of the mzkit supported spectrum object.
    ''' </param>
    ''' <param name="n">
    ''' the number of the target spectrum to decomposed, 
    ''' this number should be query from the DDA experiment 
    ''' database.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a set of the decomposed spectrum object
    ''' </returns>
    <ExportAPI("dia_nmf")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function dia_nmf(<RRawVectorArgument> spectrum As Object, n As Integer,
                            Optional maxItrs As Integer = 1000,
                            Optional tolerance As Double = 0.001,
                            Optional eps As Double = 0.0001,
                            Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(spectrum, env, suppress:=True)

        If pull.isError Then
            pull = pipeline.TryCreatePipeline(Of LibraryMatrix)(spectrum, env, suppress:=True)

            If pull.isError Then
                If TypeOf spectrum Is SpectrumLine Then
                    pull = pipeline.CreateFromPopulator(DirectCast(spectrum, SpectrumLine).cluster)
                Else
                    Return pull.getError
                End If
            Else
                pull = pullMatrix(pull, env)
            End If
        End If

        Dim nmf As New DIADecompose(pull.populates(Of PeakMs2)(env), tqdm:=False)
        Dim groups As NamedCollection(Of PeakMs2)() = nmf _
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

        For Each group As NamedCollection(Of PeakMs2) In groups
            sum.slots(group.name) = New LibraryMatrix(
                name:=group.name,
                spectrum:=group _
                    .description _
                    .LoadJSON(Of ms2())
            )
        Next

        Dim sample_composition As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim ionpeaks_composition As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim composition = nmf.GetSampleComposition.ToArray

        sample_composition.rownames = composition.Select(Function(a) a.name).ToArray

        For i As Integer = 0 To n - 1
            Dim offset As Integer = i
            sample_composition.add($"decomposition_{i + 1}", composition.Select(Function(v) v(offset)))
        Next

        composition = nmf.GetIonPeaksComposition.ToArray
        ionpeaks_composition.rownames = composition.Select(Function(a) a.name).ToArray

        For i As Integer = 0 To n - 1
            Dim offset As Integer = i
            ionpeaks_composition.add($"decomposition_{i + 1}", composition.Select(Function(v) v(offset)))
        Next

        Call list.setAttribute("sum_spectrum", sum)
        Call list.setAttribute("sample_composition", sample_composition)
        Call list.setAttribute("ionpeaks_composition", ionpeaks_composition)

        Return list
    End Function

    <ExportAPI("peptide_lib")>
    <RApiReturn(GetType(PeptideMass))>
    Public Function createPeptideLib(len As Integer, <RRawVectorArgument(TypeCodes.string)> Optional adducts As Object = "[M+H]+|[M+Na]+|[M+K]+|[M+NH4]+|[M-H]-|[M+Acetate]-|[M+HCOO]-") As Object
        Dim precursors As String() = CLRVector.asCharacter(adducts)
        Dim seqs = PeptideMass.CreateLibrary(len).ToArray
        Dim libs = seqs.AsParallel _
            .Select(Function(si) PeptideMass.CalculateMass(si, precursors)) _
            .ToArray

        Return libs
    End Function

End Module

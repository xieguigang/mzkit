Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.SIMD
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module DIADecompose

    ''' <summary>
    ''' make the spectrum set decompose into multiple spectrum groups via the NMF method
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="n"></param>
    ''' <param name="maxItrs"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="eps"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function DecomposeSpectrum(spectrum As IEnumerable(Of PeakMs2),
                                               n As Integer,
                                               Optional maxItrs As Integer = 1000,
                                               Optional tolerance As Double = 0.001,
                                               Optional eps As Double = 0.0001) As IEnumerable(Of NamedCollection(Of PeakMs2))
        Dim specPool As New List(Of PeakMs2)
        Dim fragmentSet As New List(Of Double)

        For Each spec As PeakMs2 In spectrum
            Call specPool.Add(spec)
            Call fragmentSet.AddRange(spec.mzInto.Select(Function(a) a.mz))
        Next

        Dim fragments As MzPool = fragmentSet.CreateCentroidFragmentSet(window_size:=0.75)
        Dim rowPacks As New List(Of Double())
        Dim fragment_size As Integer = fragments.size

        For Each spec As PeakMs2 In specPool
            Dim mz As Double() = spec.mzInto.Select(Function(i) i.mz).ToArray
            Dim into As Double() = spec.mzInto.Select(Function(i) i.intensity).ToArray

            into = Divide.f64_op_divide_f64_scalar(into, into.Max)
            into = Multiply.f64_scalar_op_multiply_f64(100, into)

            Call rowPacks.Add(SpectraEncoder.DeconvoluteScan(mz, into, fragment_size, fragments))
        Next

        Dim decompose = NMF.Factorisation(
            A:=New NumericMatrix(rowPacks),
            k:=n,
            max_iterations:=maxItrs,
            tolerance:=tolerance,
            epsilon:=eps)
        Dim W = decompose.W
        Dim H = decompose.H
        Dim mzSet As Double() = fragments.ionSet

        ' produce the decompose spectrum set
        For i As Integer = 0 To n - 1
            Dim factor = H(i)
            Dim decomposer As New List(Of PeakMs2)
            Dim sum As Double() = New Double(mzSet.Length - 1) {}

            For j As Integer = 0 To rowPacks.Count - 1
                Dim offset = W(j)
                Dim weight = offset(i)

                If weight < 0.0001 Then
                    Continue For
                End If

                Dim intensity = (factor * rowPacks(j)) * weight
                Dim ms2 As ms2() = mzSet _
                    .Select(Function(mzi, k) New ms2(mzi, intensity(k))) _
                    .Where(Function(a) a.intensity > 0.001) _
                    .ToArray
                Dim spectral As New PeakMs2(specPool(j)) With {
                    .mzInto = ms2,
                    .lib_guid = specPool(j).lib_guid & $"_decompose_{i + 1}"
                }

                sum = Add.f64_op_add_f64(sum, (intensity / intensity.Max).Array)

                Call decomposer.Add(spectral)
            Next

            Yield New NamedCollection(Of PeakMs2) With {
                .name = $"decompose_{i + 1}",
                .value = decomposer.ToArray,
                .description = mzSet _
                    .Select(Function(mzi, k) New ms2(mzi, sum(k))) _
                    .ToArray _
                    .GetJson
            }
        Next
    End Function

End Module

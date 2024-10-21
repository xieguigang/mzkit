Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.SIMD

Public Module DIADecompose

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

        ' produce the decompose spectrum set

    End Function

End Module

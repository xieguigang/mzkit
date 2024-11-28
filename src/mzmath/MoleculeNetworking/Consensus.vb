Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SIMD

''' <summary>
''' Evaluate the consensus fragment peaks
''' </summary>
Public Class Consensus

    Public Property mz As Double
    Public Property intensity As Double
    Public Property ratio As Double

    Public Shared Iterator Function Measure(Of T As ISpectrum)(cluster As IEnumerable(Of T)) As IEnumerable(Of Consensus)
        ' upstream data maybe contains null value
        Dim total = (From spec As T
                     In cluster.SafeQuery
                     Where Not spec Is Nothing).ToArray

        If total.Length = 0 Then
            Return
        End If

        Dim mzSet As MzPool = total.Select(Function(a) a.GetIons) _
            .IteratesALL _
            .Select(Function(i) i.mz) _
            .CreateCentroidFragmentSet(window_size:=0.75, verbose:=False)
        Dim rowPacks As New List(Of Double())
        Dim fragment_size = mzSet.size

        For Each spec As T In total
            Dim ions = spec.GetIons.ToArray
            Dim mz As Double() = ions.Select(Function(i) i.mz).ToArray
            Dim into As Double() = ions.Select(Function(i) i.intensity).ToArray

            into = Divide.f64_op_divide_f64_scalar(into, into.Max)

            Call rowPacks.Add(SpectraEncoder.DeconvoluteScan(mz, into, fragment_size, mzSet))
        Next

        For i As Integer = 0 To fragment_size - 1
            Dim offset As Integer = i
            Dim v = rowPacks.Select(Function(a) a(offset)).ToArray
            Dim rank As Double = v.Sum / rowPacks.Count * 100
            Dim hits As Integer = v.Where(Function(a) a > 0).Count

            If hits > 1 Then
                Yield New Consensus With {
                    .mz = mzSet(i),
                    .intensity = rank,
                    .ratio = hits / rowPacks.Count
                }
            End If
        Next
    End Function

End Class

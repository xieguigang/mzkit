Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Public Module MultipleStageMS

    ''' <summary>
    ''' evaluate the correlation score for join the ms2 and ms3 products
    ''' </summary>
    ''' <param name="ms2">
    ''' intensity value of the spectrum data should be normalized before find the ms3 product
    ''' </param>
    ''' <param name="ms3">precursor information of the ms3 spectrum</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' rt of ms2 &amp; ms3 keeps the same, the rt of ms3 should be always greater than the rt of ms2;
    ''' precursor of the ms3 should be one of the top fragment peak of ms2
    ''' </remarks>
    Public Function MultipleStageCor(ms2 As PeakMs2, ms3 As IMs1, Optional maxdt As Double = 2) As Double
        Dim rt As Double = std.Abs(ms2.rt - ms3.rt) / maxdt
        Dim precursor As Double = ms3.mz
        Dim fragment2 = ms2.mzInto _
            .Where(Function(a) std.Abs(a.mz - ms3.mz) <= 0.3) _
            .Select(Function(a) a.intensity) _
            .ToArray

        If fragment2.Any AndAlso rt <= 1 Then
            Return rt * fragment2.Average
        Else
            Return 0
        End If
    End Function

End Module

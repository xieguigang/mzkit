Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' a spectrum grid line in the grid networking
''' </summary>
Public Class SpectrumLine

    Public Property cluster As PeakMs2()
    Public Property intensity As Double()
    Public Property rt As Double
    Public Property mz As Double

    Friend Function SetRT(rt As Double) As SpectrumLine
        _rt = rt
        Return Me
    End Function

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F3")}@{(rt / 60).ToString("F2")}min, {cluster.Length} files: {cluster.Select(Function(s) s.file).GetJson}"
    End Function

End Class
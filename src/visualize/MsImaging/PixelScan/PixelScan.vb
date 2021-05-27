Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public MustInherit Class PixelScan

    Public MustOverride ReadOnly Property X As Integer
    Public MustOverride ReadOnly Property Y As Integer

    Public MustOverride Function GetMs() As ms2()

End Class

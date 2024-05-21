Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Module [Global]

    ''' <summary>
    ''' 20 ppm
    ''' </summary>
    Public ReadOnly comparer As IEqualityComparer(Of ISpectrumPeak) = Tolerance.PPM(20)

End Module

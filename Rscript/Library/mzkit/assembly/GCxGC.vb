Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCxGC")>
Module GCxGC

    <ExportAPI("TIC2D")>
    Public Function TIC2D(TIC As ChromatogramTick(), modtime As Double) As D2Chromatogram()
        Return TIC.Demodulate2D(modtime)
    End Function

End Module

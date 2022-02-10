
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("mz_deco")>
Public Module mz_deco

    <ExportAPI("read.xcms_peaks")>
    Public Function readXcmsPeaks(file As String) As PeakSet
        Return New PeakSet With {.peaks = xcms2.Load(file)}
    End Function

    <ExportAPI("peak_subset")>
    Public Function peakSubset(peaktable As PeakSet, sampleNames As String()) As PeakSet
        Return peaktable.Subset(sampleNames)
    End Function
End Module

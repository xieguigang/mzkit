Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' GC-MS rawdata handler package module
''' </summary>
<Package("GCMS")>
Module GCMSData

    ''' <summary>
    ''' Create gc-ms features data object
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    <ExportAPI("gcms_features")>
    Public Function CreateGCMSFeatures(raw As mzPack, peaks As PeakSet) As GCMSPeak()
        Dim scans As PeakMs2() = raw.MS _
            .Select(Function(ms1)
                        Return New PeakMs2(ms1.scan_id, ms1.GetMs) With {.rt = ms1.rt}
                    End Function) _
            .ToArray
        Dim features As GCMSPeak() = peaks.peaks _
            .AsParallel _
            .Select(Function(ROI)
                        Return GCMSPeak.CreateFeature(scans, peak:=ROI)
                    End Function) _
            .ToArray

        Return features
    End Function

End Module

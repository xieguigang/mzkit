Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq

Module Module1

    Sub Main()
        Using file As New BinaryStreamReader("E:\test.mzPack")
            Dim dataMS1 As ScanMS1() = file.EnumerateIndex _
                .Select(Function(id) file.ReadScan(id, skipProducts:=True)) _
                .ToArray
            Dim scans As ms1_scan() = dataMS1 _
                .Select(Function(d) d.GetMs1Scans) _
                .IteratesALL _
                .ToArray

            Dim peaktable As PeakFeature() = scans.GetMzGroups(Tolerance.DeltaMass(0.05)).DecoMzGroups({3, 30}).ToArray

            Pause()
        End Using
    End Sub

End Module

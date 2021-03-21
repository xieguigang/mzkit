Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Module Module1

    Sub Main()
        Using file As New BinaryStreamReader("E:\test.mzPack")
            Dim dataMS1 As ScanMS1() = file.EnumerateIndex _
                .Select(Function(id) file.ReadScan(id, skipProducts:=True)) _
                .ToArray

        End Using
    End Sub

End Module

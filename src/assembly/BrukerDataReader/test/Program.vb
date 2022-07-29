Imports System
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader

Module Program
    Sub Main(args As String())
        Dim mcf_idx As New IndexParser("F:\MSI\YP202130530-V.d\b6ad08c2-7356-4a7c-88a5-47809c687c81_2.mcf_idx")

        For Each index In mcf_idx.GetContainerIndex
            Call Console.WriteLine(index.ToString)
        Next
    End Sub
End Module

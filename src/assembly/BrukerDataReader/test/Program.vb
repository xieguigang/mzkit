Imports System
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

Module Program
    Sub Main(args As String())
        Dim mcf_idx As New IndexParser("F:\MSI\YP202130530-V.d\b6ad08c2-7356-4a7c-88a5-47809c687c81_2.mcf_idx")
        Dim bytes As Byte()
        Dim blob As New mcfParser("F:\MSI\YP202130530-V.d\b6ad08c2-7356-4a7c-88a5-47809c687c81_2.mcf")

        For Each index In mcf_idx.GetContainerIndex
            Console.WriteLine(index.ToString)
            bytes = blob.GetBlob(index)
            bytes = New MemoryStream(bytes).UnGzipStream.ToArray

            Dim bin As New BinaryDataReader(bytes)
            Dim str = bin.ReadString(BinaryStringFormat.ZeroTerminated)

            '  Pause()
            Console.WriteLine()
        Next
    End Sub
End Module

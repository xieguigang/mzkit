
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Language

Namespace SingleCells

    ''' <summary>
    ''' single cells metabolomics rawdata toolkit
    ''' </summary>
    Public Module PackSingleCellsRaw

        ''' <summary>
        ''' pack the single cells data sample files into one dataset
        ''' </summary>
        ''' <param name="single_samples">
        ''' the single cell raw data sample files, one sample file may bundle multiple cell scan data.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function PackRawData(single_samples As IEnumerable(Of mzPack), Optional tag As String = Nothing) As mzPack
            Dim single_cells As New List(Of ScanMS1)
            Dim sample_names As New List(Of String)
            Dim metadata As New Dictionary(Of String, String)
            Dim sample_index As i32 = 1
            Dim total_cells As Integer = 0

            For Each sample As mzPack In single_samples
                Call metadata.Add($"sample_{++sample_index}", sample.source)
                Call VBDebugger.EchoLine($" processing {sample.source}...")

                For Each cell As ScanMS1 In sample.MS
                    cell.meta("cluster") = sample.source
                    cell.meta("sample") = sample.source

                    Call single_cells.Add(cell)
                Next
            Next

            Call metadata.Add("sample_sources", sample_names.Count)
            Call metadata.Add("total_cells", total_cells)

            Return New mzPack With {
                .Annotations = New Dictionary(Of String, String),
                .Application = FileApplicationClass.SingleCellsMetabolomics,
                .Chromatogram = Nothing,
                .metadata = metadata,
                .MS = single_cells.ToArray,
                .Scanners = Nothing,
                .source = If(tag, $"pack({sample_names.JoinBy(", ")})"),
                .Thumbnail = Nothing
            }
        End Function
    End Module
End Namespace
#Region "Microsoft.VisualBasic::6855149b6b57bf261755ad76b60b6058, assembly\Comprehensive\SingleCells\PackSingleCellsRaw.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 67
    '    Code Lines: 46 (68.66%)
    ' Comment Lines: 10 (14.93%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (16.42%)
    '     File Size: 2.74 KB


    '     Module PackSingleCellsRaw
    ' 
    '         Function: PackRawData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
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
        ''' <remarks>
        ''' all scan id for each ms1 scan data must be the single cell unique reference id
        ''' </remarks>
        <Extension>
        Public Function PackRawData(single_samples As IEnumerable(Of mzPack),
                                    Optional tag As String = Nothing,
                                    Optional ignore_ms2 As Boolean = True,
                                    Optional clean_source_tag As Boolean = False) As mzPack

            Dim single_cells As New List(Of ScanMS1)
            Dim sample_names As New List(Of String)
            Dim metadata As New Dictionary(Of String, String)
            Dim sample_index As i32 = 1
            Dim source_tag As String
            Dim bar As Tqdm.ProgressBar = Nothing

            For Each sample As mzPack In Tqdm.Wrap(single_samples.ToArray, bar:=bar)
                If sample.metadata IsNot Nothing AndAlso sample.metadata.ContainsKey("sample") Then
                    source_tag = sample.metadata!sample
                Else
                    source_tag = If(
                        clean_source_tag,
                        sample.source,
                        sample.source.BaseName
                    )
                End If

                Call sample_names.Add(source_tag)
                Call metadata.Add($"sample_{++sample_index}", source_tag)
                Call bar.SetLabel($" processing { source_tag}...")

                For Each cell As ScanMS1 In sample.MS
                    If cell.meta Is Nothing Then
                        cell.meta = New Dictionary(Of String, String)
                    End If
                    If ignore_ms2 Then
                        cell.products = Nothing
                    End If

                    cell.meta("cluster") = source_tag
                    cell.meta("sample") = source_tag

                    Call single_cells.Add(cell)
                Next
            Next

            sample_names = sample_names.Distinct.AsList

            Call metadata.Add("num_single_sources", sample_names.Count)
            Call metadata.Add("total_cells", single_cells.Count)

            Call VBDebugger.EchoLine($"number of single sources: {sample_names.Count}")
            Call VBDebugger.EchoLine($"total cells: {single_cells.Count}")

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

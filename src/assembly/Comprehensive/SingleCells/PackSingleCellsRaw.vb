#Region "Microsoft.VisualBasic::cc314e716d05fb0c4be7354afa08be45, assembly\Comprehensive\SingleCells\PackSingleCellsRaw.vb"

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

    '   Total Lines: 55
    '    Code Lines: 36 (65.45%)
    ' Comment Lines: 10 (18.18%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (16.36%)
    '     File Size: 2.17 KB


    '     Module PackSingleCellsRaw
    ' 
    '         Function: PackRawData
    ' 
    ' 
    ' /********************************************************************************/

#End Region


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

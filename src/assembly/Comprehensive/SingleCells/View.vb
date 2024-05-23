#Region "Microsoft.VisualBasic::ea2a98a62dcebf5008a84bcf03494828, assembly\Comprehensive\SingleCells\View.vb"

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

    '   Total Lines: 29
    '    Code Lines: 24 (82.76%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (17.24%)
    '     File Size: 1.08 KB


    '     Module View
    ' 
    '         Function: LoadScanMeta, ResolveSingleCells
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace SingleCells

    Public Module View

        <Extension>
        Public Function ResolveSingleCells(raw As mzPack) As IEnumerable(Of UMAPPoint)
            Return raw.MS.AsParallel.Select(Function(cell) LoadScanMeta(cell))
        End Function

        <Extension>
        Public Function LoadScanMeta(cell As ScanMS1) As UMAPPoint
            Dim meta As StringReader = StringReader.WrapDictionary(cell.meta)
            Dim cluster As String = meta.GetString("cluster")

            Return New UMAPPoint() With {
                .[class] = cluster,
                .label = cell.scan_id,
                .x = meta.GetDouble("umap1"),
                .y = meta.GetDouble("umap2"),
                .z = meta.GetDouble("umap3")
            }
        End Function
    End Module
End Namespace

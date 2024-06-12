#Region "Microsoft.VisualBasic::dd42b48f02529f4a13b72c7f24f6c316, Rscript\Library\mzkit_app\src\mzkit\comprehensive\SingleCellsPack.vb"

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
    '    Code Lines: 23 (79.31%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (20.69%)
    '     File Size: 1.05 KB


    ' Module SingleCellsPack
    ' 
    '     Function: PackSingleCells
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.SingleCells
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' package tools for the single cells metabolomics rawdata processing
''' </summary>
<Package("cellsPack")>
Module SingleCellsPack

    ''' <summary>
    ''' pack of the multiple raw data files into one data pack
    ''' </summary>
    ''' <param name="rawdata">a character vector of the raw data file path.</param>
    ''' <param name="source_tag">usually be the organism source name</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pack_cells")>
    Public Function PackSingleCells(<RRawVectorArgument>
                                    rawdata As Object,
                                    Optional source_tag As String = Nothing,
                                    Optional env As Environment = Nothing) As Object

        Dim cell_packs As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rawdata, env)

        If cell_packs.isError Then
            Return cell_packs.getError
        End If

        Return cell_packs _
            .populates(Of mzPack)(env) _
            .PackRawData(source_tag)
    End Function

    ''' <summary>
    ''' pack of the single cells metabolomics data in multiple sample groups
    ''' </summary>
    ''' <param name="groups">
    ''' could be a character vector of the folder path of the raw data files, 
    ''' it is recommended that using a tuple list for set this sample group value, 
    ''' the key name in the tuple list is the sample group name and the corresponding
    ''' value is the folder path of the single cells rawdata files.
    ''' </param>
    ''' <param name="source_tag">usually be the organism source name</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pack_cells.group")>
    Public Function PackSingleCellsInSampleGroup(<RRawVectorArgument> groups As Object,
                                                 Optional source_tag As String = Nothing,
                                                 Optional env As Environment = Nothing) As Object

    End Function

End Module

#Region "Microsoft.VisualBasic::0c266c54e0912f2fa4c9c61fdf9fef56, Rscript\Library\mzkit_app\src\mzkit\comprehensive\SingleCellsPack.vb"

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

    '   Total Lines: 145
    '    Code Lines: 80 (55.17%)
    ' Comment Lines: 49 (33.79%)
    '    - Xml Docs: 95.92%
    ' 
    '   Blank Lines: 16 (11.03%)
    '     File Size: 6.36 KB


    ' Module SingleCellsPack
    ' 
    '     Function: PackSingleCells, PackSingleCellsInSampleGroup
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.SingleCells
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' package tools for the single cells metabolomics rawdata processing
''' </summary>
''' <remarks>
''' works for the single-cell flow cytometry rawdata
''' </remarks>
<Package("cellsPack")>
Module SingleCellsPack

    ''' <summary>
    ''' pack of the multiple raw data files into one data pack
    ''' </summary>
    ''' <param name="rawdata">a character vector of the raw data file path.</param>
    ''' <param name="source_tag">usually be the organism source name</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function is different with the function ``pack_cells.group``, the rawdata file 
    ''' used at here is already been a sample pack, each scan inside the sample pack is a
    ''' single cell.
    ''' </remarks>
    <ExportAPI("pack_cells")>
    Public Function PackSingleCells(<RRawVectorArgument>
                                    rawdata As Object,
                                    Optional source_tag As String = Nothing,
                                    Optional env As Environment = Nothing) As Object

        Dim cell_packs As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rawdata, env)

        If cell_packs.isError Then
            cell_packs = CLRVector.asCharacter(rawdata) _
                .SafeQuery _
                .Select(Function(filepath)
                            Return MzWeb.openFromFile(filepath)
                        End Function) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)

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
    ''' <remarks>
    ''' this function required of each single rawdata file just contains only 
    ''' one single cell data.
    ''' </remarks>
    ''' <example>
    ''' require(mzkit);
    '''
    ''' # set single cells sample data folders
    ''' # each folder is one sample
    ''' let single_cell_source = ["/datafiles/bulk_group_1"
    ''' "/datafiles/bulk_group_2"
    ''' "/datafiles/bulk_group_3"];
    '''
    ''' # create single cells rawdata pack
    ''' single_cell_source 
    ''' |> pack_cells.group(tag = "Saccharomyces_cerevisiae")
    ''' |> write.mzPack(file = "/datafiles/Saccharomyces_cerevisiae.mzPack")
    ''' ;
    ''' </example>
    <ExportAPI("pack_cells.group")>
    Public Function PackSingleCellsInSampleGroup(<RRawVectorArgument> groups As Object,
                                                 Optional source_tag As String = Nothing,
                                                 Optional verbose As Boolean = False,
                                                 Optional env As Environment = Nothing) As Object
        Dim cellpacks As New List(Of mzPack)
        Dim raw As mzPack

        Call VBDebugger.EchoLine("read single cells raw data files...")

        If TypeOf groups Is list Then
            Dim dirlist As list = groups

            For Each tag As String In dirlist.getNames
                Dim pathSet As String() = CLRVector.asCharacter(dirlist.getByName(tag))

                For Each path As String In pathSet
                    If path.FileExists Then
                        raw = MzWeb.openFromFile(path, verbose:=verbose)
                        raw.source = $"{tag}-{raw.source}"
                        raw.metadata = New Dictionary(Of String, String) From {{"sample", tag}}
                        raw.MS(0).scan_id = raw.source
                        cellpacks.Add(raw)
                    Else
                        Dim rawfiles As String() = (ls - l - r - {"*.mzXML", "*.mzML", "*.mzPack"} <= path).ToArray

                        For Each file As String In Tqdm.Wrap(rawfiles)
                            raw = MzWeb.openFromFile(file, verbose:=verbose)
                            raw.source = $"{tag}-{raw.source}"
                            raw.metadata = New Dictionary(Of String, String) From {{"sample", tag}}
                            raw.MS(0).scan_id = raw.source
                            cellpacks.Add(raw)
                        Next
                    End If
                Next
            Next
        Else
            For Each path As String In CLRVector.asCharacter(groups)
                Dim tag As String = path.BaseName
                Dim rawfiles As String() = (ls - l - r - {"*.mzXML", "*.mzML", "*.mzPack"} <= path).ToArray

                For Each file As String In Tqdm.Wrap(rawfiles)
                    raw = MzWeb.openFromFile(file, verbose:=verbose)
                    raw.source = $"{tag}-{raw.source}"
                    raw.metadata = New Dictionary(Of String, String) From {{"sample", tag}}
                    raw.MS(0).scan_id = raw.source
                    cellpacks.Add(raw)
                Next
            Next
        End If

        Call VBDebugger.EchoLine($"get {cellpacks.Count} single cells!")

        Return cellpacks.PackRawData(source_tag, clean_source_tag:=True)
    End Function

End Module

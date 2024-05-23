#Region "Microsoft.VisualBasic::2a34f9ef3bbb87575f1ab2f44ffdf55f, assembly\Comprehensive\MsImaging\MergeSliders\MergeFakeSTImagingSliders.vb"

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

    '   Total Lines: 71
    '    Code Lines: 47 (66.20%)
    ' Comment Lines: 13 (18.31%)
    '    - Xml Docs: 84.62%
    ' 
    '   Blank Lines: 11 (15.49%)
    '     File Size: 3.23 KB


    ' Module MergeFakeSTImagingSliders
    ' 
    '     Function: JoinSTImagingSamples, MergeDataWithLayout, TweaksSTData
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Public Module MergeFakeSTImagingSliders

    ''' <summary>
    ''' merge the STImaging slide data files in linear
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <param name="relativePos"></param>
    ''' <param name="padding"></param>
    ''' <param name="norm"></param>
    ''' <param name="println"></param>
    ''' <returns>
    ''' generated a new faked ms-imaging data object for 10x genomics STImaging data 
    ''' </returns>
    Public Function JoinSTImagingSamples(samples As IEnumerable(Of mzPack),
                                         Optional relativePos As Boolean = True,
                                         Optional padding As Integer = 0,
                                         Optional norm As Boolean = True,
                                         Optional println As Action(Of String) = Nothing) As mzPack

        ' load polygon shape for each imaging slider
        Dim polygons = samples.PullPolygons(println).ToArray
        Dim unionGeneIds As Index(Of String) = polygons _
            .Select(Function(m) m.ms.Annotations.Values) _
            .IteratesALL _
            .Distinct _
            .Indexing
        Dim mergeSt As New MergeSTSlides(relativePos, norm, println, unionGeneIds)
        Dim union As mzPack = MergeSliders.JoinMSISamples(samples, relativePos, padding, norm, println, mergeSt)

        Return TweaksSTData(union, polygons.Select(Function(m) m.ms.metadata), unionGeneIds)
    End Function

    Private Function TweaksSTData(union As mzPack,
                                  polygons As IEnumerable(Of Dictionary(Of String, String)),
                                  unionGeneIds As Index(Of String)) As mzPack
        Dim res As Double = 55
        Dim layer_annotations As New Dictionary(Of String, String)

        For Each map As KeyValuePair(Of String, Integer) In unionGeneIds.Map
            Call layer_annotations.Add(map.Value.ToString, map.Key)
        Next

        If Not polygons.Any(Function(m) m.ContainsKey("resolution")) Then
            union.metadata("resolution") = res
        End If

        ' set the gene idset
        union.Annotations = layer_annotations
        union.Application = FileApplicationClass.STImaging

        Return union
    End Function

    Public Function MergeDataWithLayout(raw As Dictionary(Of String, mzPack), layout As String()()) As mzPack
        Dim unionGeneIds As Index(Of String) = raw.Values _
           .Select(Function(m) m.Annotations.Values) _
           .IteratesALL _
           .Distinct _
           .Indexing
        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim mergeST As New MergeSTSlides(True, True, println, unionGeneIds)
        Dim union As mzPack = MergeLayoutSliders.MergeDataWithLayout(raw, layout, mergeST)

        Return TweaksSTData(union, raw.Values.Select(Function(m) m.metadata), unionGeneIds)
    End Function
End Module

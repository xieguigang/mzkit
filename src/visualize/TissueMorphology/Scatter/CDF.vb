#Region "Microsoft.VisualBasic::a6e603723b4db43a8dfd25c17dd11aac, mzkit\src\visualize\TissueMorphology\Scatter\CDF.vb"

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

'   Total Lines: 217
'    Code Lines: 168
' Comment Lines: 18
'   Blank Lines: 31
'     File Size: 8.92 KB


' Module CDF
' 
'     Function: (+2 Overloads) GetDimension, IsTissueMorphologyCDF, (+2 Overloads) ReadTissueMorphology, (+2 Overloads) ReadUMAP, WriteCDF
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports any = Microsoft.VisualBasic.Scripting
Imports CDFDimension = Microsoft.VisualBasic.DataStorage.netCDF.Components.Dimension

''' <summary>
''' the cdf file data handler
''' </summary>
Public Module CDF

    <Extension>
    Public Function IsTissueMorphologyCDF(cdf As netCDFReader) As Boolean
        Static attrNames As Index(Of String) = {"regions", "umap_sample"}
        Static varNames As String() = {
            "sampleX", "sampleY", "cluster", "umapX", "umapY", "umapZ"
        }

        Return cdf.globalAttributes.All(Function(a) a.name Like attrNames) AndAlso
            varNames.All(Function(name) cdf.dataVariableExists(name))
    End Function

    <Extension>
    Public Function GetDimension(tissueMorphology As TissueRegion()) As Size
        Dim allPixels As Point() = tissueMorphology _
            .Select(Function(t) t.points) _
            .IteratesALL _
            .ToArray

        If allPixels.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim w = Aggregate p In allPixels Into Max(p.X)
        Dim h = Aggregate p In allPixels Into Max(p.Y)

        Return New Size(w, h)
    End Function

    <Extension>
    Public Function GetDimension(cdf As netCDFReader) As Size
        If cdf.attributeExists("scan_x") AndAlso cdf.attributeExists("scan_y") Then
            Dim scan_x As Integer = any.ToString(cdf("scan_x")).ParseInteger
            Dim scan_y As Integer = any.ToString(cdf("scan_y")).ParseInteger

            Return New Size(scan_x, scan_y)
        Else
            Return cdf _
                .ReadTissueMorphology _
                .ToArray _
                .GetDimension
        End If
    End Function

    <Extension>
    Public Function WriteCDF(tissueMorphology As TissueRegion(), file As Stream,
                             Optional dimension As Size = Nothing,
                             Optional umap As UMAPPoint() = Nothing) As Boolean

        Using cdf As New CDFWriter(file)
            Dim attrs As New List(Of attribute)
            Dim pixels As New List(Of Integer)
            Dim i As i32 = 1
            Dim vec As integers
            Dim dims As Dimension
            Dim uniqueId As String

            If umap Is Nothing Then
                umap = {}
            End If
            If dimension.IsEmpty Then
                dimension = tissueMorphology.GetDimension
            End If

            attrs.Add(New attribute With {.name = "scan_x", .type = CDFDataTypes.INT, .value = dimension.Width})
            attrs.Add(New attribute With {.name = "scan_y", .type = CDFDataTypes.INT, .value = dimension.Height})
            attrs.Add(New attribute With {.name = "regions", .type = CDFDataTypes.INT, .value = tissueMorphology.Length})
            attrs.Add(New attribute With {.name = "umap_sample", .type = CDFDataTypes.INT, .value = umap.Length})
            cdf.GlobalAttributes(attrs.PopAll)

            ' write region data
            For Each region As TissueRegion In tissueMorphology
                attrs.Add(New attribute With {.name = "label", .type = CDFDataTypes.CHAR, .value = region.label})
                attrs.Add(New attribute With {.name = "color", .type = CDFDataTypes.CHAR, .value = region.color.ToHtmlColor})
                attrs.Add(New attribute With {.name = "size", .type = CDFDataTypes.INT, .value = region.nsize})

                For Each p As Point In region.points
                    Call pixels.Add(p.X)
                    Call pixels.Add(p.Y)
                Next

                vec = New integers(pixels.PopAll)
                uniqueId = $"region_{++i}"
                dims = New Dimension With {.name = $"sizeof_{uniqueId}", .size = vec.Length}
                cdf.AddVariable(uniqueId, vec, dims, attrs.PopAll)
            Next

            ' write umap sample data
            Dim sampleX As Integer() = umap.Select(Function(p) p.Pixel.X).ToArray
            Dim sampleY As Integer() = umap.Select(Function(p) p.Pixel.Y).ToArray
            Dim umapX As Double() = umap.Select(Function(p) p.x).ToArray
            Dim umapY As Double() = umap.Select(Function(p) p.y).ToArray
            Dim umapZ As Double() = umap.Select(Function(p) p.z).ToArray
            Dim cluster_labels As Dictionary(Of String, String) = Nothing
            Dim clusters As Integer() = umap.encodeClusterLabels(labels:=cluster_labels)
            Dim umapsize As New Dimension With {.name = "umap_size", .size = umap.Length}
            Dim labels As String() = umap.Select(Function(p) Strings.Trim(p.label)).ToArray
            Dim str As chars
            Dim strLen As Dimension

            Call cdf.AddVariable("sampleX", New integers(sampleX), umapsize)
            Call cdf.AddVariable("sampleY", New integers(sampleY), umapsize)
            Call cdf.AddVariable("cluster", New integers(clusters), umapsize)

            Call cdf.AddVariable("umapX", New doubles(umapX), umapsize)
            Call cdf.AddVariable("umapY", New doubles(umapY), umapsize)
            Call cdf.AddVariable("umapZ", New doubles(umapZ), umapsize)

            ' cluster labels is optional
            For Each cluster In cluster_labels
                str = New chars(cluster.Value)
                strLen = New Dimension($"sizeof_{cluster.Key}", str.Length)
                cdf.AddVariable(cluster.Key, str, strLen)
            Next

            ' cells labels is optional
            If Not labels.All(Function(s) s = "") Then
                Dim chrs As New chars(labels)
                Dim chrSize As Dimension = CDFDimension.FromVector(chrs)

                Call cdf.AddVariable("spot_labels", chrs, [dim]:=chrSize)
            End If
        End Using

        Return True
    End Function

    <Extension>
    Private Function encodeClusterLabels(umap As UMAPPoint(), ByRef labels As Dictionary(Of String, String)) As Integer()
        labels = New Dictionary(Of String, String)

        If umap.All(Function(c) c.class.IsPattern("\d+")) Then
            Return umap.Select(Function(u) Integer.Parse(u.class)).ToArray
        End If

        Dim labelIndex As Index(Of String) = umap _
            .Select(Function(l) l.class) _
            .Distinct _
            .Indexing
        Dim index As Integer() = New Integer(umap.Length - 1) {}

        For i As Integer = 0 To umap.Length - 1
            index(i) = labelIndex.IndexOf(umap(i).class)
        Next

        For Each label As SeqValue(Of String) In labelIndex
            Call labels.Add($"cluster_label_{label.i}", label.value)
        Next

        Return index
    End Function

    <Extension>
    Private Function ReadClusterLabelv2(cdf As netCDFReader) As String()
        Dim id As integers = cdf.getDataVariable("cluster")
        Dim vars As String() = id.Select(Function(i) $"cluster_label_{i}").ToArray
        Dim labels As String() = New String(id.Length - 1) {}
        Dim cache As New Dictionary(Of String, String)
        Dim offset As Integer

        For i As Integer = 0 To id.Length - 1
            offset = i
            labels(i) = cache.ComputeIfAbsent(vars(i),
               lazyValue:=Function(varName)
                              If cdf.dataVariableExists(varName) Then
                                  Return CType(cdf.getDataVariable(varName), chars).CharString
                              Else
                                  Return id(offset).ToString
                              End If
                          End Function)
        Next

        Return labels
    End Function

    <Extension>
    Public Iterator Function ReadUMAP(cdf As netCDFReader) As IEnumerable(Of UMAPPoint)
        Dim sampleX As integers = cdf.getDataVariable("sampleX")
        Dim sampleY As integers = cdf.getDataVariable("sampleY")
        Dim umapX As doubles = cdf.getDataVariable("umapX")
        Dim umapY As doubles = cdf.getDataVariable("umapY")
        Dim umapZ As doubles = cdf.getDataVariable("umapZ")
        Dim clusterVar = cdf.getDataVariableEntry("cluster")
        Dim clusters As String()
        Dim labels As String() = {}

        If clusterVar.type = CDFDataTypes.INT Then
            ' version 1 and v2 format
            clusters = cdf.ReadClusterLabelv2
        Else
            ' new format
            clusters = DirectCast(cdf.getDataVariable("cluster"), chars).LoadJSON(Of String())
        End If

        ' label is optional for make data compatibability
        If cdf.dataVariableExists("spot_labels") Then
            labels = DirectCast(cdf.getDataVariable("spot_labels"), chars).LoadJSON(Of String())
        End If

        For i As Integer = 0 To clusters.Length - 1
            Yield New UMAPPoint With {
                .[class] = clusters(i),
                .Pixel = New Point(sampleX(i), sampleY(i)),
                .x = umapX(i),
                .y = umapY(i),
                .z = umapZ(i),
                .label = labels.ElementAtOrDefault(i)
            }
        Next
    End Function

    <Extension>
    Public Function ReadUMAP(file As Stream) As UMAPPoint()
        Using cdf As New netCDFReader(file)
            Return cdf.ReadUMAP.ToArray
        End Using
    End Function

    <Extension>
    Public Iterator Function ReadTissueMorphology(cdf As netCDFReader) As IEnumerable(Of TissueRegion)
        Dim regions As Integer = any.ToString(cdf.getAttribute("regions")).ParseInteger

        For i As Integer = 1 To regions
            Dim refId As String = $"region_{i}"
            Dim var As variable = cdf.getDataVariableEntry(refId)
            Dim name As String = var.FindAttribute("label").value
            Dim color As String = var.FindAttribute("color").value
            Dim nsize As Integer = var.FindAttribute("size").value.ParseInteger
            Dim data As integers = cdf.getDataVariable(var)
            Dim pixels As Point() = data _
                .Split(2) _
                .Select(Function(p)
                            Return New Point With {
                                .X = p(Scan0),
                                .Y = p(1)
                            }
                        End Function) _
                .ToArray

            Yield New TissueRegion With {
                .color = color.TranslateColor,
                .label = name,
                .points = pixels
            }
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' target file stream will be close automatically
    ''' </remarks>
    <Extension>
    Public Function ReadTissueMorphology(file As Stream) As TissueRegion()
        Using cdf As New netCDFReader(file)
            ' 20220825 由于在这使用了using进行文件资源的自动释放
            ' 所以在这里不可以使用迭代器进行数据返回
            ' 否则文件读取模块会因为using语句自动释放资源导致报错
            Return cdf.ReadTissueMorphology.ToArray
        End Using
    End Function

End Module

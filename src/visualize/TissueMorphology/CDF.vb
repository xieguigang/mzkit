Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports any = Microsoft.VisualBasic.Scripting

''' <summary>
''' the cdf file data handler
''' </summary>
Public Module CDF

    <Extension>
    Public Function WriteCDF(tissueMorphology As TissueRegion(), umap As UMAPPoint(), file As Stream) As Boolean
        Using cdf As New CDFWriter(file)
            Dim attrs As New List(Of attribute)
            Dim pixels As New List(Of Integer)
            Dim i As i32 = 1
            Dim vec As integers
            Dim dims As Dimension
            Dim uniqueId As String

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
            Dim clusters As Integer() = umap.Select(Function(p) p.class).ToArray
            Dim umapsize As New Dimension With {.name = "umap_size", .size = umap.Length}

            Call cdf.AddVariable("sampleX", New integers(sampleX), umapsize)
            Call cdf.AddVariable("sampleY", New integers(sampleY), umapsize)
            Call cdf.AddVariable("cluster", New integers(clusters), umapsize)

            Call cdf.AddVariable("umapX", New doubles(umapX), umapsize)
            Call cdf.AddVariable("umapY", New doubles(umapY), umapsize)
            Call cdf.AddVariable("umapZ", New doubles(umapZ), umapsize)
        End Using

        Return True
    End Function

    <Extension>
    Public Function ReadUMAP(file As Stream) As UMAPPoint()
        Using cdf As New netCDFReader(file)
            Dim sampleX As integers = cdf.getDataVariable("sampleX")
            Dim sampleY As integers = cdf.getDataVariable("sampleY")
            Dim umapX As doubles = cdf.getDataVariable("umapX")
            Dim umapY As doubles = cdf.getDataVariable("umapY")
            Dim umapZ As doubles = cdf.getDataVariable("umapZ")
            Dim clusters As integers = cdf.getDataVariable("cluster")

            Return clusters _
                .Select(Function(cl, i)
                            Return New UMAPPoint With {
                                .[class] = cl,
                                .Pixel = New Point(sampleX(i), sampleY(i)),
                                .x = umapX(i),
                                .y = umapY(i),
                                .z = umapZ(i)
                            }
                        End Function) _
                .ToArray
        End Using
    End Function

    <Extension>
    Public Iterator Function ReadTissueMorphology(file As Stream) As IEnumerable(Of TissueRegion)
        Using cdf As New netCDFReader(file)
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
        End Using
    End Function

End Module

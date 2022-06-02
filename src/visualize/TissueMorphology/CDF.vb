Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

''' <summary>
''' the cdf file data handler
''' </summary>
Public Module CDF

    <Extension>
    Public Function WriteCDF(tissueMorphology As TissueRegion(), file As Stream) As Boolean
        Using cdf As New CDFWriter(file)
            Dim attrs As New List(Of attribute)
            Dim pixels As New List(Of Integer)
            Dim i As i32 = 1
            Dim vec As integers
            Dim dims As Dimension
            Dim uniqueId As String

            attrs.Add(New attribute With {.name = "regions", .type = CDFDataTypes.INT, .value = tissueMorphology.Length})
            cdf.GlobalAttributes(attrs.PopAll)

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
        End Using

        Return True
    End Function

End Module

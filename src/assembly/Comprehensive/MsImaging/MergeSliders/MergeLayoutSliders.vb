Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Public Module MergeLayoutSliders

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="layout">
    ''' the grid layout, value should be whitespace or the file base name
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function MergeDataWithLayout(raw As Dictionary(Of String, mzPack), layout As String()(),
                                        Optional merge As MergeLinear = Nothing,
                                        Optional ByRef offsets As Dictionary(Of String, Integer()) = Nothing) As mzPack

        Dim polygons = raw.ToDictionary(
            Function(m) m.Key,
            Function(m)
                Return New Polygon2D(m.Value.MS.Select(Function(s1) s1.GetMSIPixel))
            End Function)

        merge = If(merge, New MergeSMSlides(
            relativePos:=True,
            norm:=True,
            println:=AddressOf RunSlavePipeline.SendMessage
        ))

        Dim averageWidth = Aggregate p As Polygon2D In polygons.Values Into Average(p.GetRectangle.Width)
        Dim averageHeight = Aggregate p As Polygon2D In polygons.Values Into Average(p.GetRectangle.Height)
        Dim union As New List(Of ScanMS1)
        Dim padding As Size = merge.Padding
        Dim top As Integer = padding.Height
        Dim left As Integer = padding.Width
        Dim scan_x As Integer
        Dim scan_y As Integer
        Dim mzmin As New List(Of Double)
        Dim mzmax As New List(Of Double)
        Dim res As New List(Of Double)

        ' helper data for arrange the cdf layout
        offsets = New Dictionary(Of String, Integer())

        For Each row As String() In layout
            Dim maxHeight As Double = averageHeight

            For Each col As String In row
                If col.IsPattern("\s+") OrElse col.StringEmpty Then
                    ' is a blank space, just offset the box
                    left += padding.Width * 2 + averageWidth
                Else
                    Dim sample As mzPack = raw(col)
                    Dim sample_shape = polygons(col)
                    Dim rect As RectangleF = sample_shape.GetRectangle
                    Dim minX As Integer = sample_shape.xpoints.Min
                    Dim deltaY As Integer = sample_shape.ypoints.Min * -1 + top

                    Call offsets.Add(col, {left, top, minX, deltaY})
                    Call merge.JoinOneSample(
                        shape:=sample_shape,
                        sample:=sample,
                        left:=left,
                        top:=top
                    ).DoCall(AddressOf union.AddRange)

                    left += padding.Width * 2 + rect.Width

                    With sample.MS _
                        .Select(Function(i) i.mz) _
                        .IteratesALL _
                        .ToArray

                        If .Length > 0 Then
                            mzmin.Add(.Min)
                            mzmax.Add(.Max)
                        End If
                    End With

                    If Not sample.metadata.IsNullOrEmpty Then
                        res.Add(sample.metadata.TryGetValue("resolution", [default]:=17))
                    End If

                    If rect.Height > maxHeight Then
                        maxHeight = rect.Height
                    End If
                End If
            Next

            If left > scan_x Then
                scan_x = left
            End If

            ' offset the top
            top += padding.Height + maxHeight
            ' reset left offset
            left = padding.Width

            If top > scan_y Then
                scan_y = top
            End If
        Next

        Dim poly As New Polygon2D(union.Select(Function(i) i.GetMSIPixel))
        Dim metadata As New Metadata With {
            .[class] = FileApplicationClass.MSImaging.ToString,
            .mass_range = New DoubleRange(mzmin.Min, mzmax.Max),
            .resolution = res.Average,
            .scan_x = poly.width + padding.Width * 2,
            .scan_y = poly.height + padding.Height * 2
        }

        Return New mzPack With {
            .MS = union.ToArray,
            .Application = FileApplicationClass.MSImaging,
            .source = raw.Keys.JoinBy("+"),
            .metadata = metadata.GetMetadata
        }
    End Function
End Module

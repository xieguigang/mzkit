
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("tissue")>
Module Tissue

    <ExportAPI("scan_tissue")>
    Public Function scanTissue(tissue As Image, Optional colors As String() = Nothing) As Cell()
        Return HistologicalImage.GridScan(target:=tissue, colors:=colors).ToArray
    End Function

    <ExportAPI("heatmap_layer")>
    Public Function getTargets(tissue As Cell(),
                               Optional heatmap As String = "density",
                               Optional target As String = "black") As PixelData()

        Dim objs As (obj As [Object], cell As Cell)() = Nothing
        Dim project =
            Function(selector As Func(Of [Object], Double))
                Return objs _
                    .Select(Function(i)
                                Return New PixelData With {
                                    .Scale = selector(i.obj),
                                    .X = i.cell.ScaleX,
                                    .Y = i.cell.ScaleY
                                }
                            End Function) _
                    .ToArray
            End Function

        Select Case Strings.LCase(target)
            Case "black"
                objs = tissue.Select(Function(i) (i.Black, i)).ToArray
            Case Else
                objs = tissue.Select(Function(i) (i.layers(target), i)).ToArray
        End Select

        Select Case Strings.LCase(heatmap)
            Case "pixels" : Return project(Function(i) i.Pixels)
            Case "density" : Return project(Function(i) i.Density)
            Case "ratio" : Return project(Function(i) i.Ratio)
            Case Else
                Throw New NotImplementedException(heatmap)
        End Select
    End Function
End Module

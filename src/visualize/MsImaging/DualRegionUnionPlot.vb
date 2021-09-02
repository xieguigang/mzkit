Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class DualRegionUnionPlot : Inherits Plot

    ReadOnly region1 As SingleIonLayer
    ReadOnly region2 As SingleIonLayer
    ReadOnly pixelScale As Size
    ReadOnly cutoff As DoubleRange
    ReadOnly pixelDrawer As Boolean
    ReadOnly colorSet1 As Color()
    ReadOnly colorSet2 As Color()

    Public Sub New(region1 As SingleIonLayer, region2 As SingleIonLayer,
                   pixelScale As Size,
                   cutoff As DoubleRange,
                   pixelDrawer As Boolean,
                   colorSet1 As Color(),
                   colorSet2 As Color(),
                   theme As Theme)

        MyBase.New(theme)

        Me.region1 = region1
        Me.region2 = region2
        Me.pixelDrawer = pixelDrawer
        Me.cutoff = cutoff
        Me.pixelScale = pixelScale
        Me.colorSet1 = colorSet1
        Me.colorSet2 = colorSet2
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class

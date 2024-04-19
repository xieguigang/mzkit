Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

''' <summary>
''' show the mass windows histogram in scatter plot view
''' </summary>
Public Class PlotMassWindow : Inherits Plot

    ReadOnly mz As Double()
    ReadOnly hist As Double()
    ReadOnly bins As MassWindow()

    Sub New(mz As IEnumerable(Of Double), theme As Theme, Optional mzdiff As Double = 0.001)
        Call MyBase.New(theme)

        With MzBins.GetScatter(mz, mzdiff)
            Me.mz = .x
            Me.hist = .y
        End With

        bins = MzBins.GetMzBins()
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging

Public Class PlotTooltip : Inherits ToolTip

    Dim info As Protocols

    Public Sub LoadInfo(info As Protocols)
        Me.info = info
    End Sub

    Private Sub PlotTooltip_Popup(sender As Object, e As PopupEventArgs) Handles Me.Popup
        e.ToolTipSize = New Size(200, 133)
    End Sub

    Private Sub PlotTooltip_Draw(sender As Object, e As DrawToolTipEventArgs) Handles Me.Draw
        If e.ToolTipText.StringEmpty Then
            Return
        End If

        e.Graphics.FillRectangle(Brushes.White, e.Bounds)

        Dim [lib] As LibraryMatrix

        If info(e.ToolTipText) Is Nothing Then
            [lib] = info.Cluster(e.ToolTipText).representation
        Else
            [lib] = New LibraryMatrix With {
                .name = e.ToolTipText,
                .ms2 = info(e.ToolTipText).mzInto
            }
        End If

        Using plot As Image = MassSpectra.MirrorPlot([lib]).AsGDIImage
            e.Graphics.DrawImage(plot, 0, 0, 200, 133)
        End Using
    End Sub
End Class

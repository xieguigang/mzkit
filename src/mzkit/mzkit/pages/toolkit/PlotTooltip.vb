#Region "Microsoft.VisualBasic::214db27f749b4c5c6a8fd88fea05142b, src\mzkit\mzkit\pages\toolkit\PlotTooltip.vb"

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

    ' Class PlotTooltip
    ' 
    '     Sub: LoadInfo, PlotTooltip_Draw, PlotTooltip_Popup
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
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

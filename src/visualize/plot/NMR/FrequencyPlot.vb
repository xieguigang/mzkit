#Region "Microsoft.VisualBasic::c5e7e5c946bafe709ea88bfac708bd10, E:/mzkit/src/visualize/plot//NMR/FrequencyPlot.vb"

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

    '   Total Lines: 41
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.38 KB


    ' Class nmrSpectrumPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.NMRFidTool
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class nmrSpectrumPlot : Inherits Plot

    ReadOnly freq As Spectrum

    Public Sub New(freq As Spectrum, theme As Theme)
        MyBase.New(theme)

        Me.freq = freq
        Me.main = "NMR frequency Plot"
        Me.xlabel = "frequency(Hz)"
        Me.ylabel = "Amplitude"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim freq As ms2() = Me.freq _
            .RealChannelData _
            .Select(Function(f, i)
                        Return New ms2 With {
                            .mz = f,
                            .intensity = Me.freq.ImaginaryChannelData(i)
                        }
                    End Function) _
            .ToArray
        Dim peaks As New PeakAssign(main, freq, Nothing, 0.25, theme) With {
            .main = main,
            .legendTitle = legendTitle,
            .xlabel = xlabel,
            .ylabel = ylabel,
            .zlabel = zlabel
        }

        Call peaks.Plot(g, canvas)
    End Sub
End Class

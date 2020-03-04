#Region "Microsoft.VisualBasic::56d77b1dc7b4d51b209a4075684da8ea, Rscript\Library\mzkit.quantify\Visual.vb"

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

' Module Visual
' 
'     Function: chromatogramPlot, DrawStandardCurve, MRMchromatogramPeakPlot
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("mzkit.quantify.visual")>
Module Visual

    ''' <summary>
    ''' Draw standard curve
    ''' </summary>
    ''' <param name="model">The linear model of the targeted metabolism model data.</param>
    ''' <param name="title">The plot title</param>
    ''' <param name="samples">The point data of samples</param>
    ''' <returns></returns>
    <ExportAPI("standard_curve")>
    Public Function DrawStandardCurve(model As StandardCurve,
                                      Optional title$ = "",
                                      Optional samples As NamedValue(Of Double)() = Nothing,
                                      Optional size$ = "1600,1200",
                                      Optional margin$ = "padding: 200px 100px 150px 150px",
                                      Optional factorFormat$ = "G4",
                                      Optional sampleLabelFont$ = CSSFont.Win10NormalLarger,
                                      Optional labelerIterations% = 1000) As GraphicsData

        Return StandardCurvesPlot.StandardCurves(
            model:=model,
            samples:=samples,
            name:=title,
            size:=size,
            margin:=margin,
            factorFormat:=factorFormat,
            sampleLabelFont:=sampleLabelFont,
            labelerIterations:=labelerIterations
        )
    End Function

    <ExportAPI("chromatogram.plot")>
    Public Function chromatogramPlot(mzML$, ions As IonPair(), Optional labelLayoutTicks% = 2000) As GraphicsData
        Return ions.MRMChromatogramPlot(mzML, labelLayoutTicks:=labelLayoutTicks)
    End Function

    <ExportAPI("MRM.chromatogramPeaks.plot")>
    Public Function MRMchromatogramPeakPlot(chromatogram As ChromatogramTick(), Optional title$ = "MRM Chromatogram Peak Plot") As GraphicsData
        Return chromatogram.Plot(
            title:=title,
            showMRMRegion:=True,
            showAccumulateLine:=True
        )
    End Function
End Module

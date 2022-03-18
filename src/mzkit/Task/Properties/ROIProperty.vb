#Region "Microsoft.VisualBasic::ec9fea590615845c3287f5005b8d1e31, mzkit\src\mzkit\Task\Properties\ROIProperty.vb"

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

    '   Total Lines: 24
    '    Code Lines: 21
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 1.03 KB


    ' Class ROIProperty
    ' 
    '     Properties: baseline, peakArea, rt, rtmax, rtmin
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Class ROIProperty

    <Description("The min value of the rt of current region of interested on the chromatography.")>
    Public Property rtmin As Double
    <Description("The max value of the rt of current region of interested on the chromatography.")>
    Public Property rtmax As Double
    <Description("The time point of the max intensity point of current region of interested on the chromatography.")>
    Public Property rt As Double
    <Description("The peak area of current region of interested on the chromatography.")>
    Public Property peakArea As Double
    <Description("The noise baseline value of current region of interested on the chromatography.")>
    Public Property baseline As Double

    Sub New(ROI As ROI)
        rtmin = ROI.time.Min
        rtmax = ROI.time.Max
        rt = ROI.rt
        peakArea = ROI.integration
        baseline = ROI.baseline
    End Sub
End Class

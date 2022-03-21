#Region "Microsoft.VisualBasic::c0c9a930e24852d71d6bce9becae0c84, mzkit\src\mzkit\Task\Properties\MRMROIProperty.vb"

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

    '   Total Lines: 42
    '    Code Lines: 36
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.43 KB


    ' Class MRMROIProperty
    ' 
    '     Properties: baseline, peakArea, precursor, product, rt
    '                 rtmax, rtmin
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports chromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Public Class MRMROIProperty

    Public Property precursor As Double
    Public Property product As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property rt As Double
    Public Property peakArea As Double
    Public Property baseline As Double

    Sub New(chr As chromatogram)
        Dim TIC = chr.Ticks

        If chr.precursor IsNot Nothing AndAlso chr.product IsNot Nothing Then
            precursor = chr.precursor.MRMTargetMz
            product = chr.product.MRMTargetMz
        End If

        Dim ROI = TIC.Shadows _
            .PopulateROI(
                baselineQuantile:=0.65,
                angleThreshold:=5,
                peakwidth:=New Double() {8, 30},
                snThreshold:=3
            ) _
          .OrderByDescending(Function(r) r.integration).FirstOrDefault

        If Not ROI Is Nothing Then
            rtmin = ROI.time.Min
            rtmax = ROI.time.Max
            rt = ROI.rt
            peakArea = ROI.integration
            baseline = ROI.baseline
        End If
    End Sub
End Class

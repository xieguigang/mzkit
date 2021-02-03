#Region "Microsoft.VisualBasic::56b510756a6c8638e939451ba074adee, TargetedMetabolomics\LinearQuantitative\Linear\Points\TargetPeakPoint.vb"

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

    '     Class TargetPeakPoint
    ' 
    '         Properties: ChromatogramSummary, Name, Peak, SampleName
    ' 
    '         Function: GetIonTPA, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math

Namespace LinearQuantitative.Linear

    ''' <summary>
    ''' Dump MRM target quantification result in XML format.
    ''' </summary>
    Public Class TargetPeakPoint : Implements INamedValue

        ''' <summary>
        ''' the feature name
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' the metabolite unique id
        ''' </remarks>
        Public Property Name As String Implements INamedValue.Key
        ''' <summary>
        ''' the sample name
        ''' </summary>
        ''' <returns></returns>
        Public Property SampleName As String
        ''' <summary>
        ''' the peak location in time line
        ''' </summary>
        ''' <returns></returns>
        Public Property Peak As ROIPeak
        Public Property ChromatogramSummary As Quantile()

        Public Overrides Function ToString() As String
            Return $"[{SampleName}: {Name}] = {Peak}"
        End Function

        Public Function GetIonTPA(baselineQuantile As Double) As IonTPA
            Dim deconv = Peak.ticks _
                .Shadows _
                .TPAIntegrator(Peak, baselineQuantile, peakAreaMethod:=PeakAreaMethods.NetPeakSum)
            Dim maxinto As ChromatogramTick = Peak.ticks _
                .OrderByDescending(Function(t) t.Intensity) _
                .First

            Return New IonTPA With {
                .name = Name,
                .area = deconv.area,
                .baseline = deconv.baseline,
                .maxPeakHeight = deconv.maxPeakHeight,
                .peakROI = Peak.window,
                .rt = maxinto.Time
            }
        End Function
    End Class
End Namespace

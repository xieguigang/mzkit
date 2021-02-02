#Region "Microsoft.VisualBasic::6f5492f81f254c22f5874553b0f25c92, TargetedMetabolomics\LinearQuantitative\Linear\Points\TargetPeakPoint.vb"

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
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

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
    End Class
End Namespace

#Region "Microsoft.VisualBasic::b42a2d93e27c9855228cbd8efb369dd9, mzmath\TargetedMetabolomics\LinearQuantitative\Models\IonTPA.vb"

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

    '   Total Lines: 27
    '    Code Lines: 19 (70.37%)
    ' Comment Lines: 3 (11.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (18.52%)
    '     File Size: 916 B


    '     Class IonTPA
    ' 
    '         Properties: area, baseline, maxPeakHeight, name, peakROI
    '                     refer_rt, rt, sn, source
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports std = System.Math

Namespace LinearQuantitative

    ''' <summary>
    ''' ROI Peak data of the given ion
    ''' </summary>
    Public Class IonTPA : Implements INamedValue

        Public Property name As String Implements INamedValue.Key
        Public Property refer_rt As Double
        Public Property rt As Double
        Public Property peakROI As DoubleRange
        Public Property area As Double
        Public Property baseline As Double
        Public Property maxPeakHeight As Double
        Public Property sn As Double
        Public Property source As String

        Public Overrides Function ToString() As String
            Return $"{name}[{peakROI.Min}, {std.Round(peakROI.Max)}] = {area}"
        End Function

    End Class
End Namespace

#Region "Microsoft.VisualBasic::0d0a369cc74c36be28218c645fd54520, src\mzkit\mzkit\pages\dockWindow\documents\QuantifyParameters.vb"

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

    ' Class QuantifyParameters
    ' 
    '     Properties: angle_threshold, peakMax, peakMin
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Public Class QuantifyParameters

    <Category("Peak Width")>
    <DisplayName("min")>
    <Description("the min peak width in rt(seconds).")>
    Public Property peakMin As Double
    <Category("Peak Width")>
    <DisplayName("max")>
    <Description("the max peak width in rt(seconds).")>
    Public Property peakMax As Double

    <Category("Peak Finding")>
    <Description("The threshold value of sin(alpha) angle value, value of this parameter should be in range of [0,90]")>
    Public Property angle_threshold As Double = 8

End Class


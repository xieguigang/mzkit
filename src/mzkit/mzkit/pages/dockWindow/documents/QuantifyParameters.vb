#Region "Microsoft.VisualBasic::b6bfdb5c6bb36f99325ba07d49a9459d, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\QuantifyParameters.vb"

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

    '   Total Lines: 49
    '    Code Lines: 39
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.55 KB


    ' Class QuantifyParameters
    ' 
    '     Properties: angle_threshold, peakMax, peakMin, tolerance, toleranceMethod
    ' 
    '     Function: GetMRMArguments, GetTolerance
    ' 
    ' Enum ToleranceMethods
    ' 
    '     da, ppm
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public Class QuantifyParameters

    <Category("Peak Width")>
    <DisplayName("min")>
    <Description("the min peak width in rt(seconds).")>
    Public Property peakMin As Double = 5
    <Category("Peak Width")>
    <DisplayName("max")>
    <Description("the max peak width in rt(seconds).")>
    Public Property peakMax As Double = 30

    <Category("Peak Finding")>
    <Description("The threshold value of sin(alpha) angle value, value of this parameter should be in range of [0,90]")>
    Public Property angle_threshold As Double = 6

    <Category("Peak Finding")>
    Public Property toleranceMethod As ToleranceMethods = ToleranceMethods.da
    <Category("Peak Finding")>
    Public Property tolerance As Double = 0.3

    Public Function GetTolerance() As Tolerance
        Select Case toleranceMethod
            Case ToleranceMethods.da
                Return New DAmethod(tolerance)
            Case Else
                Return New PPMmethod(tolerance)
        End Select
    End Function

    Public Function GetMRMArguments() As MRMArguments
        Dim args As MRMArguments = MRMArguments.GetDefaultArguments

        args.peakwidth = {peakMin, peakMax}
        args.angleThreshold = angle_threshold
        args.tolerance = GetTolerance()

        Return args
    End Function

End Class

Public Enum ToleranceMethods
    da
    ppm
End Enum

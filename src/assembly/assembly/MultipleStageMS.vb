#Region "Microsoft.VisualBasic::c0113b8981815f165a3f81187d5785a6, assembly\assembly\MultipleStageMS.vb"

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

    '   Total Lines: 34
    '    Code Lines: 18 (52.94%)
    ' Comment Lines: 12 (35.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (11.76%)
    '     File Size: 1.32 KB


    ' Module MultipleStageMS
    ' 
    '     Function: MultipleStageCor
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Public Module MultipleStageMS

    ''' <summary>
    ''' evaluate the correlation score for join the ms2 and ms3 products
    ''' </summary>
    ''' <param name="ms2">
    ''' intensity value of the spectrum data should be normalized before find the ms3 product
    ''' </param>
    ''' <param name="ms3">precursor information of the ms3 spectrum</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' rt of ms2 &amp; ms3 keeps the same, the rt of ms3 should be always greater than the rt of ms2;
    ''' precursor of the ms3 should be one of the top fragment peak of ms2
    ''' </remarks>
    Public Function MultipleStageCor(ms2 As PeakMs2, ms3 As IMs1, Optional maxdt As Double = 2) As Double
        Dim rt As Double = std.Abs(ms2.rt - ms3.rt) / maxdt
        Dim precursor As Double = ms3.mz
        Dim fragment2 = ms2.mzInto _
            .Where(Function(a) std.Abs(a.mz - ms3.mz) <= 0.3) _
            .Select(Function(a) a.intensity) _
            .ToArray

        If fragment2.Any AndAlso rt <= 1 Then
            Return rt * fragment2.Average
        Else
            Return 0
        End If
    End Function

End Module


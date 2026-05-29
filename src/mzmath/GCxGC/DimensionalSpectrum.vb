#Region "Microsoft.VisualBasic::7b8ad210806a3f224841a613670d25ab, mzmath\GCxGC\DimensionalSpectrum.vb"

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
    '    Code Lines: 7 (25.93%)
    ' Comment Lines: 16 (59.26%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (14.81%)
    '     File Size: 746 B


    ' Class DimensionalSpectrum
    ' 
    '     Properties: baseIntensity, ms2, rt1, totalIon
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class DimensionalSpectrum

    ''' <summary>
    ''' the rt of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rt1 As Double
    ''' <summary>
    ''' the dimension 2 spectrum data
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As PeakMs2()

    ''' <summary>
    ''' total ion of current dimension 1 ms1 scan data
    ''' </summary>
    ''' <returns></returns>
    Public Property totalIon As Double
    ''' <summary>
    ''' the base peak intensity value of current dimension 1 ms1 scan data 
    ''' </summary>
    ''' <returns></returns>
    Public Property baseIntensity As Double

End Class

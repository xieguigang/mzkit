#Region "Microsoft.VisualBasic::05b8f3c27273b0e56a8ac9d94d9bd3c4, G:/mzkit/src/mzmath/MSEngine//Search/SpectrumEqualityComparer.vb"

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

    '   Total Lines: 14
    '    Code Lines: 12
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 653 B


    ' Class SpectrumEqualityComparer
    ' 
    '     Function: Equals, GetHashCode
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Public Class SpectrumEqualityComparer
    Implements IEqualityComparer(Of SpectrumPeak)
    Private Shared ReadOnly EPS As Double = 1000000.0
    Public Overloads Function Equals(x As SpectrumPeak, y As SpectrumPeak) As Boolean Implements IEqualityComparer(Of SpectrumPeak).Equals
        Return std.Abs(x.mz - y.mz) <= EPS
    End Function

    Public Overloads Function GetHashCode(obj As SpectrumPeak) As Integer Implements IEqualityComparer(Of SpectrumPeak).GetHashCode
        Return std.Round(obj.mz, 6).GetHashCode()
    End Function
End Class


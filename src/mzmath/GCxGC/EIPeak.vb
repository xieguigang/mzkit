#Region "Microsoft.VisualBasic::a43b9860005a029d21432c5c514aa69a, mzmath\GCxGC\EIPeak.vb"

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

    '   Total Lines: 32
    '    Code Lines: 13 (40.62%)
    ' Comment Lines: 14 (43.75%)
    '    - Xml Docs: 92.86%
    ' 
    '   Blank Lines: 5 (15.62%)
    '     File Size: 980 B


    ' Class EIPeak
    ' 
    '     Properties: peak, spectrum
    ' 
    '     Function: GetRepresentativeSpectrum, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class EIPeak(Of T)

    ''' <summary>
    ''' the ms1 peak
    ''' </summary>
    ''' <returns></returns>
    Public Property peak As T
    ''' <summary>
    ''' all spectrum that pick from current peak rt range.
    ''' </summary>
    ''' <returns></returns>
    Public Property spectrum As LibraryMatrix()

    ''' <summary>
    ''' make average spectrum of current <see cref="spectrum"/> collection.
    ''' </summary>
    ''' <param name="centroid"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetRepresentativeSpectrum(Optional centroid As Double = 0.1) As LibraryMatrix
        Return spectrum.SpectrumSum(centroid, average:=True)
    End Function

    Public Overrides Function ToString() As String
        Return peak.ToString
    End Function

End Class


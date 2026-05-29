#Region "Microsoft.VisualBasic::3e8d7e7598c98e5e91acb9d515d472fc, mzmath\MSEngine\AnnotationPack\Ms2Score.vb"

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

    '   Total Lines: 65
    '    Code Lines: 27 (41.54%)
    ' Comment Lines: 31 (47.69%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (10.77%)
    '     File Size: 1.92 KB


    ' Class Ms2Score
    ' 
    '     Properties: entropy, forward, intensity, jaccard, libname
    '                 ms2, precursor, reverse, rt, score
    '                 source
    ' 
    '     Function: GetSampleSpectrum, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

''' <summary>
''' the ms2 alignment result
''' </summary>
Public Class Ms2Score

    ''' <summary>
    ''' the precursor m/z of the ms2 spectrum
    ''' </summary>
    ''' <returns></returns>
    Public Property precursor As Double
    ''' <summary>
    ''' rt of the ms2 spectrum precursor ion
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double
    ''' <summary>
    ''' intensity of the ms2 spectrum its precursor ion
    ''' </summary>
    ''' <returns></returns>
    Public Property intensity As Double
    ''' <summary>
    ''' the library hit name
    ''' </summary>
    ''' <returns></returns>
    Public Property libname As String
    ''' <summary>
    ''' ms2 alignment score
    ''' </summary>
    ''' <returns></returns>
    Public Property score As Double

    Public Property forward As Double
    Public Property reverse As Double
    Public Property jaccard As Double
    Public Property entropy As Double

    ''' <summary>
    ''' the ms2 spectrum of current alignment hit result
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As SSM2MatrixFragment()
    ''' <summary>
    ''' the source file name
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSampleSpectrum() As IEnumerable(Of ms2)
        Return ms2 _
            .Where(Function(a) a.query > 0) _
            .Select(Function(a)
                        Return New ms2(a.mz, a.query)
                    End Function)
    End Function

    Public Overrides Function ToString() As String
        Return $"{libname}@{source}"
    End Function

End Class

#Region "Microsoft.VisualBasic::1ce6b3b3d90cf460951fa6f4d511b14b, G:/mzkit/src/assembly/assembly//MarkupData/nmrML/spectrum1D.vb"

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

    '   Total Lines: 71
    '    Code Lines: 49
    ' Comment Lines: 5
    '   Blank Lines: 17
    '     File Size: 2.31 KB


    '     Class spectrum1D
    ' 
    '         Properties: id, numberOfDataPoints, spectrumDataArray, xAxis
    ' 
    '         Function: ParseMatrix
    ' 
    '     Class xAxis
    ' 
    '         Properties: endValue, startValue, unitAccession, unitCvRef, unitName
    ' 
    '         Function: GetPPM
    ' 
    '     Class spectrumList
    ' 
    '         Properties: spectrum1D
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Math

Namespace MarkupData.nmrML

    Public Class spectrum1D

        Public Property spectrumDataArray As fidData
        Public Property xAxis As xAxis

        <XmlAttribute> Public Property numberOfDataPoints As Integer
        <XmlAttribute> Public Property id As String

        Public Function ParseMatrix(SW As Double) As LibraryMatrix
            Dim spec1r As Double() = spectrumDataArray.DecodeBytes

            If spectrumDataArray.byteFormat.ToLower = "complex128" Then
                spec1r = spec1r _
                    .Split(2) _
                    .Select(Function(i) i(1)) _
                    .ToArray
            End If

            Dim ppm As Double() = xAxis.GetPPM(spec1r.Length, SW)

            Call Array.Reverse(ppm)

            Return New LibraryMatrix With {
                .ms2 = ppm _
                    .Select(Function(p, i)
                                Return New ms2 With {
                                    .mz = p,
                                    .intensity = spec1r(i)
                                }
                            End Function) _
                    .ToArray
            }
        End Function

    End Class

    Public Class xAxis

        <XmlAttribute> Public Property unitAccession As String
        <XmlAttribute> Public Property unitName As String
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property startValue As Double
        <XmlAttribute> Public Property endValue As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SI">size of the spectrum</param>
        ''' <returns></returns>
        Public Function GetPPM(SI As Integer, SW As Double) As Double()
            Dim dppm As Double = SW / (SI - 1)
            Dim ppm_max = startValue
            Dim ppm_min = endValue
            Dim ppm As Double() = seq(from:=ppm_min, to:=ppm_max, by:=dppm).ToArray

            Return ppm
        End Function
    End Class

    Public Class spectrumList

        <XmlElement> Public Property spectrum1D As spectrum1D()

    End Class
End Namespace

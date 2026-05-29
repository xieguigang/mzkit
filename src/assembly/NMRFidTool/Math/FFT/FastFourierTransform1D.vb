#Region "Microsoft.VisualBasic::5159dddf6314b4164fca87f656cb863c, assembly\NMRFidTool\Math\FFT\FastFourierTransform1D.vb"

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

    '   Total Lines: 80
    '    Code Lines: 40 (50.00%)
    ' Comment Lines: 28 (35.00%)
    '    - Xml Docs: 25.00%
    ' 
    '   Blank Lines: 12 (15.00%)
    '     File Size: 3.27 KB


    '     Class FastFourierTransform1D
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) computeFFT
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 
'  Copyright (c) 2013 EMBL, European Bioinformatics Institute.
' 
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
' 
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU Lesser General Public License for more details.
' 
'  You should have received a copy of the GNU Lesser General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.
' 

Imports Microsoft.VisualBasic.Math.SignalProcessing.FFT

Namespace fidMath.FFT

    ''' 
    ''' <summary>
    ''' @author  Luis F. de Figueiredo
    ''' User: ldpf
    ''' Date: 24/05/2013
    ''' Time: 16:48
    ''' To change this template use File | Settings | File Templates.
    ''' </summary>
    Public Class FastFourierTransform1D : Implements FastFourierTransform

        Friend spectrum As Spectrum

        Public Sub New(spectrum As Spectrum)
            Me.spectrum = spectrum

        End Sub

        Public Overridable Function computeFFT(offset As Integer) As Spectrum Implements FastFourierTransform.computeFFT
            Dim fid = spectrum.Fid
            ' run the FFT
            Dim fftd As New DoubleFFT_1D(fid.Length / 2 - offset)
            fftd.ComplexForward(fid, offset)

            Dim realChannel = New Double(fid.Length / 2 - 1) {}
            Dim imagChannel = New Double(fid.Length / 2 - 1) {}
            Dim maxFFTdata As Double = 0
            Dim minFFTdata As Double = 0

            ' calculate the maximum and minimum to normalize the intentisity
            For i = 0 To fid.Length - 1
                minFFTdata = Math.Min(minFFTdata, fid(i))
                maxFFTdata = Math.Max(maxFFTdata, fid(i))
            Next

            ' extract the real an imaginary parts and normalize
            For i = 0 To fid.Length - 1 Step 2
                realChannel(i / 2) = fid(i) / (maxFFTdata - minFFTdata)
                imagChannel(i / 2) = fid(i + 1) / (maxFFTdata - minFFTdata)
            Next
            ' flip each half...
            Dim tmpReal = New Double(realChannel.Length - 1) {}
            Dim tmpImag = New Double(imagChannel.Length - 1) {}
            For i As Integer = 0 To realChannel.Length / 2 - 1
                tmpReal(realChannel.Length / 2 - i) = realChannel(i)
                tmpImag(realChannel.Length / 2 - i) = imagChannel(i)
                tmpReal(realChannel.Length - i - 1) = realChannel(realChannel.Length / 2 + i - 1)
                tmpImag(imagChannel.Length - i - 1) = imagChannel(realChannel.Length / 2 + i - 1)
            Next
            spectrum.RealChannelData = tmpReal
            spectrum.ImaginaryChannelData = tmpImag
            Return spectrum
        End Function

        Public Overridable Function computeFFT() As Spectrum Implements FastFourierTransform.computeFFT
            Return computeFFT(0)
        End Function
    End Class

End Namespace

#Region "Microsoft.VisualBasic::ea7901349ffa8e16db57ebc4c5d65378, src\assembly\assembly\MarkupData\mzML\UVScan.vb"

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

    '     Class UVScan
    ' 
    '         Properties: intensity, scan_time, total_ion_current, wavelength
    ' 
    '         Function: GetSignalModel, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace MarkupData.mzML

    Public Class UVScan

        Public Property wavelength As Double()
        Public Property intensity As Double()
        Public Property total_ion_current As Double
        Public Property scan_time As Double

        Public Overrides Function ToString() As String
            Return $"total_ions:{total_ion_current.ToString("G3")} at {CInt(scan_time)} sec"
        End Function

        Public Function GetSignalModel() As GeneralSignal
            Return New GeneralSignal With {
                .description = ToString(),
                .Measures = wavelength,
                .measureUnit = "wavelength",
                .reference = ToString(),
                .Strength = intensity,
                .meta = New Dictionary(Of String, String) From {
                    {"title", .reference}
                }
            }
        End Function

    End Class
End Namespace

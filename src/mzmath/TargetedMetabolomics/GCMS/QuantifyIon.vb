#Region "Microsoft.VisualBasic::be734c97809a8abea1747de0cedf291b, TargetedMetabolomics\GCMS\QuantifyIon.vb"

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

    '     Class QuantifyIon
    ' 
    '         Properties: id, ms, ri, rt
    ' 
    '         Function: FromIons, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS

    ''' <summary>
    ''' 定量离子模型
    ''' </summary>
    Public Class QuantifyIon : Implements INamedValue

        Public Property id As String Implements INamedValue.Key
        Public Property rt As DoubleRange
        ''' <summary>
        ''' 保留指数
        ''' </summary>
        ''' <returns></returns>
        Public Property ri As Double
        Public Property ms As ms2()

        Public Overrides Function ToString() As String
            Return $"Dim {id} As [{rt.Min}, {rt.Max}]"
        End Function

        Public Shared Function FromIons(ions As IEnumerable(Of MSLIon), rtwin As Double) As IEnumerable(Of QuantifyIon)
            Return ions _
                .Select(Function(ion)
                            Return New QuantifyIon With {
                                .id = ion.Name,
                                .ms = ion.Peaks,
                                .rt = New DoubleRange(ion.RT - rtwin, ion.RT + rtwin)
                            }
                        End Function)
        End Function

    End Class
End Namespace

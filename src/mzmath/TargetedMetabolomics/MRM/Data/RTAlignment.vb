#Region "Microsoft.VisualBasic::0f77e8a93e3c203d9fc59fa5ece5b328, src\mzmath\TargetedMetabolomics\MRM\Data\RTAlignment.vb"

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

    '     Class RTAlignment
    ' 
    '         Properties: actualRT, ion, samples
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CalcRtShifts, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.Distributions

Namespace MRM

    Public Class RTAlignment

        Public Property samples As NamedValue(Of Double)()
        Public Property actualRT As Double
        Public Property ion As IsomerismIonPairs

        Sub New(ion As IsomerismIonPairs, sampleValues As NamedValue(Of Double)())
            Me.ion = ion
            Me.samples = sampleValues
            Me.actualRT = samples.Values.TabulateMode
        End Sub

        Sub New()

        End Sub

        Public Iterator Function CalcRtShifts() As IEnumerable(Of NamedValue(Of Double))
            For Each sample As NamedValue(Of Double) In samples
                Yield New NamedValue(Of Double) With {
                    .Name = sample.Name,
                    .Value = sample.Value - actualRT,
                    .Description = sample.Value
                }
            Next
        End Function

        Public Overrides Function ToString() As String
            Return ion.ToString
        End Function
    End Class
End Namespace

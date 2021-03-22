#Region "Microsoft.VisualBasic::775fd7ce5d7ce32331eb1730421898eb, src\mzmath\ms2_math-core\UV\PDAPoint.vb"

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

    '     Class PDAPoint
    ' 
    '         Properties: scan_time, total_ion
    ' 
    '         Function: FromSignal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace UV

    Public Class PDAPoint : Implements ITimeSignal

        Public Property scan_time As Double Implements ITimeSignal.time
        Public Property total_ion As Double Implements ITimeSignal.intensity

        Public Shared Iterator Function FromSignal(PDA As GeneralSignal) As IEnumerable(Of PDAPoint)
            Dim x As Double() = PDA.Measures
            Dim y As Double() = PDA.Strength

            For i As Integer = 0 To PDA.Measures.Length - 1
                Yield New PDAPoint With {
                    .scan_time = x(i),
                    .total_ion = y(i)
                }
            Next
        End Function
    End Class
End Namespace

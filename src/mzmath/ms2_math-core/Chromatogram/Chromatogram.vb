﻿#Region "Microsoft.VisualBasic::56e7146797e03acbf5463410259c6f48, mzmath\ms2_math-core\Chromatogram\Chromatogram.vb"

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
    '    Code Lines: 36 (55.38%)
    ' Comment Lines: 19 (29.23%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (15.38%)
    '     File Size: 2.03 KB


    '     Class Chromatogram
    ' 
    '         Properties: BPC, length, maxInto, name, scan_time
    '                     TIC
    ' 
    '         Function: GetBpc, GetTic, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Chromatogram

    ''' <summary>
    ''' A data union model of TIC/BPC
    ''' </summary>
    Public Class Chromatogram : Implements INamedValue

        Public Property scan_time As Double()

        ''' <summary>
        ''' total ion current
        ''' </summary>
        ''' <returns></returns>
        Public Property TIC As Double()
        ''' <summary>
        ''' base peak intensity
        ''' </summary>
        ''' <returns></returns>
        Public Property BPC As Double()

        ''' <summary>
        ''' usually be a sample name
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String Implements INamedValue.Key

        ''' <summary>
        ''' the length of <see cref="scan_time"/> data points.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property length As Integer
            Get
                Return scan_time.Length
            End Get
        End Property

        Public ReadOnly Property maxInto As Double
            Get
                If length = 0 Then
                    Return 0
                Else
                    Return TIC.Max
                End If
            End Get
        End Property

        Public Iterator Function GetTic() As IEnumerable(Of ChromatogramTick)
            For i As Integer = 0 To scan_time.Length - 1
                Yield New ChromatogramTick(scan_time(i), TIC(i))
            Next
        End Function

        Public Iterator Function GetBpc() As IEnumerable(Of ChromatogramTick)
            For i As Integer = 0 To scan_time.Length - 1
                Yield New ChromatogramTick(scan_time(i), BPC(i))
            Next
        End Function

        Public Overrides Function ToString() As String
            Return $"Chromatogram between scan_time [{CInt(scan_time.Min)},{CInt(scan_time.Max)}]"
        End Function
    End Class
End Namespace

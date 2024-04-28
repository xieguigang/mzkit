#Region "Microsoft.VisualBasic::67f756742465ce471bda9de673b779ff, G:/mzkit/src/mzmath/ms2_math-core//Spectra/Models/Trimming/RelativeIntensityCutoff.vb"

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

    '   Total Lines: 61
    '    Code Lines: 39
    ' Comment Lines: 10
    '   Blank Lines: 12
    '     File Size: 2.01 KB


    '     Class RelativeIntensityCutoff
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: lowAbundanceTrimming, ToString
    ' 
    '         Sub: lowAbundanceTrimming
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Spectra

    ''' <summary>
    ''' cutoff for n/max(n) relative intensity data result
    ''' </summary>
    Public Class RelativeIntensityCutoff : Inherits LowAbundanceTrimming

        ''' <summary>
        ''' create a new percentage cutoff model
        ''' </summary>
        ''' <param name="cutoff">
        ''' ``[0,1]`` percentage cutoff value
        ''' </param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(cutoff As Double)
            MyBase.New(cutoff)
        End Sub

        Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
            Dim maxInto As Double = -999

            For Each fragment As ms2 In spectrum
                If fragment.intensity > maxInto Then
                    maxInto = fragment.intensity
                End If
            Next

            Return spectrum _
                .Where(Function(a) (a.intensity / maxInto) >= m_threshold) _
                .ToArray
        End Function

        Protected Overrides Sub lowAbundanceTrimming(ByRef mz() As Double, ByRef into() As Double)
            Dim maxInto As Double = into.Max
            Dim mzList As New List(Of Double)
            Dim intoList As New List(Of Double)

            For i As Integer = 0 To mz.Length - 1
                If into(i) / maxInto >= m_threshold Then
                    mzList.Add(mz(i))
                    intoList.Add(into(i))
                End If
            Next

            mz = mzList.ToArray
            into = intoList.ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return $"relative_intensity >= {m_threshold * 100}%"
        End Function

        Public Shared Widening Operator CType(cutoff As Double) As RelativeIntensityCutoff
            Return New RelativeIntensityCutoff(cutoff)
        End Operator

    End Class
End Namespace

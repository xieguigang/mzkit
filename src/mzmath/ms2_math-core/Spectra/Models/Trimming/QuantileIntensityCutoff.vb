#Region "Microsoft.VisualBasic::62214fdce419e1e11fa93ca72f58afc3, G:/mzkit/src/mzmath/ms2_math-core//Spectra/Models/Trimming/QuantileIntensityCutoff.vb"

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

    '   Total Lines: 42
    '    Code Lines: 32
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.47 KB


    '     Class QuantileIntensityCutoff
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
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Spectra

    Public Class QuantileIntensityCutoff : Inherits LowAbundanceTrimming

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(cutoff As Double)
            MyBase.New(cutoff)
        End Sub

        Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
            Dim quantile = spectrum.Select(Function(a) a.intensity).GKQuantile
            Dim threshold As Double = quantile.Query(Me.m_threshold)

            Return spectrum.Where(Function(a) a.intensity >= threshold).ToArray
        End Function

        Protected Overrides Sub lowAbundanceTrimming(ByRef mz() As Double, ByRef into() As Double)
            Dim quantile = into.GKQuantile
            Dim threshold As Double = quantile.Query(Me.m_threshold)
            Dim mzList As New List(Of Double)
            Dim intoList As New List(Of Double)

            For i As Integer = 0 To mz.Length - 1
                If into(i) >= threshold Then
                    mzList.Add(mz(i))
                    intoList.Add(into(i))
                End If
            Next

            mz = mzList.ToArray
            into = intoList.ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return $"intensity_quantile >= {m_threshold}"
        End Function

    End Class
End Namespace

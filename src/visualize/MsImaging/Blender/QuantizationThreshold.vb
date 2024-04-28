#Region "Microsoft.VisualBasic::10b83477b304e5ad587c81df2401f791, E:/mzkit/src/visualize/MsImaging//Blender/QuantizationThreshold.vb"

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

    '   Total Lines: 127
    '    Code Lines: 73
    ' Comment Lines: 33
    '   Blank Lines: 21
    '     File Size: 4.16 KB


    '     Delegate Function
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Class QuantizationThreshold
    ' 
    '         Properties: qcut
    ' 
    '         Function: RankQuantileThreshold, ThresholdValue, TrIQThreshold
    ' 
    '     Class RankQuantileThreshold
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ThresholdValue, ToString
    ' 
    '     Class TrIQThreshold
    ' 
    '         Properties: levels
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ThresholdValue, ToString
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Blender

    Public Delegate Function IThreshold(intensity As Double(), q As Double) As Double
    Public Delegate Function IQuantizationThreshold(intensity As Double()) As Double

    Public MustInherit Class QuantizationThreshold

        Public Property qcut As Double = 0.65

        ''' <summary>
        ''' auto check for intensity cut threshold value
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <returns>
        ''' percentage cutoff value
        ''' </returns>
        Public MustOverride Function ThresholdValue(intensity As Double(), q As Double) As Double

        Public Function ThresholdValue(intensity As Double()) As Double
            Return ThresholdValue(intensity, qcut)
        End Function

        Public Shared Function RankQuantileThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New RankQuantileThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <param name="qcut"></param>
        ''' <returns>
        ''' threshold value range in range ``[0,1]``.
        ''' </returns>
        Public Shared Function TrIQThreshold(intensity As Double(), qcut As Double) As Double
            Static q As New TrIQThreshold
            Return q.ThresholdValue(intensity, qcut)
        End Function

        'Public Shared Narrowing Operator CType(q As QuantizationThreshold) As IQuantizationThreshold
        '    Return AddressOf q.ThresholdValue
        'End Operator
    End Class

    Public Class RankQuantileThreshold : Inherits QuantizationThreshold

        Sub New()
        End Sub

        Sub New(q As Double)
            qcut = q
        End Sub

        Public Overrides Function ToString() As String
            If qcut > 0 Then
                Return $"Quantile({qcut})"
            Else
                Return "Quantile"
            End If
        End Function

        ''' <summary>
        ''' auto check for intensity cut threshold value
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <returns>
        ''' percentage cutoff value
        ''' </returns>
        Public Overrides Function ThresholdValue(intensity As Double(), qcut As Double) As Double
            If intensity.IsNullOrEmpty Then
                Return 0
            Else
                Dim maxBin As Double() = intensity.TabulateBin(topBin:=True, bags:=5)
                Dim qtile As Double = New FastRankQuantile(maxBin).Query(qcut)
                Dim per As Double = qtile / intensity.Max

                Return per
            End If
        End Function
    End Class

    Public Class TrIQThreshold : Inherits QuantizationThreshold

        Public Property levels As Integer = 100

        Sub New()
            Call Me.New(0.9)
        End Sub

        Sub New(q As Double)
            qcut = q
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="intensity"></param>
        ''' <param name="qcut"></param>
        ''' <returns>
        ''' [0, 1] 
        ''' </returns>
        Public Overrides Function ThresholdValue(intensity() As Double, qcut As Double) As Double
            If intensity.IsNullOrEmpty Then
                Return 0
            ElseIf qcut >= 1 Then
                Return 1
            ElseIf qcut <= 0.0 Then
                Return intensity.Min / intensity.Max
            Else
                Return intensity.FindThreshold(qcut, N:=100) / intensity.Max
            End If
        End Function

        Public Overrides Function ToString() As String
            If qcut > 0 Then
                Return $"TrIQ({qcut})"
            Else
                Return "TrIQ"
            End If
        End Function
    End Class
End Namespace

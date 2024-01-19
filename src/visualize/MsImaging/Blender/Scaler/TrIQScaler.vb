#Region "Microsoft.VisualBasic::6a6284378dfa39b1357921a8789cf68b, mzkit\src\visualize\MsImaging\Blender\Scaler\TrIQScaler.vb"

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

'   Total Lines: 27
'    Code Lines: 19
' Comment Lines: 0
'   Blank Lines: 8
'     File Size: 788 B


'     Class TrIQScaler
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: DoIntensityScale, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Blender.Scaler

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class TrIQScaler : Inherits Scaler

        ''' <summary>
        ''' The TrIQ threshold
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Description("The TrIQ threshold.")>
        <XmlAttribute>
        Public Property threshold As Double

        Sub New(threshold As Double)
            Me.threshold = threshold
        End Sub

        Sub New()
            Call Me.New(0.8)
        End Sub

        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Dim q As Double = TrIQ.FindThreshold(into, threshold)
            Dim v As New Vector(into)

            If into.Any AndAlso into.Max > 0 AndAlso q = 0.0 Then
                ' do nothing
            Else
                v(v > q) = Vector.Scalar(q)
            End If

            Return v
        End Function

        Public Overrides Function ToScript() As String
            Return $"TrIQ({threshold.ToString("F4")})"
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::4174d0919e3bbc85b1ae30b5bfd9c757, visualize\MsImaging\Blender\Scaler\QuantileScaler.vb"

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

    '   Total Lines: 56
    '    Code Lines: 34
    ' Comment Lines: 11
    '   Blank Lines: 11
    '     File Size: 2.02 KB


    '     Class QuantileScaler
    ' 
    '         Properties: percentail, q
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: DoIntensityScale, ToScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Blender.Scaler

    ''' <summary>
    ''' fill the lower bound with a specific threshold value
    ''' </summary>
    Public Class QuantileScaler : Inherits Scaler

        ''' <summary>
        ''' the quantile or percentail threshold value for define the intensity lower bound.
        ''' </summary>
        ''' <returns></returns>
        <Description("the quantile or percentail threshold value for define the intensity lower bound.")>
        Public Property q As Double

        ''' <summary>
        ''' set the method for measure the signal intensity lower bound value: true means measure with percentage and false means measure with the quantile algorithm.
        ''' </summary>
        ''' <returns></returns>
        <Description("set the method for measure the signal intensity lower bound value: true means measure with percentage and false means measure with the quantile algorithm.")>
        Public Property percentail As Boolean

        Sub New()
            Call Me.New(0.5, percentail:=False)
        End Sub

        Sub New(Optional q As Double = 0.5, Optional percentail As Boolean = False)
            Me.q = q
            Me.percentail = percentail
        End Sub

        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Dim v As New Vector(into)
            Dim cutoff As Double

            If percentail Then
                cutoff = v.Max * q
            Else
                cutoff = into _
                    .GKQuantile _
                    .Query(q)
            End If

            v(v > cutoff) = Vector.Scalar(cutoff)

            Return v
        End Function

        Public Overrides Function ToScript() As String
            Return $"Q({q.ToString("F4")},percentail:={percentail.ToString.ToLower})"
        End Function
    End Class
End Namespace

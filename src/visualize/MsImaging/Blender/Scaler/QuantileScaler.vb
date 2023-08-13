#Region "Microsoft.VisualBasic::27842e10890776068977ff897d35fb2f, mzkit\src\visualize\MsImaging\Blender\Scaler\QuantileScaler.vb"

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

    '   Total Lines: 28
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 794 B


    '     Class QuantileScaler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DoIntensityScale, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Blender.Scaler

    Public Class QuantileScaler : Inherits Scaler

        ReadOnly q As Double

        Sub New(Optional q As Double = 0.5)
            Me.q = q
        End Sub

        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Dim quantile As QuantileEstimationGK = into.GKQuantile
            Dim cutoff As Double = quantile.Query(q)
            Dim v As New Vector(into)

            v(v > cutoff) = Vector.Scalar(cutoff)

            Return v
        End Function

        Public Overrides Function ToScript() As String
            Return $"Q({q.ToString("F4")})"
        End Function
    End Class
End Namespace

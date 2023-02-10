#Region "Microsoft.VisualBasic::4e221464574b5a02b8dfc032ba93cb75, mzkit\src\visualize\MsImaging\Blender\Scaler\DenoiseScaler.vb"

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
    '    Code Lines: 21
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 778 B


    '     Class DenoiseScaler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DoIntensityScale, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Namespace Blender.Scaler

    Public Class DenoiseScaler : Inherits Scaler

        ReadOnly q As Double

        Sub New(Optional q As Double = 0.01)
            Me.q = q
        End Sub

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim denoise As PixelData() = layer.MSILayer _
                .DensityCut(q) _
                .ToArray

            Return New SingleIonLayer With {
                .MSILayer = denoise,
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"denoise(q:={q})"
        End Function
    End Class
End Namespace

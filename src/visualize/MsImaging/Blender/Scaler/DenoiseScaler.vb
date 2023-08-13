#Region "Microsoft.VisualBasic::9d608782b73a0a68adb7e5b71bdfb3d3, mzkit\src\visualize\MsImaging\Blender\Scaler\DenoiseScaler.vb"

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
'    Code Lines: 21
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 776 B


'     Class DenoiseScaler
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: DoIntensityScale, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Blender.Scaler

    Public Class DenoiseScaler : Inherits Scaler

        ''' <summary>
        ''' The density cutoff that treated the pixel as noise pixels
        ''' </summary>
        ''' <returns></returns>
        <Description("The density cutoff that treate some pixel as the noise pixels.")>
        Public Property q As Double

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

        Public Overrides Function ToScript() As String
            Return $"denoise({q})"
        End Function
    End Class
End Namespace

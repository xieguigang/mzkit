#Region "Microsoft.VisualBasic::b56247b0944e233bc2282132870eca7a, mzkit\src\visualize\MsImaging\Blender\Scaler\SoftenScaler.vb"

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

'   Total Lines: 77
'    Code Lines: 50
' Comment Lines: 15
'   Blank Lines: 12
'     File Size: 3.11 KB


'     Class SoftenScaler
' 
'         Function: DoIntensityScale, PopulateRectangle, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

Namespace Blender.Scaler

    Public Class SoftenScaler : Inherits Scaler

        Private Iterator Function PopulateRectangle(img As Grid(Of PixelData), x As Integer, y As Integer) As IEnumerable(Of Double)
            ' A  B  C  D
            ' E  F  G  H
            ' I  J  K  L
            ' M  N  O  P

            ' F = (A + B + C + E + F + G + I + J + K) / 9
            Dim A = img.GetData(x - 1, y - 1)
            Dim B = img.GetData(x, y - 1)
            Dim C = img.GetData(x + 1, y - 1)
            Dim E = img.GetData(x - 1, y)
            Dim F = img.GetData(x, y)
            Dim G = img.GetData(x + 1, y)
            Dim I = img.GetData(x - 1, y + 1)
            Dim J = img.GetData(x, y + 1)
            Dim K = img.GetData(x + 1, y + 1)

            If A Is Nothing Then Yield 0.0 Else Yield A.intensity
            If B Is Nothing Then Yield 0.0 Else Yield B.intensity
            If C Is Nothing Then Yield 0.0 Else Yield C.intensity
            If E Is Nothing Then Yield 0.0 Else Yield E.intensity
            If F Is Nothing Then Yield 0.0 Else Yield F.intensity
            If G Is Nothing Then Yield 0.0 Else Yield G.intensity
            If I Is Nothing Then Yield 0.0 Else Yield I.intensity
            If J Is Nothing Then Yield 0.0 Else Yield J.intensity
            If K Is Nothing Then Yield 0.0 Else Yield K.intensity
        End Function

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim img As Grid(Of PixelData) = Grid(Of PixelData).Create(layer.MSILayer, Function(p) p.x, Function(p) p.y)
            Dim OutPutWid = layer.DimensionSize.Width
            Dim OutPutHei = layer.DimensionSize.Height

            For x As Integer = 1 To OutPutWid
                For y As Integer = 1 To OutPutHei
                    Dim u As Double = PopulateRectangle(img, x, y).Average
                    Dim hit As Boolean = False
                    Dim pixel As PixelData = img.GetData(x, y, hit)

                    If u < 0 Then u = 0

                    If hit Then
                        pixel.intensity = u
                    Else
                        ' do not add empty pixel
                        'If u > 0 Then
                        '    pixel = New PixelData With {
                        '        .intensity = u,
                        '        .mz = -1,
                        '        .x = x,
                        '        .y = y
                        '    }

                        '    Call img.Add(pixel)
                        'End If
                    End If
                Next
            Next

            Return New SingleIonLayer With {
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz,
                .MSILayer = img.EnumerateData.ToArray
            }
        End Function

        Public Overrides Function ToString() As String
            Return "soften()"
        End Function
    End Class
End Namespace

Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Blender.Scaler

    Public Class SoftenScaler : Inherits Scaler

        ReadOnly max As QuantizationThreshold

        Sub New(max As QuantizationThreshold)
            Me.max = max
        End Sub

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim img As Grid(Of PixelData) = Grid(Of PixelData).Create(layer.MSILayer, Function(p) p.x, Function(p) p.y)
            Dim OutPutWid = layer.DimensionSize.Width
            Dim OutPutHei = layer.DimensionSize.Height
            Dim max As Double = Me.max.ThresholdValue(layer.GetIntensity)

            For x As Integer = 1 To OutPutWid - 2
                For y As Integer = 1 To OutPutHei - 2
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
                    Dim u = New Double() {
                        A?.intensity,
                        B?.intensity,
                        C?.intensity,
                        E?.intensity,
                        F?.intensity,
                        G?.intensity,
                        I?.intensity,
                        J?.intensity,
                        K?.intensity
                    }.Average

                    If u < 0 Then u = 0
                    If u > max Then u = max

                    Dim hit As Boolean = False
                    Dim pixel As PixelData = img.GetData(x, y, hit)

                    If hit Then
                        pixel.intensity = u
                    Else
                        pixel = New PixelData With {.intensity = u, .mz = -1, .x = x, .y = y}
                        img.Add(pixel)
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
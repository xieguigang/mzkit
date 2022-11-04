Public MustInherit Class Scaler : Implements LayerScaler

    Public Interface LayerScaler
        Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
    End Interface

    Public Overridable Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer Implements LayerScaler.DoIntensityScale
        Dim pixels = layer.MSILayer
        Dim intensity As Double() = pixels.Select(Function(i) i.intensity).ToArray
        Dim maxScale As Double

        intensity = DoIntensityScale(intensity)
        maxScale = intensity.Max
        pixels = pixels _
            .Select(Function(p, i)
                        Return New PixelData With {
                            .intensity = intensity(i),
                            .level = .intensity / maxScale,
                            .mz = p.mz,
                            .sampleTag = p.sampleTag,
                            .x = p.x,
                            .y = p.y
                        }
                    End Function) _
            .ToArray

        Return New SingleIonLayer With {
            .DimensionSize = layer.DimensionSize,
            .IonMz = layer.IonMz,
            .MSILayer = pixels
        }
    End Function

    Protected MustOverride Function DoIntensityScale(into As Double()) As Double()

End Class


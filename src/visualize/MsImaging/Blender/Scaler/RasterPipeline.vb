Imports System.Runtime.CompilerServices

Public Class RasterPipeline : Implements Scaler.LayerScaler

    ReadOnly pipeline As New List(Of Scaler)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(scaler As Scaler)
        Call pipeline.Add(scaler)
    End Sub

    Public Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer Implements Scaler.LayerScaler.DoIntensityScale
        For Each shader As Scaler In pipeline
            layer = shader.DoIntensityScale(layer)
        Next

        Return layer
    End Function

    Public Overrides Function ToString() As String
        Return pipeline.JoinBy(" -> ")
    End Function
End Class

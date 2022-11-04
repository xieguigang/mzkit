Imports System.Runtime.CompilerServices

Public Class RasterPipeline : Implements Scaler.LayerScaler, IEnumerable(Of Scaler)

    ReadOnly pipeline As New List(Of Scaler)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(scaler As Scaler)
        Call pipeline.Add(scaler)
    End Sub

    Public Function [Then](scaler As Scaler) As RasterPipeline
        Call pipeline.Add(scaler)
        Return Me
    End Function

    Public Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer Implements Scaler.LayerScaler.DoIntensityScale
        For Each shader As Scaler In pipeline
            layer = shader.DoIntensityScale(layer)
        Next

        Return layer
    End Function

    Public Shared Function GetDefaultPipeline() As RasterPipeline
        Return New RasterPipeline From {
            New LogScaler,
            New TrIQScaler
        }
    End Function

    Public Overrides Function ToString() As String
        Return pipeline.JoinBy(" -> ")
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of Scaler) Implements IEnumerable(Of Scaler).GetEnumerator
        For Each scaler As Scaler In pipeline
            Yield scaler
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class

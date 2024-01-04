Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Blender.Scaler

    ''' <summary>
    ''' this scaler will set spot intensity data to zero if its raw value is 
    ''' less than the specific cutoff threshold.
    ''' </summary>
    Public Class IntensityCutScaler : Inherits Scaler

        <XmlAttribute> Public Property percentage As Boolean = True
        <XmlAttribute> Public Property cutoff As Double = 0.05

        Sub New()
            Call Me.New(0.05, quantile:=False)
        End Sub

        Sub New(threshold As Double, quantile As Boolean)
            cutoff = threshold
            percentage = Not quantile
        End Sub

        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Dim threshold As Double = If(percentage, into.Max * cutoff, into.GKQuantile.Query(cutoff))
            Dim v As New Vector(into)

            v(v < threshold) = Vector.Zero

            Return v
        End Function

        Public Overrides Function ToScript() As String
            Return $"cut({cutoff}, {percentage})"
        End Function
    End Class
End Namespace
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Blender.Scaler

    Public Class PowerScaler : Inherits Scaler

        <XmlAttribute> Public Property pow As Double = 2

        Sub New()
            Call Me.New(p:=2)
        End Sub

        Sub New(p As Double)
            pow = p
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Return New Vector(into) ^ pow
        End Function

        Public Overrides Function ToScript() As String
            Return $"power({pow})"
        End Function
    End Class
End Namespace
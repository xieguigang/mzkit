Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace Blender.Scaler

    Public Class LogScaler : Inherits Scaler

        ReadOnly base As Double = stdNum.E

        Sub New(Optional base As Double = stdNum.E)
            Me.base = base
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function DoIntensityScale(into() As Double) As Double()
            Return New Vector(into).Log(base)
        End Function

        Public Overrides Function ToString() As String
            Return $"log({base.ToString("F4")})"
        End Function
    End Class
End Namespace
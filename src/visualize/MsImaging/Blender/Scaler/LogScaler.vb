Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Public Class LogScaler : Inherits Scaler

    ReadOnly base As Double = stdNum.E

    Sub New(Optional base As Double = stdNum.E)
        Me.base = base
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function DoIntensityScale(into() As Double) As Double()
        Return New Vector(into).Log(base)
    End Function
End Class

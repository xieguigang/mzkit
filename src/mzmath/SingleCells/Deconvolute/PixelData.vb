Imports System.Runtime.CompilerServices

Namespace Deconvolute

    Public Class PixelData

        Public Property X As Integer
        Public Property Y As Integer
        Public Property intensity As Double()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{X},{Y}]"
        End Function

    End Class
End Namespace
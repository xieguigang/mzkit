Imports System.Runtime.CompilerServices

Namespace Deconvolute

    ''' <summary>
    ''' A pixel data or single cell data
    ''' </summary>
    ''' <remarks>
    ''' the value of property <see cref="X"/> and <see cref="Y"/> is zero when the 
    ''' matrix data is a single cell data matrix
    ''' </remarks>
    Public Class PixelData

        Public Property X As Integer
        Public Property Y As Integer
        ''' <summary>
        ''' scan id or the single cell label
        ''' </summary>
        ''' <returns></returns>
        Public Property label As String
        Public Property intensity As Double()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{X},{Y}]"
        End Function

    End Class
End Namespace
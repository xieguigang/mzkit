Imports System.Drawing
Imports System.IO

Public Class ReadRawPack : Inherits PixelReader

    Public Overrides ReadOnly Property dimension As Size

    Dim raw As mzPackPixel()

    Sub New(mzpack As String)
        Using file As Stream = mzpack.Open
            Me.raw = Assembly.mzPack _
                .ReadAll(file).MS _
                .Select(Function(pixel)
                            Return New mzPackPixel(pixel)
                        End Function) _
                .ToArray
        End Using
    End Sub

    Protected Overrides Sub release()
        Erase raw
    End Sub
End Class

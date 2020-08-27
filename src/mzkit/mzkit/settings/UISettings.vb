Public Class UISettings

    Public Property x As Integer
    Public Property y As Integer
    Public Property width As Integer
    Public Property height As Integer
    Public Property window As FormWindowState

    Public Property rememberWindowsLocation As Boolean = True

    Public Function getLocation() As Point
        If x < 0 Then x = 0
        If y < 0 Then y = 0

        Return New Point(x, y)
    End Function

    Public Function getSize() As Size
        Return New Size(width, height)
    End Function
End Class

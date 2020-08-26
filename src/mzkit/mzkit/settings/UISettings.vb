Public Class UISettings

    Public Property x As Integer
    Public Property y As Integer
    Public Property width As Integer
    Public Property height As Integer

    Public Function getLocation() As Point
        Return New Point(x, y)
    End Function

    Public Function getSize() As Size
        Return New Size(width, height)
    End Function
End Class

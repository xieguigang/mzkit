Imports RibbonLib

Public Class UISettings

    Public Property x As Integer
    Public Property y As Integer
    Public Property width As Integer
    Public Property height As Integer
    Public Property window As FormWindowState

    Public Property rememberWindowsLocation As Boolean = True


    ' set ribbon colors
    ' _ribbon.SetColors(Color.Wheat, Color.IndianRed, Color.BlueViolet)
    Public Property background As Integer()
    Public Property highlight As Integer()
    Public Property text As Integer()

    Public Sub setColors(ribbon As Ribbon)
        If Me.background.IsNullOrEmpty OrElse Me.highlight.IsNullOrEmpty OrElse Me.text.IsNullOrEmpty Then
            Return
        End If

        Dim background As Color = Color.FromArgb(Me.background(0), Me.background(1), Me.background(2))
        Dim highlight As Color = Color.FromArgb(Me.highlight(0), Me.highlight(1), Me.highlight(2))
        Dim text As Color = Color.FromArgb(Me.text(0), Me.text(1), Me.text(2))

        ribbon.SetColors(background, highlight, text)
    End Sub

    Public Function getLocation() As Point
        If x < 0 Then x = 0
        If y < 0 Then y = 0

        Return New Point(x, y)
    End Function

    Public Function getSize() As Size
        Return New Size(width, height)
    End Function
End Class

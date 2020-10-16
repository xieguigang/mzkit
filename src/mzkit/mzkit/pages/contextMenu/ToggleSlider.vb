Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D

Partial Public Class ToggleSlider
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        DoubleBuffered = True
        AddHandler Click, AddressOf ToggleSlider_Click
        AddHandler timer1.Tick, AddressOf Timer1_Tick
        AutoSize = True
    End Sub

    Public Event CheckChanged As EventHandler
    Private Checked_bool As Boolean

    Public Property Checked As Boolean
        Get
            Return Checked_bool
        End Get
        Set(ByVal value As Boolean)
            Checked_bool = value
            Invalidate()
        End Set
    End Property

    Private ToggleColorDisabled_Color As Color = Color.Green

    Public Property ToggleCircleColor As Color
        Get
            Return ToggleColorDisabled_Color
        End Get
        Set(ByVal value As Color)
            ToggleColorDisabled_Color = value
            Invalidate()
        End Set
    End Property

    Private Bar_Color As Color = Color.Gray

    Public Property ToggleColorBar As Color
        Get
            Return Bar_Color
        End Get
        Set(ByVal value As Color)
            Bar_Color = value
            Invalidate()
        End Set
    End Property

    Private Text As String = "toggleSlider1"

    Public Property ToggleBarText As String
        Get
            Return Text
        End Get
        Set(ByVal value As String)
            Text = value
            Invalidate()
        End Set
    End Property

    Private posx As Integer = 0
    Private posy As Integer = 0
    Private init_ As Boolean = True
    Private circlecolor_ As Color = New Color()
    Private animating_ As Boolean = False

    Protected Overrides Sub OnPaint(ByVal pevent As PaintEventArgs)
        If init_ = True Then
            circlecolor_ = ToggleColorDisabled_Color
        End If

        pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality
        Dim circle_size As Size = New Size(Convert.ToInt32(MyBase.Font.SizeInPoints * 5), Convert.ToInt32(MyBase.Font.SizeInPoints * 5))
        RoundedRect(Bar_Color, pevent.Graphics, New Rectangle(circle_size.Width / 4, circle_size.Height / 5 / 2, circle_size.Width / 2, 3 * (circle_size.Height / 5) / 2), 5)
        Dim brush_gradient As LinearGradientBrush = New LinearGradientBrush(New Point(circle_size.Width / 4, circle_size.Height / 5 / 2), New Point(circle_size.Width / 2, circle_size.Height / 2), ToggleColorDisabled_Color, ToggleColorDisabled_Color)

        If animating_ = False Then
            If Not Checked_bool Then
                posx = 0
            Else
                posx = circle_size.Width / 2
            End If
        End If

        pevent.Graphics.FillEllipse(New SolidBrush(ToggleColorDisabled_Color), CSng(posx), CSng(posy), CSng(circle_size.Width / 2), CSng(circle_size.Height / 2))
        TextRenderer.DrawText(pevent.Graphics, ToggleBarText, Font, New Point(circle_size.Width, circle_size.Height / 10), ForeColor)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
    End Sub

    Private Sub ToggleSlider_Click(ByVal sender As Object, ByVal e As EventArgs)
        Animate()
    End Sub

    Private timer1 As Timer = New Timer()

    Private Sub Animate()
        timer1.Interval = 1
        timer1.Start()
        animating_ = True
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Dim circle_size As Size = New Size(Convert.ToInt32(MyBase.Font.SizeInPoints * 5), Convert.ToInt32(MyBase.Font.SizeInPoints * 5))

        If Checked_bool = True Then
            If Not posx <= 0 Then
                posx -= 3
                Invalidate()
            Else
                Checked_bool = False
                animating_ = False
                RaiseEvent CheckChanged(Me, e)
                timer1.Stop()
            End If
        Else
            init_ = False

            If Not posx >= circle_size.Width / 2 Then
                posx += 3
                Invalidate()
            Else
                Checked_bool = True
                animating_ = False
                RaiseEvent CheckChanged(Me, e)
                timer1.Stop()
            End If
        End If
    End Sub

    Public Shared Function RoundedRect(ByVal c As Color, ByVal g As Graphics, ByVal bounds As Rectangle, ByVal radius As Integer) As GraphicsPath
        Dim diameter = radius * 2
        Dim size As Size = New Size(diameter, diameter)
        Dim arc As Rectangle = New Rectangle(bounds.Location, size)
        Dim path As GraphicsPath = New GraphicsPath()

        If radius = 0 Then
            path.AddRectangle(bounds)
            Return path
        End If

        ' top left arc  
        path.AddArc(arc, 180, 90)

        ' top right arc  
        arc.X = bounds.Right - diameter
        path.AddArc(arc, 270, 90)

        ' bottom right arc  
        arc.Y = bounds.Bottom - diameter
        path.AddArc(arc, 0, 90)

        ' bottom left arc 
        arc.X = bounds.Left
        path.AddArc(arc, 90, 90)
        g.FillPath(New SolidBrush(c), path)
        path.CloseFigure()
        Return path
    End Function
End Class
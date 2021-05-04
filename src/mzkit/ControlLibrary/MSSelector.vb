Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

<Assembly: InternalsVisibleTo("mzkit_win32")>

Public Class MSSelector

    Public Event ShowTIC()
    Public Event ShowBPC()
    Public Event FilterMs2(rtmin As Double, rtmax As Double)
    Public Event RangeSelect(rtmin As Double, rtmax As Double)

    Public Property SelectedColor As Color
        Get
            Return RtRangeSelector1.SelectedColor
        End Get
        Set(value As Color)
            RtRangeSelector1.SelectedColor = value
        End Set
    End Property

    Public Property FillColor As Color
        Get
            Return RtRangeSelector1.FillColor
        End Get
        Set(value As Color)
            RtRangeSelector1.FillColor = value
        End Set
    End Property

    Public Property rtmin As Double
        Get
            Return RtRangeSelector1.rtmin
        End Get
        Set(value As Double)
            RtRangeSelector1.rtmin = value
        End Set
    End Property

    Public Property rtmax As Double
        Get
            Return RtRangeSelector1.rtmax
        End Get
        Set(value As Double)
            RtRangeSelector1.rtmax = value
        End Set
    End Property

    Public ReadOnly Property Pinned As Boolean
        Get
            Return PinToolStripMenuItem.Checked
        End Get
    End Property

    Private Sub MSSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        BackColor = Color.White
    End Sub

    Dim rawTIC As ChromatogramTick()

    Public Sub SetTIC(TIC As ChromatogramTick())
        rawTIC = TIC
        RtRangeSelector1.SetTIC(TIC)
    End Sub

    Public Sub RefreshRtRangeSelector()
        Call RtRangeSelector1.RefreshRtRangeSelector()
    End Sub

    Private Sub showTICClick() Handles TICToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = True
        BPCToolStripMenuItem.Checked = False

        RaiseEvent ShowTIC()
    End Sub

    Private Sub showBPCClick() Handles BPCToolStripMenuItem.Click
        TICToolStripMenuItem.Checked = False
        BPCToolStripMenuItem.Checked = True

        RaiseEvent ShowBPC()
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterMs2ToolStripMenuItem.Click
        RaiseEvent FilterMs2(RtRangeSelector1.rtmin, RtRangeSelector1.rtmax)
    End Sub

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        PinToolStripMenuItem.Checked = Not PinToolStripMenuItem.Checked
    End Sub

    Private Sub ResetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem.Click
        PinToolStripMenuItem.Checked = False
        RtRangeSelector1.SetTIC(rawTIC)
        RtRangeSelector1.RefreshRtRangeSelector()
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles RtRangeSelector1.RangeSelect
        RaiseEvent RangeSelect(min, max)

        If Not Pinned Then
            ' update range 
            Dim newRange As ChromatogramTick() = rawTIC _
                .Where(Function(t)
                           Return t.Time >= min AndAlso t.Time <= max
                       End Function) _
                .ToArray

            RtRangeSelector1.SetTIC(newRange)
            RtRangeSelector1.RefreshRtRangeSelector()
        End If
    End Sub
End Class

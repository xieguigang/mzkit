#Region "Microsoft.VisualBasic::982b194a814f7922fcaf170617e0d024, mzkit\src\mzkit\ControlLibrary\MSSelector.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 144
    '    Code Lines: 102
    ' Comment Lines: 15
    '   Blank Lines: 27
    '     File Size: 4.66 KB


    ' Class MSSelector
    ' 
    '     Properties: FillColor, Pinned, rtmax, rtmin, SelectedColor
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: FilterMs2ToolStripMenuItem_Click, MSSelector_Load, PinToolStripMenuItem_Click, RefreshRtRangeSelector, ResetToolStripMenuItem_Click
    '          RtRangeSelector1_RangeSelect, SetTIC, showBPCClick, showTICClick, XICToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

<Assembly: InternalsVisibleTo("mzkit_win32")>

''' <summary>
''' 主要是应用于非靶向数据的查看
''' </summary>
Public Class MSSelector

    Public Event ShowTIC()
    Public Event ShowBPC()
    Public Event FilterMs2(rtmin As Double, rtmax As Double)
    Public Event RangeSelect(rtmin As Double, rtmax As Double)
    Public Event XICSelector(rtmin As Double, rtmax As Double)

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
        FillColor = Color.SteelBlue
    End Sub

    Dim rawTIC As ChromatogramTick()

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' DoubleBuffered = True

        ' SetStyle(ControlStyles.UserPaint, True)
        ' SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        ' SetStyle(ControlStyles.DoubleBuffer, True)
    End Sub

    Public Sub SetTIC(TIC As ChromatogramTick())
        rawTIC = TIC
        RtRangeSelector1.SetTIC(TIC)
        RtRangeSelector1.AllowMoveRange = Pinned
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
        RtRangeSelector1.AllowMoveRange = Pinned
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

            If newRange.Length > 0 Then
                RtRangeSelector1.SetTIC(newRange)
                RtRangeSelector1.RefreshRtRangeSelector()
            End If
        End If
    End Sub

    ''' <summary>
    ''' 弹出对话框，然后选择XIC谱图的目标离子
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub XICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XICToolStripMenuItem.Click
        RaiseEvent XICSelector(RtRangeSelector1.rtmin, RtRangeSelector1.rtmax)
    End Sub
End Class

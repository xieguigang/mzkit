#Region "Microsoft.VisualBasic::6fef7c51950409b66f5caa6fcdb0a9ae, mzkit\src\mzkit\mzkit\forms\Task\frmProgressSpinner.vb"

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

    '   Total Lines: 58
    '    Code Lines: 39
    ' Comment Lines: 6
    '   Blank Lines: 13
    '     File Size: 1.91 KB


    ' Class frmProgressSpinner
    ' 
    '     Sub: CloseWindow, DoLoading, frmProgressSpinner_Closed, frmProgressSpinner_Load, frmProgressSpinner_Paint
    '          OnFrameChanged, RunAnimation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading

Public Class frmProgressSpinner

    Dim theImage As Image = My.Resources.spinner

    <DebuggerStepThrough>
    Sub RunAnimation()
        ' the image Is set up for animation using the
        ' ImageAnimator class And an event handler
        ImageAnimator.Animate(theImage, New EventHandler(AddressOf OnFrameChanged))
    End Sub

    <DebuggerStepThrough>
    Private Sub OnFrameChanged(o As Object, e As EventArgs)
        Me.Invalidate()

        ' BackgroundImage = theImage
    End Sub

    <DebuggerStepThrough>
    Private Sub frmProgressSpinner_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g = e.Graphics

        'Get the next frame ready for rendering for the image in this thread.
        ImageAnimator.UpdateFrames()
        'Draw the current frame in the animation for all of the images
        g.DrawImageUnscaled(theImage, 0, 0, theImage.Width, theImage.Height)
    End Sub

    Private Sub frmProgressSpinner_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.TransparencyKey = Color.Green
        Me.BackColor = Color.Green
        Me.DoubleBuffered = False

        RunAnimation()
        TaskbarStatus.SetLoopStatus()
    End Sub

    Private Sub frmProgressSpinner_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        TaskbarStatus.Stop()
    End Sub

    Public Shared Sub DoLoading(loading As Action)
        Dim spinner As New frmProgressSpinner

        Call New Thread(Sub()
                            Call loading()
                            Call spinner.Invoke(Sub() spinner.Close())
                        End Sub).Start()
        Call spinner.ShowDialog()
    End Sub

    Friend Sub CloseWindow()
        Call Invoke(Sub() Call Close())
    End Sub
End Class

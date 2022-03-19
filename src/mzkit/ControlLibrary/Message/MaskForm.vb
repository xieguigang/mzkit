#Region "Microsoft.VisualBasic::ebcb768b9e3995ca55b1c4ecba3ac80d, mzkit\src\mzkit\ControlLibrary\Message\MaskForm.vb"

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

    '   Total Lines: 34
    '    Code Lines: 22
    ' Comment Lines: 2
    '   Blank Lines: 10
    '     File Size: 877.00 B


    ' Class MaskForm
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ShowDialogForm
    ' 
    ' /********************************************************************************/

#End Region

Public Class MaskForm

    Sub New(point As Point, size As Size)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Opacity = 0.5
        BackColor = Color.LightGray
        FormBorderStyle = FormBorderStyle.None
        StartPosition = FormStartPosition.Manual

        Dim dx As Integer = 7
        Dim dy As Integer = 7

        Me.Location = New Point(point.X + dx, point.Y)
        Me.Size = New Size(size.Width - dx * 2, size.Height - dy)

        Me.Text = ""
        Me.ShowInTaskbar = False
        Me.ShowIcon = False

    End Sub

    Public Function ShowDialogForm(dialog As Form) As DialogResult
        Me.Show()
        Dim result = dialog.ShowDialog
        Me.Close()
        Return result
    End Function

End Class

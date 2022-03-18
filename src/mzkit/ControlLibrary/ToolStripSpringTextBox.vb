#Region "Microsoft.VisualBasic::89afa5571800be291157f72b6b751c0e, mzkit\src\mzkit\ControlLibrary\ToolStripSpringTextBox.vb"

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

    '   Total Lines: 60
    '    Code Lines: 29
    ' Comment Lines: 19
    '   Blank Lines: 12
    '     File Size: 2.51 KB


    ' Class ToolStripSpringTextBox
    ' 
    '     Function: GetPreferredSize
    ' 
    ' /********************************************************************************/

#End Region

Public Class ToolStripSpringTextBox
    Inherits ToolStripTextBox

    Public Overrides Function GetPreferredSize(
        ByVal constrainingSize As Size) As Size

        ' Use the default size if the text box is on the overflow menu
        ' or is on a vertical ToolStrip.
        If IsOnOverflow Or Owner.Orientation = Orientation.Vertical Then
            Return DefaultSize
        End If

        ' Declare a variable to store the total available width as 
        ' it is calculated, starting with the display width of the 
        ' owning ToolStrip.
        Dim width As Int32 = Owner.DisplayRectangle.Width

        ' Subtract the width of the overflow button if it is displayed. 
        If Owner.OverflowButton.Visible Then
            width = width - Owner.OverflowButton.Width -
                Owner.OverflowButton.Margin.Horizontal()
        End If

        ' Declare a variable to maintain a count of ToolStripSpringTextBox 
        ' items currently displayed in the owning ToolStrip. 
        Dim springBoxCount As Int32 = 0

        For Each item As ToolStripItem In Owner.Items

            ' Ignore items on the overflow menu.
            If item.IsOnOverflow Then Continue For

            If TypeOf item Is ToolStripSpringTextBox Then
                ' For ToolStripSpringTextBox items, increment the count and 
                ' subtract the margin width from the total available width.
                springBoxCount += 1
                width -= item.Margin.Horizontal
            Else
                ' For all other items, subtract the full width from the total
                ' available width.
                width = width - item.Width - item.Margin.Horizontal
            End If
        Next

        ' If there are multiple ToolStripSpringTextBox items in the owning
        ' ToolStrip, divide the total available width between them. 
        If springBoxCount > 1 Then width = CInt(width / springBoxCount)

        ' If the available width is less than the default width, use the
        ' default width, forcing one or more items onto the overflow menu.
        If width < DefaultSize.Width Then width = DefaultSize.Width

        ' Retrieve the preferred size from the base class, but change the
        ' width to the calculated width. 
        Dim preferredSize As Size = MyBase.GetPreferredSize(constrainingSize)
        preferredSize.Width = width
        Return preferredSize

    End Function
End Class

#Region "Microsoft.VisualBasic::9696f7084a899e5a36cd42137c150409, mzkit\src\mzkit\mzkit\forms\frmTweaks\frmTweaks.vb"

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

    '   Total Lines: 22
    '    Code Lines: 15
    ' Comment Lines: 1
    '   Blank Lines: 6
    '     File Size: 619.00 B


    ' Class frmTweaks
    ' 
    '     Properties: draw
    ' 
    '     Sub: frmTweaks_Load, PropertyGrid1_PropertyValueChanged
    ' 
    ' /********************************************************************************/

#End Region

Imports Task

Public Class frmTweaks

    Friend ReadOnly params As New PlotProperty

    Public Property draw As Action(Of PlotProperty)

    Private Sub frmTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        PropertyGrid1.SelectedObject = params
        PropertyGrid1.Refresh()

        Me.TabText = "Plot Styles"
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        ' 进行重绘？
        If Not draw Is Nothing Then
            Call _draw(params)
        End If
    End Sub
End Class

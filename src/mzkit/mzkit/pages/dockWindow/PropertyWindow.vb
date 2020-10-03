#Region "Microsoft.VisualBasic::0249413909871cce7e8cb7e74bbc4de2, src\mzkit\mzkit\pages\dockWindow\PropertyWindow.vb"

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

    '     Class DummyPropertyWindow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: DummyPropertyWindow_Closing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Windows.Forms
Imports Task

Namespace DockSample
    Partial Public Class DummyPropertyWindow
        Inherits ToolWindow

        Public Sub New()
            InitializeComponent()
            ' comboBox.SelectedIndex = 0
            propertyGrid.SelectedObject = New SpectrumProperty("n/a", "n/a", {})

            DoubleBuffered = True
        End Sub

        Private Sub DummyPropertyWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
            e.Cancel = True
            Me.Hide()
        End Sub

        Public Function getPropertyObject() As Object
            Return propertyGrid.SelectedObject
        End Function
    End Class
End Namespace


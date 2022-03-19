#Region "Microsoft.VisualBasic::379e050c44f144a03cc35ae8108903a3, mzkit\src\mzkit\mzkit\pages\dockWindow\tools\PropertyWindow.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20
    ' Comment Lines: 1
    '   Blank Lines: 5
    '     File Size: 777.00 B


    '     Class PropertyWindow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getPropertyObject
    ' 
    '         Sub: DummyPropertyWindow_Closing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Task

Namespace DockSample
    Partial Public Class PropertyWindow
        Inherits ToolWindow

        Public Sub New()
            InitializeComponent()
            ' comboBox.SelectedIndex = 0
            propertyGrid.SelectedObject = New SpectrumProperty("n/a", "n/a", 0, New ScanMS2)

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

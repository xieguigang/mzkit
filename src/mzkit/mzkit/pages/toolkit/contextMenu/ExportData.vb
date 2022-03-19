#Region "Microsoft.VisualBasic::f5bb727885d548578a74dac0e073bb88, mzkit\src\mzkit\mzkit\pages\toolkit\contextMenu\ExportData.vb"

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

    '   Total Lines: 20
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 552.00 B


    ' Class ExportData
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: OnOpening
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports RibbonLib

Public Class ExportData
    Inherits ContextMenuStrip

    Private _contextPopupID As UInteger
    Private _ribbon As Ribbon

    Public Sub New(ribbon As Ribbon, contextPopupID As UInteger)
        MyBase.New()
        _contextPopupID = contextPopupID
        _ribbon = ribbon
    End Sub

    Protected Overrides Sub OnOpening(e As CancelEventArgs)
        _ribbon.ShowContextPopup(_contextPopupID, Cursor.Position.X, Cursor.Position.Y)
        e.Cancel = True
    End Sub
End Class

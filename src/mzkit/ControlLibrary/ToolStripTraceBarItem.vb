#Region "Microsoft.VisualBasic::ea8c09108b0279702da72abc7477afb6, mzkit\src\mzkit\ControlLibrary\ToolStripTraceBarItem.vb"

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

    '   Total Lines: 30
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 828.00 B


    ' Class ToolStripTraceBarItem
    ' 
    '     Properties: TrackBar
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: SetValueRange
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Windows.Forms.Design

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip Or ToolStripItemDesignerAvailability.StatusStrip)>
Public Class ToolStripTraceBarItem : Inherits ToolStripControlHost

    Public Event AdjustValue(value As Integer)

    Public ReadOnly Property TrackBar As TrackBar
        Get
            Return Control
        End Get
    End Property

    Sub New()
        Call MyBase.New(New TrackBar)
    End Sub

    Public Sub SetValueRange(min As Integer, max As Integer)
        With DirectCast(Control, TrackBar)
            .Minimum = min
            .Maximum = max
            .Value = min
        End With

        AddHandler TrackBar.ValueChanged,
            Sub()
                RaiseEvent AdjustValue(TrackBar.Value)
            End Sub
    End Sub
End Class

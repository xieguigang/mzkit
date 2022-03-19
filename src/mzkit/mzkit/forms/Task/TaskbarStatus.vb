#Region "Microsoft.VisualBasic::8d9b208d4376f4463a75b6b2c3157db6, mzkit\src\mzkit\mzkit\forms\Task\TaskbarStatus.vb"

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

    '   Total Lines: 23
    '    Code Lines: 14
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 671.00 B


    ' Module TaskbarStatus
    ' 
    '     Sub: [Stop], SetLoopStatus, SetProgress
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.Windows.Taskbar

Module TaskbarStatus

    ''' <summary>
    ''' Keep a reference to the Taskbar instance
    ''' </summary>
    Dim windowsTaskbar As TaskbarManager = TaskbarManager.Instance

    Public Sub SetLoopStatus()
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate)
    End Sub

    Public Sub [Stop]()
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress)
    End Sub

    Public Sub SetProgress(percentage As Integer)
        windowsTaskbar.SetProgressState(TaskbarProgressBarState.Normal)
        windowsTaskbar.SetProgressValue(percentage, 100)
    End Sub

End Module

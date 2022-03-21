#Region "Microsoft.VisualBasic::2a2051f5d5cfcae3f58f6f8ceb194065, mzkit\src\mzkit\mzkit\forms\Inputs\InputDialog.vb"

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

    '   Total Lines: 19
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 731.00 B


    ' Class InputDialog
    ' 
    '     Sub: Input
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary

Public Class InputDialog

    Public Shared Sub Input(Of Form As {New, InputDialog})(setConfig As Action(Of Form),
                                                           Optional cancel As Action = Nothing,
                                                           Optional config As Form = Nothing)
        Dim getConfig As Form = If(config, New Form)
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getConfig) = DialogResult.OK Then
            Call setConfig(getConfig)
        ElseIf Not cancel Is Nothing Then
            Call cancel()
        End If
    End Sub

End Class

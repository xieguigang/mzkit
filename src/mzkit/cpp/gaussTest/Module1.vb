#Region "Microsoft.VisualBasic::3468b2902df573682236b6120d983620, src\mzkit\cpp\gaussTest\Module1.vb"

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

' Module Module1
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.InteropServices
Imports imagefilter
Imports imagefilter.Interop
Imports Microsoft.VisualBasic.Imaging

Module Module1

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Public Function SetDllDirectory(lpPathName As String) As Boolean
    End Function

    Sub Main()

        Call SetDllDirectory($"{App.HOME}/tools/cpp/")

        Dim blur As New imagefilter.GaussImageManager("D:\mzkit\src\mzkit\splash.PNG")
        Dim args As New GeneratorParameters With {.BlurLevel = 50, .GaussMaskSize = 9, .NumberOfThreads = 4}
        Dim result = blur.GenerateBlurredImageAsync(args)

        Call result.Result.FlushStream("./blur.bmp")

        Pause()
    End Sub

End Module


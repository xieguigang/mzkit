#Region "Microsoft.VisualBasic::d5d23a9896acf97e45d63092590ab09c, mzkit\src\mzkit\cpp\gaussTest\Module1.vb"

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
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 690.00 B


    ' Module Module1
    ' 
    '     Function: SetDllDirectory
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports imagefilter.Interop

Module Module1

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Public Function SetDllDirectory(lpPathName As String) As Boolean
    End Function

    Sub Main()

        Call SetDllDirectory($"{App.HOME}/tools/cpp/")

        Dim blur As New imagefilter.GaussImageManager("D:\mzkit\src\mzkit\splash.PNG") With {.debugMode = True}
        Dim args As New GeneratorParameters With {.BlurLevel = 1, .GaussMaskSize = 3, .NumberOfThreads = 1}
        Dim result = blur.GenerateBlurredImage(args)

        Call result.FlushStream("./blur.bmp")

        Pause()
    End Sub

End Module

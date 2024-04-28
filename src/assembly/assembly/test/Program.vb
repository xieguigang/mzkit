#Region "Microsoft.VisualBasic::0dcb843875fd84c16e56fd27d7ee9901, G:/mzkit/src/assembly/assembly/test//Program.vb"

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
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 700 B


    ' Module Program
    ' 
    '     Sub: Main, readRaman
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports Microsoft.VisualBasic.Linq

Module Program
    Sub Main(args As String())
        Call readRaman()
        Call read_imzML.Main1()
        Console.WriteLine("Hello World!")
    End Sub

    Sub readRaman()
        Using file = "E:\mzkit\src\mzkit\extdata\raman_spectroscopy\LS4.txt".OpenReader
            Dim data = Raman.FileReader.ParseTextFile(file)
            Dim obj = DynamicType.Create(data.Comments.JoinIterates(data.DetailedInformation).JoinIterates(data.MeasurementInformation).ToDictionary(Function(a) a.Key, Function(a) CObj(a.Value)))

            Pause()
        End Using
    End Sub
End Module

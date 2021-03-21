#Region "Microsoft.VisualBasic::be5b99d23495b7186c7645938b3a6741, src\assembly\MSFileReader.Interop\Includes.vb"

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

    ' Module Includes
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Module Includes

    ReadOnly _defaultPaths = {
        App.HOME & "\XRawfile2_x64.dll",                           ' Theoretical Bundled 64 bit MSFileReader DLL
        App.CurrentDirectory & "\XRawfile2_x64.dll",
        "C:\Program Files\Thermo\MSFileReader\XRawfile2_x64.dll",  ' Default Installation Path on 64 bit systems
        "C:\Program Files (x86)\Thermo\MSFileReader\XRawfile2.dll",
        "C:\Program Files\Thermo\MSFileReader\XRawfile2.dll"       ' Default Installation Path on 32 bit systems
    }

    Public Declare Function Open Lib "XRawfile2_x64.dll" (szFileName As String) As Integer
End Module

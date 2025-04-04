﻿#Region "Microsoft.VisualBasic::99547276ceb3581488eaec0ebf297310, assembly\Comprehensive\test\Module1.vb"

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

    '   Total Lines: 21
    '    Code Lines: 16 (76.19%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (23.81%)
    '     File Size: 676 B


    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal

Module Module1

    Sub Main()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)

        Dim demo As String = "F:\20211123_CDF\P210702366.netcdf"
        Dim gcxgc = netCDFReader.Open(demo).ToMzPack(5)

        Using file As Stream = "F:\20211123_CDF\P210702366.mzpack".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call gcxgc.Write(file)
        End Using

        Pause()
    End Sub
End Module

#Region "Microsoft.VisualBasic::5cdfe47b03d8a25a7d5ee1861fe5abbc, assembly\LoadR.NET5\test\Program.vb"

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

    '   Total Lines: 62
    '    Code Lines: 41
    ' Comment Lines: 10
    '   Blank Lines: 11
    '     File Size: 2.38 KB


    ' Module Program
    ' 
    '     Sub: Main, readtest2
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports SMRUCC.Rsharp.RDataSet
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct.LinkedList
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")

        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)
        readtest2()

        Using buffer As Stream = "D:\mzkit\src\assembly\LoadR.NET5\PeaksMs2.rda".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim data = Reader.ParseData(buffer).object.LinkVisitor("x")
            Dim list As RObject() = data.value.CAR.value.data
            Dim peaksData = list.Select(AddressOf LoadR.LoadPeakMs2).ToArray

            Pause()
        End Using
    End Sub

    Sub readtest2()
        Using buffer As Stream = "D:\mzkit\src\assembly\LoadR.NET5\003_Ex2_Orbitrap_CID.mzXML.RData".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim xcms = XcmsRData.ReadRData(buffer)

            Pause()
        End Using

        ' mz1, rt2, into, ms2
        Using buffer As Stream = "D:\mzkit\src\assembly\LoadR.NET5\003_Ex2_Orbitrap_CID.mzXML.RData".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim root = Reader.ParseData(buffer).object
            Dim data = root.LinkVisitor("mz1")
            Dim data2 = root.LinkVisitor("rt2")
            Dim data3 = root.LinkVisitor("into")
            Dim mz1 As Double() = data.value.CAR.value.data
            Dim rt2 As Double() = data2.value.CAR.value.data
            Dim into As Double() = data3.value.CAR.value.data

            Dim all = ConvertToR.ToRObject(root)

            ' > ms2[["M476T1, 476.3065@0min"]]
            ' mz intensity
            ' [1,] 175.1938  6.712281
            ' [2,] 417.5124 20.837078
            ' [3,] 419.4352 10.454858
            ' [4,] 431.6585 28.139475
            ' [5,] 465.2776  4.005749
            ' [6,] 467.3275  9.030666
            ' [7,] 489.1284  8.694260

            Dim test = DirectCast(all!ms2, list)
            Dim names = test.getNames
            Dim data_ms2 = test("M476T1, 476.3065@0min")

            Pause()
        End Using
    End Sub
End Module

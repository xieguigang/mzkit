#Region "Microsoft.VisualBasic::e3f9a8aac45e5528d84a220e792d89df, mzkit\src\assembly\BrukerDataReader\test\Program.vb"

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

    '   Total Lines: 42
    '    Code Lines: 31
    ' Comment Lines: 2
    '   Blank Lines: 9
    '     File Size: 1.30 KB


    ' Module Program
    ' 
    '     Sub: Main, mcf, spectrum
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program
    Sub Main(args As String())
        Call xmassReader()
        Call spectrum()
    End Sub

    Sub xmassReader()
        Dim proj = XMass.Project.FromResultFolder("E:\Bruker_MSImaging\0.1MS-80%-jichun-1\0_E5\1\1Ref")

        Pause()
    End Sub

    Sub spectrum()
        Dim reader As New PeakReader("F:\MSI\YP202130530-V.d\peaks.sqlite")
        ' Dim metadtaa = reader.GetProperties.ToArray

        For Each scan In reader.GetSpectra
            Call Console.WriteLine(scan.GetJson)
            Call Console.WriteLine()
        Next

        Pause()
    End Sub

    Sub mcf()
        Dim mcf_idx As New IndexParser("F:\MSI\YP202130530-V.d\b6ad08c2-7356-4a7c-88a5-47809c687c81_2.mcf_idx")
        Dim bytes As Byte()
        Dim blob As New mcfParser("F:\MSI\YP202130530-V.d\b6ad08c2-7356-4a7c-88a5-47809c687c81_2.mcf")

        For Each index In mcf_idx.GetContainerIndex
            Console.WriteLine(index.ToString)
            bytes = blob.GetBlob(index)
            bytes = New MemoryStream(bytes).UnGzipStream.ToArray

            Dim bin As New BinaryDataReader(bytes)
            Dim str = bin.ReadString(BinaryStringFormat.ZeroTerminated)

            '  Pause()
            Console.WriteLine()
        Next
    End Sub

End Module

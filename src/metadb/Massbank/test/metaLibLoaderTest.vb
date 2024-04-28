#Region "Microsoft.VisualBasic::383dd638d47a76a4050733141b89322d, E:/mzkit/src/metadb/Massbank/test//metaLibLoaderTest.vb"

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

    '   Total Lines: 44
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 1.39 KB


    ' Module metaLibLoaderTest
    ' 
    '     Sub: lipidmapsRepository, Main, readRepotest
    ' 
    ' Class DataSetOfMetaLib
    ' 
    '     Properties: MetaLib
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports BioNovoGene.BioDeep.Chemistry.LipidMaps

Module metaLibLoaderTest

    Sub Main()
        Call readRepotest()
        Call lipidmapsRepository()

        Dim libs = LoadUltraLargeXMLDataSet(Of MetaLib)(path:="D:\Database\CID-Synonym-filtered\CID-Synonym-filtered.metlib_kegg.Xml").ToArray

        Dim libs2 = "D:\Database\CID-Synonym-filtered\CID-Synonym-filtered.metlib_cas.Xml".LoadXml(Of DataSetOfMetaLib)


        Pause()
    End Sub

    Sub readRepotest()
        Using file = "D:\mzkit\Rscript\Library\mzkit_app\data\LIPIDMAPS.msgpack".Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim repo = file.ReadRepository

            Pause()
        End Using
    End Sub

    Sub lipidmapsRepository()
        Dim sdffile = SDF.SDF.IterateParser("C:\Users\Administrator\Downloads\structures.sdf", parseStruct:=False).CreateMeta.ToArray

        Using file = "D:\mzkit\Rscript\Library\mzkit_app\data\LIPIDMAPS.msgpack".Open(IO.FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call sdffile.WriteRepository(file)
        End Using

        Pause()
    End Sub
End Module

Public Class DataSetOfMetaLib

    <XmlElement>
    Public Property MetaLib As MetaLib()

End Class

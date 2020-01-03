#Region "Microsoft.VisualBasic::a81753c132bee569700b407d9df51442, src\metadb\Massbank\test\pubchemTest.vb"

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

    ' Module pubchemTest
    ' 
    '     Sub: fileTest, Main, queryMeta, sdfParserTest, xmlLoaderTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.DATA.File
Imports SMRUCC.MassSpectrum.DATA.NCBI
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module pubchemTest

    Sub queryMeta()
        Dim cas$ = "285979-85-3"
        Dim CID = PubChem.QueryCID(cas)
        Dim pug = PubChem.FetchPugViewByCID(CID.First)
        Dim meta = pug.GetMetaInfo

        Pause()
    End Sub

    Sub xmlLoaderTest()

        Dim xml = "D:\MassSpectrum-toolkits\DATA\DATA\pubchem_ftp_xml.Xml".ReadAllText
        Dim data = PCCompound.Compound.LoadFromXml(xml)

        Pause()
    End Sub

    Sub Main()

        Call xmlLoaderTest()
        Call queryMeta()

        Dim keys = SMRUCC.MassSpectrum.DATA.File.SDF.ScanKeys("D:\Database\pubchem").ToArray

        Call keys.GetJson.SaveTo("D:\Database\pubchem_sdf_keys.json")

        Pause()

        Call sdfParserTest()
        Call fileTest()


        Dim cas$ = "345909-34-4"
        Dim result = PubChem.Query.QueryPugViews(cas)

        Pause()
    End Sub

    Sub sdfParserTest()
        Dim mols = SDF.IterateParser("D:\Database\pubchem\Compound_000000001_000025000.sdf").ToArray


        Pause()
    End Sub

    Sub fileTest()
        Dim file = "D:\MassSpectrum-toolkits\DATA\DATA\pubchem\CID_5957.xml"
        Dim xml = file.LoadXml(Of PugViewRecord)

        Call xml.GetJson.SaveTo(file.ChangeSuffix("json"))

        Dim meta = xml.GetMetaInfo

        Pause()
    End Sub
End Module

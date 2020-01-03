#Region "Microsoft.VisualBasic::a67cae0d1b69afaafb1338f1c12bff92, src\metadb\Massbank\test\foodbQuery.vb"

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

    ' Module foodbQuery
    ' 
    '     Sub: Main, queryByContent, queryByID, subset
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.MassSpectrum.DATA.TMIC.FooDB
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Module foodbQuery

    Dim mysql As MySqli = New ConnectionUri With {.IPAddress = "localhost", .Database = "foodb", .Password = "root", .Port = 3306, .User = "root"}

    Sub Main()
        Call subset()
        Call queryByContent()

        ' Call queryByID()
    End Sub

    Sub queryByID()


    End Sub

    Sub subset()
        Dim hmdb = metabolite.Load("D:\smartnucl_integrative\DATA\2017-12-22.MetaReference\hmdb_metabolites.xml")
        Dim index As Index(Of String) = {
            "HMDB0000043",
            "HMDB0000097",
            "HMDB0000925",
            "HMDB0000562",
            "HMDB0000062",
            "HMDB0000742",
            "HMDB0000172",
            "HMDB0000883",
            "HMDB0000687",
            "HMDB0000159",
            "HMDB0000158",
            "HMDB0000929",
"HMDB0000906"}.Indexing

        For Each m In hmdb
            If m.accession Like index Then
                Call {m}.GetXml.SaveTo($"./hmdb/{m.accession}.xml")
            ElseIf Not m.secondary_accessions.accession Is Nothing Then
                For Each id In m.secondary_accessions.accession
                    If id Like index Then
                        Call {m}.GetXml.SaveTo($"./hmdb/{m.accession}.xml")
                    End If
                Next
            End If
        Next
    End Sub

    Sub queryByContent()
        For Each xml As String In "D:\MassSpectrum-toolkits\DATA\Massbank\test\bin\x64\Release\hmdb".EnumerateFiles("*.Xml")
            Dim tests As metabolite() = xml.LoadXml(Of metabolite())

            For Each metabolite In tests
                Dim foods = metabolite.GetAssociatedFoods(mysql).ToArray
                If foods.Length > 0 Then
                    Call foods.SaveTo($"./foods/{metabolite.accession}-{foods.First.name.NormalizePathString}.csv")
                End If
            Next
        Next


        Pause()
    End Sub
End Module

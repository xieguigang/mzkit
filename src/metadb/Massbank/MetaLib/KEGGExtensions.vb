#Region "Microsoft.VisualBasic::1f34aa1f2c5836a758615c4004c72cf9, Massbank\MetaLib\KEGGExtensions.vb"

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

    '     Module KEGGExtensions
    ' 
    '         Function: KEGGDrugGlyan2Compound
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.Medical

Namespace MetaLib

    Public Module KEGGExtensions

        ''' <summary>
        ''' 将KEGG数据库之中的药物编号以及Glyan物质的编号转换为Compound编号
        ''' </summary>
        ''' <param name="input">
        ''' 包含有两个字段的tuple数据：
        ''' 
        ''' + keggDrug KEGG的药物数据库的文件路径
        ''' + KEGGcpd KEGG的代谢物数据库的文件夹路径，在这个文件夹里面应该包含有Glyan物质的数据
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function KEGGDrugGlyan2Compound(input As (keggDrug$, KEGGcpd$)) As Dictionary(Of String, String)
            Dim drugs = DrugParser.ParseStream(input.keggDrug).ToArray
            Dim idMaps As New Dictionary(Of String, String)

            Call "Convert drug id to compound id...".__DEBUG_ECHO

            For Each d As Medical.Drug In drugs
                Dim CId As String() = d.CompoundID

                If Not CId.IsNullOrEmpty Then
                    idMaps(d.Entry) = CId.First
                End If
            Next

            Dim gl = (ls - l - r - "*.Xml" <= input.KEGGcpd) _
                .Where(Function(f) f.BaseName.IsPattern("G\d+")) _
                .Select(AddressOf LoadXml(Of Glycan))

            Call "Scan glycan and convert glucan id to compound id...".__DEBUG_ECHO

            For Each glycan As Glycan In gl
                Dim CId = glycan.CompoundId

                If Not CId.IsNullOrEmpty Then
                    idMaps(glycan.entry) = CId.First
                End If
            Next

            Return idMaps
        End Function
    End Module
End Namespace

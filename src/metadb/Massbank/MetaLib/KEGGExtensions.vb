﻿#Region "Microsoft.VisualBasic::68b3348ed668e3f2b76f5a722276fbb7, metadb\Massbank\MetaLib\KEGGExtensions.vb"

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

    '   Total Lines: 87
    '    Code Lines: 57 (65.52%)
    ' Comment Lines: 15 (17.24%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 15 (17.24%)
    '     File Size: 3.57 KB


    '     Module KEGGExtensions
    ' 
    '         Function: KEGGDrugGlyan2Compound, Xref
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.Medical
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace MetaLib

    Public Module KEGGExtensions

        ''' <summary>
        ''' extract the cross reference link data from kegg compound annotation data model
        ''' </summary>
        ''' <param name="kegg"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Xref(kegg As Compound) As xref
            Dim xl As New xref With {.extras = New Dictionary(Of String, String())}

            For Each link As IGrouping(Of String, DBLink) In kegg.DbLinks.SafeQuery.GroupBy(Function(xr) xr.DBName)
                Select Case link.Key.ToLower
                    Case "cas" : xl.CAS = link.Select(Function(l) l.entry).Distinct.ToArray
                    Case "pubchem" : xl.pubchem = link.OrderBy(Function(l) CLng(Val(l.entry))).FirstOrDefault?.entry.Match("\d+")
                    Case "chebi"
                        Dim id = link.FirstOrDefault?.entry

                        If Not id.StringEmpty(, True) Then
                            xl.chebi = "CHEBI:" & id.Match("\d+")
                        End If
                    Case "knapsack" : xl.KNApSAcK = link.FirstOrDefault?.entry
                    Case "lipidmaps" : xl.lipidmaps = link.FirstOrDefault?.entry
                    Case Else
                        xl.extras(link.Key) = link _
                            .Select(Function(l) l.entry) _
                            .ToArray
                End Select
            Next

            Return xl
        End Function

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

            Call "Convert drug id to compound id...".debug

            For Each d As Medical.Drug In drugs
                Dim CId As String() = d.CompoundID

                If Not CId.IsNullOrEmpty Then
                    idMaps(d.Entry) = CId.First
                End If
            Next

            Dim gl = (ls - l - r - "*.Xml" <= input.KEGGcpd) _
                .Where(Function(f) f.BaseName.IsPattern("G\d+")) _
                .Select(AddressOf LoadXml(Of Glycan))

            Call "Scan glycan and convert glucan id to compound id...".debug

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

#Region "Microsoft.VisualBasic::e74125b1cc07e83526890dadb565402a, src\metadb\Massbank\Public\NCBI\PubChem\Web\MetaInfoReader.vb"

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

    '     Module MetaInfoReader
    ' 
    '         Function: GetInform, GetInformList, GetMetaInfo, getSynonyms, navigateView
    '                   removesDbEntry
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports MetaInfo = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace NCBI.PubChem

    <HideModuleName>
    Public Module MetaInfoReader

        ''' <summary>
        ''' 如果<paramref name="path"/>的末端是使用索引语法,则索引的起始下标是从零开始的
        ''' </summary>
        ''' <param name="view"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetInform(view As PugViewRecord, path$) As Information
            Dim parts = path.Trim("/"c).Split("/"c)
            Dim section = view.navigateView(parts)

            Return section.GetInformation(parts.Last, multipleInfo:=False)
        End Function

        <Extension>
        Private Function navigateView(view As PugViewRecord, parts As String()) As Section
            If parts.Length = 1 OrElse view Is Nothing Then
                Return Nothing
            End If

            Dim sec As Section = view(parts(Scan0))

            For Each part As String In parts.Skip(1).Take(parts.Length - 2)
                If sec Is Nothing Then
                    Return Nothing
                Else
                    sec = sec(part)
                End If
            Next

            Return sec
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="view"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetInformList(view As PugViewRecord, path$) As Information()
            Dim parts = path.Trim("/"c).Split("/"c)
            Dim section = view.navigateView(parts)

            Return section.GetInformation(parts.Last, multipleInfo:=True)
        End Function

        ReadOnly nameDatabase As Index(Of String) = {
            "Human Metabolome Database (HMDB)",
            "ChEBI",
            "DrugBank",
            "European Chemicals Agency (ECHA)",
            "MassBank of North America (MoNA)"
        }

        <Extension>
        Private Iterator Function getSynonyms(names As Section) As IEnumerable(Of String)
            Dim depositor = names("Depositor-Supplied Synonyms")
            Dim mesh = names("MeSH Entry Terms")

            If Not depositor Is Nothing Then
                For Each info As Information In depositor.Information
                    If info.InfoType Is GetType(String) Then
                        Yield info.InfoValue
                    Else
                        For Each value As String In DirectCast(info.InfoValue, String())
                            Yield value
                        Next
                    End If
                Next
            End If

            If Not mesh Is Nothing Then
                For Each info As Information In mesh.Information
                    If info.InfoType Is GetType(String) Then
                        Yield info.InfoValue
                    Else
                        For Each value As String In DirectCast(info.InfoValue, String())
                            Yield value
                        Next
                    End If
                Next
            End If
        End Function

        ''' <summary>
        ''' 从pubchem数据库之中提取注释所需要的必须基本信息
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetMetaInfo(view As PugViewRecord) As MetaInfo
            If view Is Nothing Then
                Return Nothing
            End If

            Dim identifier As Section = view("Names and Identifiers")
            Dim formula = view.GetInform("/Names and Identifiers/Molecular Formula/#0")
            Dim descriptors = identifier("Computed Descriptors")
            Dim SMILES = view.GetInform("/Names and Identifiers/Computed Descriptors/Canonical SMILES/#0")
            Dim InChIKey = descriptors("InChI Key").GetInformationString("#0")
            Dim InChI = descriptors("InChI").GetInformationString("#0")
            Dim otherNames = identifier("Other Identifiers")
            Dim synonyms = identifier("Synonyms").getSynonyms.Distinct.OrderBy(Function(s) s).ToArray
            Dim computedProperties = view("Chemical and Physical Properties")("Computed Properties")
            ' Dim properties = Table.ToDictionary(computedProperties)
            Dim CASNumber$()

            If synonyms Is Nothing Then
                synonyms = {}
            End If

            If otherNames Is Nothing Then
                CASNumber = synonyms _
                    .Where(Function(id) id.IsPattern("\d+([-]\d+)+")) _
                    .ToArray
            Else
                CASNumber = otherNames("CAS")?.GetInformationStrings("CAS", True)
            End If

            Dim exact_mass# = computedProperties("Exact Mass").GetInformationNumber(Nothing)
            Dim xref As New xref With {
                .InChI = InChI,
                .CAS = CASNumber,
                .InChIkey = InChIKey,
                .pubchem = view.RecordNumber,
                .chebi = synonyms.FirstOrDefault(Function(id) id.IsPattern("CHEBI[:]\d+")),
                .KEGG = synonyms.FirstOrDefault(Function(id)
                                                    ' KEGG编号是C开头,后面跟随5个数字
                                                    Return id.IsPattern("C\d{5}", RegexOptions.Singleline)
                                                End Function),
                .HMDB = view.Reference.GetHMDBId,
                .SMILES = SMILES.InfoValue
            }
            Dim commonName$ = view.RecordTitle

            If commonName.StringEmpty Then
                commonName = view _
                    .Reference _
                    .FirstOrDefault(Function(r) r.SourceName Like nameDatabase) _
                   ?.Name
            End If

            ' 20190618 formula可能会存在多个的情况
            Dim formulaStr As String = ""

            If Not formula Is Nothing Then
                If formula.InfoType Is GetType(String) Then
                    formulaStr = formula.InfoValue
                Else
                    formulaStr = DirectCast(formula.InfoValue, String()).FirstOrDefault
                End If
            End If

            Return New MetaInfo With {
                .formula = formulaStr,
                .xref = xref,
                .name = commonName,
                .exact_mass = exact_mass,
                .ID = view.RecordNumber,
                .synonym = synonyms.removesDbEntry.ToArray
            }
        End Function

        <Extension>
        Private Iterator Function removesDbEntry(synonyms As String()) As IEnumerable(Of String)
            For Each name As String In synonyms
                If name.IsPattern("\d+") Then
                    Continue For
                End If
                If name.Match("\d+").Length > 2 Then
                    Continue For
                End If
                If xref.IsCASNumber(name) Then
                    Continue For
                End If

                Yield name
            Next
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::86ec878e69511c370cd2243b70f5549d, Massbank\Public\NCBI\PubChem\Web\PugView.vb"

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

    '     Class PugViewRecord
    ' 
    '         Properties: RecordNumber, RecordTitle, RecordType, Reference
    ' 
    '         Function: GetMetaInfo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports MetaInfo = SMRUCC.MassSpectrum.DATA.MetaLib.MetaLib

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord : Inherits InformationSection

        Public Property RecordType As String
        Public Property RecordNumber As String
        Public Property RecordTitle As String

        <XmlElement(NameOf(Reference))>
        Public Property Reference As Reference()

        Public Const HMDB$ = "Human Metabolome Database (HMDB)"

        Shared ReadOnly nameDatabase As Index(Of String) = {"Human Metabolome Database (HMDB)", "ChEBI", "DrugBank", "European Chemicals Agency (ECHA)", "MassBank of North America (MoNA)"}

        ''' <summary>
        ''' 从pubchem数据库之中提取注释所需要的必须基本信息
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMetaInfo() As MetaInfo
            Dim identifier = Me("Names and Identifiers")
            Dim formula = identifier("Molecular Formula").GetInformationString("Molecular Formula")
            Dim descriptors = identifier("Computed Descriptors")
            Dim SMILES = descriptors("Canonical SMILES").GetInformationString("Canonical SMILES")
            Dim InChIKey = descriptors("InChI Key").GetInformationString("InChI Key")
            Dim InChI = descriptors("InChI").GetInformationString("InChI")
            Dim otherNames = identifier("Other Identifiers")
            Dim synonyms = identifier _
                ("Synonyms") _
                ("Depositor-Supplied Synonyms").GetInformationStrings _
                ("Depositor-Supplied Synonyms")
            Dim computedProperties = Me("Chemical and Physical Properties")("Computed Properties")
            ' Dim properties = Table.ToDictionary(computedProperties)
            Dim CASNumber$

            If synonyms Is Nothing Then
                synonyms = {}
            End If

            If otherNames Is Nothing Then
                CASNumber = synonyms.FirstOrDefault(Function(id) id.IsPattern("\d+([-]\d+)+"))
            Else
                CASNumber = otherNames("CAS")?.GetInformationString("CAS")
            End If

            Dim exact_mass# = computedProperties("Exact Mass").GetInformationNumber("Exact Mass")
            Dim xref As New MetaLib.xref With {
                .InChI = InChI,
                .CAS = {CASNumber},
                .InChIkey = InChIKey,
                .pubchem = RecordNumber,
                .chebi = synonyms.FirstOrDefault(Function(id) id.IsPattern("CHEBI[:]\d+")),
                .KEGG = synonyms.FirstOrDefault(Function(id)
                                                    ' KEGG编号是C开头,后面跟随5个数字
                                                    Return id.IsPattern("C\d{5}", RegexOptions.Singleline)
                                                End Function),
                .HMDB = Reference.GetHMDBId
            }
            Dim commonName$ = RecordTitle

            If commonName.StringEmpty Then
                commonName = Reference _
                    .FirstOrDefault(Function(r) r.SourceName Like nameDatabase) _
                   ?.Name
            End If

            Return New MetaInfo With {
                .formula = formula,
                .xref = xref,
                .name = commonName,
                .mass = exact_mass,
                .ID = RecordNumber
            }
        End Function

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace

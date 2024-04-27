#Region "Microsoft.VisualBasic::169414a2043ce8479d0c5546dc421fb3, G:/mzkit/src/metadb/Massbank//MetaLib/Models/CrossReference/Xref.vb"

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

    '   Total Lines: 157
    '    Code Lines: 94
    ' Comment Lines: 50
    '   Blank Lines: 13
    '     File Size: 6.45 KB


    '     Class xref
    ' 
    '         Properties: CAS, chebi, ChEMBL, ChemIDplus, chemspider
    '                     DrugBank, extras, foodb, HMDB, InChI
    '                     InChIkey, KEGG, KEGGdrug, KNApSAcK, lipidmaps
    '                     MeSH, MetaCyc, metlin, pubchem, SMILES
    '                     Wikipedia
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: Join, PopulateXrefs, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace MetaLib.CrossReference

    ''' <summary>
    ''' The database cross reference set of a specific metabolite object.
    ''' (对某一个物质在数据库之间的相互引用编号)
    ''' </summary>
    Public Class xref

        ''' <summary>
        ''' chebi main id, Chemical Entities of Biological Interest (ChEBI)
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property chebi As String
        <MessagePackMember(1)> Public Property KEGG As String
        <MessagePackMember(2)> Public Property KEGGdrug As String
        ''' <summary>
        ''' The pubchem cid
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(3)> Public Property pubchem As String
        <MessagePackMember(4)> Public Property HMDB As String
        <MessagePackMember(5)> Public Property metlin As String
        <MessagePackMember(6)> Public Property DrugBank As String
        <MessagePackMember(7)> Public Property ChEMBL As String
        <MessagePackMember(8)> Public Property Wikipedia As String
        <MessagePackMember(9)> Public Property lipidmaps As String
        <MessagePackMember(10)> Public Property MeSH As String
        <MessagePackMember(11)> Public Property ChemIDplus As String
        <MessagePackMember(12)> Public Property MetaCyc As String
        <MessagePackMember(13)> Public Property KNApSAcK As String
        <MessagePackMember(14)> Public Property foodb As String
        <MessagePackMember(15)> Public Property chemspider As String
        ''' <summary>
        ''' Multiple CAS id may exists
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        <MessagePackMember(16)> Public Property CAS As String()
        <MessagePackMember(17)> Public Property InChIkey As String
        <MessagePackMember(18)> Public Property InChI As String
        <MessagePackMember(19)> Public Property SMILES As String

        ''' <summary>
        ''' other additional database id set
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(20)> Public Property extras As Dictionary(Of String, String())

        Sub New()
        End Sub

        ''' <summary>
        ''' extract the cross reference id set from a chebi metabolite data
        ''' </summary>
        ''' <param name="chebi"></param>
        Sub New(chebi As ChEBIEntity)
            Me.chebi = chebi.chebiId
            Me.KEGG = chebi.FindDatabaseLinkValue(AccessionTypes.KEGG_Compound)
            Me.Wikipedia = chebi.FindDatabaseLinkValue(AccessionTypes.Wikipedia)
            Me.SMILES = chebi.smiles
            Me.InChI = chebi.inchi
            Me.InChIkey = chebi.inchiKey
            Me.CAS = chebi.RegistryNumbers _
                .SafeQuery _
                .Where(Function(r) r.type = "CAS Registry Number") _
                .Select(Function(r) r.data) _
                .ToArray
        End Sub

        ''' <summary>
        ''' extract the cross reference id set from a hmdb metabolite data
        ''' </summary>
        ''' <param name="meta"></param>
        Sub New(meta As HMDB.MetaDb)
            Me.chebi = "CHEBI:" & meta.chebi_id
            Me.KEGG = Strings.Trim(meta.kegg_id).Trim(ASCII.TAB)
            Me.Wikipedia = meta.wikipedia_id
            Me.SMILES = meta.smiles
            Me.InChI = meta.inchi
            Me.InChIkey = meta.inchikey
            Me.CAS = {meta.CAS}
            Me.HMDB = meta.accession
        End Sub

        ''' <summary>
        ''' This function will fill current <see cref="xref"/> object with 
        ''' additional property data from <paramref name="another"/> data object.
        ''' </summary>
        ''' <param name="another"></param>
        ''' <returns>construct a new cross reference set data object</returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Join(another As xref) As xref
            Return CrossReference.Join(Me, another)
        End Function

        ''' <summary>
        ''' Convert a cross reference set as a database id collection
        ''' </summary>
        ''' <param name="parseList">
        ''' this parameter will treat the xref id as a set of the id 
        ''' collection, where the id set elements is seperated by the 
        ''' ``;`` symbol.
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateXrefs(Optional parseList As Boolean = False,
                                      Optional exclude_struct As Boolean = False) As IEnumerable(Of NamedValue(Of String))

            Static structs As Index(Of String) = New String() {
                NameOf(SMILES),
                NameOf(InChIkey),
                NameOf(InChI)
            }

            If Not exclude_struct Then
                Return Me.PullCollection(parseList)
            Else
                Return From xref As NamedValue(Of String)
                       In Me.PullCollection
                       Where Not xref.Name Like structs
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' fill of the corss reference id content in <paramref name="b"/> into <paramref name="a"/>
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Shared Operator &(a As xref, b As xref) As xref
            If a Is Nothing Then
                Return b
            ElseIf b Is Nothing Then
                Return a
            Else
                Return a.Join(b)
            End If
        End Operator
    End Class
End Namespace

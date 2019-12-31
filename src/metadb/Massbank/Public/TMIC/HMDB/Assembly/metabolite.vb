#Region "Microsoft.VisualBasic::22c399ac04cccfef5e46b5c2d8bf75c3, DATA\Massbank\Public\TMIC\HMDB\Assembly\metabolite.vb"

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

    '     Class MetaReference
    ' 
    '         Properties: accession, average_molecular_weight, bigg_id, biocyc_id, biological_properties
    '                     cas_registry_number, chebi_id, chemical_formula, chemspider_id, description
    '                     drugbank_id, drugbank_metabolite_id, foodb_id, het_id, inchi
    '                     inchikey, iupac_name, kegg_id, knapsack_id, Mass
    '                     meta_cyc_id, metagene, metlin_id, monisotopic_molecular_weight, name
    '                     nugowiki, pdb_id, phenol_explorer_compound_id, phenol_explorer_metabolite_id, pubchem_compound_id
    '                     secondary_accessions, smiles, synonyms, synthesis_reference, taxonomy
    '                     traditional_iupac, wikipedia_id, wikipidia
    ' 
    '     Class biological_properties
    ' 
    '         Properties: biospecimen_locations, cellular_locations, pathways, tissue_locations
    ' 
    '     Class pathway
    ' 
    '         Properties: kegg_map_id, name, smpdb_id
    ' 
    '     Class metabolite
    ' 
    '         Properties: abnormal_concentrations, creation_date, diseases, experimental_properties, normal_concentrations
    '                     ontology, predicted_properties, state, update_date, version
    ' 
    '         Function: Load, ToString
    ' 
    '     Structure biospecimen_locations
    ' 
    '         Properties: biospecimen
    ' 
    '         Function: ToString
    ' 
    '     Structure tissue_locations
    ' 
    '         Properties: tissue
    ' 
    '         Function: ToString
    ' 
    '     Structure secondary_accessions
    ' 
    '         Properties: accession
    ' 
    '         Function: ToString
    ' 
    '     Structure synonyms
    ' 
    '         Properties: synonym
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace TMIC.HMDB

    ''' <summary>
    ''' 主要是为了简化数据额存储
    ''' </summary>
    Public Class MetaReference : Implements INamedValue, IMolecule

        ''' <summary>
        ''' hmdb的主编号
        ''' </summary>
        ''' <returns></returns>
        Public Property accession As String Implements IKeyedEntity(Of String).Key, IMolecule.EntryId
        Public Property secondary_accessions As secondary_accessions
        Public Property name As String Implements IMolecule.Name
        Public Property description As String
        Public Property synonyms As synonyms
        Public Property chemical_formula As String Implements IMolecule.Formula

#Region "有些代谢物的分子质量的值空字符串，在进行XML反序列化的时候会出错，所以在这里改成字符串来避免出错"
        Public Property average_molecular_weight As String
        Public Property monisotopic_molecular_weight As String

        ''' <summary>
        ''' 因为XML反序列化的时候，有些分子可能会还不存在<see cref="average_molecular_weight"/>实验数据，
        ''' 这个属性为空字符串，则转换为Double的时候会报错
        ''' 在这里使用一个额外的属性来避免这种错误
        ''' </summary>
        ''' <returns></returns>
        Private Property Mass As Double Implements IMolecule.Mass
            Get
                Return Val(average_molecular_weight)
            End Get
            Set(value As Double)
                average_molecular_weight = value
            End Set
        End Property
#End Region

        Public Property iupac_name As String
        Public Property traditional_iupac As String
        <XmlElement>
        Public Property cas_registry_number As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchikey As String
        Public Property taxonomy As taxonomy

#Region "和其他的外部数据库的编号映射关系: xref"
        <Xref> Public Property drugbank_id As String
        <Xref> Public Property drugbank_metabolite_id As String
        <Xref> Public Property phenol_explorer_compound_id As String
        <Xref> Public Property phenol_explorer_metabolite_id As String
        <Xref> Public Property foodb_id As String
        <Xref> Public Property knapsack_id As String
        <Xref> Public Property chemspider_id As String
        <Xref> Public Property kegg_id As String
        <Xref> Public Property biocyc_id As String
        <Xref> Public Property bigg_id As String
        <Xref> Public Property wikipidia As String
        <Xref> Public Property nugowiki As String
        <Xref> Public Property metagene As String
        <Xref> Public Property metlin_id As String
        <Xref> Public Property pubchem_compound_id As String
        <Xref> Public Property het_id As String
        <Xref> Public Property chebi_id As String
        <Xref> Public Property wikipedia_id As String
        <Xref> Public Property meta_cyc_id As String
        <Xref> Public Property pdb_id As String
#End Region

        Public Property synthesis_reference As String
        Public Property biological_properties As biological_properties

    End Class

    Public Class biological_properties
        Public Property biospecimen_locations As biospecimen_locations
        Public Property tissue_locations As tissue_locations
        Public Property cellular_locations As cellular_locations
        Public Property pathways As pathway()
    End Class

    Public Class pathway
        Public Property name As String
        Public Property smpdb_id As String
        Public Property kegg_map_id As String
    End Class

    ''' <summary>
    ''' 当前这个对象类型的<see cref="INamedValue.Key"/>接口主键为<see cref="accession"/>属性
    ''' </summary>
    Public Class metabolite : Inherits MetaReference

        Public Property version As String
        Public Property creation_date As String
        Public Property update_date As String

        ''' <summary>
        ''' 固态还是液态？
        ''' </summary>
        ''' <returns></returns>
        Public Property state As String

        Public Property experimental_properties As Properties
        Public Property predicted_properties As Properties
        Public Property diseases As disease()

        Public Property normal_concentrations As concentration()
        Public Property abnormal_concentrations As concentration()
        Public Property ontology As ontology

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return name
        End Function

        ''' <summary>
        ''' Iterates of the HMDB dataset
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(path$) As IEnumerable(Of metabolite)
            Return path.LoadUltraLargeXMLDataSet(Of metabolite)(NameOf(metabolite), xmlns:="http://www.hmdb.ca")
        End Function
    End Class

    Public Structure biospecimen_locations

        <XmlElement>
        Public Property biospecimen As String()

        Public Overrides Function ToString() As String
            Return biospecimen.GetJson
        End Function
    End Structure

    Public Structure tissue_locations

        <XmlElement>
        Public Property tissue As String()

        Public Overrides Function ToString() As String
            Return tissue.GetJson
        End Function
    End Structure

    ''' <summary>
    ''' 次级编号
    ''' </summary>
    Public Structure secondary_accessions

        <XmlElement>
        Public Property accession As String()

        Public Overrides Function ToString() As String
            Return Me.accession.GetJson
        End Function

        Public Shared Narrowing Operator CType(acc As secondary_accessions) As String()
            Return acc.accession
        End Operator
    End Structure

    Public Structure synonyms

        <XmlElement>
        Public Property synonym As String()

        Public Overrides Function ToString() As String
            Return Me.synonym.GetJson
        End Function
    End Structure
End Namespace

#Region "Microsoft.VisualBasic::22d3014de5c11438660d72f3040653e8, G:/mzkit/src/metadb/Massbank//Public/TMIC/HMDB/Assembly/metabolite.vb"

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

    '   Total Lines: 246
    '    Code Lines: 145
    ' Comment Lines: 68
    '   Blank Lines: 33
    '     File Size: 10.94 KB


    '     Class MetaReference
    ' 
    '         Properties: accession, average_molecular_weight, bigg_id, biocyc_id, biological_properties
    '                     cas_registry_number, chebi_id, chemical_formula, chemspider_id, description
    '                     drugbank_id, drugbank_metabolite_id, ExactMass, fbonto_id, foodb_id
    '                     het_id, inchi, inchikey, iupac_name, kegg_id
    '                     knapsack_id, Mass, meta_cyc_id, metagene, metlin_id
    '                     monisotopic_molecular_weight, name, nugowiki, pdb_id, phenol_explorer_compound_id
    '                     phenol_explorer_metabolite_id, pubchem_compound_id, secondary_accessions, smiles, synonyms
    '                     synthesis_reference, taxonomy, traditional_iupac, vmh_id, wikipedia_id
    '                     wikipidia
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
    '         Properties: abnormal_concentrations, creation_date, diseases, experimental_properties, general_references
    '                     normal_concentrations, ontology, predicted_properties, protein_associations, state
    '                     update_date, version
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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace TMIC.HMDB

    ''' <summary>
    ''' A simple data model for the hmdb metabolite information 
    ''' 
    ''' the <see cref="synonyms"/> is just a simple string collection in the hmdb xml file.
    ''' </summary>
    ''' <remarks>
    ''' 主要是为了简化数据额存储
    ''' </remarks>
    Public Class MetaReference : Implements INamedValue, IMolecule, IReadOnlyId, GenericCompound

        ''' <summary>
        ''' HMDB ID, hmdb的主编号
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property accession As String Implements IKeyedEntity(Of String).Key, IMolecule.EntryId, IReadOnlyId.Identity
        ''' <summary>
        ''' Secondary Accession Numbers
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(1)> Public Property secondary_accessions As secondary_accessions
        ''' <summary>
        ''' Common Name
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(2)> Public Property name As String Implements IMolecule.Name, GenericCompound.CommonName
        <MessagePackMember(3)> Public Property description As String
        ''' <summary>
        ''' other synonym names from external database
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(4)> Public Property synonyms As synonyms
        <MessagePackMember(5)> Public Property chemical_formula As String Implements IMolecule.Formula, GenericCompound.Formula

#Region "有些代谢物的分子质量的值空字符串，在进行XML反序列化的时候会出错，所以在这里改成字符串来避免出错"
        <MessagePackMember(6)> Public Property average_molecular_weight As String
        <MessagePackMember(7)> Public Property monisotopic_molecular_weight As String

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

        Private ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return FormulaScanner.EvaluateExactMass(formula:=chemical_formula)
            End Get
        End Property
#End Region

        <MessagePackMember(8)> Public Property iupac_name As String
        <MessagePackMember(9)> Public Property traditional_iupac As String
        <XmlElement>
        <MessagePackMember(10)> Public Property cas_registry_number As String
        <MessagePackMember(11)> Public Property smiles As String
        <MessagePackMember(12)> Public Property inchi As String
        <MessagePackMember(13)> Public Property inchikey As String
        <MessagePackMember(14)> Public Property taxonomy As taxonomy

#Region "和其他的外部数据库的编号映射关系: xref"
        <MessagePackMember(15)> <Xref> Public Property drugbank_id As String
        <MessagePackMember(16)> <Xref> Public Property drugbank_metabolite_id As String
        <MessagePackMember(17)> <Xref> Public Property phenol_explorer_compound_id As String
        <MessagePackMember(18)> <Xref> Public Property phenol_explorer_metabolite_id As String
        <MessagePackMember(19)> <Xref> Public Property foodb_id As String
        <MessagePackMember(20)> <Xref> Public Property knapsack_id As String
        <MessagePackMember(21)> <Xref> Public Property chemspider_id As String
        <MessagePackMember(22)> <Xref> Public Property kegg_id As String
        <MessagePackMember(23)> <Xref> Public Property biocyc_id As String
        <MessagePackMember(24)> <Xref> Public Property bigg_id As String
        <MessagePackMember(25)> <Xref> Public Property wikipidia As String
        <MessagePackMember(26)> <Xref> Public Property nugowiki As String
        <MessagePackMember(27)> <Xref> Public Property metagene As String
        <MessagePackMember(28)> <Xref> Public Property metlin_id As String
        <MessagePackMember(29)> <Xref> Public Property pubchem_compound_id As String
        <MessagePackMember(30)> <Xref> Public Property het_id As String
        <MessagePackMember(31)> <Xref> Public Property chebi_id As String
        <MessagePackMember(32)> <Xref> Public Property wikipedia_id As String
        <MessagePackMember(33)> <Xref> Public Property meta_cyc_id As String
        <MessagePackMember(34)> <Xref> Public Property pdb_id As String
        <MessagePackMember(35)> <Xref> Public Property vmh_id As String
        <MessagePackMember(36)> <Xref> Public Property fbonto_id As String
#End Region

        <MessagePackMember(37)> Public Property synthesis_reference As String
        <MessagePackMember(38)> Public Property biological_properties As biological_properties
    End Class

    Public Class biological_properties
        <MessagePackMember(0)> Public Property biospecimen_locations As biospecimen_locations
        <MessagePackMember(1)> Public Property tissue_locations As tissue_locations
        <MessagePackMember(2)> Public Property cellular_locations As cellular_locations
        <MessagePackMember(3)> Public Property pathways As pathway()
    End Class

    Public Class pathway
        <MessagePackMember(0)> Public Property name As String
        <MessagePackMember(1)> Public Property smpdb_id As String
        <MessagePackMember(2)> Public Property kegg_map_id As String
    End Class

    ''' <summary>
    ''' the xml file data model of the hmdb metabolite, this object contains the full 
    ''' record data of a hmdb metabolite.
    ''' 
    ''' the <see cref="ontology"/> data contains the category ontology information about this
    ''' hmdb metabolite, example as:
    ''' 
    ''' 1. Physiological effect, example as Health effect
    ''' 2. Disposition, example as the <see cref="biospecimen_locations"/>, <see cref="tissue_locations"/>, etc
    ''' 3. Process, example as the biological process information
    ''' 4. Role, the biological roles of current metabolite
    ''' 
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' 当前这个对象类型的<see cref="INamedValue.Key"/>接口主键为<see cref="accession"/>属性
    ''' </remarks>
    Public Class metabolite : Inherits MetaReference

        <MessagePackMember(39)> Public Property version As String
        <MessagePackMember(40)> Public Property creation_date As String
        <MessagePackMember(41)> Public Property update_date As String

        ''' <summary>
        ''' solid/liquid, 固态还是液态？
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(42)> Public Property state As String

        ''' <summary>
        ''' Physical Properties: Experimental Molecular Properties	
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(43)> Public Property experimental_properties As Properties
        ''' <summary>
        ''' Physical Properties: Predicted Molecular Properties
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(44)> Public Property predicted_properties As Properties
        <MessagePackMember(45)> Public Property diseases As disease()

        <MessagePackMember(46)> Public Property normal_concentrations As concentration()
        <MessagePackMember(47)> Public Property abnormal_concentrations As concentration()
        <MessagePackMember(48)> Public Property ontology As ontology

        <MessagePackMember(49)> Public Property general_references As reference()
        <MessagePackMember(50)> Public Property protein_associations As protein()

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
        <MessagePackMember(0)>
        Public Property biospecimen As String()

        Public Overrides Function ToString() As String
            Return biospecimen.GetJson
        End Function
    End Structure

    Public Structure tissue_locations

        <XmlElement>
        <MessagePackMember(0)>
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
        <MessagePackMember(0)>
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
        <MessagePackMember(0)>
        Public Property synonym As String()

        Public Overrides Function ToString() As String
            Return Me.synonym.GetJson
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(snames As synonyms) As String()
            Return snames.synonym
        End Operator
    End Structure
End Namespace

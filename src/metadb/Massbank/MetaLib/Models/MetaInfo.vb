#Region "Microsoft.VisualBasic::38f89486af25a42a6e1c644578decc68, metadb\Massbank\MetaLib\Models\MetaInfo.vb"

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

    '   Total Lines: 105
    '    Code Lines: 74 (70.48%)
    ' Comment Lines: 18 (17.14%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 13 (12.38%)
    '     File Size: 4.54 KB


    '     Class MetaInfo
    ' 
    '         Properties: description, exact_mass, formula, ID, IUPACName
    '                     name, synonym, xref
    ' 
    '         Function: Equals, ToSimpleAnnotation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace MetaLib.Models

    ''' <summary>
    ''' the very basic metabolite annotation data model
    ''' </summary>
    ''' <remarks>
    ''' this data model includes the metabolite annotation information, includes:
    ''' 
    ''' 1. basic metabolite information, example as formula, name, etc
    ''' 2. database cross reference: <see cref="xref"/>
    ''' </remarks>
    Public Class MetaInfo : Implements INamedValue
        Implements IEquatable(Of MetaInfo)
        ' the abstract metabolite annotation data model
        Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider
        Implements GenericCompound
        Implements IMetabolite(Of xref)

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key, IReadOnlyId.Identity
        <MessagePackMember(1)> <XmlAttribute> Public Property formula As String Implements IFormulaProvider.Formula
        <MessagePackMember(2)> <XmlAttribute> Public Property exact_mass As Double Implements IExactMassProvider.ExactMass

        <MessagePackMember(3)> Public Property name As String Implements ICompoundNameProvider.CommonName

        ''' <summary>
        ''' IUPAC name refers to the systematic name given to a chemical compound according to the rules and 
        ''' guidelines established by the International Union of Pure and Applied Chemistry (IUPAC). IUPAC is 
        ''' an international organization that aims to advance the field of chemistry by developing standards
        ''' and nomenclature for chemicals.
        ''' 
        ''' The IUPAC name of a compound provides a unique and unambiguous identifier for that compound, which
        ''' is essential for clear communication among chemists and other scientists. These names are based 
        ''' on a set of systematic rules that take into account the structure and composition of the compound.
        ''' 
        ''' For example, the IUPAC name for the common sugar glucose is (2R,3S,4R,5R)-2,3,4,5,6-pentahydroxyhexanal.
        ''' This name describes the specific arrangement of atoms and functional groups in the glucose molecule.
        ''' 
        ''' IUPAC names are particularly important for complex organic compounds, where common names can be 
        ''' confusing or non-existent. They are widely used in scientific literature, patents, and other 
        ''' technical documents related to chemistry.
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(4)> Public Property IUPACName As String
        <MessagePackMember(5)> Public Property description As String

        ''' <summary>
        ''' the synonym of current metabolite, multiple names in different languages maybe contains inside this data names pool
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        <MessagePackMember(6)> Public Property synonym As String()

        ''' <summary>
        ''' The database cross reference of current metabolite and the molecule structure data.
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(7)> Public Property xref As xref Implements IMetabolite(Of xref).CrossReference

#Region "化合物分类"

        <MessagePackMember(8)> Public Property kingdom As String Implements ICompoundClass.kingdom
        <MessagePackMember(9)> Public Property super_class As String Implements ICompoundClass.super_class
        <MessagePackMember(10)> Public Property [class] As String Implements ICompoundClass.class
        <MessagePackMember(11)> Public Property sub_class As String Implements ICompoundClass.sub_class
        <MessagePackMember(12)> Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

#End Region

        ''' <summary>
        ''' get database cross reference id by database name
        ''' </summary>
        ''' <param name="field">
        ''' the database name
        ''' </param>
        ''' <returns>
        ''' db_xref
        ''' </returns>
        Default Public ReadOnly Property GetXrefId(field As String) As String
            Get
                Dim arg As xref = xref

                Select Case field.ToLower
                    Case "biocyc", "metacyc" : Return arg.MetaCyc
                    Case "cas" : Return arg.CAS.JoinBy("; ")
                    Case "chebi" : Return arg.chebi
                    Case "chembl" : Return arg.ChEMBL
                    Case "drugbank" : Return arg.DrugBank
                    Case "hmdb" : Return arg.HMDB
                    Case "kegg" : Return arg.KEGG
                    Case "knapsack" : Return arg.KNApSAcK
                    Case "lipidmaps" : Return arg.lipidmaps
                    Case "pubchem" : Return arg.pubchem
                    Case "wikipedia" : Return arg.Wikipedia
                    Case "metlin" : Return arg.metlin
                    Case "mesh" : Return arg.MeSH
                    Case Else
                        Return arg.extras.TryGetValue(field).JoinBy("; ")
                End Select
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' display the name for debug view
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Overloads Function Equals(other As MetaInfo) As Boolean Implements IEquatable(Of MetaInfo).Equals
            Static metaEquals As MetaEquals

            If metaEquals Is Nothing Then
                metaEquals = New MetaEquals
            End If

            If other Is Nothing Then
                Return False
            Else
                Return metaEquals.Equals(Me, other)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToSimpleAnnotation() As MetaboliteAnnotation
            Return New MetaboliteAnnotation With {
                .Id = ID,
                .CommonName = name,
                .ExactMass = exact_mass,
                .Formula = formula
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(m As MetaInfo) As MetaboliteAnnotation
            Return m.ToSimpleAnnotation
        End Operator
    End Class
End Namespace

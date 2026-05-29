#Region "Microsoft.VisualBasic::c2808442e9b4d14e5161ded5d6d24859, metadb\Massbank\Public\LOTUS\NaturalProduct.vb"

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

'   Total Lines: 156
'    Code Lines: 108 (69.23%)
' Comment Lines: 26 (16.67%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 22 (14.10%)
'     File Size: 6.86 KB


'     Class NaturalProduct
' 
'         Properties: allTaxa, chemicalTaxonomyNPclassifierClass, chemicalTaxonomyNPclassifierPathway, chemicalTaxonomyNPclassifierSuperclass, ExactMass
'                     inchi, inchikey, iupac_name, lotus_id, molecular_formula
'                     smiles, synonyms, taxonomyReferenceObjects, traditional_name, wikidata_id
'                     xrefs
' 
'         Function: CreateMetabolite, CreateReference, GetNCBITaxonomyReference, Parse
' 
'     Class Taxonomy
' 
'         Properties: classx, cleaned_organism_id, family, genus, kingdom
'                     organism_value, phylum, species, superkingdom
' 
'         Function: GetTaxonomyName, ToString
' 
'     Class TaxonomyReference
' 
'         Properties: NCBI
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Metabolite = BioNovoGene.BioDeep.Chemoinformatics.Metabolite.MetaLib

Namespace LOTUS

    ''' <summary>
    ''' Natural Products Online is an open source project for Natural Products (NPs) storage,
    ''' search and analysis. This page hosts LOTUS, one of the biggest and best annotated
    ''' resources for NPs occurrences available free of charge and without any restriction. 
    ''' LOTUS is a living database, which is hosted both here and on Wikidata.The Wikidata 
    ''' version allows for community curation and addition of novel data. The current version
    ''' allows a more user friendly experience (such as structural search, taxonomy oriented 
    ''' query, flat table and structures exports). If you use LOTUS in your research, please 
    ''' cite the following work: Adriano Rutz, Maria Sorokina, Jakub Galgonek, Daniel Mietchen,
    ''' Egon Willighagen, Arnaud Gaudry, James G Graham, Ralf Stephan, Roderic Page, Jiří 
    ''' Vondrášek, Christoph Steinbeck, Guido F Pauli, Jean-Luc Wolfender, Jonathan Bisson, 
    ''' Pierre-Marie Allard (2022) The LOTUS initiative for open knowledge management in 
    ''' natural products research. eLife 11:e70780. https://doi.org/10.7554/eLife.70780
    ''' </summary>
    ''' <remarks>
    ''' https://lotus.naturalproducts.net/
    ''' </remarks>
    Public Class NaturalProduct : Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider

        Public Property lotus_id As String Implements IReadOnlyId.Identity
        Public Property wikidata_id As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchikey As String
        Public Property traditional_name As String Implements ICompoundNameProvider.CommonName
        Public Property synonyms As String()
        Public Property iupac_name As String
        Public Property molecular_formula As String Implements IFormulaProvider.Formula
        Public Property xrefs As String()
        Public Property chemicalTaxonomyNPclassifierPathway As String
        Public Property chemicalTaxonomyNPclassifierSuperclass As String
        Public Property chemicalTaxonomyNPclassifierClass As String

        Public Property allTaxa As String()
        Public Property taxonomyReferenceObjects As Dictionary(Of String, TaxonomyReference)

        Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return FormulaScanner.EvaluateExactMass(molecular_formula)
            End Get
        End Property

        Public Iterator Function GetNCBITaxonomyReference() As IEnumerable(Of NamedValue(Of Taxonomy))
            For Each ref In taxonomyReferenceObjects.SafeQuery
                If ref.Value.NCBI.IsNullOrEmpty Then
                    Continue For
                End If

                Dim doi As String = ref.Key.Replace("$x$x$", ".")

                For Each tax As Taxonomy In ref.Value.NCBI
                    Yield New NamedValue(Of Taxonomy)(doi, tax) With {
                        .Description = ref.Key
                    }
                Next
            Next
        End Function

        Public Function CreateReference() As xref
            Dim extras As New Dictionary(Of String, String()) From {
                {"LOTUS", {lotus_id}}
            }

            If Not wikidata_id.StringEmpty(, True) Then
                extras.Add("wikidata", {wikidata_id.Split("/"c).Last})
            End If

            Return New xref With {
                .SMILES = smiles,
                .InChI = inchi,
                .InChIkey = inchikey,
                .extras = extras
            }
        End Function

        ''' <summary>
        ''' Convert the lotus natural product model as mzkit internal metabolite object.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateMetabolite() As Metabolite
            Return New Metabolite With {
                .ID = lotus_id,
                .Formula = molecular_formula,
                .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
                .IUPACName = iupac_name,
                .name = traditional_name,
                .organism = allTaxa,
                .synonym = synonyms,
                .pathways = {chemicalTaxonomyNPclassifierPathway},
                .super_class = chemicalTaxonomyNPclassifierSuperclass,
                .class = chemicalTaxonomyNPclassifierClass,
                .xref = CreateReference()
            }
        End Function

        ''' <summary>
        ''' Parse the lotus NPOC2021 bson dump file as metabolite data model
        ''' </summary>
        ''' <param name="NPOC2021"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Parse(NPOC2021 As Stream) As IEnumerable(Of NaturalProduct)
            For Each np As JsonObject In BSONFormat.LoadList(NPOC2021, tqdm:=True)
                Yield np.CreateObject(Of NaturalProduct)(decodeMetachar:=False)
            Next
        End Function
    End Class

    Public Class Taxonomy

        Public Property cleaned_organism_id As Integer
        Public Property organism_value As String
        Public Property superkingdom As String
        Public Property kingdom As String
        Public Property phylum As String
        Public Property classx As String
        Public Property family As String
        Public Property genus As String
        Public Property species As String

        Public Function GetTaxonomyName() As String
            For Each rank_name As String In {organism_value, species, genus, family, classx, phylum, kingdom, superkingdom}
                If Not rank_name.StringEmpty(, True) Then
                    Return rank_name
                End If
            Next

            Return "unknown"
        End Function

        Public Overrides Function ToString() As String
            Return $"({cleaned_organism_id}) {GetTaxonomyName()}"
        End Function

    End Class

    Public Class TaxonomyReference

        Public Property NCBI As Taxonomy()

    End Class
End Namespace

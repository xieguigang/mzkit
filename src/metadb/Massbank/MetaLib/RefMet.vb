#Region "Microsoft.VisualBasic::8ce53fe7ff93f69f65d8ed6792671287, metadb\Massbank\MetaLib\RefMet.vb"

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

'   Total Lines: 103
'    Code Lines: 53 (51.46%)
' Comment Lines: 41 (39.81%)
'    - Xml Docs: 90.24%
' 
'   Blank Lines: 9 (8.74%)
'     File Size: 5.19 KB


'     Class RefMet
' 
'         Properties: chebi_id, exactmass, formula, hmdb_id, inchi_key
'                     kegg_id, lipidmaps_id, main_class, pubchem_cid, refmet_id
'                     refmet_name, smiles, sub_class, super_class
' 
'         Function: CastModel, CreateReference, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace MetaLib

    ''' <summary>
    ''' RefMet: A Reference set of Metabolite names
    ''' </summary>
    ''' <remarks>
    ''' RefMet Naming Conventions
    ''' 
    ''' The names used in RefMet are generally based on common, officially accepted terms and 
    ''' incorporate notations which are appropriate for the type of analytical technique used.
    ''' In general, high-throughput untargeted MS experiments are not capable of deducing 
    ''' stereochemistry, double bond position/geometry and sn position (for glycerolipids/
    ''' lycerophospholipids). Secondly, the type of MS technique employed, as well as the mass
    ''' accuacy of the instrument will produce identifications at different levels of detail. 
    ''' For example, MS/MS methods are capable of identifying acyl chain substituents in lipids 
    ''' (e.g. PC 16:0/20:4) whereas MS methods only using precursor ion information might report 
    ''' these ions as "sum-composition" species (e.g. PC 36:4). RefMet covers both types of 
    ''' notations in an effort to enable data-sharing and comparative analysis of metabolomics 
    ''' data, using an analytical chemistry-centric approach.
    ''' 
    ''' The "sum-composition" lipid species indicate the number Of carbons And number Of "double
    ''' bond equivalents" (DBE), but Not chain positions Or Double bond regiochemistry And geometry.
    ''' The concept Of a Double bond equivalent unites a range Of chemical functionality which 
    ''' gives rise To isobaric features by mass spectometry. For example a chain containing a ring 
    ''' results In loss Of 2 hydrogen atoms (compared To a linear Structure) And thus has 1 DBE 
    ''' since the mass And molecular formula Is identical To a linear Structure With one Double bond.
    ''' Similarly, conversion Of a hydroxyl group To ketone results In loss Of 2 hydrogen atoms,
    ''' therefore the ketone Is assigned 1 DBE. Where applicable, the number Of oxygen atoms Is added 
    ''' To the abbreviation, separated by a semi-colon. Oxygen atoms In the Class-specific functional
    ''' group (e.g. the carboxylic acid group For fatty acids Or the phospholcholine group For PC) 
    ''' are excluded. In the Case Of sphingolipids, all oxygen atoms apart from the amide oxygen are
    ''' included, In order To discrminate, For example, between 1-deoxyceramides (;O), ceramides (;O2) 
    ''' And phytoceramides (;O3).
    ''' 
    ''' Some notes pertaining To different metabolite classes are outlined below.
    ''' </remarks>
    Public Class RefMet : Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider, INamedValue

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' there is a bug about the column name in the download csv file
        ''' </remarks>
        <Column(" refmet_id")>
        Public Property refmet_id As String

        Public Property refmet_name As String Implements IReadOnlyId.Identity, ICompoundNameProvider.CommonName, INamedValue.Key
        Public Property super_class As String
        Public Property main_class As String
        Public Property sub_class As String
        Public Property formula As String Implements IFormulaProvider.Formula
        Public Property exactmass As Double Implements IExactMassProvider.ExactMass
        Public Property pubchem_cid As String
        Public Property chebi_id As String
        Public Property hmdb_id As String
        Public Property lipidmaps_id As String
        Public Property kegg_id As String

        Public Property inchi_key As String
        Public Property smiles As String

        Public Overrides Function ToString() As String
            Return refmet_name
        End Function

        Public Function CastModel() As MetaInfo
            Return New MetaInfo With {
                .ID = refmet_id,
                .name = Strings.Trim(refmet_name).Trim(""""c).Trim("'"c).Trim,
                .super_class = Strings.Trim(super_class).Trim(""""c).Trim("'"c).Trim,
                .[class] = Strings.Trim(main_class).Trim(""""c).Trim("'"c).Trim,
                .sub_class = Strings.Trim(sub_class).Trim(""""c).Trim("'"c).Trim,
                .formula = Strings.Trim(formula).Trim(""""c).Trim("'"c).Trim,
                .exact_mass = exactmass,
                .IUPACName = .name,
                .xref = CreateReference()
            }
        End Function

        Public Function CreateReference() As xref
            Return New xref With {
                .pubchem = Strings.Trim(pubchem_cid).Trim(""""c).Trim("'"c).Trim,
                .SMILES = Strings.Trim(smiles).Trim(""""c).Trim("'"c).Trim,
                .chebi = "ChEBI:" & Strings.Trim(chebi_id).Trim(""""c).Trim("'"c).Trim,
                .HMDB = Strings.Trim(hmdb_id).Trim(""""c).Trim("'"c).Trim,
                .lipidmaps = Strings.Trim(lipidmaps_id).Trim(""""c).Trim("'"c).Trim,
                .KEGG = Strings.Trim(kegg_id).Trim(""""c).Trim("'"c).Trim,
                .extras = New Dictionary(Of String, String()) From {
                    {NameOf(RefMet), {refmet_id}}
                }
            }
        End Function

    End Class
End Namespace

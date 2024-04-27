#Region "Microsoft.VisualBasic::0f521114befbb371c8683f79a637282d, G:/mzkit/src/metadb/Massbank//Public/TMIC/HMDB/HmdbConvertor.vb"

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

    '   Total Lines: 56
    '    Code Lines: 51
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 2.41 KB


    '     Module HmdbConvertor
    ' 
    '         Function: ConvertInternal, CreateReferenceData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Text
Imports Metadata2 = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace TMIC.HMDB

    Public Module HmdbConvertor

        <Extension>
        Public Iterator Function ConvertInternal(metabolites As IEnumerable(Of metabolite)) As IEnumerable(Of Metadata2)
            For Each metabolite As metabolite In metabolites
                Yield metabolite.CreateReferenceData
            Next
        End Function

        <Extension>
        Private Function CreateReferenceData(metabolite As metabolite) As Metadata2
            Dim id = metabolite.accession
            Dim xref As New xref With {
                .CAS = {metabolite.cas_registry_number},
                .chebi = metabolite.chebi_id,
                .HMDB = metabolite.accession,
                .InChI = metabolite.inchi,
                .InChIkey = metabolite.inchikey,
                .KEGG = Strings.Trim(metabolite.kegg_id).Trim(ASCII.TAB),
                .SMILES = metabolite.smiles,
                .Wikipedia = metabolite.wikipedia_id,
                .pubchem = metabolite.pubchem_compound_id,
                .chemspider = metabolite.chemspider_id,
                .DrugBank = metabolite.drugbank_id,
                .foodb = metabolite.foodb_id,
                .MetaCyc = metabolite.meta_cyc_id
            }
            Dim metabo_tax = metabolite.taxonomy

            Return New Metadata2 With {
                .ID = id,
                .[class] = metabo_tax.class,
                .exact_mass = FormulaScanner.EvaluateExactMass(metabolite.chemical_formula),
                .formula = metabolite.chemical_formula,
                .kingdom = metabo_tax.kingdom,
                .molecular_framework = metabo_tax.molecular_framework,
                .sub_class = metabo_tax.sub_class,
                .super_class = metabo_tax.super_class,
                .description = metabolite.description,
                .xref = xref,
                .name = metabolite.name,
                .organism = {"Homo sapiens"},
                .IUPACName = metabolite.iupac_name,
                .synonym = metabolite.synonyms
            }
        End Function
    End Module
End Namespace

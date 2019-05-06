#Region "Microsoft.VisualBasic::70af44b5992c1a69715aab5ceb74de3b, Massbank\Public\TMIC\HMDB\BriefTable.vb"

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

'     Class MetaInfo
' 
'         Properties: CAS, chebi, HMDB, KEGG
' 
'     Class BriefTable
' 
'         Properties: AdultConcentrationAbnormal, AdultConcentrationNormal, ChildrenConcentrationAbnormal, ChildrenConcentrationNormal, disease
'                     NewbornConcentrationAbnormal, NewbornConcentrationNormal, Sample, water_solubility
' 
'         Function: Clone
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace TMIC.HMDB

    Public Class MetaInfo : Inherits MetaLib.MetaInfo

        Public Property HMDB As String
        Public Property KEGG As String
        Public Property chebi As String
        Public Property CAS As String

    End Class

    Public Class BriefTable : Inherits MetaInfo
        Implements ICloneable

        Public Property Sample As String
        Public Property disease As String()
        Public Property water_solubility As String

        <Column("concentration (children-normal)")>
        Public Property ChildrenConcentrationNormal As String()
        <Column("concentration (children-abnormal)")>
        Public Property ChildrenConcentrationAbnormal As String()
        <Column("concentration (adult-normal)")>
        Public Property AdultConcentrationNormal As String()
        <Column("concentration (adult-abnormal)")>
        Public Property AdultConcentrationAbnormal As String()
        <Column("concentration (newborn-normal)")>
        Public Property NewbornConcentrationNormal As String()
        <Column("concentration (newborn-abnormal)")>
        Public Property NewbornConcentrationAbnormal As String()

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

    Public Class MetaDb

        Public Property accession As String
        Public Property secondary_accessions As String()
        Public Property name As String
        Public Property chemical_formula As String
        Public Property exact_mass As Double
        Public Property iupac_name As String
        Public Property traditional_iupac As String
        Public Property CAS As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchikey As String
        Public Property kingdom As String
        Public Property super_class As String
        Public Property [class] As String
        Public Property sub_class As String
        Public Property molecular_framework As String
        Public Property direct_parent As String
        Public Property state As String
        Public Property cellular_locations As String()
        Public Property biospecimen As String()
        Public Property tissue As String()
        Public Property chebi_id As Long
        Public Property pubchem_cid As Long
        Public Property kegg_id As String
        Public Property wikipedia_id As String

        Public Shared Function FromMetabolite(metabolite As metabolite) As MetaDb
            Dim metabolite_taxonomy = metabolite.taxonomy
            Dim biosample = metabolite.biological_properties

            Return New MetaDb With {
                .accession = metabolite.accession,
                .secondary_accessions = metabolite.secondary_accessions.accession,
                .chebi_id = metabolite.chebi_id,
                .pubchem_cid = metabolite.pubchem_compound_id,
                .chemical_formula = metabolite.chemical_formula,
                .kegg_id = metabolite.kegg_id,
                .wikipedia_id = metabolite.wikipedia_id,
                .inchi = metabolite.inchi,
                .inchikey = metabolite.inchikey,
                .name = metabolite.name,
                .state = metabolite.state,
                .traditional_iupac = metabolite.traditional_iupac,
                .smiles = metabolite.smiles,
                .iupac_name = metabolite.iupac_name,
                .CAS = metabolite.cas_registry_number,
                .exact_mass = Val(metabolite.monisotopic_molecular_weight),
                .direct_parent = metabolite_taxonomy?.direct_parent,
                .kingdom = metabolite_taxonomy?.kingdom,
                .super_class = metabolite_taxonomy?.super_class,
                .sub_class = metabolite_taxonomy?.sub_class,
                .molecular_framework = metabolite_taxonomy?.molecular_framework,
                .[class] = metabolite_taxonomy?.class,
                .biospecimen = biosample?.biospecimen_locations.biospecimen,
                .cellular_locations = biosample?.cellular_locations.cellular,
                .tissue = biosample?.tissue_locations.tissue
            }
        End Function

        Public Shared Sub WriteTable(metabolites As IEnumerable(Of metabolite), out As Stream)
            Using table As New WriteStream(Of MetaDb)(New StreamWriter(out))
                For Each metabolite As metabolite In metabolites
                    Call table.Flush(FromMetabolite(metabolite))
                Next
            End Using
        End Sub
    End Class
End Namespace

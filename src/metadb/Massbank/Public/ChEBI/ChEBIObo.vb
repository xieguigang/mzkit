#Region "Microsoft.VisualBasic::27ae7507f30b350dd6ba38153453aa92, metadb\Massbank\Public\ChEBI\ChEBIObo.vb"

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

'   Total Lines: 92
'    Code Lines: 68 (73.91%)
' Comment Lines: 16 (17.39%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 8 (8.70%)
'     File Size: 4.27 KB


'     Module ChEBIObo
' 
'         Function: ExtractTerm, ImportsMetabolites, SafeGetString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo

Namespace ChEBI

    Public Module ChEBIObo

        ''' <summary>
        ''' Extract the metabolite annotation information from the chebi ontology database file
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        Public Iterator Function ImportsMetabolites(chebi As OBOFile) As IEnumerable(Of metadata)
            For Each term As RawTerm In chebi.GetRawTerms
                Yield term.ExtractTerm
            Next
        End Function

        ''' <summary>
        ''' convert the chebi ontology term as the mzkit metabolite annotation data model
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExtractTerm(term As RawTerm) As metadata
            Dim obo_data = term.GetValueSet
            Dim id As String = obo_data(RawTerm.Key_id).First
            Dim name As String = obo_data(RawTerm.Key_name).First
            Dim def As String = obo_data(RawTerm.Key_def).JoinBy("; ")
            Dim synonym As String() = obo_data(RawTerm.Key_synonym) _
                .SafeQuery _
                .Select(Function(si) DelimiterParser.GetTokens(si).First) _
                .ToArray
            Dim properties = obo_data(RawTerm.Key_property_value).ParsePropertyValues
            Dim xref = obo_data(RawTerm.Key_xref).ParseXref
            Dim kegg_id = xref.TryGetValue("KEGG", [default]:={})
            Dim formula_str As String = properties.SafeGetString("http://purl.obolibrary.org/obo/chebi/formula")

            Return New metadata With {
                .description = Strings.Trim(def).Trim(""""c, " "c),
                .ID = id,
                .name = name,
                .synonym = synonym,
                .IUPACName = name,
                .formula = formula_str,
                .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
                .xref = New xref With {
                    .CAS = xref.TryGetValue("CAS"),
                    .chebi = id,
                    .KEGG = kegg_id.Where(Function(cid) cid.StartsWith("C"c)).FirstOrDefault,
                    .InChI = properties.SafeGetString("http://purl.obolibrary.org/obo/chebi/inchi"),
                    .InChIkey = properties.SafeGetString("http://purl.obolibrary.org/obo/chebi/inchikey"),
                    .SMILES = properties.SafeGetString("http://purl.obolibrary.org/obo/chebi/smiles"),
                    .MetaCyc = xref.TryGetValue("MetaCyc").DefaultFirst,
                    .DrugBank = xref.TryGetValue("DrugBank").DefaultFirst,
                    .Wikipedia = xref.TryGetValue("Wikipedia").DefaultFirst,
                    .HMDB = xref.TryGetValue("HMDB").DefaultFirst,
                    .KNApSAcK = xref.TryGetValue("KNApSAcK").DefaultFirst,
                    .lipidmaps = xref.TryGetValue("LIPID_MAPS_instance").DefaultFirst,
                    .KEGGdrug = kegg_id.Where(Function(cid) cid.StartsWith("D"c)).DefaultFirst
                }
            }
        End Function

        ''' <summary>
        ''' deal with the missing value data safely
        ''' </summary>
        ''' <param name="properties"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        <Extension>
        Private Function SafeGetString(properties As Dictionary(Of String, NamedValue()), key As String) As String
            If properties.ContainsKey(key) Then
                Return properties(key).First.text
            Else
                key = key.Split("/"c).Last

                If properties.ContainsKey(key) Then
                    Return properties(key).First.text
                End If

                Return Nothing
            End If
        End Function
    End Module
End Namespace

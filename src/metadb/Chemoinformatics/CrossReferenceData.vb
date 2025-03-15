#Region "Microsoft.VisualBasic::3cfeae4093ab70404e2747d6d378abf5, metadb\Chemoinformatics\CrossReferenceData.vb"

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

    '   Total Lines: 245
    '    Code Lines: 182 (74.29%)
    ' Comment Lines: 32 (13.06%)
    '    - Xml Docs: 81.25%
    ' 
    '   Blank Lines: 31 (12.65%)
    '     File Size: 11.24 KB


    ' Module CrossReferenceData
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Function: Compares, GetTopClass, MakeGroups, (+2 Overloads) PickId, UnionData
    '                   UniqueGroups
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Similarity

Public Module CrossReferenceData

    Public Delegate Function SetMeta(Of T As GenericCompound)(ByRef m As T, id As String, name As String, formula As String, exact_mass As Double) As T

    ''' <summary>
    ''' Union a collection of the metabolite data into a single metabolite data model
    ''' </summary>
    ''' <typeparam name="C"></typeparam>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="group">a collection of the metabolite data that should be populates 
    ''' from the <see cref="UniqueGroups(Of C, T)(IEnumerable(Of T))"/> function.
    ''' </param>
    ''' <returns></returns>
    Public Function UnionData(Of C As {New, ICrossReference}, T As {New, IMetabolite(Of C)})(group As IEnumerable(Of T), setMeta As SetMeta(Of T)) As T
        Dim groupData As T() = group.ToArray

        If groupData.Length = 0 Then
            Return Nothing
        ElseIf groupData.Length = 1 Then
            Return groupData(Scan0)
        End If

        Dim xrefs As C() = groupData.Select(Function(a) a.CrossReference).ToArray
        Dim xref As C = xrefs.PickId
        Dim name As String = groupData.Select(Function(a) a.CommonName).TopMostFrequent(New DirectTextComparer(False, False))
        Dim formula As String = groupData.Select(Function(a) a.Formula).TopMostFrequent
        Dim exact_mass As Double = FormulaScanner.EvaluateExactMass(formula)
        Dim classData As ICompoundClass = groupData.GetTopClass
        Dim id As String = xref.PickId(name)
        Dim m As T = setMeta(New T With {
            .[class] = classData.[class],
            .CrossReference = xref,
            .kingdom = classData.kingdom,
            .molecular_framework = classData.molecular_framework,
            .sub_class = classData.sub_class,
            .super_class = classData.super_class
        }, id, name, formula, exact_mass)

        Return m
    End Function

    <Extension>
    Private Function PickId(Of X As {New, ICrossReference})(c As X()) As X
        Dim chebi = c.Select(Function(a) a.chebi).TopMostFrequent
        Dim kegg = c.Select(Function(a) a.KEGG).TopMostFrequent
        Dim kegg_drug = c.Select(Function(a) a.KEGGdrug).TopMostFrequent
        Dim pubchem = c.Select(Function(a) a.pubchem).TopMostFrequent
        Dim hmdb = c.Select(Function(a) a.HMDB).TopMostFrequent
        Dim metlin = c.Select(Function(a) a.metlin).TopMostFrequent
        Dim drugbank = c.Select(Function(a) a.DrugBank).TopMostFrequent
        Dim chembl = c.Select(Function(a) a.ChEMBL).TopMostFrequent
        Dim wikipedia = c.Select(Function(a) a.Wikipedia).TopMostFrequent
        Dim lipidmaps = c.Select(Function(a) a.lipidmaps).TopMostFrequent
        Dim mesh = c.Select(Function(a) a.MeSH).TopMostFrequent
        Dim chemidplus = c.Select(Function(a) a.ChemIDplus).TopMostFrequent
        Dim metacyc = c.Select(Function(a) a.MetaCyc).TopMostFrequent
        Dim knapsack = c.Select(Function(a) a.KNApSAcK).TopMostFrequent
        Dim foodb = c.Select(Function(a) a.foodb).TopMostFrequent
        Dim chemspider = c.Select(Function(a) a.chemspider).TopMostFrequent
        Dim cas = c.Select(Function(a) a.CAS).IteratesALL.TopMostFrequent
        Dim inchikey = c.Select(Function(a) a.InChIkey).TopMostFrequent
        Dim struct_data = c.Where(Function(a) a.InChIkey.TextEquals(inchikey)).FirstOrDefault
        Dim inchi = struct_data?.InChI
        Dim smiles = struct_data?.SMILES

        Return New X With {
            .SMILES = smiles,
            .InChI = inchi,
            .CAS = If(cas.StringEmpty(True, True), {}, {cas}),
            .chebi = chebi,
            .ChEMBL = chembl,
            .ChemIDplus = chemidplus,
            .chemspider = chemspider,
            .DrugBank = drugbank,
            .foodb = foodb,
            .HMDB = hmdb,
            .InChIkey = inchikey,
            .KEGG = kegg,
            .KEGGdrug = kegg_drug,
            .KNApSAcK = knapsack,
            .lipidmaps = lipidmaps,
            .MeSH = mesh,
            .MetaCyc = metacyc,
            .metlin = metlin,
            .pubchem = pubchem,
            .Wikipedia = wikipedia
        }
    End Function

    <Extension>
    Private Function PickId(Of X As ICrossReference)(c As X, name As String) As String
        If Not c.KEGG.StringEmpty(True, True) Then Return c.KEGG
        If Not c.HMDB.StringEmpty(True, True) Then Return c.HMDB
        If Not c.Wikipedia.StringEmpty(True, True) Then Return c.Wikipedia
        If Not c.MeSH.StringEmpty(True, True) Then Return c.MeSH
        If Not c.MetaCyc.StringEmpty(True, True) Then Return c.MetaCyc
        If Not c.lipidmaps.StringEmpty(True, True) Then Return c.lipidmaps
        If Not c.pubchem.StringEmpty(True, True) Then Return c.pubchem
        If Not c.chebi.StringEmpty(True, True) Then Return c.chebi

        Dim cas_id As String = c.CAS.SafeQuery.Where(Function(a) Not a.StringEmpty(True, True)).FirstOrDefault

        If Not cas_id Is Nothing Then
            Return "CAS:" & cas_id
        End If

        Return name.Replace(" "c, "_").StringReplace("[-_,]+", "_")
    End Function

    <Extension>
    Private Function GetTopClass(Of C As ICompoundClass)(classList As C()) As ICompoundClass
        Dim top = classList _
            .OrderByDescending(Function(a)
                                   Dim count As Integer = 0

                                   If Not a.kingdom.StringEmpty(True, True) Then count += 1
                                   If Not a.class.StringEmpty(True, True) Then count += 1
                                   If Not a.sub_class.StringEmpty(True, True) Then count += 1
                                   If Not a.super_class.StringEmpty(True, True) Then count += 1
                                   If Not a.molecular_framework.StringEmpty(True, True) Then count += 1

                                   Return count
                               End Function) _
            .First

        Return top
    End Function

    Public Iterator Function UniqueGroups(Of C As ICrossReference, T As IMetabolite(Of C))(list As IEnumerable(Of T)) As IEnumerable(Of NamedCollection(Of T))
        Dim masses = list.GroupBy(Function(a) a.ExactMass, offsets:=0.1).ToArray

        For Each mass As NamedCollection(Of T) In masses
            For Each unique In MakeGroups(Of C, T)(mass)
                Yield unique
            Next
        Next
    End Function

    ''' <summary>
    ''' Check of the name/id equality via the string comparision directly
    ''' </summary>
    ''' <typeparam name="C"></typeparam>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="massGroup">
    ''' a group of the metabolite data with the same exact mass value
    ''' </param>
    ''' <returns></returns>
    Private Iterator Function MakeGroups(Of C As ICrossReference, T As IMetabolite(Of C))(massGroup As IEnumerable(Of T)) As IEnumerable(Of NamedCollection(Of T))
        Dim groups As New AVLClusterTree(Of T)(AddressOf Compares(Of C, T), Function(a) a.CommonName)

        For Each i As T In massGroup
            Call groups.Add(i)
        Next

        For Each meta As ClusterKey(Of T) In groups.AsEnumerable
            Dim listSet As T() = meta.ToArray
            Dim name As IGrouping(Of String, String) = listSet _
                .Select(Function(a) a.CommonName) _
                .GroupBy(Function(a) a.ToLower) _
                .OrderByDescending(Function(a) a.Count) _
                .First

            Yield New NamedCollection(Of T)(name.Key, listSet)
        Next
    End Function

    ''' <summary>
    ''' Check of the two metabolite information similarity via the jaccard index
    ''' </summary>
    ''' <typeparam name="C"></typeparam>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    Private Function Compares(Of C As ICrossReference, T As IMetabolite(Of C))(x As T, y As T) As Integer
        Dim i As Integer = 0
        Dim union As Integer = 20
        Dim check_name As Boolean = False

        ' has the same database reference id?
        If x.Identity = y.Identity Then i += 1
        If x.CommonName.TextEquals(y.CommonName, False, False) Then
            i += 1
            check_name = True
        End If

        Dim cx As C = x.CrossReference
        Dim cy As C = y.CrossReference

        If cx.KEGGdrug.TextEquals(cy.KEGGdrug, False, False) Then i += 1
        If cx.chemspider.TextEquals(cy.chemspider, False, False) Then i += 1
        If cx.chebi.TextEquals(cy.chebi, False, False) Then i += 1
        If cx.Wikipedia.TextEquals(cy.Wikipedia, False, False) Then i += 1
        If cx.DrugBank.TextEquals(cy.DrugBank, False, False) Then i += 1
        If cx.lipidmaps.TextEquals(cy.lipidmaps, False, False) Then i += 1
        If cx.KEGG.TextEquals(cy.KEGG, False, False) Then i += 1
        If cx.ChEMBL.TextEquals(cy.ChEMBL, False, False) Then i += 1
        If cx.ChemIDplus.TextEquals(cy.ChemIDplus, False, False) Then i += 1
        If cx.foodb.TextEquals(cy.foodb, False, False) Then i += 1
        If cx.HMDB.TextEquals(cy.HMDB, False, False) Then i += 1
        If cx.KNApSAcK.TextEquals(cy.KNApSAcK, False, False) Then i += 1
        If cx.MeSH.TextEquals(cy.MeSH, False, False) Then i += 1
        If cx.MetaCyc.TextEquals(cy.MetaCyc, False, False) Then i += 1
        If cx.metlin.TextEquals(cy.metlin, False, False) Then i += 1
        If cx.pubchem.TextEquals(cy.pubchem, False, False) Then i += 1

        If Not (cx.CAS.IsNullOrEmpty OrElse cy.CAS.IsNullOrEmpty) Then
            If cx.CAS.Any(Function(id) cy.CAS.Any(Function(id2) id.TextEquals(id2, False, False))) Then
                i += 1
            End If
        End If

        ' has any evidence matched then we check of the formula composition
        If check_name OrElse i > 0 Then
            Dim f1 = FormulaScanner.ScanFormula(x.Formula)
            Dim f2 = FormulaScanner.ScanFormula(y.Formula)

            ' 20250113 has the same formula composition and the same cross reference or names
            ' then we could check these two metabolite identical
            If f1 = f2 OrElse (Not f1 Is Nothing AndAlso f1.CompareFormalCharge(f2)) Then
                Return 0
                ' some spectrum metadata has the identical name but no formula information?
                ' deal with this as identical?
            ElseIf check_name AndAlso f1 Is Nothing AndAlso f2 Is Nothing Then
                Return 0
            End If
        End If

        Dim jaccard As Double = i / union

        If jaccard > 0.2 Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Module

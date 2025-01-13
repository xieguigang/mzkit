Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Module CrossReferenceData

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

            Yield New NamedCollection(Of T)(name.Key, meta.ToArray)
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

        ' has the same database reference id?
        If x.Identity = y.Identity Then i += 1
        If x.CommonName.TextEquals(y.CommonName, False, False) Then i += 1

        Dim cx As C = x.CrossReference
        Dim cy As C = y.CrossReference

        If cx.KEGGdrug.TextEquals(cy.KEGGdrug, False, False) Then i += 1
        If cx.chemspider.TextEquals(cy.chemspider, False, False) Then i += 1
        If cx.chebi.TextEquals(cy.chebi, False, False) Then i += 1
        If cx.Wikipedia.TextEquals(cy.Wikipedia, False, False) Then i += 1
        If cx.DrugBank.TextEquals(cy.DrugBank, False, False) Then i += 1
        If cx.lipidmaps.TextEquals(cy.lipidmaps, False, False) Then i += 1
        If cx.CAS.Any(Function(id) cy.CAS.Any(Function(id2) id.TextEquals(id2, False, False))) Then i += 1
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

        If i > 6 Then
            Dim f1 = FormulaScanner.ScanFormula(x.Formula)
            Dim f2 = FormulaScanner.ScanFormula(y.Formula)

            If f1 = f2 Then i += 1
        End If

        Dim jaccard As Double = i / union

        If jaccard > 0.4 Then
            Return 0
        Else
            Return -1
        End If
    End Function
End Module

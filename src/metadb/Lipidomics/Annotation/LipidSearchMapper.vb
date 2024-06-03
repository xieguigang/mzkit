
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Lipidomics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' a helper module for mapping the lipidsearch name to lipidmaps id 
''' </summary>
Public Class LipidSearchMapper(Of T As {IExactMassProvider, IReadOnlyId, ICompoundNameProvider, IFormulaProvider})

    ReadOnly classes As New Dictionary(Of String, AVLClusterTree(Of LipidName))
    ReadOnly formula As New Dictionary(Of String, Dictionary(Of String, T))

    Sub New(lipidmaps As IEnumerable(Of T), getLipidName As Func(Of T, String))
        For Each lipid As T In lipidmaps
            Dim name_str As String = getLipidName(lipid)
            Dim name As LipidName = If(name_str.StringEmpty, Nothing, LipidName.ParseLipidName(name_str))

            If name Is Nothing Then
                Continue For
            Else
                name.id = lipid.Identity
            End If

            If Not classes.ContainsKey(name.className) Then
                classes.Add(name.className, emptyTree)
            End If

            Call classes(name.className).Add(name)
        Next
    End Sub

    Private Shared Function emptyTree() As AVLClusterTree(Of LipidName)
        Return New AVLClusterTree(Of LipidName)(AddressOf compares, Function(n) n.ToString)
    End Function

    Private Shared Function compares(a As LipidName, b As LipidName) As Integer
        If a.chains.Length > b.chains.Length Then
            Return 1
        ElseIf a.chains.Length < b.chains.Length Then
            Return -1
        End If

        Dim total1 = Aggregate c In a.chains Into Sum(c.carbons)
        Dim total2 = Aggregate c In b.chains Into Sum(c.carbons)

        If total1 > total2 Then
            Return 1
        ElseIf total1 < total2 Then
            Return -1
        End If

        Dim dbd1 = Aggregate c In a.chains Into Sum(c.doubleBonds)
        Dim dbd2 = Aggregate c In b.chains Into Sum(c.doubleBonds)

        If dbd1 > dbd2 Then
            Return 1
        ElseIf dbd1 < dbd2 Then
            Return -1
        End If

        Return 0
    End Function

End Class

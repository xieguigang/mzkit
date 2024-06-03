
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Lipidomics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' a helper module for mapping the lipidsearch name to lipidmaps id 
''' </summary>
Public Class LipidSearchMapper(Of T As {IExactMassProvider, IReadOnlyId, ICompoundNameProvider, IFormulaProvider})

    ReadOnly classes As New Dictionary(Of String, AVLClusterTree(Of LipidName))

    Sub New(lipidmaps As IEnumerable(Of T), getLipidName As Func(Of T, String))
        For Each lipid As T In lipidmaps
            Dim name As LipidName = LipidName.ParseLipidName(getLipidName(lipid))

            If Not classes.ContainsKey(name.className) Then
                classes.Add(name.className, emptyTree)
            End If

            Call classes(name.className).Add(name)
        Next
    End Sub

    Private Shared Function emptyTree() As AVLClusterTree(Of LipidName)
        Return New AVLClusterTree(Of LipidName)(AddressOf compares)
    End Function

    Private Shared Function compares(a As LipidName, b As LipidName) As Integer

    End Function

End Class

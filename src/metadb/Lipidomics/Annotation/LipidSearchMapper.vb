
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Lipidomics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' a helper module for mapping the lipidsearch name to lipidmaps id 
''' </summary>
Public Class LipidSearchMapper(Of T As {IExactMassProvider, IReadOnlyId, ICompoundNameProvider, IFormulaProvider})

    ReadOnly classes As New Dictionary(Of String, AVLClusterTree(Of LipidName))
    ReadOnly formula As New Dictionary(Of String, Dictionary(Of String, List(Of T)))

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="lipidmaps"></param>
    ''' <param name="getLipidName">ABBREVIATION</param>
    Sub New(lipidmaps As IEnumerable(Of T), getLipidName As Func(Of T, String))
        For Each lipid As T In lipidmaps
            Dim name_str As String = getLipidName(lipid)
            Dim name As LipidName = If(name_str.StringEmpty, Nothing, LipidName.ParseLipidName(name_str))

            If name Is Nothing Then
                Continue For
            Else
                name.id = lipid.Identity
            End If

            Dim class$ = name.className.ToLower
            Dim abbreviation As String = name.ToOverviewName

            If Not classes.ContainsKey([class]) Then
                classes.Add([class], emptyTree)
            End If
            If Not formula.ContainsKey([class]) Then
                formula.Add([class], New Dictionary(Of String, List(Of T)))
            End If

            If Not formula([class]).ContainsKey(abbreviation) Then
                formula([class]).Add(abbreviation, New List(Of T))
            End If

            Call classes([class]).Add(name)
            Call formula([class])(abbreviation).Add(lipid)
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="class">the lipidsearch class name</param>
    ''' <param name="fattyAcid"></param>
    ''' <returns></returns>
    Public Iterator Function FindLipidReference(class$, fattyAcid As String) As IEnumerable(Of String)
        Dim name As LipidName = LipidName.ParseLipidName([class] & fattyAcid)

        [class] = [class].ToLower

        If Not classes.ContainsKey([class]) Then
            Return
        End If

        ' search by exact structure matches
        Dim query = classes([class]).Search(name).ToArray
        Dim abbreviation As String = name.ToOverviewName

        For Each lipid As LipidName In query
            Yield lipid.id
        Next

        Dim classNames = formula([class])

        If classNames.ContainsKey(abbreviation) Then
            For Each lipid As T In classNames(abbreviation)
                Yield lipid.Identity
            Next
        End If
    End Function

End Class
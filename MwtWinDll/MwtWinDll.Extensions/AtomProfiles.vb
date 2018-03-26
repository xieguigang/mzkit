Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure AtomProfiles

    ''' <summary>
    ''' Values值的和必须要等于100或者全部为零！
    ''' </summary>
    Dim atoms As Dictionary(Of String, Integer)

    Sub New(atoms As IEnumerable(Of String))
        Me.atoms = atoms.ToDictionary(Function(atom) atom, Function() 0%)
    End Sub

    Const InvalidArguments$ = "Atom composition neither SUM(profile) <> 100 or ALL_SUM should equals to ZERO!"

    Public Sub SetAtoms(ByRef finder As MolecularWeightCalculator)
        Dim sum% = atoms.Values.Sum

        If sum = 100% Then
            For Each atom In atoms
                Call finder.FormulaFinder.AddCandidateElement(atom.Key, atom.Value)
            Next
        ElseIf sum = 0% Then
            For Each atom$ In atoms.Keys
                Call finder.FormulaFinder.AddCandidateElement(atom)
            Next
        Else
            Dim ex As New Exception(atoms.GetJson)
            Dim json$ = atoms.Values.ToArray.GetJson

            Throw New ArgumentOutOfRangeException(InvalidArguments & vbCrLf & vbCrLf & json, ex)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return atoms.GetJson
    End Function
End Structure
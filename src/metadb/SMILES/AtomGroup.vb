
Public Class AtomGroup : Inherits Atom

    Public Property mainElement As String

    Public Shared ReadOnly Property AtomGroups As Dictionary(Of String, AtomGroup) = LoadAtoms()

    Sub New(label As String, main As String, ParamArray valence As Integer())
        Call MyBase.New(label, valence)
        mainElement = main
    End Sub

    Private Shared Function LoadAtoms() As Dictionary(Of String, AtomGroup)
        Dim atoms = DefaultAtomGroups.ToArray
        Dim groups As New Dictionary(Of String, AtomGroup)

        For Each atom As AtomGroup In atoms
            Call groups.Add(atom.GetIonLabel, atom)

            If Not groups.ContainsKey(atom.label) Then
                Call groups.Add(atom.label, atom)
            End If
        Next

        Return groups
    End Function

    ''' <summary>
    ''' Check of the given atom group label is existsed inside the default atom group list
    ''' </summary>
    ''' <param name="label"></param>
    ''' <returns></returns>
    Public Shared Function CheckDefaultLabel(label As String) As Boolean
        Return _AtomGroups.ContainsKey(label)
    End Function

    Public Shared Function GetDefaultValence(label As String, val As Integer) As Integer
        If _AtomGroups.ContainsKey(label) Then
            Return _AtomGroups(label).valence(0)
        Else
            Return val
        End If
    End Function

    Public Shared Iterator Function DefaultAtomGroups() As IEnumerable(Of AtomGroup)
        Yield New AtomGroup("OH", "O", -1)
        Yield New AtomGroup("NO3", "N", -1)
        Yield New AtomGroup("SO4", "S", -2)
        Yield New AtomGroup("CO3", "C", -2)
        Yield New AtomGroup("NH4", "N", 1)
        Yield New AtomGroup("NH2", "N", 1, 2)
        Yield New AtomGroup("SO3", "S", -2)
        Yield New AtomGroup("MnO4", "Mn", -1)
        Yield New AtomGroup("HCO3", "C", -1)
        Yield New AtomGroup("PO4", "P", -3)
        Yield New AtomGroup("CH", "C", -1, -2, -3)
        Yield New AtomGroup("CH2", "C", -1, -2)
        Yield New AtomGroup("CH3", "C", -1)
        Yield New AtomGroup("CH4", "C", -1)
        Yield New AtomGroup("CH5", "C", -1) ' 氢化甲基阴离子（hydridomethyl anion）
        Yield New AtomGroup("COOH", "C", -1)
        Yield New AtomGroup("COO", "C", -1)

        ' deepseek 20250312
        Yield New AtomGroup("NO2", "N", -1)      ' 亚硝酸根
        Yield New AtomGroup("CN", "C", -1)       ' 氰根
        Yield New AtomGroup("O2", "O", -2)       ' 过氧根
        Yield New AtomGroup("SCN", "S", -1)      ' 硫氰根（主原子S）
        Yield New AtomGroup("ClO", "Cl", -1)     ' 次氯酸根
        Yield New AtomGroup("ClO3", "Cl", -1)    ' 氯酸根
        Yield New AtomGroup("ClO4", "Cl", -1)    ' 高氯酸根
        Yield New AtomGroup("HPO4", "P", -2)     ' 磷酸氢根
        Yield New AtomGroup("H2PO4", "P", -1)    ' 磷酸二氢根
        Yield New AtomGroup("SiO3", "Si", -2)    ' 硅酸根
        Yield New AtomGroup("BO3", "B", -3)      ' 硼酸根
        Yield New AtomGroup("SH", "S", -1)       ' 巯基（假设作为阴离子）
    End Function
End Class

Public Class MolecularFingerprint
    Private Const BIT_VECTOR_SIZE As Integer = 1024
    Private Const RADIUS As Integer = 2
    Private Const BASE_PRIME As Integer = 67

    ' prettier-ignore
    Private Shared ReadOnly ATOM_SYMBOLS As String() = {
        "H", "He", "Li", "Be", "B", "C", "N", "O", "F", "Ne", "Na", "Mg", "Al", "Si", "P", "S", "Cl", "Ar",
        "K", "Ca", "Sc", "Ti", "V", "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br",
        "Kr", "Rb", "Sr", "Y", "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te",
        "I", "Xe", "Cs", "Ba", "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm",
        "Yb", "Lu", "Hf", "Ta", "W", "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn",
        "Fr", "Ra", "Ac", "Th", "Pa", "U", "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr",
        "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og"
    }

    Private Shared ReadOnly BOND_TYPES As Char() = {"-"c, "="c, "#"c, ":"c, "/"c, "\"c}

    Private Shared Function Hash(s As String) As Integer
        Dim h As Integer = 0
        For i As Integer = 0 To s.Length - 1
            h = BASE_PRIME * h + AscW(s(i))
        Next
        Return h
    End Function

    Private Shared Function GetNeighbors(atomIndex As Integer, bonds As List(Of Integer()), atoms As String(), radius As Integer) As List(Of Integer)
        Dim neighbors As New List(Of Integer)
        Dim visited As New HashSet(Of Integer)
        Dim queue As New Queue(Of Tuple(Of Integer, Integer))()
        queue.Enqueue(Tuple.Create(atomIndex, 0))

        While queue.Count > 0
            Dim item = queue.Dequeue()
            Dim currentAtomIndex = item.Item1
            Dim currentRadius = item.Item2

            If currentRadius > radius Then Exit While
            If visited.Contains(currentAtomIndex) Then Continue While

            visited.Add(currentAtomIndex)
            If currentRadius > 0 Then neighbors.Add(currentAtomIndex)

            For Each bond In bonds
                If bond(0) = currentAtomIndex OrElse bond(1) = currentAtomIndex Then
                    Dim neighborIndex = If(bond(0) = currentAtomIndex, bond(1), bond(0))
                    queue.Enqueue(Tuple.Create(neighborIndex, currentRadius + 1))
                End If
            Next
        End While

        Return neighbors
    End Function

    Private Shared Function GenerateSubstructureHash(atomIndex As Integer, atoms As String(), bonds As List(Of Integer()), radius As Integer) As Integer
        Dim substructure As String = atoms(atomIndex)
        Dim neighbors = GetNeighbors(atomIndex, bonds, atoms, radius)
        neighbors.Sort()

        For Each neighbor In neighbors
            Dim bond = bonds.FirstOrDefault(Function(b) (b(0) = atomIndex And b(1) = neighbor) OrElse (b(0) = neighbor And b(1) = atomIndex))
            If bond IsNot Nothing Then
                Dim bondType = BOND_TYPES(bond(2))
                substructure += bondType.ToString() & atoms(neighbor)
            End If
        Next

        Return (Hash(substructure) And Integer.MaxValue) Mod BIT_VECTOR_SIZE
    End Function

    Public Shared Function ConvertToMorganFingerprint(smiles As String, Optional radius As Integer = RADIUS) As Integer()
        Dim atoms As New List(Of String)
        Dim bonds As New List(Of Integer())
        Dim atomMap As New Dictionary(Of String, Integer)
        Dim currentAtomIndex As Integer = 0

        Dim i As Integer = 0
        While i < smiles.Length
            Dim symbol As String = smiles(i).ToString()
            Dim nextChar As String = If(i < smiles.Length - 1, smiles(i + 1).ToString(), Nothing)

            ' 处理双字母元素（如He）
            If Not String.IsNullOrEmpty(nextChar) AndAlso ATOM_SYMBOLS.Contains(symbol & nextChar) Then
                symbol &= nextChar
                i += 1 ' 额外移动指针
            End If

            If ATOM_SYMBOLS.Contains(symbol) Then
                atoms.Add(symbol)
                atomMap(symbol) = currentAtomIndex
                currentAtomIndex += 1
            ElseIf BOND_TYPES.Contains(symbol(0)) Then
                Dim bondTypeIndex = Array.IndexOf(BOND_TYPES, symbol(0))
                Dim sourceAtomIndex = atomMap(atoms(atoms.Count - 2))
                Dim targetAtomIndex = atomMap(atoms(atoms.Count - 1))
                bonds.Add({sourceAtomIndex, targetAtomIndex, bondTypeIndex})
            End If
            i += 1
        End While

        Dim fingerprint(BIT_VECTOR_SIZE - 1) As Integer

        For index As Integer = 0 To atoms.Count - 1
            Dim atomHash = GenerateSubstructureHash(index, atoms.ToArray(), bonds, radius)
            fingerprint(atomHash) = 1
        Next

        Return fingerprint
    End Function
End Class
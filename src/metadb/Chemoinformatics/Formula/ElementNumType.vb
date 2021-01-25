
Namespace Formula

    Friend Structure ElementNumType

        Public H As Integer
        Public C As Integer
        Public Si As Integer
        Public N As Integer
        Public P As Integer
        Public O As Integer
        Public S As Integer
        Public Cl As Integer
        Public I As Integer
        Public F As Integer
        Public Br As Integer
        Public Other As Integer

        Sub New(formula As Formula)
            H = formula("H")
            C = formula("C")
            Si = formula("Si")
            N = formula("N")
            P = formula("P")
            O = formula("O")
            S = formula("S")
            Cl = formula("Cl")
            I = formula("I")
            F = formula("F")
            Br = formula("Br")

            Dim counts As New Dictionary(Of String, Integer)(formula.CountsByElement)

            Call counts.Remove("H")
            Call counts.Remove("C")
            Call counts.Remove("Si")
            Call counts.Remove("N")
            Call counts.Remove("P")
            Call counts.Remove("O")
            Call counts.Remove("S")
            Call counts.Remove("Cl")
            Call counts.Remove("I")
            Call counts.Remove("F")
            Call counts.Remove("Br")

            Other = counts.Values.Sum
        End Sub
    End Structure
End Namespace

''' <summary>
''' An excellent reference is: Timar, Z. in Handbook of Analysis of Oligonucleotides and Related Products. (eds. J.V. Bonilla & G.S. Srivatsa) 167-218 (CRC Press, 2011)
''' </summary>
Public Class Fragmentation

    Public Cut As String
    Public AdjustMassFromWhichEnd As String
    Public C As String
    Public H As String
    Public N As String
    Public O As String
    Public P As String
    Public S As String
    Public SelectAllowedCuts As Boolean

    Sub New(Cut As String, AdjustMassFromWhichEnd As String,
            C As String,
            H As String,
            N As String,
            O As String,
            P As String,
            S As String,
            SelectAllowedCuts As Boolean)

        Me.Cut = Cut
        Me.AdjustMassFromWhichEnd = AdjustMassFromWhichEnd
        Me.C = C
        Me.H = H
        Me.N = N
        Me.O = O
        Me.P = P
        Me.S = S
        Me.SelectAllowedCuts = SelectAllowedCuts
    End Sub

    Public Iterator Function LoadFragmentation() As IEnumerable(Of Fragmentation)
        Yield New Fragmentation("w", "5'", 0, 2, 0, 4, 1, 0, True)
        Yield New Fragmentation("x", "5'", 0, 0, 0, 3, 1, 0, True)
        Yield New Fragmentation("y", "5'", 0, 1, 0, 1, 0, 0, True)
        Yield New Fragmentation("z", "5'", 0, -1, 0, 0, 0, 0, True)
        Yield New Fragmentation("a-B", "3'", "special", "special", "special", "special", "special", "special", True)
        Yield New Fragmentation("a", "3'", 0, -2, 0, -4, -1, 0, True)
        Yield New Fragmentation("b", "3'", 0, 0, 0, -3, -1, 0, True)
        Yield New Fragmentation("c", "3'", 0, -1, 0, -1, 0, 0, True)
        Yield New Fragmentation("d", "3'", 0, 1, 0, 0, 0, 0, True)
    End Function

End Class

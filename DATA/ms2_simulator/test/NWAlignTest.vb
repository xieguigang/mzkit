Imports SMRUCC.MassSpectrum.Math

Module NWAlignTest

    Sub Main()
        Dim a As LibraryMatrix = {1, 3, 4, 5, 6, 7, 8, 9, 10}
        Dim b As LibraryMatrix = {1, 3, 4, 5, 6, 7, 8, 9, 10, 12}
        Dim c As LibraryMatrix = {1, 2, 4, 5, 6, 7, 8, 9, 10}

        Dim self = GlobalAlignment.NWGlobalAlign(a, a)
        Dim insert = GlobalAlignment.NWGlobalAlign(a, b)
        Dim substitue = GlobalAlignment.NWGlobalAlign(a, c)

        Pause()
    End Sub
End Module

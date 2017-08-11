Imports SMRUCC.proteomics.PNL.OMICS.MwtWinDll.Extensions.IFormulaFinder

Module Program

    Sub Main()

        println("C00047:  C6H14N2O2")

        Dim profile As New AtomProfiles({"C", "H", "N", "O"})
        Dim list = profile.SearchByLimitDaMass(146.1055, deltaPPM:=10)

        For Each f In list
            Call f.ToString.__DEBUG_ECHO
        Next

        Pause()
    End Sub

End Module

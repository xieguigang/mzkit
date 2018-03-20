Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.proteomics.PNL.OMICS.MwtWinDll.Extensions.IFormulaFinder

Module Program

    Sub Main()

        'println("C00047:  C6H14N2O2")

        'Dim profile As New AtomProfiles({"C", "H", "N", "O"})
        'Dim list = profile.SearchByLimitDaMass(146.1055, deltaPPM:=10)

        'For Each f In list
        '    Call f.ToString.__DEBUG_ECHO
        'Next

        'Pause()

        Call test2()

    End Sub

    Sub test2()
        Dim mz = 477.0631
        Dim profile As New AtomProfiles({"C", "H", "N", "O", "S", "P"})
        Dim list = profile.SearchByMZAndLimitCharges(New IntRange(-2, 2), mz, 20)

        For Each formual In list
            Call Console.WriteLine(formual)
        Next

        Pause()
    End Sub

End Module

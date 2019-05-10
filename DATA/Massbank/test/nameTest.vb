Imports SMRUCC.MassSpectrum.DATA.MetaLib

Module nameTest

    Sub Main()
        Dim test1 = "Cys Asn Lys Gln".IsOligopeptideName
        Dim test2 = "Cys Asn Lys".IsOligopeptideName
        Dim test3 = "Cys Asn".IsOligopeptideName
        Dim test4 = "Cys".IsOligopeptideName
        Dim test5 = "Cys Asn Lys Gln Gln".IsOligopeptideName

        Pause()
    End Sub

End Module

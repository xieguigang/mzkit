Module Module1

    Sub Main()
        Call parserTest()

        ' Call PubChem.MySql.CreateMySqlDatabase("N:\pubchem\raw\SDF\uncompress", "N:\pubchem\raw\mysql", "N:\pubchem\raw\extras\metalib.Xml")
    End Sub

    Sub parserTest()

        Dim line$ = "2.0000-9999.9999    0.0000 I   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim line2 = "4.9966   -1.2250    0.0000 H   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim line3 = "-96589-9999.999949999.1992 H   0  0  0  0  0  0  0  0  0  0  0  0"
        Dim atom3 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line3)
        Dim atom2 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line2)
        Dim atom1 = SMRUCC.MassSpectrum.DATA.File.Atom.Parse(line)

        Pause()
    End Sub

End Module

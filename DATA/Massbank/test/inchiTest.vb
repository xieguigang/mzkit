Imports SMRUCC.MassSpectrum.DATA.IUPAC
Imports SMRUCC.MassSpectrum.DATA.IUPAC.InChILayers

Module inchiTest

    Sub Main()

        Dim ascorbicAcid As String = "InChI=1S/C6H8O6/c7-1-2(8)5-3(9)4(10)6(11)12-5/h2,5,7-10H,1H2/t2-,5+/m0/s1"

        Dim inchi As New InChI(ascorbicAcid)

        Dim key As String = inchi.Key

        Dim bounds = MainLayer.ParseBounds("7-1-2(8)5-3(9)4(10)6(11)12-5").ToArray


        Pause()
    End Sub
End Module

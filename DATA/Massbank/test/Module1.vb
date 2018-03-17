Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.DATA

Module Module1

    Sub Main()

        Call convertorTest()
        '  Call dumpPubChem()


        Dim keys = SMRUCC.MassSpectrum.DATA.File.SDF.ScanKeys("D:\smartnucl_integrative\DATA\NCBI\SDF").ToArray

        Call keys.GetJson.SaveTo("./keys.json")


        Pause()
    End Sub

    Sub convertorTest()

        Dim a As New UnitValue(Of Units)(100, Units.cuin_cuft)
        Dim b = a.ConvertTo(Units.milligrams_kg)
        Dim c = a.ConvertTo(Units.cuin_cuft)
        Dim d = a.ConvertTo(Units.drops_gallon_US)
        Dim e = a.ConvertTo(Units.ppm)

        Call $"{a} = {b}".__DEBUG_ECHO
        Call $"{a} = {c}".__DEBUG_ECHO
        Call $"{a} = {d}".__DEBUG_ECHO
        Call $"{a} = {e}".__DEBUG_ECHO

        Pause()
    End Sub

    Sub dumpPubChem()

        Call SMRUCC.MassSpectrum.DATA.File.DumpingPubChemAnnotations("D:\smartnucl_integrative\DATA\NCBI\SDF", "./ddddd.csv")


        Pause()
    End Sub
End Module

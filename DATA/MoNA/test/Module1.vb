Imports SMRUCC.MassSpectrum.DATA.MoNA
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()
        Call exportIdList()
    End Sub

    Sub exportIdList()
        Dim idlist = SDFReader.ParseFile("D:\Database\MoNA\MoNA-export-GNPS-sdf\MoNA-export-GNPS.sdf", skipSpectraInfo:=True).ToDictionary(Function(s) s.ID, Function(s) s.name)

        Call idlist.GetJson.SaveTo("D:\Database\MoNA\GNPS.json")
    End Sub
End Module

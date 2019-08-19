Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.Math

Module Module2

    Sub Main()
        Dim identify = EntityObject.LoadDataSet("Y:\干血片\pos\20190719\doMSMSalignment.report1.csv") _
           .Select(Function(d) (ID:=d.ID, name:=d!name)) _
           .ToDictionary(Function(n) n.ID, Function(n) n.name.StringReplace("_\d+", ""))
        Dim data = EntityObject.LoadDataSet("\\192.168.1.239\linux\project\干血片\design2_20190818\46_DEM\stroke.cor.csv").ToArray

        For Each compound In data
            Dim peaks = compound.ID.Trim("+"c).Trim("["c, "]"c).peakGroup(dt:=10).Value
            Dim names = peaks.Objects.Where(Function(id) identify.ContainsKey(id)).Select(Function(id) identify(id)).Distinct.JoinBy(" / ")

            compound("metabolites") = names
        Next

        Call data.SaveDataSet("\\192.168.1.239\linux\project\干血片\design2_20190818\46_DEM\stroke.cor.metabolites.csv")

        Pause()
    End Sub
End Module

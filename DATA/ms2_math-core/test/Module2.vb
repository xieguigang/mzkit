Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.Math

Module Module2

    Sub Main()
        Dim identify = EntityObject.LoadDataSet("Y:\干血片\pos\20190719\doMSMSalignment.report1.csv") _
           .Select(Function(d) (ID:=d.ID, name:=d!name)) _
           .ToDictionary(Function(n) n.ID, Function(n) n.name)
        Dim data = EntityObject.LoadDataSet("Y:\干血片\design1_DEM_20190819\stroke.cor.csv").ToArray

        For Each compound In data
            Dim peaks = compound.ID.Trim("+"c).Trim("["c, "]"c).peakGroup.Value
            Dim names = peaks.Objects.Where(Function(id) identify.ContainsKey(id)).Select(Function(id) identify(id)).Distinct.JoinBy(" / ")

            compound("metabolites") = names
        Next

        Call data.SaveDataSet("Y:\干血片\design1_DEM_20190819\stroke.cor_metabolites.csv")

        Pause()
    End Sub
End Module

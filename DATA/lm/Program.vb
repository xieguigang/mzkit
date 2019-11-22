Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/fitbest")>
    <Usage("/fitbest /data <measure.csv> /ref <reference.csv> [/format <numericFormat, default=G5> /out <result.csv>]")>
    Public Function lm(args As CommandLine) As Integer
        Dim in$ = args <= "/data"
        Dim ref$ = args <= "/ref"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.reference_to={ref.BaseName}.csv"
        Dim numFormat$ = args("/format") Or "G5"
        Dim data = DataSet.LoadDataSet([in]).ToDictionary
        Dim reference As DataSet() = DataSet.LoadDataSet(ref).ToArray
        Dim result As New List(Of EntityObject)
        Dim models As New List(Of NamedValue(Of IFitted))

        For Each metabolite As DataSet In reference
            Dim refData As DataSet = data(metabolite.ID)
            Dim fit As IFitted = LmMath.lm.CreateLinearModel(refData, metabolite)
            Dim modelFit As New EntityObject With {
                .ID = metabolite.ID,
                .Properties = New Dictionary(Of String, String) From {
                    {"R2", fit.CorrelationCoefficient},
                    {"formula", "f(x)=" & fit.Polynomial.ToString(numFormat)},
                    {"|||||||", ""}
                }
            }

            For Each sample In refData.Properties
                modelFit(sample.Key) = fit.GetY(sample.Value)
            Next

            models += New NamedValue(Of IFitted) With {
                .Name = metabolite.ID,
                .Value = fit
            }
            result += modelFit
        Next

        Call result.SaveDataSet(out)

        Dim load As File = File.Load(out)

        For Each model As NamedValue(Of IFitted) In models
            Call load.AppendLine()
            Call load.AppendLine({model.Name})
            Call load.AppendLine({"X", $"Y({model.Name})", "f(x)"})

            Dim X = model.Value.X
            Dim Y = model.Value.Y
            Dim yfit = model.Value.Yfit

            For i As Integer = 0 To X.Dim - 1
                Call load.AppendLine(New String() {X(i), Y(i), yfit(i)})
            Next
        Next

        Return load.Save(out).CLICode
    End Function

End Module

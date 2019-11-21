Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/fitbest")>
    <Usage("/fitbest /data <measure.csv> /ref <reference.csv> [/out <result.csv>]")>
    Public Function lm(args As CommandLine) As Integer
        Dim in$ = args <= "/data"
        Dim ref$ = args <= "/ref"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.reference_to={ref.BaseName}.csv"
        Dim data = DataSet.LoadDataSet([in]).ToDictionary
        Dim reference As DataSet() = DataSet.LoadDataSet(ref).ToArray
        Dim result As New List(Of EntityObject)

        For Each metabolite As DataSet In reference
            Dim refData As DataSet = data(metabolite.ID)
            Dim fit As IFitted = LmMath.lm.CreateLinearModel(metabolite, refData)

            result += New EntityObject With {
                .ID = metabolite.ID,
                .Properties = New Dictionary(Of String, String) From {
                    {"R2", fit.CorrelationCoefficient},
                    {"formula", fit.Polynomial.ToString}
                }
            }
        Next

        Return result _
            .SaveDataSet(out) _
            .CLICode
    End Function

End Module

#Region "Microsoft.VisualBasic::aae3c4e05d937f65d8f7c4b0855d6f01, src\mzmath\lm\Program.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Program
    ' 
    '     Function: lm, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
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
    <Description("Create the best fitting linear model.")>
    <Argument("/data", False, CLITypes.File,
              AcceptTypes:={GetType(DataSet)},
              Extensions:="*.csv",
              Description:="The experiment result data.")>
    <Argument("/ref", False, CLITypes.File,
              AcceptTypes:={GetType(DataSet)},
              Extensions:="*.csv",
              Description:="The reference content values.")>
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

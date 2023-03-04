Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace ASCII.Raman

    Public Module FileReader

        ReadOnly schema As Dictionary(Of String, PropertyInfo)

        Sub New()
            schema = DataFramework _
                .Schema(Of Spectroscopy)(PropertyAccess.Writeable, nonIndex:=True, primitive:=True) _
                .ToDictionary(Function(p)
                                  Dim [alias] = p.Value.GetCustomAttribute(Of Field)

                                  If [alias] Is Nothing Then
                                      Return p.Key
                                  Else
                                      Return [alias].Name
                                  End If
                              End Function,
                              Function(p)
                                  Return p.Value
                              End Function)
        End Sub

        Public Function ParseTextFile(txt As StreamReader) As Spectroscopy
            Dim line As Value(Of String) = ""
            Dim data As NamedValue(Of String)
            Dim value As Object
            Dim raman As New Spectroscopy
            Dim type As Type

            Do While Not (line = txt.ReadLine).StringEmpty
                data = line.GetTagValue(vbTab, trim:=True, failureNoName:=False)

                If line.Value.TextEquals("xydata") Then
                    Exit Do
                End If

                If schema.ContainsKey(data.Name) Then
                    type = schema(data.Name).PropertyType
                Else
                    Continue Do
                End If

                If data.Value.StringEmpty Then
                    value = Nothing
                Else
                    value = Scripting.CTypeDynamic(data.Value, type)
                End If

                Call schema(data.Name).SetValue(raman, value)
            Loop

            Dim pt As PointF
            Dim spectroscopy As New List(Of PointF)
            Dim metadata As New Dictionary(Of String, Dictionary(Of String, String))
            Dim information As Dictionary(Of String, String) = Nothing

            Do While Not (line = txt.ReadLine).StringEmpty
                Dim t = line.Split(vbTab)
                pt = New PointF(Val(t(0)), Val(t(1)))
                spectroscopy.Add(pt)
            Loop

            raman.xyData = spectroscopy.ToArray

            Do While Not (line = txt.ReadLine) Is Nothing
                If line.Value.StringEmpty OrElse line.First = "#"c Then
                    Continue Do
                End If
                If line.Value.IsPattern("\[.+\]") Then
                    information = New Dictionary(Of String, String)
                    metadata.Add(line, information)
                Else
                    data = line.GetTagValue(vbTab, trim:=True, failureNoName:=False)
                    information.Add(data.Name, data.Value)
                End If
            Loop

            raman.Comments = metadata.TryGetValue("[Comments]")
            raman.DetailedInformation = metadata.TryGetValue("[Detailed Information]")
            raman.MeasurementInformation = metadata.TryGetValue("[Measurement Information]")

            Return raman
        End Function
    End Module
End Namespace
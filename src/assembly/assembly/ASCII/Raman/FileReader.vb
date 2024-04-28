#Region "Microsoft.VisualBasic::fe55cac20302ca28071a17c1a7ddc074, G:/mzkit/src/assembly/assembly//ASCII/Raman/FileReader.vb"

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


    ' Code Statistics:

    '   Total Lines: 100
    '    Code Lines: 82
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 3.80 KB


    '     Module FileReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) ParseTextFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Public Function ParseTextFile(file As String) As Spectroscopy
            Using buf = file.OpenReader
                Return ParseTextFile(buf)
            End Using
        End Function

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

#Region "Microsoft.VisualBasic::c39717706b3aec913df754e1be462abf, assembly\ASCII\MSL\FileReader.vb"

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

    '     Module FileReader
    ' 
    '         Function: dataBlockParser, GetMapName, Load, ParseModelSchema, ParseMs2Peaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ValueTypes
Imports r = System.Text.RegularExpressions.Regex
Imports any = Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace ASCII.MSL

    Public Module FileReader

        Private Function dataBlockParser(path As String) As IEnumerable(Of String())
            Dim raw As String() = path.ReadAllText _
                .Trim(" "c, CChar(vbTab), CChar(vbCr), CChar(vbLf)) _
                .LineTokens

            If raw.Any(Function(l) l.StringEmpty) Then
                Return raw.Split(Function(s) s.StringEmpty, DelimiterLocation.NotIncludes)
            Else
                Return raw _
                    .Split(Function(s) InStr(s, "NAME:") = 1, DelimiterLocation.NextFirst) _
                    .Where(Function(b) Not b.IsNullOrEmpty)
            End If
        End Function

        Private Function ParseModelSchema() As (properties As Dictionary(Of String, PropertyInfo), schema As Dictionary(Of String, String))
            Dim properties As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of MSLIon)(PropertyAccess.Readable, True)
            ' property name => column name
            Dim schema As Dictionary(Of String, String) = properties.Values _
                .ToDictionary(Function(field) field.Name,
                              Function(field)
                                  Return GetMapName(field)
                              End Function)

            Return (properties, schema)
        End Function

        Private Function GetMapName(field As PropertyInfo) As String
            Dim map As ColumnAttribute = field.GetCustomAttribute(GetType(ColumnAttribute))

            If map Is Nothing Then
                Return LCase(field.Name)
            Else
                Return LCase(map.Name)
            End If
        End Function

        ReadOnly empty As Index(Of String) = {"-", "NA", "n/a", "null", "NULL"}

        Public Iterator Function Load(path$, Optional unit As TimeScales = TimeScales.Second) As IEnumerable(Of MSLIon)
            Dim properties As Dictionary(Of String, PropertyInfo)
            Dim schema As Dictionary(Of String, String)
            Dim chemicals = dataBlockParser(path).ToArray

            With ParseModelSchema()
                schema = .schema
                properties = .properties
            End With

            Call schema.Remove(NameOf(MSLIon.Peaks))

            For Each lines As String() In chemicals
                Dim data As String()() = lines _
                    .Split(Function(s)
                               Return InStr(s, "NUM PEAKS:", CompareMethod.Text) = 1
                           End Function, DelimiterLocation.NotIncludes) _
                    .ToArray
                Dim meta As String() = data(Scan0)
                Dim peaks As String() = data.ElementAtOrDefault(1)
                Dim metaTable = meta _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToDictionary(Function(tuple)
                                      Return tuple.Name.ToLower
                                  End Function)
                Dim o As Object = Activator.CreateInstance(GetType(MSLIon))

                For Each prop In schema
                    Dim write As PropertyInfo = properties(prop.Key)
                    Dim valueStr As String = metaTable.TryGetValue(prop.Value).Value
                    Dim value As Object

                    If valueStr Like empty Then
                        If write.PropertyType Is GetType(Double) OrElse write.PropertyType Is GetType(Double?) Then
                            value = Nothing
                        Else
                            value = any.CTypeDynamic(valueStr, write.PropertyType)
                        End If
                    Else
                        value = any.CTypeDynamic(valueStr, write.PropertyType)
                    End If

                    If write.Name = NameOf(MSLIon.RT) Then
                        If unit = TimeScales.Minute Then
                            value = CDbl(value) * 60
                        End If
                    End If

                    Call write.SetValue(o, value)
                Next

                Yield DirectCast(o, MSLIon).With(Sub(msl) msl.Peaks = peaks.ParseMs2Peaks)
            Next
        End Function

        Const fragment$ = "\(\s*\d+(\.\d+)?\s*\d+(\.\d+)?\s*\)"

        <Extension>
        Private Function ParseMs2Peaks(peaks As String()) As ms2()
            Return peaks _
                .Select(Function(s)
                            Return r.Matches(s, fragment).ToArray
                        End Function) _
                .IteratesALL _
                .Select(Function(s)
                            Dim p As String() = s _
                                .GetStackValue("(", ")") _
                                .Trim _
                                .StringSplit("\s+")

                            Return New ms2 With {
                                .intensity = Val(p(1)),
                                .mz = Val(p(0))
                            }
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace

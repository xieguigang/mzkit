Imports System.Data.Linq.Mapping
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ValueTypes
Imports r = System.Text.RegularExpressions.Regex

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

        Public Iterator Function Load(path$, Optional unit As TimeScales = TimeScales.Second) As IEnumerable(Of MSLIon)
            Dim properties = DataFramework.Schema(Of MSLIon)(PropertyAccess.Readable, True)
            ' property name => column name
            Dim schema As Dictionary(Of String, String) = properties.Values _
                .ToDictionary(Function(field) field.Name,
                              Function(field)
                                  Dim map As ColumnAttribute = field.GetCustomAttribute(GetType(ColumnAttribute))

                                  If map Is Nothing Then
                                      Return LCase(field.Name)
                                  Else
                                      Return LCase(map.Name)
                                  End If
                              End Function)
            Dim chemicals = dataBlockParser(path).ToArray

            Call schema.Remove(NameOf(MSLIon.Peaks))

            For Each lines In chemicals
                Dim data = lines.Split(Function(s) InStr(s, "NUM PEAKS:", CompareMethod.Text) = 1, DelimiterLocation.NotIncludes).ToArray
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
                    Dim value As Object = Scripting.CTypeDynamic(metaTable.TryGetValue(prop.Value).Value, write.PropertyType)

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
                                .quantity = .intensity,
                                .mz = Val(p(0))
                            }
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
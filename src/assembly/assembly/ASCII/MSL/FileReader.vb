Imports System.Data.Linq.Mapping
Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports r = System.Text.RegularExpressions.Regex

Namespace ASCII.MSL

    Public Module FileReader

        Public Iterator Function Load(path As String) As IEnumerable(Of MSL)
            Dim chemicals = path _
                .ReadAllLines _
                .Split(Function(s) s.StringEmpty, DelimiterLocation.NotIncludes)
            Dim properties = DataFramework.Schema(Of MSL)(PropertyAccess.Readable, True)
            ' property name => column name
            Dim schema As Dictionary(Of String, String) = properties.Values _
                .ToDictionary(Function(field) field.Name,
                              Function(field)
                                  Dim map As ColumnAttribute = field.GetCustomAttribute(GetType(ColumnAttribute))

                                  If map Is Nothing Then
                                      Return field.Name
                                  Else
                                      Return map.Name
                                  End If
                              End Function)

            Call schema.Remove(NameOf(MSL.Peaks))

            For Each lines In chemicals
                Dim data = lines.Split(Function(s) InStr(s, "NUM PEAKS:") = 1, DelimiterLocation.NotIncludes)
                Dim meta = data(Scan0)
                Dim peaks = data.ElementAtOrDefault(1)
                Dim metaTable = meta _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToDictionary
                Dim o As Object = Activator.CreateInstance(GetType(MSL))

                For Each prop In schema
                    Dim value = metaTable.TryGetValue(prop.Value).Value
                    Dim write As PropertyInfo = properties(prop.Key)

                    Call write.SetValue(o, value)
                Next

                Dim peaksData As ms2() = peaks _
                    .Select(Function(s) r.Matches(s, "\(\s*\d+\s*\d+\s*\)").ToArray) _
                    .IteratesALL _
                    .Select(Function(s)
                                Dim p = s _
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

                Yield DirectCast(o, MSL).With(Sub(msl) msl.Peaks = peaksData)
            Next
        End Function
    End Module
End Namespace
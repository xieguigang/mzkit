Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Math.Spectra
Imports r = System.Text.RegularExpressions.Regex

Namespace ASCII.MGF

    Public Module MgfReader

        Const regexp_META$ = "((,\s*)?\S+[:]"".*?"")+"

        Public Iterator Function StreamParser(path$) As IEnumerable(Of Ions)
            Dim lines$() = path.ReadAllLines

            For Each ion As String() In lines.Split(Function(s) s = "END IONS", DelimiterLocation.NotIncludes)
                Yield ParseIonBlock(ion)
            Next
        End Function

        Private Function ParseIonBlock(ion As String()) As Ions
            Dim properties = ion _
                 .Where(Function(s) InStr(s, "=") > 1) _
                 .Select(Function(s)
                             Return s.GetTagValue("=", trim:=True)
                         End Function) _
                 .ToDictionary()
            Dim peaks = ion _
                .Skip(properties.Count + 1) _
                .Select(Function(s) s.StringSplit("\s+")) _
                .Select(Function(l)
                            Return New ms2 With {
                                .mz = l(0),
                                .intensity = l(1)
                            }
                        End Function) _
                .ToArray
            Dim getValue = Function(key$)
                               Dim s$ = properties _
                                  .TryGetValue(key, [default]:=Nothing) _
                                  .Value

                               Return s Or EmptyString
                           End Function
            Dim mass As NamedValue
            Dim title$ = getValue("TITLE")
            Dim meta As Dictionary(Of String, String)

            With r.Match(title, regexp_META, RegexICSng).Value
                If .StringEmpty Then
                    ' 没有添加其他的注释信息
                    meta = New Dictionary(Of String, String)
                Else
                    title = title.Replace(.ByRef, "")
                    meta = .StringSplit(",\s+") _
                        .Select(Function(s) s.GetTagValue(":")) _
                        .ToDictionary(Function(key) key.Name,
                                      Function(val) val.Value.Trim(""""c))
                End If
            End With

            With getValue("PEPMASS").StringSplit("\s+")
                mass = New NamedValue(.First, .Last)
            End With

            Return New Ions With {
                .Peaks = peaks,
                .RtInSeconds = Val(getValue("RTINSECONDS")),
                .PepMass = mass,
                .Title = title,
                .Meta = meta,
                .Charge = CInt(Val(getValue("CHARGE"))) Or 1.AsDefault
            }
        End Function
    End Module
End Namespace
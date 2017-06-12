Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.Linq
Imports r = System.Text.RegularExpressions.Regex
Imports Microsoft.VisualBasic.Language

Namespace ASCII.MGF

    Public Class Ions

        Public Property Title As String
        ''' <summary>
        ''' The meta data collection in the title property
        ''' </summary>
        ''' <returns></returns>
        Public Property Meta As Dictionary(Of String, String)
        ''' <summary>
        ''' MS1 rt in seconds format
        ''' </summary>
        ''' <returns></returns>
        Public Property RtInSeconds As Double
        Public Property PepMass As NamedValue
        ''' <summary>
        ''' MS/MS peaks
        ''' </summary>
        ''' <returns></returns>
        Public Property Peaks As MSSignal()

        Public Overrides Function ToString() As String
            Return $"{Title} ({Peaks.SafeQuery.Count} peaks)"
        End Function

        Public Shared Iterator Function StreamParser(path$) As IEnumerable(Of Ions)
            Dim lines$() = path.ReadAllLines

            For Each ion As String() In lines.Split(Function(s) s = "END IONS", DelimiterLocation.NotIncludes)
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
                                Return New MSSignal With {
                                    .x = l(0),
                                    .y = l(1)
                                }
                            End Function) _
                    .ToArray
                Dim getValue = Function(key$)
                                   Dim s$ = properties _
                                      .TryGetValue(key, [default]:=Nothing) _
                                      .Value
                                   If s Is Nothing Then
                                       s = ""
                                   End If

                                   Return s
                               End Function
                Dim mass As NamedValue
                Dim title$ = getValue("TITLE")
                Dim meta As Dictionary(Of String, String)

                Const regexp_META$ = "((,\s*)?\S+[:]"".*?"")+"

                With r.Match(title, regexp_META, RegexICSng).Value
                    title = title.Replace(.ref, "")
                    meta = .ref _
                        .StringSplit(",\s+") _
                        .Select(Function(s) s.GetTagValue(":")) _
                        .ToDictionary(Function(key) key.Name, 
                                      Function(val) val.Value.Trim(""""c))
                End With

                With getValue("PEPMASS").StringSplit("\s+")
                    mass = New NamedValue(.First, .Last)
                End With

                Yield New Ions With {
                    .Peaks = peaks,
                    .RtInSeconds = Val(getValue("RTINSECONDS")),
                    .PepMass = mass,
                    .Title = title,
                    .Meta = meta
                }
            Next
        End Function
    End Class
End Namespace

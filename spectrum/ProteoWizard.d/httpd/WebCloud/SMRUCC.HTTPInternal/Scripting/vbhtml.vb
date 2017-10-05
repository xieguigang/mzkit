Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Scripting

    Public Module vbhtml

        Const Expression$ = "<%= [^>]+? %>"
        Const ValueExpression$ = "<\?vb\s+[$].+?=\s*""[^""]*""\s+\?>"

        ''' <summary>
        ''' ``&lt;%= relative_path %>``
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadHTML(wwwroot$, path$, Optional encoding As Encodings = Encodings.UTF8) As String
            Dim codepage As Encoding = encoding.CodePage
            Dim parent$ = path.ParentPath
            Dim html As New StringBuilder(path.ReadAllText(codepage))
            Dim includes$() = Regex _
                .Matches(html.ToString, vbhtml.Expression, RegexICSng) _
                .ToArray
            Dim values$() = Regex _
                .Matches(html.ToString, vbhtml.ValueExpression, RegexICMul) _
                .ToArray
            Dim table As (raw$, exp As NamedValue(Of String))() = values _
                .Select(Function(s)
                            Dim exp = Mid(s.Trim("<"c, ">"c, "?"c), 4) _
                                .Trim _
                                .GetTagValue("=", trim:=True)
                            With exp
                                .Value = .Value.GetStackValue("""", """")
                                .Name = .Name.Trim("$"c, " "c)
                            End With
                            Return (s, exp)
                        End Function).ToArray

            ' <%= @Key %>
            Dim strings As New Dictionary(Of String, String)

            ' <%= include_path %>

            For Each include As String In includes
                Dim rel_path$ = include.Trim("<"c, ">"c, "%"c)
                rel_path = Mid(rel_path, 2).Trim  ' 去除等号

                If rel_path.First = "@"c Then
                    ' 因为对资源的引用可能会在多处有重复的引用
                    ' 所以在这里不能够直接进行添加
                    strings(include) = rel_path.Substring(1)
                    Continue For
                Else
                    rel_path = parent & "/" & rel_path
                    rel_path = FileIO.FileSystem.GetFileInfo(rel_path).FullName
                End If

                Dim content$ = rel_path.ReadAllText(codepage)
                Call html.Replace(include, content)
            Next

            If table.Length > 0 Then
                Dim exp = table _
                    .Select(Function(e) e.exp) _
                    .ToDictionary
                Dim getValue = Function(name$)
                                   If exp.ContainsKey(name) Then
                                       Return exp(name).Value
                                   Else
                                       Return ""
                                   End If
                               End Function
                ' 替换操作应该放在插值操作前面，否则后面将无法清除掉这些原始
                ' 的插值标记， 因为标记里面的$变量都会被替换为值了
                For Each t In table
                    Call html.Replace(t.raw, "")
                Next

                Call html.Interpolate(getValue)
            End If
            If strings.Count > 0 Then
                Call html.__applyStrings(wwwroot, strings)
            End If

            Return html.ToString
        End Function

        ''' <summary>
        ''' 字符串资源默认是在 ``&lt;wwwroot>/includes/strings.XML``
        ''' </summary>
        ''' <param name="html"></param>
        ''' <param name="strings"></param>
        <Extension>
        Private Sub __applyStrings(ByRef html As StringBuilder, wwwroot$, strings As Dictionary(Of String, String))
            Dim xml As XmlDocument = (wwwroot & "/includes/strings.XML").LoadXmlDocument
            Dim XmlNodeList As XmlNodeList = xml.GetElementsByTagName("string")
            Dim values As New Dictionary(Of String, String)

            For Each xmlNode As XmlNode In XmlNodeList
                For Each a As XmlAttribute In xmlNode.Attributes
                    If a.Name = "name" Then
                        values.Add(a.InnerText, xmlNode.InnerText)
                        Exit For
                    End If
                Next
            Next

            For Each ref In strings
                Call html.Replace(ref.Key, values(ref.Value))
            Next
        End Sub
    End Module
End Namespace
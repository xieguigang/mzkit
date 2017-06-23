Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Parsers

Namespace ASCII.MSP

    Public Module Comments

        ''' <summary>
        ''' 解析放置于注释之中的代谢物注释元数据
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        <Extension> Public Function ToTable(comments$) As Dictionary(Of String, String)
            Dim tokens$() = CLIParser.GetTokens(comments)
            Dim data As Dictionary(Of String, String) = tokens _
                .Select(Function(s)
                            Return s.GetTagValue("=", trim:=True)
                        End Function) _
                .ToDictionary(Function(k) k.Name,
                              Function(x) x.Value)
            Return data
        End Function

        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As Dictionary(Of String, String) = comments.ToTable

        End Function
    End Module

    Public Structure MetaData

    End Structure
End Namespace
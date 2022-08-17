Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace NCBI.MeSH

    Public Module Reader

        Public Function ParseTree(file As StreamReader) As Tree
            Dim line As Value(Of String) = ""
            Dim str As String()
            Dim term As Term
            Dim tree As New Tree With {.term = New Term With {.term = "/", .tree = {}}}
            Dim node As Tree

            Do While Not (line = file.ReadLine) Is Nothing
                str = line.Split(";"c)
                term = New Term With {
                    .term = str(Scan0),
                    .tree = str(1).Split("."c)
                }
                node = tree

                For Each lv As String In term.tree
                    If Not node.childs.ContainsKey(lv) Then
                        node.childs.Add(lv, New Tree)
                    End If

                    node = node.childs(lv)
                Next

                node.term = term
            Loop

            Return tree
        End Function
    End Module
End Namespace
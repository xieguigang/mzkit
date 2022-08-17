Imports System.IO
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace NCBI.MeSH

    Public Module Reader

        Public Function ParseTree(file As StreamReader) As Tree(Of Term)
            Dim line As Value(Of String) = ""
            Dim str As String()
            Dim term As Term
            Dim tree As New Tree(Of Term) With {
                .Data = New Term With {.term = "/", .tree = {}},
                .Childs = New Dictionary(Of String, Tree(Of Term)),
                .label = "NCBI MeSH"
            }
            Dim node As Tree(Of Term)

            Do While Not (line = file.ReadLine) Is Nothing
                str = line.Split(";"c)
                term = New Term With {
                    .term = str(Scan0),
                    .tree = str(1).Split("."c)
                }
                node = tree

                For Each lv As String In term.tree
                    If Not node.Childs.ContainsKey(lv) Then
                        node.Childs.Add(lv, New Tree(Of Term) With {
                            .Childs = New Dictionary(Of String, Tree(Of Term)),
                            .label = lv,
                            .Parent = node
                        })
                    End If

                    node = node.Childs(lv)
                Next

                node.Data = term
            Loop

            Return tree
        End Function
    End Module
End Namespace
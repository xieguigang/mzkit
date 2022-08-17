Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace NCBI.MeSH

    Public Module Reader

        Public Function ParseRDF(file As StreamReader) As RDF
            Dim line As Value(Of String) = ""
            Dim str As String

            Do While Not (line = file.ReadLine) Is Nothing
                str = line.Value

            Loop
        End Function
    End Module
End Namespace
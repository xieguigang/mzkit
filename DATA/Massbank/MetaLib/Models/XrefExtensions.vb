Imports System.Runtime.CompilerServices

Namespace MetaLib.Models

    <HideModuleName>
    Public Module XrefExtensions

        <Extension>
        Public Function FormatChEbiID(id As String) As String
            id = id.Match("\d+")

            If Val(id) <= 0 Then
                Return ""
            Else
                Return $"CHEBI:{id}"
            End If
        End Function

        <Extension>
        Public Function FormatHMDBId(id As String) As String
            If Not xref.IsHMDB(id) Then
                Return ""
            Else
                id = id.Match("\d+").ParseInteger.ToString

                If Val(id) <= 0 Then
                    Return ""
                Else
                    Return $"HMDB{id.FormatZero("0000000")}"
                End If
            End If
        End Function
    End Module
End Namespace
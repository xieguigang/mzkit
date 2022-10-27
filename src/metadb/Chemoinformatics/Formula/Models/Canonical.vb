Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Formula

    Public Module Canonical

        ''' <summary>
        ''' C -> H -> N -> O -> P -> S -> Cl -> others
        ''' </summary>
        ReadOnly orders As Index(Of String) = {"C", "H", "N", "O", "P", "S", "Cl"}
        ReadOnly order_string As String() = orders.Objects

        Public Function BuildCanonicalFormula(countsByElement As Dictionary(Of String, Integer)) As String
            Dim sb As New List(Of String)
            Dim n As Integer

            For Each elementName As String In order_string
                If countsByElement.ContainsKey(elementName) Then
                    n = countsByElement(elementName)

                    If n > 0 Then
                        sb.Add(If(n = 1, elementName, elementName & n.ToString))
                    End If
                End If
            Next

            For Each element As KeyValuePair(Of String, Integer) In From e As KeyValuePair(Of String, Integer)
                                                                    In countsByElement
                                                                    Where orders.IndexOf(e.Key) = -1
                                                                    Order By e.Key
                If element.Value > 0 Then
                    sb.Add(If(element.Value = 1, element.Key, element.Key & element.Value))
                End If
            Next

            Return sb.JoinBy("")
        End Function
    End Module
End Namespace
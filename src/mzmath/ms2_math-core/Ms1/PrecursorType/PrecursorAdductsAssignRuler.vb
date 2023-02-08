Namespace Ms1.PrecursorType

    Public Class PrecursorAdductsAssignRuler

        ''' <summary>
        ''' 制定一些加和物离子规则，例如：
        ''' 
        ''' 化学式中含有活泼离子，例如
        ''' 
        ''' 1. Na+，负离子下很可能为[M-Na]-
        ''' 2. Cl-, 正离子下很可能为[M-Cl]+
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <param name="ionMode"></param>
        ''' <returns></returns>
        Public Function PossibleTypes(formula As String, ionMode As Integer) As String
            If formula.StringEmpty Then
                Return Nothing
            End If

            If ionMode = 1 Then
                Return IonPositiveTypes(formula)
            Else
                Return IonNegativeTypes(formula)
            End If
        End Function

        Private Function IonNegativeTypes(formula As String) As String
            For Each metal As String In {"Na", "Li", "H"}
                If formula.Contains(metal) Then
                    Return $"[M-{metal}]-"
                End If
            Next

            Return Nothing
        End Function

        Private Function IonPositiveTypes(formula As String) As String
            For Each ion As String In {"Cl"}
                If formula.Contains(ion) Then
                    Return $"[M-{ion}]+"
                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace
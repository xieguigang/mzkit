Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Ms1.PrecursorType

    Public Module MolWeight

        ReadOnly weights As New Dictionary(Of String, Double) From {
            {"H", 1.007276},
            {"Na", 22.98976928},
            {"NH4", 18.035534},
            {"K", 39.0983},
            {"H2O", 18.01471},
            {"ACN", 41.04746},      ' Acetonitrile (CH3CN)
            {"CH3OH", 32.03773},
            {"DMSO", 78.12089},     ' dimethyl sulfoxide (CH3)2SO 
            {"IsoProp", 60.058064}, ' Unknown
            {"Cl", 35.446},
            {"FA", 46.00548},       ' Unknown
            {"Hac", 60.04636},      ' AceticAcid (CH3COOH)
            {"Br", 79.901},
            {"TFA", 113.9929}       ' Unknown
        }

        Public Function Weight(symbol As String) As Double
            If weights.ContainsKey(symbol) Then
                Return weights(symbol)
            Else
                Return -1
            End If
        End Function

        Public Function Eval(formula As String) As Double
            Static symbol As Index(Of Char) = {"+"c, "-"c}

            If formula.First.IsOneOfA(symbol) Then
                formula = "0H" & formula
            End If

            Dim mt = r.Split(formula, "[+-]")
            Dim op = r.Matches(formula, "[+-]").ToArray
            Dim x# = 0
            Dim [next] As Char = "+"c

            For i As Integer = 0 To mt.Length - 1
                Dim token = MolWeight.Mul(mt(i))
                Dim m = token.Value
                Dim name = token.Name

                If Weight(name) = -1.0# Then
                    Dim msg$ = $"Unknown symbol in: '{formula}', where symbol={token}"
                    Throw New Exception(msg)
                End If

                If [next] = "+"c Then
                    x += (m * weights(name))
                Else
                    x -= (m * weights(name))
                End If

                If ((Not op.IsNullOrEmpty) AndAlso (i <= op.Length - 1)) Then
                    [next] = op(i)
                End If
            Next

            Return x
        End Function

        Private Function Mul(token As String) As NamedValue(Of Integer)
            Dim n$ = ""
            Dim len% = Strings.Len(token)

            Static x0 = Asc("0")
            Static x9 = Asc("9")

            For i As Integer = 0 To len - 1
                Dim x% = Asc(token(i))

                If (x >= x0 AndAlso x <= x9) Then
                    n = n & token(i)
                Else
                    Exit For
                End If
            Next

            If Strings.Len(n) = 0 Then
                Return New NamedValue(Of Integer) With {
                    .Name = token,
                    .Value = 1
                }
            Else
                token = token.Substring(n.Length)
            End If

            Return New NamedValue(Of Integer) With {
                .Name = token,
                .Value = CInt(Val(n))
            }
        End Function
    End Module
End Namespace
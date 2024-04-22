Imports System.Text.RegularExpressions

Namespace MetaLib.CommonNames

    Friend MustInherit Class RankingRule

        Public MustOverride ReadOnly Property Penalty As Double
        Public MustOverride ReadOnly Property Type As String
        Public MustOverride Function Match(name As String) As Boolean

        Public Overrides Function ToString() As String
            Return Type
        End Function

    End Class

    Friend Class ChemicalFormulaRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 1.5
        Public Overrides ReadOnly Property Type As String = "chemical formula string"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("([A-Z]([a-z]?)(\d+)?)+", RegexOptions.Singleline)
        End Function
    End Class

    Friend Class NumberRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 10000
        Public Overrides ReadOnly Property Type As String = "number"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("\d+(\.\d+)?")
        End Function
    End Class

    Friend Class DatabaseIdRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 2.5
        Public Overrides ReadOnly Property Type As String = "database id"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("[a-zA-Z]+\s*\d+")
        End Function
    End Class

    Friend Class InchiKeyRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 100
        Public Overrides ReadOnly Property Type As String = "inchikey"

        Public Overrides Function Match(name As String) As Boolean
            If name.ToUpper = name Then
                ' inchikey
                If name.All(Function(c) Char.IsDigit(c) OrElse Char.IsLetter(c) OrElse c = "-"c) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class

    Friend Class UpperCaseName : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 1.5
        Public Overrides ReadOnly Property Type As String = "upper case name"

        Public Overrides Function Match(name As String) As Boolean
            If name.All(Function(c)
                            If Char.IsLetter(c) AndAlso Char.IsUpper(c) Then
                                Return True
                            ElseIf c = " "c Then
                                Return True
                            Else
                                Return False
                            End If
                        End Function) Then

                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
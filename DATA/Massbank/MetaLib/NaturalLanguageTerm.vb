Imports System.Runtime.CompilerServices

Public Module NaturalLanguageTerm

    <Extension> Public Function ParseVendorName(text As String) As String
        Static prefix$() = {"Thermo", "Waters", "Agilent"}

        For Each name As String In prefix
            If InStr(text, name) > 0 Then
                Return name
            End If
        Next

        Dim postfix$ = Strings.Trim(text?.Split(","c).LastOrDefault)
        Dim isNamePattern = Function(name As String) As Boolean
                                Return name.NotEmpty AndAlso
                                       name.IsPattern("[a-z0-9]", RegexICSng)
                            End Function

        If isNamePattern(postfix) Then
            Return postfix
        ElseIf postfix = Trim(text) Then
            postfix = Strings.Split(text).FirstOrDefault

            If isNamePattern(postfix) Then
                Return postfix
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function
End Module

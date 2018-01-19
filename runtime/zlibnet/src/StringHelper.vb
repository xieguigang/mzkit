Imports System.Collections.Generic
Imports System.Text
Imports System.IO

Module StringHelper

    <System.Runtime.CompilerServices.Extension>
    Public Function SetEndDirSep(s As String) As String
        If s.EndsWithDirSep() Then
            Return s
        Else
            Return s & Path.DirectorySeparatorChar
        End If
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function TrimStartDirSep(s As String) As String
        Return s.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    End Function
    <System.Runtime.CompilerServices.Extension>
    Public Function TrimEndDirSep(s As String) As String
        Return s.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    End Function
    <System.Runtime.CompilerServices.Extension>
    Public Function EndsWithDirSep(str As String) As Boolean
        If str.Length = 0 Then
            Return False
        End If
        Dim lastChar As Char = str(str.Length - 1)
        Return lastChar = Path.DirectorySeparatorChar OrElse lastChar = Path.AltDirectorySeparatorChar
    End Function
    <System.Runtime.CompilerServices.Extension>
    Public Function StartsWithDirSep(str As String) As Boolean
        If str.Length = 0 Then
            Return False
        End If
        Dim firstChar As Char = str(0)
        Return firstChar = Path.DirectorySeparatorChar OrElse firstChar = Path.AltDirectorySeparatorChar
    End Function
    <System.Runtime.CompilerServices.Extension>
    Public Function WildcardMatch(str As String, wildcompare As String, ignoreCase As Boolean) As Boolean
        If ignoreCase Then
            Return str.ToLower().WildcardMatch(wildcompare.ToLower())
        Else
            Return str.WildcardMatch(wildcompare)
        End If
    End Function

    ''' <summary>Check if <paramref name="str"/> only contains Ascii 8 bit characters.</summary>
    <System.Runtime.CompilerServices.Extension>
    Public Function IsAscii(str As String) As Boolean
        For Each ch As Char In str
            If AscW(ch) > &HFF Then
                Return False
            End If
        Next
        Return True
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function WildcardMatch(str As String, wildcompare As String) As Boolean
        If String.IsNullOrEmpty(wildcompare) Then
            Return str.Length = 0
        End If

        ' workaround: *.* should get all
        wildcompare = wildcompare.Replace("*.*", "*")

        Dim pS As Integer = 0
        Dim pW As Integer = 0
        Dim lS As Integer = str.Length
        Dim lW As Integer = wildcompare.Length

        While pS < lS AndAlso pW < lW AndAlso wildcompare(pW) <> "*"c
            Dim wild As Char = wildcompare(pW)
            If wild <> "?"c AndAlso wild <> str(pS) Then
                Return False
            End If
            pW += 1
            pS += 1
        End While

        Dim pSm As Integer = 0
        Dim pWm As Integer = 0
        While pS < lS AndAlso pW < lW
            Dim wild As Char = wildcompare(pW)
            If wild = "*"c Then
                pW += 1
                If pW = lW Then
                    Return True
                End If
                pWm = pW
                pSm = pS + 1
            ElseIf wild = "?"c OrElse wild = str(pS) Then
                pW += 1
                pS += 1
            Else
                pW = pWm
                pS = pSm
                pSm += 1
            End If
        End While
        While pW < lW AndAlso wildcompare(pW) = "*"c
            pW += 1
        End While
        Return pW = lW AndAlso pS = lS
    End Function
End Module

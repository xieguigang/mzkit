Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Lipidomics

    Public Enum LipidClassAbbreviation

        <Description("Sphingosine")> SPH

    End Enum

    Public Enum LipidMAPSAlias

        <Description("CAR")> AcCa

    End Enum

    Public Module AliasNameMapper

        ''' <summary>
        ''' this function will make a value copy of the given <paramref name="lipidsearch"/> and
        ''' then try to alias mapping of the lipid class name.
        ''' </summary>
        ''' <param name="lipidsearch"></param>
        ''' <returns></returns>
        Public Function LipidSearchToLipidMaps(lipidsearch As LipidName) As LipidName
            Static aliasMap As Dictionary(Of String, String) = Enums(Of LipidMAPSAlias)() _
                .ToDictionary(Function(f) f.ToString.ToLower,
                              Function(f)
                                  Return f.Description
                              End Function)

            lipidsearch = New LipidName(lipidsearch)

            If aliasMap.ContainsKey(lipidsearch.className.ToLower) Then
                lipidsearch.className = aliasMap(lipidsearch.className.ToLower)
            End If

            Return lipidsearch
        End Function

    End Module

    Public Module AbbreviationNameMapper

        ReadOnly mapping As Dictionary(Of String, String) = Enums(Of LipidClassAbbreviation) _
            .ToDictionary(Function(f) f.ToString.ToLower,
                            Function(f)
                                Return f.Description
                            End Function)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckFullName(abbr As String) As Boolean
            Return mapping.ContainsKey(abbr.ToLower)
        End Function

        Public Function ToFullName(abbr As String) As String
            Dim lcase As String = abbr.ToLower

            If mapping.ContainsKey(lcase) Then
                Return mapping(lcase)
            Else
                Return abbr
            End If
        End Function

        Public Function GetAbbreviationName(full As String) As String
            Static mapping As Dictionary(Of String, String) = Enums(Of LipidClassAbbreviation) _
                .ToDictionary(Function(f) f.Description.ToLower,
                              Function(f)
                                  Return f.ToString
                              End Function)
            Dim lcase As String = full.ToLower

            If mapping.ContainsKey(lcase) Then
                Return mapping(lcase)
            Else
                Return full
            End If
        End Function

    End Module
End Namespace
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace TMIC.HMDB.Spectra

    Public Module Extensions

        <Extension>
        Public Iterator Function PopulateSpectras(repository As String) As IEnumerable(Of NamedValue(Of SpectraFile))
            For Each file As String In repository.EnumerateFiles("*.xml")
                Dim header = file.IterateAllLines _
                                 .Take(2) _
                                 .Last
                Dim rootNodeName$ = header _
                    .Match("[<]\S+") _
                    .Trim("<"c) _
                    .ToLower

                Select Case rootNodeName
                    Case "c-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of CMS))
                    Case "ms-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of MSMS))
                    Case "ei-ms" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of EIMS))
                    Case "nmr-one-d" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of NMR1D))
                    Case "nmr-two-d" : Yield New NamedValue(Of SpectraFile)(file.FileName, file.LoadXml(Of NMR2D))
                    Case Else
                        Throw New NotImplementedException($"{rootNodeName} @ {file}")
                End Select
            Next
        End Function

        Public Function ParseIonizationMode(mode As String) As IonizationModes
            Select Case Strings.LCase(mode)
                Case "positive", "pos", "+", "1", "+1", "p"
                    Return IonizationModes.Positive
                Case "negative", "neg", "-", "-1", "n"
                    Return IonizationModes.Negative
                Case Else
                    Throw New NotImplementedException(mode)
            End Select
        End Function
    End Module

    Public Enum IonizationModes As Integer
        Negative = -1
        Positive = 1
    End Enum
End Namespace

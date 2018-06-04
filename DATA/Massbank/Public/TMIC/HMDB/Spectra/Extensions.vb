Namespace TMIC.HMDB.Spectra

    Public Module Extensions

        Public Iterator Function PopulateSpectras(repository As String) As IEnumerable(Of SpectraFile)
            For Each file As String In repository.EnumerateFiles("*.xml")
                Dim header = file.IterateAllLines _
                                 .Take(2) _
                                 .Last
                Dim rootNodeName$ = header _
                    .Match("[<]\S+") _
                    .Trim("<"c) _
                    .ToLower

                Select Case rootNodeName
                    Case "c-ms" : Yield file.LoadXml(Of CMS)
                    Case "ms-ms" : Yield file.LoadXml(Of MSMS)
                    Case "ei-ms" : Yield file.LoadXml(Of EIMS)
                    Case "nmr-one-d" : Yield file.LoadXml(Of NMR1D)
                    Case "nmr-two-d" : Yield file.LoadXml(Of NMR2D)
                    Case Else
                        Throw New NotImplementedException($"{rootNodeName} @ {file}")
                End Select
            Next
        End Function
    End Module
End Namespace

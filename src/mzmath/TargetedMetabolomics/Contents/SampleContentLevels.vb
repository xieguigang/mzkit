Namespace Content

    Public Class SampleContentLevels

        ReadOnly levels As Dictionary(Of String, Double)

        Default Public ReadOnly Property Content(sampleLevel As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return levels(levelKey(sampleLevel))
            End Get
        End Property

        Sub New(levels As Dictionary(Of String, Double))
            levels = levels _
                .ToDictionary(Function(L) levelKey(L.Key),
                              Function(L)
                                  Return L.Value
                              End Function)
        End Sub

        Private Function levelKey(sampleLevel As String) As String
            Return "L" & sampleLevel.Match("\d+").ParseInteger
        End Function

    End Class
End Namespace
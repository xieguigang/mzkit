Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Content

    Public Class SampleContentLevels

        ReadOnly levels As Dictionary(Of String, Double)
        ReadOnly directMap As Boolean

        Default Public ReadOnly Property Content(sampleLevel As String) As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return levels(If(directMap, sampleLevel, levelKey(sampleLevel)))
            End Get
        End Property

        Sub New(levels As Dictionary(Of String, Double), Optional directMap As Boolean = False)
            Me.directMap = directMap
            Me.levels = levels _
                .ToDictionary(Function(L) If(directMap, L.Key, levelKey(L.Key)),
                              Function(L)
                                  Return L.Value
                              End Function)
        End Sub

        Private Function levelKey(sampleLevel As String) As String
            Return "L" & sampleLevel.Match("\d+").ParseInteger
        End Function

        Public Overrides Function ToString() As String
            Return levels.GetJson
        End Function

    End Class
End Namespace
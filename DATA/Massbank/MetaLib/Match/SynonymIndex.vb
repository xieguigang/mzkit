Imports Microsoft.VisualBasic.Data.Trinity

Namespace MetaLib

    Public Interface ICompoundNames

        Function GetSynonym() As IEnumerable(Of String)

    End Interface

    Public Class SynonymIndex(Of T As ICompoundNames)

        ReadOnly bin As WordSimilarityIndex(Of T)

        Sub New(Optional equalsName As Double = 0.9)
            bin = New WordSimilarityIndex(Of T)(New WordSimilarity(equalsName))
        End Sub

        Public Function BuildIndex(compounds As IEnumerable(Of T)) As SynonymIndex(Of T)
            For Each compound As T In compounds
                For Each name As String In compound.GetSynonym
                    Call bin.AddTerm(name, compound)
                Next
            Next

            Return Me
        End Function

        Public Function FindCandidateCompounds(name As String) As IEnumerable(Of T)
            Return bin.FindMatches(name)
        End Function
    End Class
End Namespace
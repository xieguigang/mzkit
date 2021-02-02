Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative

Namespace Content

    Public Class ContentTable

        ''' <summary>
        ''' [ion -> [sample, content]]
        ''' </summary>
        ReadOnly matrix As Dictionary(Of String, SampleContentLevels)
        ''' <summary>
        ''' [ion -> standards]
        ''' </summary>
        ReadOnly standards As Dictionary(Of String, Standards)
        ReadOnly [IS] As Dictionary(Of String, [IS])

        Default Public ReadOnly Property Content(sampleLevel As String, ion As String) As Double
            Get
                Return matrix(ion)(sampleLevel)
            End Get
        End Property

        Public Function hasDefined(ion As String) As Boolean
            Return standards.ContainsKey(ion)
        End Function

        Public Function GetIS(ion As String) As [IS]
            Return [IS].TryGetValue(ion, [default]:=New [IS])
        End Function

        Public Function GetStandards(ion As String) As Standards
            Return standards(ion)
        End Function
    End Class
End Namespace
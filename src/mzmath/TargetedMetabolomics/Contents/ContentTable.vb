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

        Default Public ReadOnly Property Content(sampleLevel As String, ion As String) As Double
            Get
                Return matrix(ion)(sampleLevel)
            End Get
        End Property

        Public Function GetStandards(ion As String) As Standards
            Return standards(ion)
        End Function
    End Class

    Public Class SampleContentLevels

        ReadOnly levels As Dictionary(Of String, Double)

        Default Public ReadOnly Property Content(sampleLevel As String) As Double
            Get
                Return levels(sampleLevel)
            End Get
        End Property

    End Class
End Namespace
Namespace Content

    Public Class SolutionChemical

        ''' <summary>
        ''' the name of the chemical in target solution
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' concentration content of this chemical in target solution
        ''' </summary>
        ''' <returns></returns>
        Public Property content As Double
        ''' <summary>
        ''' the type of the concentration type
        ''' </summary>
        ''' <returns></returns>
        Public Property type As ConcentrationType

        Public Property mass As Double

        Public Overrides Function ToString() As String
            Return $"{name} {mass.ToString("F4")}g ({content} {type.Description})"
        End Function

    End Class
End Namespace
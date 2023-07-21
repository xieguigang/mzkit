Namespace PackLib.Validation

    Public Class DataSetParameters

        Public Property Ions As Integer
        Public Property RawFiles As Integer
        Public Property AverageNumberOfSpectrum As Integer
        Public Property rtmin As Double
        Public Property rtmax As Double

        ''' <summary>
        ''' The raw file base name
        ''' </summary>
        ''' <returns></returns>
        Public Property rawname As String
        Public Property IdRange As String()

    End Class
End Namespace
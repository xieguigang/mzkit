''' <summary>
''' The chromatogram signal ticks
''' </summary>
Public Class ChromatogramTick

    ''' <summary>
    ''' The signal tick time in second
    ''' </summary>
    ''' <returns></returns>
    Public Property Time As Double
    ''' <summary>
    ''' number of detector counts
    ''' </summary>
    ''' <returns></returns>
    Public Property Intensity As Double

    Sub New()
    End Sub

    Sub New(time#, into#)
        Me.Time = time
        Me.Intensity = into
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Intensity}@{Time}s"
    End Function
End Class

''' <summary>
''' Model of the GCxGC 2d peak
''' </summary>
Public Class Peak2D

    ''' <summary>
    ''' the unique reference id of current peak data
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String
    ''' <summary>
    ''' the rt of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rt1 As Double
    ''' <summary>
    ''' the rt of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rt2 As Double
    ''' <summary>
    ''' the rt range of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmin1 As Double
    ''' <summary>
    ''' the rt range of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmax1 As Double
    ''' <summary>
    ''' the rt range of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmin2 As Double
    ''' <summary>
    ''' the rt range of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmax2 As Double
    ''' <summary>
    ''' the volumn data of current peak shape, the metabolite expression value
    ''' </summary>
    ''' <returns></returns>
    Public Property volumn As Double
    ''' <summary>
    ''' the max intensity of current peak object
    ''' </summary>
    ''' <returns></returns>
    Public Property maxIntensity As Double
    Public Property sn As Double

    Public Overrides Function ToString() As String
        Return id
    End Function

End Class

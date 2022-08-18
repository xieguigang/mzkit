Public Class Cell

    ''' <summary>
    ''' the location X of the grid
    ''' </summary>
    ''' <returns></returns>
    Public Property X As Integer
    ''' <summary>
    ''' the location Y of the grid
    ''' </summary>
    ''' <returns></returns>
    Public Property Y As Integer
    ''' <summary>
    ''' average value of Red channel
    ''' </summary>
    ''' <returns></returns>
    Public Property R As Double
    ''' <summary>
    ''' average value of Green channel
    ''' </summary>
    ''' <returns></returns>
    Public Property G As Double
    ''' <summary>
    ''' average value of Blue channel
    ''' </summary>
    ''' <returns></returns>
    Public Property B As Double
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property Black As [Object]

End Class

Public Class [Object]

    Public Property Pixels As Integer
    Public Property Density As Double

End Class
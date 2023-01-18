Imports System.Drawing

''' <summary>
''' 3d scatter data point, a spatial spot or a single cell data
''' </summary>
Public Class UMAPPoint

    ''' <summary>
    ''' the spatial point of current spot if it is the sptial data, value
    ''' of this property is empty for the single cell data
    ''' </summary>
    ''' <returns></returns>
    Public Property Pixel As Point
    ''' <summary>
    ''' the cell label of current spot
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' this property value may be nothing for the sptial data, 
    ''' label value should not be nothing if the data is single 
    ''' cell data.
    ''' </remarks>
    Public Property label As String
    Public Property x As Double
    Public Property y As Double
    Public Property z As Double
    ''' <summary>
    ''' the cell cluster data
    ''' </summary>
    ''' <returns></returns>
    Public Property [class] As Integer

End Class

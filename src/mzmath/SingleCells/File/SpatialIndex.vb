Imports Microsoft.VisualBasic.Imaging

''' <summary>
''' the binary file offset of a spatial spot
''' </summary>
Public Structure SpatialIndex : Implements IPoint3D

    ''' <summary>
    ''' spatial z-axis
    ''' </summary>
    ''' <returns></returns>
    Public Property Z As Integer Implements IPoint3D.Z
    ''' <summary>
    ''' spatial x-axis
    ''' </summary>
    ''' <returns></returns>
    Public Property X As Integer Implements RasterPixel.X
    ''' <summary>
    ''' spatial y-axis
    ''' </summary>
    ''' <returns></returns>
    Public Property Y As Integer Implements RasterPixel.Y

    ''' <summary>
    ''' the spot point offset in the binary data file
    ''' </summary>
    Dim offset As Long

    Sub New(x As Integer, y As Integer, z As Integer, offset As Long)
        Me.X = x
        Me.Y = y
        Me.Z = z
        Me.offset = offset
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{X},{Y},{Z}]=&{StringFormats.Lanudry(bytes:=offset)}"
    End Function

    ''' <summary>
    ''' calculate the spot data offset in the binary file
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="offset"></param>
    ''' <returns></returns>
    Public Shared Operator +(index As SpatialIndex, offset As Integer) As Long
        Return index.offset + offset
    End Operator

End Structure

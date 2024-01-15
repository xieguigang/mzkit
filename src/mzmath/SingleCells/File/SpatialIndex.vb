Imports Microsoft.VisualBasic.Imaging

Public Structure SpatialIndex : Implements IPoint3D

    Public Property Z As Integer Implements IPoint3D.Z
    Public Property X As Integer Implements RasterPixel.X
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

End Structure

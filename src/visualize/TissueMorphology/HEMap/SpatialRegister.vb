Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF

Public Class SpatialRegister

    Public Property HEstain As Image
    Public Property mappings As SpotMap()
    Public Property offset As PointF
    Public Property rotation As Double
    Public Property mirror As Boolean
    Public Property label As String
    Public Property spotColor As String
    Public Property viewSize As Size
    Public Property MSIscale As Size

    Public Sub Save(file As Stream)
        Using buf As New CDFWriter(file)
            Call Save(buf)
        End Using
    End Sub

    Private Sub Save(buf As CDFWriter)

    End Sub
End Class

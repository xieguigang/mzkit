Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector

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
        Dim view_size As New attribute("view_size", $"{viewSize.Width},{viewSize.Height}")
        Dim msi_scale As New attribute("msi_scale", $"{MSIscale.Width},{MSIscale.Height}")
        Dim spot_color As New attribute("spot_color", spotColor)
        Dim label As New attribute("label", _label)
        Dim mirror As New attribute("mirror", _mirror.ToString.ToLower)
        Dim rotation As New attribute("rotation", _rotation.ToString, CDFDataTypes.FLOAT)
        Dim offset As New attribute("offset", $"{_offset.X},{_offset.Y}")
        Dim spot_number As New attribute("spot_number", mappings.Length.ToString, CDFDataTypes.INT)
        Dim img As New MemoryStream
        Dim mapping_dims As New Dimension("mapping_size", mappings.Length)

        Call HEstain.Save(img, ImageFormat.Png)
        Call buf.GlobalAttributes(view_size, msi_scale, spot_color, label, mirror, rotation, offset, spot_number)

        Call buf.AddVariable("heatmap", New doubles(mappings.Select(Function(si) CDbl(si.heatmap))), mapping_dims)
        Call buf.AddVariable("x", New doubles(mappings.Select(Function(si) si.STX)), mapping_dims)
        Call buf.AddVariable("y", New doubles(mappings.Select(Function(si) si.STY)), mapping_dims)
        Call buf.AddVariable("x0", New integers(mappings.Select(Function(si) si.spotXY(0))), mapping_dims)
        Call buf.AddVariable("y0", New integers(mappings.Select(Function(si) si.spotXY(1))), mapping_dims)
    End Sub
End Class

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports any = Microsoft.VisualBasic.Scripting

Namespace HEMap

    Public Class SpatialRegister

        Public Property HEstain As Image
        Public Property mappings As SpotMap()
        Public Property offset As PointF
        Public Property rotation As Double
        Public Property mirror As Boolean
        Public Property label As String
        Public Property spotColor As String

        ''' <summary>
        ''' the new dimension size of the MSI data
        ''' </summary>
        ''' <returns></returns>
        Public Property viewSize As Size
        Public Property MSIscale As Size

        ''' <summary>
        ''' dimension value of the MSI raw scans size
        ''' </summary>
        ''' <returns></returns>
        Public Property MSIdims As Size

        Public Sub Save(file As Stream)
            Using buf As New CDFWriter(file)
                Call Save(buf)
            End Using
        End Sub

        Public Shared Function ParseFile(file As Stream) As SpatialRegister
            Dim buf As New netCDFReader(file)
            Dim view_size As String = buf!view_size
            Dim msi_scale As String = buf!msi_scale
            Dim spot_color As String = buf!spot_color
            Dim label As String = buf!label
            Dim mirror As String = buf!mirror
            Dim rotation As Single = buf!rotation
            Dim offset As String = buf!offset
            Dim spot_number As Integer = buf!spot_number
            Dim msi_dims As String = buf!msi_dims
            Dim heatmap As doubles = buf.getDataVariable("heatmap")
            Dim x As doubles = buf.getDataVariable("x")
            Dim y As doubles = buf.getDataVariable("y")
            Dim x0 As integers = buf.getDataVariable("x0")
            Dim y0 As integers = buf.getDataVariable("y0")
            Dim img As longs = buf.getDataVariable("img")
            Dim img_size As Size = CStr(buf!img_size).SizeParser
            Dim bitmap As Bitmap = New Bitmap(img_size.Width, img_size.Height)
            Dim image As BitmapBuffer = BitmapBuffer.FromBitmap(bitmap)
            Dim mappings As SpotMap() = heatmap _
                .Select(Function(s, i)
                            Return New SpotMap With {
                                .heatmap = s,
                                .STX = x(i),
                                .STY = y(i),
                                .spotXY = {x0(i), y0(i)},
                                .flag = 1,
                                .physicalXY = .spotXY
                            }
                        End Function) _
                .ToArray

            Call image.WriteARGBStream(img.Select(Function(l) CUInt(l)).ToArray)
            Call image.Dispose()
            Call file.Close()

            Return New SpatialRegister With {
                .label = label,
                .mirror = mirror.ParseBoolean,
                .MSIscale = msi_scale.SizeParser,
                .offset = any.CTypeDynamic(offset, GetType(PointF)),
                .rotation = rotation,
                .spotColor = spot_color,
                .viewSize = view_size.SizeParser,
                .HEstain = bitmap,
                .mappings = mappings,
                .MSIdims = msi_dims.SizeParser
            }
        End Function

        Private Sub Save(buf As CDFWriter)
            Dim view_size As New attribute("view_size", $"{viewSize.Width},{viewSize.Height}")
            Dim msi_scale As New attribute("msi_scale", $"{MSIscale.Width},{MSIscale.Height}")
            Dim spot_color As New attribute("spot_color", spotColor)
            Dim label As New attribute("label", _label)
            Dim mirror As New attribute("mirror", _mirror.ToString.ToLower)
            Dim rotation As New attribute("rotation", _rotation.ToString, CDFDataTypes.FLOAT)
            Dim offset As New attribute("offset", $"{_offset.X},{_offset.Y}")
            Dim spot_number As New attribute("spot_number", mappings.Length.ToString, CDFDataTypes.INT)
            Dim img_size As New attribute("img_size", $"{HEstain.Width},{HEstain.Height}")
            Dim msi_dims As New attribute("msi_dims", $"{MSIdims.Width},{MSIdims.Height}")

            Dim mapping_dims As New Dimension("mapping_size", mappings.Length)
            Dim img As BitmapBuffer = BitmapBuffer.FromBitmap(HEstain)
            Dim stream As UInteger() = img.GetARGBStream
            Dim image_size As New Dimension("HEstain_image", stream.Length)

            Call buf.GlobalAttributes(
                view_size, msi_scale,
                spot_color, label,
                mirror, rotation,
                offset, spot_number,
                img_size,
                msi_dims
            )

            Call buf.AddVariable("heatmap", New doubles(mappings.Select(Function(si) CDbl(si.heatmap))), mapping_dims)
            Call buf.AddVariable("x", New doubles(mappings.Select(Function(si) si.STX)), mapping_dims)
            Call buf.AddVariable("y", New doubles(mappings.Select(Function(si) si.STY)), mapping_dims)
            Call buf.AddVariable("x0", New integers(mappings.Select(Function(si) si.spotXY(0))), mapping_dims)
            Call buf.AddVariable("y0", New integers(mappings.Select(Function(si) si.spotXY(1))), mapping_dims)
            Call buf.AddVariable("img", New longs(stream), image_size)
        End Sub
    End Class
End Namespace
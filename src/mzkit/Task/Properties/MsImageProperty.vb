Imports System.ComponentModel
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging

Public Class MsImageProperty

    <Category("imzML")> Public ReadOnly Property fileSize As String
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Category("imzML")> Public ReadOnly Property scan_y As Integer

    <Category("Render")> Public Property background As Color
    <Category("Render")> <DisplayName("width")> Public Property pixel_width As Integer = 10
    <Category("Render")> <DisplayName("height")> Public Property pixel_height As Integer = 10

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.White
        UUID = render.UUID
        fileSize = StringFormats.Lanudry(render.ibd.size)
    End Sub

End Class

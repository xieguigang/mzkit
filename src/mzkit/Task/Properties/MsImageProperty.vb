Imports System.ComponentModel
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Public Class MsImageProperty

    <Category("imzML")> Public ReadOnly Property fileSize As String
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Category("imzML")> Public ReadOnly Property scan_y As Integer

    <Category("Render")> Public Property background As Color
    <Category("Render")> <DisplayName("width")> Public Property pixel_width As Integer = 10
    <Category("Render")> <DisplayName("height")> Public Property pixel_height As Integer = 10
    <Category("Render")> Public Property threshold As Double = 0.01
    <Category("Render")> Public Property colors As Palettes = Palettes.Office2016
    <Category("Render")> Public Property mapLevels As Integer = 30

    <Category("Pixel M/z Data")> Public Property tolerance As Double = 0.1
    <Category("Pixel M/z Data")> Public Property method As ToleranceMethod = ToleranceMethod.Da

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.White
        UUID = render.UUID
        fileSize = StringFormats.Lanudry(render.ibd.size)
    End Sub

    Public Function GetTolerance() As Tolerance
        If method = ToleranceMethod.Da Then
            Return Ms1.Tolerance.DeltaMass(tolerance)
        Else
            Return Ms1.Tolerance.PPM(tolerance)
        End If
    End Function
End Class

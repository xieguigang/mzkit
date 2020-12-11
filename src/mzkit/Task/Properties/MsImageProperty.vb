Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging

Public Class MsImageProperty

    Public Property width As Integer
    Public Property height As Integer
    Public Property background As Color

    Public ReadOnly Property fileSize As String
    Public ReadOnly Property UUID As String

    Sub New(render As Drawer)
        width = render.dimension.Width
        height = render.dimension.Height
        background = Color.White
        UUID = render.UUID
        fileSize = StringFormats.Lanudry(render.ibd.size)
    End Sub

End Class

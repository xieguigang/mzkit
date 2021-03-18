Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Public Class mzPackReader : Inherits BinaryStreamReader

    Public Sub New(file As String)
        MyBase.New(file)
    End Sub

    Protected Overrides Sub loadIndex()
        MyBase.loadIndex()
    End Sub

    Public Function GetThumbnail() As Bitmap

    End Function
End Class

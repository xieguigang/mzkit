Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Public Class mzPackWriter : Inherits BinaryStreamWriter

    ''' <summary>
    ''' ``[readkey => tempfile]`` 
    ''' </summary>
    ReadOnly scanners As New Dictionary(Of String, String)
    ''' <summary>
    ''' temp file path of the thumbnail image
    ''' </summary>
    Dim thumbnail As String

    Public Sub New(file As String)
        MyBase.New(file)
    End Sub

    Private Sub writeScanners()

    End Sub

    Private Sub writeScannerIndex()

    End Sub

    Private Sub writeThumbnail()
        Dim start As Long = file.Position

        Call file.Write(thumbnail.ReadBinary)
        Call file.Write(start)
        Call file.Flush()
    End Sub

    Protected Overrides Sub writeIndex()
        ' write MS index
        MyBase.writeIndex()

        Call writeScanners()
        Call writeScannerIndex()
        Call writeThumbnail()
    End Sub
End Class

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData

Public Class frmSeeMs

    Dim xml As XmlSeek
    Dim index As FastSeekIndex

    Public Sub LoadRaw(raw As String)
        xml = New XmlSeek(raw).LoadIndex
        index = FastSeekIndex.LoadIndex(xml)
    End Sub

End Class
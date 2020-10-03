Public Class RawFileProperty

    Public Property source As String
    Public Property fileSize As String
    Public Property cacheSize As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public Property ms_scans As Integer
    Public Property msms_scans As Integer

    ReadOnly raw As Raw

    Sub New(raw As Raw)
        source = raw.source.FileName
        fileSize = StringFormats.Lanudry(raw.source.FileLength)
        cacheSize = StringFormats.Lanudry(raw.GetCacheFileSize)
        rtmin = raw.rtmin
        rtmax = raw.rtmax
        ms_scans = raw.scans.TryCount
        msms_scans = raw.GetMs2Scans.Count
    End Sub

    Public Function getRaw() As Raw
        Return raw
    End Function
End Class

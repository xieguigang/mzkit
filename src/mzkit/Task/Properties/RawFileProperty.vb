Public Class RawFileProperty

    Public Property source As String
    Public Property fileSize As String
    Public Property cacheSize As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public Property ms_scans As Integer
    Public Property msms_scans As Integer

    Sub New(raw As Raw)
        source = raw.source.FileName
        fileSize = StringFormats.Lanudry(raw.source.FileLength)
        cacheSize = StringFormats.Lanudry(raw.GetCacheFileSize)
        rtmin = raw.rtmin
        rtmax = raw.rtmax
        ms_scans = raw.scans.TryCount
        msms_scans = raw.GetMs2Scans.Count
    End Sub
End Class

Imports System.ComponentModel

Public Class RawFileProperty

    <Description("the data file its source path.")>
    <Category("Local File")>
    Public Property source As String
    <Description("the data file its file length.")>
    <Category("Local File")>
    Public Property fileSize As String
    <Description("the cache file its source path.")>
    <Category("Local File")>
    Public Property cacheSize As String

    <Description("the rt lower bound of the ions in current data file.")>
    <Category("Data Summary")>
    Public Property rtmin As Double
    <Description("the rt upper bound of the ions in current data file.")>
    <Category("Data Summary")>
    Public Property rtmax As Double

    <Description("the number of the MS1 scans in current data file.")>
    <Category("Data Summary")>
    Public Property ms_scans As Integer
    <Description("the number of the MS/MS scans in current data file.")>
    <Category("Data Summary")>
    Public Property msms_scans As Integer

    ReadOnly raw As Raw

    Sub New(raw As Raw)
        source = raw.source.FileName
        fileSize = StringFormats.Lanudry(raw.source.FileLength)
        cacheSize = StringFormats.Lanudry(raw.GetCacheFileSize)
        rtmin = raw.rtmin
        rtmax = raw.rtmax
        ms_scans = raw.GetMs1Scans.Count
        msms_scans = raw.GetMs2Scans.Count
    End Sub

    Public Function getRaw() As Raw
        Return raw
    End Function
End Class

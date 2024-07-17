Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Public Class XcmsSamplePeak

    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property into As Double
    Public Property intb As Double
    Public Property maxo As Double
    Public Property sn As Double
    Public Property sample As String
    Public Property ID As String

    Public Shared Iterator Function ParseCsv(file As Stream) As IEnumerable(Of XcmsSamplePeak)
        Dim s As New StreamReader(file)
        Dim line As Value(Of String) = s.ReadLine
        Dim headers As Index(Of String) = line.Split(","c)
        Dim mz As Integer = headers!mz
        Dim mzmin As Integer = headers!mzmin
        Dim mzmax As Integer = headers!mzmax
        Dim rt As Integer = headers!rt
        Dim rtmin As Integer = headers!rtmin
        Dim rtmax As Integer = headers!rtmax
        Dim into As Integer = headers!into
        Dim intb As Integer = headers!intb
        Dim maxo As Integer = headers!maxo
        Dim sn As Integer = headers!sn
        Dim sample As Integer = headers!sample
        Dim id As Integer = headers!ID

        Do While Not (line = s.ReadLine) Is Nothing
            Dim t As String() = line.Split(","c)
            Dim peak As New XcmsSamplePeak With {
                .ID = t(id),
                .intb = If(intb = -1, 0, Double.Parse(t(intb))),
                .into = Double.Parse(t(into)),
                .maxo = If(maxo = -1, 0, Double.Parse(t(maxo))),
                .mz = Double.Parse(t(mz)),
                .mzmax = Double.Parse(t(mzmax)),
                .mzmin = Double.Parse(t(mzmin)),
                .rt = Double.Parse(t(rt)),
                .rtmax = Double.Parse(t(rtmax)),
                .rtmin = Double.Parse(t(rtmin)),
                .sample = t(sample),
                .sn = If(sn = -1, 0, Double.Parse(t(sn)))
            }

            Yield peak
        Loop
    End Function

End Class

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Public Module Shimadzu

    Public Function CheckTableHeader(header As String()) As Boolean
        If header(Scan0) <> "X" Then
            Return False
        ElseIf header(1) <> "Y" Then
            Return False
        ElseIf header(2) <> "ROI" Then
            Return False
        ElseIf Not header.Skip(3).All(Function(str) str.IsSimpleNumber) Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function GetFileTag(file As Stream) As String
        If TypeOf file Is FileStream Then
            Return DirectCast(file, FileStream).Name.FileName
        Else
            Return "Shimadzu iMScope TRIO"
        End If
    End Function

    Public Function ImportsMzPack(file As Stream, Optional sample As String = Nothing) As mzPack
        Using buffer As New StreamReader(file)
            Dim headers As String() = buffer.ReadLine.Split(","c)
            Dim mz As Double() = headers _
                .Skip(3) _
                .Select(AddressOf Double.Parse) _
                .ToArray
            Dim line As Value(Of String) = ""
            Dim tokens As String()
            Dim x, y As Integer
            Dim ROI As String
            Dim intensity As Double()
            Dim scans As New List(Of ScanMS1)

            sample = If(sample.StringEmpty, GetFileTag(file), sample)

            Do While (line = buffer.ReadLine) IsNot Nothing
                tokens = line.Split(","c)
                x = Integer.Parse(tokens(0))
                y = Integer.Parse(tokens(1))
                ROI = tokens(2)
                intensity = tokens _
                    .Skip(3) _
                    .Select(AddressOf Double.Parse) _
                    .ToArray

                scans += New ScanMS1 With {
                    .BPC = intensity.Max,
                    .into = intensity,
                    .meta = New Dictionary(Of String, String) From {{"ROI", ROI}, {"x", x}, {"y", y}},
                    .mz = mz,
                    .products = Nothing,
                    .rt = buffer.BaseStream.Position,
                    .TIC = intensity.Sum,
                    .scan_id = $"[MS1][{x},{y}][{sample}] Full Scan, ROI={ROI}, total_ions={ .TIC}"
                }
            Loop

            Return New mzPack With {
                .Application = FileApplicationClass.MSImaging,
                .MS = scans.ToArray,
                .source = sample
            }
        End Using
    End Function
End Module


Imports System.IO
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData

    ''' <summary>
    ''' index work with <see cref="XmlSeek"/>
    ''' </summary>
    Public Class FastSeekIndex : Inherits Chromatogram

        Public Property indexId As String()
        Public Property Ms2Index As Dictionary(Of String, Double)

        Public Shared Function LoadIndex(file As String) As FastSeekIndex
            Select Case XmlSeek.ParseFileType(file)
                Case XmlFileTypes.imzML, XmlFileTypes.mzML
                Case XmlFileTypes.mzXML
                    Return LoadIndex_mzXML(file)
                Case Else
                    Throw New NotImplementedException(file)
            End Select
        End Function

        Public Shared Function LoadIndex_mzXML(file As String) As FastSeekIndex
            Dim stream = Iterator Function() As IEnumerable(Of String)
                             Using text As StreamReader = file.OpenReader
                                 Do While Not text.EndOfStream
                                     Yield text.ReadLine
                                 Loop
                             End Using
                         End Function

            Dim scan_time As New List(Of Double)
            Dim TIC As New List(Of Double)
            Dim BPC As New List(Of Double)
            Dim keys As New List(Of String)
            Dim Ms2Time As New Dictionary(Of String, Double)

            Dim numRegexp As New Regex("num[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim msLevelRegexp As New Regex("msLevel[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim timeRegexp As New Regex("retentionTime[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim BPCRegexp As New Regex("basePeakIntensity[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim TICRegexp As New Regex("totIonCurrent[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)

            For Each block As String In NodeIterator.CreateBlockReader("scan")(stream())
                Dim idKey As String = numRegexp.Match(block).Value
                Dim level As String = msLevelRegexp.Match(block).Value.GetTagValue("=").Value
                Dim time As String = timeRegexp.Match(block).Value.GetTagValue("=").Value

                If level = """1""" Then

                Else

                End If
            Next

            Return New FastSeekIndex With {
                .BPC = BPC.ToArray,
                .scan_time = scan_time.ToArray,
                .TIC = TIC.ToArray,
                .indexId = keys.ToArray,
                .Ms2Index = Ms2Time
            }
        End Function
    End Class
End Namespace
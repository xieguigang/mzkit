
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports chromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram
Imports indexList = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.indexList

Namespace MarkupData

    ''' <summary>
    ''' index work with <see cref="XmlSeek"/>
    ''' </summary>
    Public Class FastSeekIndex : Inherits DataReader.Chromatogram

        Public Property indexId As String()
        Public Property Ms2Index As Dictionary(Of String, Double)
        Public Property fileName As String

        Public Shared Function LoadIndex(file As String) As FastSeekIndex
            Select Case XmlSeek.ParseFileType(file)
                Case XmlFileTypes.imzML, XmlFileTypes.mzML
                    Return LoadIndex_mzML(New XmlSeek(file).LoadIndex)
                Case XmlFileTypes.mzXML
                    Return LoadIndex_mzXML(New XmlSeek(file).LoadIndex)
                Case Else
                    Throw New NotImplementedException(file)
            End Select
        End Function

        Public Shared Function LoadIndex_mzML(xml As XmlSeek) As FastSeekIndex
            Dim offsets As NamedValue(Of Long)() = xml.TryGetOffsets("scan")

            Using buffer As Stream = File.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Dim type As XmlFileTypes = XmlSeek.ParseFileType(File)
                Dim indexOffset As Long = XmlSeek.parseIndex(buffer, type, Encoding.UTF8).indexOffset
                Dim index As indexList = indexList.ParseIndexList(buffer, indexOffset)
                Dim offset1 As Long = index.chromatogram.FindOffSet("TIC")
                Dim offset2 As Long = index.chromatogram.FindOffSet("BPC")
                Dim TIC As chromatogram = XmlParser.ParseDataNode(Of chromatogram)(New StreamReader(buffer), offset1, "chromatogram")
                Dim BPC As chromatogram = XmlParser.ParseDataNode(Of chromatogram)(New StreamReader(buffer), offset2, "chromatogram")
                Dim TICvec = TIC.Ticks
                Dim scan_time As Double() = TICvec.Select(Function(t) t.Time).ToArray
                Dim indexId As String() = index.spectrum.offsets.Select(Function(a) a.idRef).ToArray

                Return New FastSeekIndex With {
                    .fileName = File.GetFullPath,
                    .scan_time = scan_time,
                    .TIC = TICvec.Select(Function(t) t.Intensity).ToArray,
                    .BPC = If(BPC Is Nothing, Nothing, BPC.Ticks.Select(Function(t) t.Intensity).ToArray),
                    .indexId = indexId
                }
            End Using
        End Function

        Const AttributeValueMask As String = """ "

        Public Shared Function LoadIndex_mzXML(xml As XmlSeek) As FastSeekIndex
            Dim offsets As NamedValue(Of Long)() = xml.TryGetOffsets("scan")
            Dim scan_time As New List(Of Double)
            Dim TIC As New List(Of Double)
            Dim BPC As New List(Of Double)
            Dim keys As New List(Of String)
            Dim Ms2Time As New Dictionary(Of String, Double)

            Dim msLevelRegexp As New Regex("msLevel[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim timeRegexp As New Regex("retentionTime[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim BPCRegexp As New Regex("basePeakIntensity[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)
            Dim TICRegexp As New Regex("totIonCurrent[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)

            Dim level As String = Nothing
            Dim time As Double
            Dim TICval, BPCval As Double

            Dim read As New ScanReadStatus
            Dim match As New Value(Of Match)

            For Each offset As NamedValue(Of Long) In offsets
                Call read.Reset()

                For Each line As String In xml.parser.GotoReadText(offset.Value)
                    If Not read.level AndAlso (match = msLevelRegexp.Match(line)).Success Then
                        level = msLevelRegexp.Match(line).Value.GetTagValue("=").Value
                    ElseIf Not read.time AndAlso (match = timeRegexp.Match(line)).Success Then
                        time = PeakMs2.RtInSecond(timeRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                    ElseIf read.level AndAlso read.time Then
                        If level = """1""" Then
                            If Not read.TIC AndAlso (match = TICRegexp.Match(line)).Success Then
                                TICval = Double.Parse(TICRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                            ElseIf Not read.BPC AndAlso (match = BPCRegexp.Match(line)).Success Then
                                BPCval = Double.Parse(BPCRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                            End If

                            If read.TIC AndAlso read.BPC Then
                                scan_time.Add(time)
                                TIC.Add(TICval)
                                BPC.Add(BPCval)

                                Exit For
                            End If
                        Else
                            Call Ms2Time.Add(offset.Name, time)

                            Exit For
                        End If
                    End If
                Next
            Next

            Return New FastSeekIndex With {
                .BPC = BPC.ToArray,
                .scan_time = scan_time.ToArray,
                .TIC = TIC.ToArray,
                .indexId = keys.ToArray,
                .Ms2Index = Ms2Time,
                .fileName = xml.fileName
            }
        End Function

        Private Class ScanReadStatus

            Public level As Boolean = False
            Public time As Boolean = False
            Public TIC As Boolean = False
            Public BPC As Boolean = False

            Public Sub Reset()
                level = False
                time = False
                TIC = False
                BPC = False
            End Sub

        End Class
    End Class
End Namespace
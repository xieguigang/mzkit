#Region "Microsoft.VisualBasic::70a4af574c18a66f4915cc17ee5d1085, E:/mzkit/src/assembly/assembly//MarkupData/FastSeekIndex.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 230
    '    Code Lines: 173
    ' Comment Lines: 23
    '   Blank Lines: 34
    '     File Size: 10.32 KB


    '     Class FastSeekIndex
    ' 
    '         Properties: fileName, indexId, Ms2Index
    ' 
    '         Function: (+2 Overloads) LoadIndex, LoadIndex_mzML, LoadIndex_mzXML
    '         Class ScanReadStatus
    ' 
    '             Function: ToString
    ' 
    '             Sub: Reset
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace MarkupData

    ''' <summary>
    ''' index work with <see cref="XmlSeek"/>
    ''' </summary>
    ''' <remarks>
    ''' 这个模块主要是为了桌面软件可以尽可能快的加载原始数据文件的索引信息
    ''' 而额外的基于最基础的字符串操作编写的
    ''' </remarks>
    Public Class FastSeekIndex : Inherits Chromatogram

        Public Property indexId As String()
        Public Property Ms2Index As Dictionary(Of String, Double)
        Public Property fileName As String

        ''' <summary>
        ''' load raw data file auto
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
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

        ''' <summary>
        ''' load raw data file auto
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Function LoadIndex(file As XmlSeek) As FastSeekIndex
            Select Case file.type
                Case XmlFileTypes.imzML, XmlFileTypes.mzML
                    Return LoadIndex_mzML(file)
                Case XmlFileTypes.mzXML
                    Return LoadIndex_mzXML(file)
                Case Else
                    Throw New NotImplementedException(file.type.Description)
            End Select
        End Function

        Public Shared Function LoadIndex_mzML(xml As XmlSeek) As FastSeekIndex
            Dim offsets As NamedValue(Of Long)() = xml.TryGetOffsets("spectrum")
            Dim scan_time As New List(Of Double)
            Dim TIC As New List(Of Double)
            Dim BPC As New List(Of Double)
            Dim keys As New List(Of String)
            Dim Ms2Time As New Dictionary(Of String, Double)

            Dim valueRegexp As New Regex("value[=]"".+?""", RegexOptions.Singleline Or RegexOptions.Compiled)

            Const msLevelKey As String = "name=""ms level"""
            Const BPCKey As String = "name=""base peak intensity"""
            Const TICKey As String = "name=""total ion current"""
            Const timeKey As String = "name=""scan start time"""

            Dim level As String = Nothing
            Dim time As Double
            Dim TICval, BPCval As Double

            Dim read As New ScanReadStatus

            For Each offset As NamedValue(Of Long) In offsets
                Call read.Reset()

                For Each line As String In xml.parser.GotoReadText(offset.Value)
                    If Not read.level AndAlso line.Contains(msLevelKey) Then
                        level = valueRegexp.Match(line).Value.GetTagValue("=").Value
                        read.level = True
                    ElseIf read.level Then
                        If level = """1""" Then
                            If Not read.TIC AndAlso line.Contains(TICKey) Then
                                TICval = Double.Parse(valueRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                read.TIC = True
                            ElseIf Not read.BPC AndAlso line.Contains(BPCKey) Then
                                BPCval = Double.Parse(valueRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                read.BPC = True
                            ElseIf Not read.time AndAlso line.Contains(timeKey) Then
                                time = Double.Parse(valueRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                read.time = True
                            End If

                            If read.TIC AndAlso read.BPC AndAlso read.time Then
                                scan_time.Add(time)
                                TIC.Add(TICval)
                                BPC.Add(BPCval)

                                Exit For
                            End If
                        Else
                            If Not read.time AndAlso line.Contains(timeKey) Then
                                time = Double.Parse(valueRegexp.Match(line).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                Ms2Time.Add(offset.Name, time)

                                Exit For
                            End If
                        End If
                    End If
                Next
            Next

            Return New FastSeekIndex With {
                .BPC = BPC.ToArray,
                .scan_time = scan_time.ToArray,
                .TIC = TIC.ToArray,
                .indexId = offsets.Select(Function(o) o.Name).ToArray,
                .Ms2Index = Ms2Time,
                .fileName = xml.fileName
            }
        End Function

        Const AttributeValueMask As String = """ "

        Public Shared Function LoadIndex_mzXML(xml As XmlSeek) As FastSeekIndex
            Dim offsets As NamedValue(Of Long)() = xml.TryGetOffsets("scan")
            Dim scan_time As New List(Of Double)
            Dim TIC As New List(Of Double)
            Dim BPC As New List(Of Double)
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
                        level = CType(match, Match).Value.GetTagValue("=").Value
                        read.level = True
                    ElseIf Not read.time AndAlso (match = timeRegexp.Match(line)).Success Then
                        time = PeakMs2.RtInSecond(CType(match, Match).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                        read.time = True
                    ElseIf read.level AndAlso read.time Then
                        If level = """1""" Then
                            If Not read.TIC AndAlso (match = TICRegexp.Match(line)).Success Then
                                TICval = Double.Parse(CType(match, Match).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                read.TIC = True
                            ElseIf Not read.BPC AndAlso (match = BPCRegexp.Match(line)).Success Then
                                BPCval = Double.Parse(CType(match, Match).Value.GetTagValue("=", trim:=AttributeValueMask).Value)
                                read.BPC = True
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
                .indexId = offsets.Select(Function(o) o.Name).ToArray,
                .Ms2Index = Ms2Time,
                .fileName = xml.fileName
            }
        End Function

        ''' <summary>
        ''' data read status, false means not read yet, and true means data have been already read.
        ''' </summary>
        Private Class ScanReadStatus

            Public level As Boolean = False
            Public time As Boolean = False
            Public TIC As Boolean = False
            Public BPC As Boolean = False

            ''' <summary>
            ''' reset all status to ``not read``
            ''' </summary>
            Public Sub Reset()
                level = False
                time = False
                TIC = False
                BPC = False
            End Sub

            Public Overrides Function ToString() As String
                If level AndAlso time AndAlso TIC AndAlso BPC Then
                    Return "read all data"
                ElseIf (Not level) AndAlso (Not time) AndAlso (Not TIC) AndAlso (Not BPC) Then
                    Return "read none"
                Else
                    Dim done As New List(Of String)

                    If level Then done.Add(NameOf(level))
                    If time Then done.Add(NameOf(time))
                    If BPC Then done.Add(NameOf(BPC))
                    If TIC Then done.Add(NameOf(TIC))

                    Return "read: " & done.JoinBy(", ")
                End If
            End Function

        End Class
    End Class
End Namespace

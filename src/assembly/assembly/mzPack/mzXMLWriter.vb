#Region "Microsoft.VisualBasic::f8007484bdf61359444ad1f93bca602e, E:/mzkit/src/assembly/assembly//mzPack/mzXMLWriter.vb"

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

    '   Total Lines: 270
    '    Code Lines: 212
    ' Comment Lines: 14
    '   Blank Lines: 44
    '     File Size: 11.06 KB


    '     Class mzXMLWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: encode
    ' 
    '         Sub: (+2 Overloads) Dispose, println, WriteData, WriteHeader, WriteIndex
    '              (+2 Overloads) writeScan, WriteSha1
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

Namespace MarkupData.mzXML

    Public Class mzXMLWriter : Implements IDisposable

        ReadOnly mzXML As BinaryDataWriter
        ReadOnly parentFiles As parentFile()
        ReadOnly msInstruments As msInstrument()
        ReadOnly dataProcessings As dataProcessing()
        ReadOnly scanOffsets As New Dictionary(Of String, Long)

        Dim disposedValue As Boolean

        Sub New(parentFiles As IEnumerable(Of parentFile),
                msInstruments As IEnumerable(Of msInstrument),
                dataProcessings As IEnumerable(Of dataProcessing),
                mzXML As Stream)

            Me.mzXML = New BinaryDataWriter(mzXML) With {
                .ByteOrder = ByteOrder.BigEndian,
                .Encoding = Encodings.UTF8WithoutBOM.CodePage
            }
            Me.parentFiles = parentFiles.ToArray
            Me.msInstruments = msInstruments.ToArray
            Me.dataProcessings = dataProcessings.ToArray

            ' ISO-8859-1
            Call println("
<?xml version=""1.0"" encoding=""utf8""?>
<mzXML xmlns=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2""
       xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
       xsi:schemaLocation=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2 http://sashimi.sourceforge.net/schema_revision/mzXML_3.2/mzXML_idx_3.2.xsd"">
")
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub println(line As String)
            Call mzXML.Write(line, BinaryStringFormat.NoPrefixOrTermination)
            Call mzXML.Write(vbLf, BinaryStringFormat.NoPrefixOrTermination)
            Call mzXML.Flush()
        End Sub

        Private Sub WriteHeader(scanCount As Integer, startTime As Double, endTime As Double)
            Call println($"<msRun scanCount=""{scanCount}"" startTime=""PT{startTime}S"" endTime=""PT{endTime}S"">")

            For Each file As parentFile In parentFiles
                Call println($"<parentFile fileName=""{file.fileName}""
                fileType=""{file.fileType}""
                fileSha1=""{file.fileShal}""/>")
            Next

            If msInstruments.IsNullOrEmpty Then
                Call println("
<msInstrument msInstrumentID=""1"">
      <msManufacturer category=""msManufacturer"" value=""Thermo Scientific""/>
      <msModel category=""msModel"" value=""LTQ Orbitrap XL""/>
      <msIonisation category=""msIonisation"" value=""electrospray ionization""/>
      <msMassAnalyzer category=""msMassAnalyzer"" value=""orbitrap""/>
      <msDetector category=""msDetector"" value=""inductive detector""/>
      <software type=""acquisition"" name=""Xcalibur"" version=""2.5.5 SP2""/>
    </msInstrument>
    <msInstrument msInstrumentID=""2"">
      <msManufacturer category=""msManufacturer"" value=""Thermo Scientific""/>
      <msModel category=""msModel"" value=""LTQ Orbitrap XL""/>
      <msIonisation category=""msIonisation"" value=""electrospray ionization""/>
      <msMassAnalyzer category=""msMassAnalyzer"" value=""radial ejection linear ion trap""/>
      <msDetector category=""msDetector"" value=""electron multiplier""/>
      <software type=""acquisition"" name=""Xcalibur"" version=""2.5.5 SP2""/>
    </msInstrument>
")
            Else
                For Each instrument As msInstrument In msInstruments
                    Call println($"<msInstrument msInstrumentID=""{instrument.msInstrumentID}"">
      <msManufacturer category=""msManufacturer"" value=""Thermo Scientific""/>
      <msModel category=""msModel"" value=""LTQ Orbitrap XL""/>
      <msIonisation category=""msIonisation"" value=""electrospray ionization""/>
      <msMassAnalyzer category=""msMassAnalyzer"" value=""orbitrap""/>
      <msDetector category=""msDetector"" value=""inductive detector""/>
      <software type=""acquisition"" name=""Xcalibur"" version=""2.5.5 SP2""/>
    </msInstrument>")
                Next
            End If

            For Each process As dataProcessing In dataProcessings
                Call println($"<dataProcessing>
      <software type=""conversion"" name=""ProteoWizard software"" version=""3.0.9220""/>
      <processingOperation name=""Conversion to mzML""/>
    </dataProcessing>")
            Next
        End Sub

        Public Sub WriteData(mzData As ScanMS1(), Optional print As Action(Of String) = Nothing)
            Dim scanCount As Integer = mzData.Select(Function(si) si.products.Length + 1).Sum

            mzData = mzData _
                .OrderBy(Function(si) si.rt) _
                .ToArray

            Dim startTime As Double = mzData.First.rt
            Dim endTime As Double = mzData.Last.rt
            Dim i As i32 = 1

            Call WriteHeader(scanCount, startTime, endTime)

            For Each scan As ScanMS1 In mzData
                Call writeScan(scan, i)

                For Each ion As ScanMS2 In scan.products
                    Call writeScan(ion, i)
                Next

                Call print(scan.scan_id)
            Next
        End Sub

        Private Sub writeScan(scan As ScanMS1, ByRef scanNum As i32)
            Dim size As Integer = 0
            Dim mzint As String = encode(scan, len:=size)
            Dim i As String = ++scanNum

            Call scanOffsets.Add(i, mzXML.Position)
            Call println($"<scan num=""{i}""
          scanType=""Full""
          centroided=""1""
          msLevel=""1""
          peaksCount=""{scan.size}""
          polarity=""{scan.meta.TryGetValue("polarity", [default]:="+")}""
          retentionTime=""PT{scan.rt}S""
          lowMz=""{scan.mz.Min}""
          highMz=""{scan.mz.Max}""
          basePeakMz=""{scan.mz(which.Max(scan.into))}""
          basePeakIntensity=""{scan.BPC}""
          totIonCurrent=""{scan.TIC}""
          msInstrumentID=""1"">
      <peaks compressionType=""none""
             compressedLen=""{size}""
             precision=""64""
             byteOrder=""network""
             contentType=""m/z-int"">{mzint}</peaks>
    </scan>")
        End Sub

        Private Function encode(msscan As MSScan, ByRef len As Integer) As String
            Dim x As New List(Of Double)

            For i As Integer = 0 To msscan.size - 1
                x.Add(msscan.into(i))
                x.Add(msscan.mz(i))
            Next

            Dim rawBytes As Byte() = x _
                .Select(AddressOf BitConverter.GetBytes) _
                .IteratesALL _
                .ToArray

            If SystemByteOrder = ByteOrder.LittleEndian Then
                rawBytes = rawBytes.Reverse.ToArray
            End If

            len = rawBytes.Length

            'Using buffer As New MemoryStream(rawBytes)
            '    Return buffer.GZipAsBase64(noMagic:=True)
            'End Using
            Return Convert.ToBase64String(rawBytes)
        End Function

        Private Sub writeScan(scan As ScanMS2, ByRef scanNum As i32)
            Dim size As Integer = 0
            Dim mzint As String = encode(scan, len:=size)
            Dim i As String = ++scanNum

            Call scanOffsets.Add(i, mzXML.Position)
            Call println($"<scan num=""{i}""
          scanType=""Full""
          centroided=""1""
          msLevel=""2""
          peaksCount=""{scan.size}""
          polarity=""{If(scan.polarity > 0, "+", "-")}""
          retentionTime=""PT{scan.rt}S""
          collisionEnergy=""{scan.collisionEnergy}""
          lowMz=""{scan.mz.Min}""
          highMz=""{scan.mz.Max}""
          basePeakMz=""{scan.mz(which.Max(scan.into))}""
          basePeakIntensity=""{scan.into.Max}""
          totIonCurrent=""{scan.into.Sum}""
          msInstrumentID=""2"">
      <precursorMz precursorScanNum=""1"" precursorIntensity=""{scan.intensity}"" precursorCharge=""{scan.charge}"" activationMethod=""{scan.activationMethod}"" windowWideness=""2.0"">{scan.parentMz}</precursorMz>
      <peaks compressionType=""none""
             compressedLen=""{size}""
             precision=""64""
             byteOrder=""network""
             contentType=""m/z-int"">{mzint}</peaks>
    </scan>")
        End Sub

        Private Sub WriteSha1()
            Call mzXML.Write("<sha1>", BinaryStringFormat.NoPrefixOrTermination)
            Call mzXML.Flush()

            Dim offset As Long = mzXML.Position

            Call mzXML.Seek(Scan0, SeekOrigin.Begin)

#Disable Warning
            Dim sha As SHA1 = New SHA1Managed()
            Dim sha1 As String = BitConverter.ToString(sha.ComputeHash(mzXML.BaseStream)).Replace("-", "").ToLower
#Enable Warning

            Call mzXML.Seek(offset, SeekOrigin.Begin)
            Call mzXML.Write(sha1, BinaryStringFormat.NoPrefixOrTermination)
            Call println("</sha1>")
            Call println("</mzXML>")
        End Sub

        Private Sub WriteIndex()
            Call println("</msRun>")

            Dim indexOffset As Long = mzXML.Position

            Call println("<index name=""scan"">")

            For Each offset In scanOffsets
                Call println($"<offset id=""{offset.Key}"">{offset.Value}</offset>")
            Next

            Call println("</index>")
            Call println($"<indexOffset>{indexOffset}</indexOffset>")
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call WriteIndex()
                    Call WriteSha1()

                    Call mzXML.Flush()
                    Call mzXML.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace

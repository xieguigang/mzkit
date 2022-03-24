Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Namespace MarkupData.mzXML

    Public Class mzXMLWriter : Implements IDisposable

        ReadOnly mzXML As BinaryDataWriter
        ReadOnly parentFiles As parentFile()
        ReadOnly msInstruments As msInstrument()
        ReadOnly dataProcessings As dataProcessing()

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

            Call println("
<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<mzXML xmlns=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2""
       xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
       xsi:schemaLocation=""http://sashimi.sourceforge.net/schema_revision/mzXML_3.2 http://sashimi.sourceforge.net/schema_revision/mzXML_3.2/mzXML_idx_3.2.xsd"">
")
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub println(line As String)
            Call Me.mzXML.Write(line, BinaryStringFormat.NoPrefixOrTermination)
        End Sub

        Private Sub WriteHeader(scanCount As Integer, startTime As Double, endTime As Double)
            Call println($"<msRun scanCount=""{scanCount}"" startTime=""PT{startTime}S"" endTime=""PT{endTime}S"">")

            For Each file As parentFile In parentFiles
                Call println($"<parentFile fileName=""{file.fileName}""
                fileType=""{file.fileType}""
                fileSha1=""{file.fileShal}""/>")
            Next
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
            For Each process As dataProcessing In dataProcessings
                Call println($"<dataProcessing>
      <software type=""conversion"" name=""ProteoWizard software"" version=""3.0.9220""/>
      <processingOperation name=""Conversion to mzML""/>
    </dataProcessing>")
            Next
        End Sub

        Public Sub WriteData(mzData As ScanMS1())
            Dim scanCount As Integer = mzData.Select(Function(i) i.products.Length + 1).Sum
            Dim startTime As Double = mzData.OrderBy(Function(i) i.rt).First.rt
            Dim endTime As Double = mzData.OrderByDescending(Function(i) i.rt).Last.rt

            Call WriteHeader(scanCount, startTime, endTime)

            For Each scan As ScanMS1 In mzData
                Call writeScan(scan)

                For Each ion As ScanMS2 In scan.products
                    Call writeScan(ion)
                Next
            Next
        End Sub

        Private Sub writeScan(scan As ScanMS1)

        End Sub

        Private Sub writeScan(scan As ScanMS2)

        End Sub

        Private Sub WriteSha1()

        End Sub

        Private Sub WriteIndex()

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
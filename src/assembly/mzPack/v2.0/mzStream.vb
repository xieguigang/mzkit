#Region "Microsoft.VisualBasic::2e04355e4db6919de0595402395642db, assembly\mzPack\v2.0\mzStream.vb"

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

    '   Total Lines: 394
    '    Code Lines: 282 (71.57%)
    ' Comment Lines: 47 (11.93%)
    '    - Xml Docs: 68.09%
    ' 
    '   Blank Lines: 65 (16.50%)
    '     File Size: 14.12 KB


    ' Class mzStream
    ' 
    '     Properties: Application, metadata, MS1, rtmax, SampleScans
    '                 sourceName
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: findScan1Name, GetMetadata, GetSampleTags, GetThumbnail, hasMs2
    '               loadAnnotations, ReadModel, ReadMS1, ReadScan, safeParseClassType
    '               (+2 Overloads) SafeParseClassType
    ' 
    '     Sub: cacheScanIndex, (+2 Overloads) Dispose, ReadChromatogramTick
    ' 
    ' /********************************************************************************/

#End Region

#If UNIX = 0 Then
Imports Microsoft.VisualBasic.ApplicationServices.Application
#End If

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports any = Microsoft.VisualBasic.Scripting
Imports asciiA = Microsoft.VisualBasic.Text.ASCII
Imports std = System.Math

''' <summary>
''' v2 mzPack format in HDS stream file
''' </summary>
Public Class mzStream : Implements IMzPackReader
    Implements IDisposable

    ReadOnly pack As StreamPack
    ''' <summary>
    ''' a global cache of the ms1 scan_id to dir path mapping
    ''' </summary>
    ReadOnly scan_id As New Dictionary(Of String, String)

    Dim disposedValue As Boolean
    Dim meta As Dictionary(Of String, String)
    Dim summary As Dictionary(Of String, Double)
    Dim sampleTags As String()
    Dim ion_annotations As Dictionary(Of String, String)

    Public ReadOnly Property Application As FileApplicationClass

    Public ReadOnly Property sourceName As String Implements IMzPackReader.source
        Get
            Return meta.TryGetValue("source")
        End Get
    End Property

    ''' <summary>
    ''' get the metadata for the mzpack raw data file
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property metadata As IReadOnlyDictionary(Of String, String)
        Get
            Return meta
        End Get
    End Property

    ''' <summary>
    ''' get all ms1 scan id
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MS1 As IEnumerable(Of String) Implements IMzPackReader.EnumerateIndex
        Get
            Return scan_id.Keys
        End Get
    End Property

    Public ReadOnly Property rtmax As Double Implements IMzPackReader.rtmax
        Get
            Return summary!rtmax
        End Get
    End Property

    Public ReadOnly Property SampleScans As Dictionary(Of String, String())
        Get
            Return sampleTags _
                .ToDictionary(Function(tag) tag,
                              Function(tag)
                                  Return scan_id _
                                      .Where(Function(r) r.Value.IndexOf(tag) > -1) _
                                      .Select(Function(a) a.Key) _
                                      .ToArray
                              End Function)
        End Get
    End Property

    Sub New(filepath As String)
        Call Me.New(
            stream:=filepath.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False)
        )
    End Sub

    ''' <summary>
    ''' create a new version 2 mzpack file reader
    ''' </summary>
    ''' <param name="stream"></param>
    Sub New(stream As Stream)
        On Error Resume Next

        pack = New StreamPack(stream, [readonly]:=True)
        Application = safeParseClassType()

        ' read 3 json metadata file

        ' 20221017 there is a possible error?
        ' Incorrect JSON string format
        meta = pack.ReadText("/.etc/metadata.json").LoadJSON(Of Dictionary(Of String, String))
        summary = pack.ReadText("/.etc/ms_scans.json").LoadJSON(Of Dictionary(Of String, Double))
        sampleTags = GetSampleTags(pack)
        ion_annotations = loadAnnotations()

        If meta Is Nothing Then
            meta = New Dictionary(Of String, String)
        End If

        Call cacheScanIndex()
    End Sub

    ''' <summary>
    ''' get all sample tags data from a json text
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <returns></returns>
    Public Shared Function GetSampleTags(buffer As StreamPack) As String()
        Const sample_tags As String = ".etc/sample_tags.json"

        If buffer.FileExists(sample_tags) Then
            Dim file = buffer.OpenBlock(".etc/sample_tags.json")
            Dim data As String() = New StreamReader(file) _
                .ReadToEnd _
                .LoadObject(GetType(String()))

            Return data
        Else
            Return New String() {"*"}
        End If
    End Function

    Private Sub cacheScanIndex()
        Dim dir As StreamGroup = pack.GetObject("/MS/")
        Dim dirs = dir.files

        For Each subdir As StreamObject In dirs
            If TypeOf subdir Is StreamGroup AndAlso Not subdir.hasAttributes Then
                For Each ms1 As StreamObject In DirectCast(subdir, StreamGroup).files
                    If ms1.hasAttribute("scan_id") Then
                        Dim scan_id As String = any.ToString(ms1.GetAttribute("scan_id"))
                        Dim dirpath As String = ms1.referencePath.ToString & "/"

                        Call Me.scan_id.Add(scan_id, dirpath)
                    End If
                Next
            Else
                Dim ms1 = subdir

                If ms1.hasAttribute("scan_id") Then
                    Dim scan_id As String = any.ToString(ms1.GetAttribute("scan_id"))
                    Dim dirpath As String = ms1.referencePath.ToString & "/"

                    Call Me.scan_id.Add(scan_id, dirpath)
                End If
            End If
        Next
    End Sub

    Private Function findScan1Name(scan_id As String) As String
        If Me.scan_id.ContainsKey(scan_id) Then
            Return Me.scan_id(scan_id)
        Else
            Return $"/MS/{mzStreamWriter.getScan1DirName(scan_id)}/"
        End If
    End Function

    Public Function ReadMS1(scan_id As String) As ScanMS1
        Dim refer As String = $"{findScan1Name(scan_id)}/Scan1.mz"
        Dim buffer As Stream = pack.OpenBlock(refer)
        Dim reader As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.LittleEndian}
        Dim ms1 As New ScanMS1 With {.meta = New Dictionary(Of String, String)}
        Dim metadata As StreamObject = pack.GetObject(refer)

        ' required of read the metadata
        Call Serialization.ReadScan1(ms1, file:=reader, readmeta:=True)

        If metadata.hasAttributes Then
            For Each tag As String In metadata.attributes
                Call ms1.meta.Add(tag, metadata.GetAttribute(tag))
            Next
        End If

        Return ms1
    End Function

    Public Function hasMs2(Optional sampling As Integer = 64) As Boolean Implements IMzPackReader.hasMs2
        For Each scanId As String In MS1.Take(sampling)
            Dim refer As String = findScan1Name(scanId)
            Dim dir = pack.GetObject(refer)
            Dim n As Integer = dir.attributes.GetValue("products")

            If n > 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Sub ReadChromatogramTick(scanId As String,
                                    <Out> ByRef scan_time As Double,
                                    <Out> ByRef BPC As Double,
                                    <Out> ByRef TIC As Double) Implements IMzPackReader.ReadChromatogramTick

        Dim refer As String = $"{findScan1Name(scanId)}/Scan1.mz"
        Dim buffer As Stream = pack.OpenBlock(refer)
        Dim reader As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.LittleEndian}
        Dim ms1 As New ScanMS1

        ' required of read the metadata
        Call Serialization.ReadScan1(ms1, file:=reader, readmeta:=True)

        scan_time = ms1.rt
        BPC = ms1.BPC
        TIC = ms1.TIC
    End Sub

    ''' <summary>
    ''' get the metadata from a specific scan data
    ''' </summary>
    ''' <param name="scan_id">the specific scan data was referenced via this scan id pointer</param>
    ''' <returns></returns>
    Public Function GetMetadata(scan_id As String) As Dictionary(Of String, String) Implements IMzPackReader.GetMetadata
        Dim refer As String = $"{findScan1Name(scan_id)}/Scan1.mz"
        Dim metadata As StreamObject = pack.GetObject(refer)
        Dim meta As New Dictionary(Of String, String)

        If metadata.hasAttributes Then
            For Each tag As String In metadata.attributes
                Call meta.Add(tag, metadata.GetAttribute(tag))
            Next
        End If

        Return meta
    End Function

    Public Function ReadScan(scan_id As String, Optional skipProducts As Boolean = False) As ScanMS1 Implements IMzPackReader.ReadScan
        Dim ms1 As ScanMS1 = ReadMS1(scan_id)
        Dim refer As String = findScan1Name(scan_id)
        Dim dir = pack.GetObject(refer)
        Dim n As Integer = dir.attributes.GetValue("products")
        Dim id2 As String() = Nothing

        If n > 0 AndAlso Not skipProducts Then
            id2 = dir.attributes.GetValue("id")
            ms1.products = New ScanMS2(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim buffer As Stream = pack.OpenBlock($"{refer}/{id2(i).MD5}.mz")
                Dim reader As New BinaryDataReader(buffer) With {
                    .ByteOrder = ByteOrder.LittleEndian
                }

                ms1.products(i) = Serialization.ReadScanMs2(reader)
            Next
        Else
            ms1.products = {}
        End If

        Return ms1
    End Function

    Public Function GetThumbnail() As Image
        If pack.GetObject("/thumbnail.png") Is Nothing Then
            Return Nothing
        End If

#Disable Warning
        Using snapshot As Stream = pack.OpenBlock("/thumbnail.png")
            Return Image.FromStream(snapshot)
        End Using
#Enable Warning
    End Function

    ''' <summary>
    ''' read all data into memory(memory load = max)
    ''' </summary>
    ''' <returns></returns>
    Public Function ReadModel(Optional ignoreThumbnail As Boolean = False,
                              Optional skipMsn As Boolean = False,
                              Optional verbose As Boolean = False) As mzPack

        Dim MsReader As Func(Of String, ScanMS1)
        Dim scans As New List(Of ScanMS1)
        Dim i As i32 = 0
        Dim allIndex As String() = MS1.ToArray
        Dim d As Integer = allIndex.Length / 10
        Dim j As Integer = 0

        If skipMsn Then
            MsReader = AddressOf ReadMS1
        Else
            MsReader = AddressOf ReadScan
        End If

        For Each id As String In allIndex
            j += 1
            scans.Add(MsReader(id))

            If ++i = d Then
                If verbose Then
                    RunSlavePipeline.SendProgress(std.Round(j / allIndex.Length, 2), id & $" ({(j / allIndex.Length * 100).ToString("F2")}%)")
                End If

                i = 0
            End If
        Next

        Return New mzPack With {
            .Application = Application,
            .MS = scans.ToArray,
            .source = sourceName,
            .Thumbnail = If(ignoreThumbnail, Nothing, GetThumbnail()),
            .metadata = meta,
            .Annotations = ion_annotations
        }
    End Function

    Private Function loadAnnotations() As Dictionary(Of String, String)
        Dim annos As New Dictionary(Of String, String)
        Dim text As String = Strings.Trim(pack.ReadText(mzStreamWriter.annotations_xml))

        If Not text.StringEmpty Then
            For Each item As NamedValue In text.LoadFromXml(Of NamedValue())
                Call annos.Add(item.name, item.text)
            Next
        End If

        Return annos
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function SafeParseClassType(pack As StreamPack) As FileApplicationClass
        Return Strings _
            .Trim(pack.ReadText("/.etc/app.cls")) _
            .Trim(asciiA.TAB, asciiA.CR, asciiA.LF) _
            .DoCall(Function(str)
                        Return SafeParseClassType(str)
                    End Function)
    End Function

    Public Shared Function SafeParseClassType(str As String) As FileApplicationClass
        If str.StringEmpty Then
            Return FileApplicationClass.LCMS
        Else
            Dim app As FileApplicationClass = Nothing
            Dim test As Boolean = [Enum].TryParse(str, app)

            If test Then
                Return app
            Else
                Return FileApplicationClass.LCMS
            End If
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function safeParseClassType() As FileApplicationClass
        Return SafeParseClassType(pack)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call pack.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

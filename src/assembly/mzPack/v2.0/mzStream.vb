Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports asciiA = Microsoft.VisualBasic.Text.ASCII

#If UNIX = 0 Then
Imports Microsoft.VisualBasic.ApplicationServices
#End If

''' <summary>
''' mzPack format in HDS stream file
''' </summary>
Public Class mzStream : Implements IDisposable

    ReadOnly pack As StreamPack

    Dim disposedValue As Boolean
    Dim meta As Dictionary(Of String, String)
    Dim summary As Dictionary(Of String, Double)

    Public ReadOnly Property Application As FileApplicationClass
    Public ReadOnly Property sourceName As String
        Get
            Return meta.TryGetValue("source")
        End Get
    End Property

    ''' <summary>
    ''' get all ms1 scan id
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MS1 As String()
        Get
            Dim dir As StreamGroup = pack.GetObject("/MS/")
            Dim dirs = dir.files

            Return dirs _
                .Select(Function(d) d.fileName) _
                .ToArray
        End Get
    End Property

    Sub New(filepath As String)
        Call Me.New(filepath.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
    End Sub

    Sub New(stream As Stream)
        pack = New StreamPack(stream)
        Application = safeParseClassType()
        meta = pack.ReadText("/.etc/metadata.json").LoadJSON(Of Dictionary(Of String, String))
        summary = pack.ReadText("/.etc/ms_scans.json").LoadJSON(Of Dictionary(Of String, Double))
    End Sub

    Public Function ReadMS1(scan_id As String) As ScanMS1
        Dim refer As String = $"/MS/{scan_id.Replace("\", "/").Replace("/", "_")}/Scan1.mz"
        Dim buffer As Stream = pack.OpenBlock(refer)
        Dim reader As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.LittleEndian}
        Dim ms1 As New ScanMS1

        Call Serialization.ReadScan1(ms1, file:=reader)
#If UNIX = 0 Then
        Call Application.DoEvents()
#End If
        Return ms1
    End Function

    Public Function ReadScan(scan_id As String) As ScanMS1
        Dim ms1 As ScanMS1 = ReadMS1(scan_id)
        Dim refer As String = $"/MS/{scan_id.Replace("\", "/").Replace("/", "_")}/"
        Dim dir = pack.GetObject(refer)
        Dim n As Integer = dir.attributes.GetValue("products")
        Dim id2 As String() = dir.attributes.GetValue("id")

        ms1.products = New ScanMS2(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim buffer As Stream = pack.OpenBlock($"{refer}/{id2(i).MD5}.mz")
            Dim reader As New BinaryDataReader(buffer) With {
                .ByteOrder = ByteOrder.LittleEndian
            }

            ms1.products(i) = Serialization.ReadScanMs2(reader)
        Next

        Return ms1
    End Function

    Public Function GetThumbnail() As Image
        If pack.GetObject("/thumbnail.png") Is Nothing Then
            Return Nothing
        End If

        Using snapshot As Stream = pack.OpenBlock("/thumbnail.png")
            Return Image.FromStream(snapshot)
        End Using
    End Function

    ''' <summary>
    ''' read all data into memory(memory load = max)
    ''' </summary>
    ''' <returns></returns>
    Public Function ReadModel(Optional ignoreThumbnail As Boolean = False,
                              Optional skipMsn As Boolean = False,
                              Optional verbose As Boolean = False) As mzPack

        Dim MsReader As Func(Of String, ScanMS1)

        If skipMsn Then
            MsReader = AddressOf ReadMS1
        Else
            MsReader = AddressOf ReadScan
        End If

        Return New mzPack With {
            .Application = Application,
            .MS = MS1 _
                .Select(MsReader) _
                .ToArray,
            .source = sourceName,
            .Thumbnail = If(ignoreThumbnail, Nothing, GetThumbnail())
        }
    End Function

    Private Function safeParseClassType() As FileApplicationClass
        Return Strings _
            .Trim(pack.ReadText("/.etc/app.cls")) _
            .Trim(asciiA.TAB, asciiA.CR, asciiA.LF) _
            .DoCall(Function(str)
                        If str = "" Then
                            Return FileApplicationClass.LCMS
                        Else
                            Return DirectCast([Enum].Parse(GetType(FileApplicationClass), str), FileApplicationClass)
                        End If
                    End Function)
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

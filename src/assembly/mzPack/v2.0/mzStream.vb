Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
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

    Public ReadOnly Property Application As FileApplicationClass

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
    End Sub

    Public Function ReadMS1(scan_id As String) As ScanMS1
        Dim refer As String = $"/MS/{scan_id.Replace("\", "/").Replace("/", "_")}/Scan1.mz"
        Dim buffer As Stream = pack.OpenBlock(refer)
        Dim reader As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.LittleEndian}
        Dim ms1 As New ScanMS1
        Call Serialization.ReadScan1(ms1, file:=reader)
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

#If UNIX = 0 Then
        Call Application.DoEvents()
#End If

        Return ms1
    End Function

    ''' <summary>
    ''' read all data into memory(memory load = max)
    ''' </summary>
    ''' <returns></returns>
    Public Function ReadModel() As mzPack
        Return New mzPack With {
            .Application = Application,
            .MS = MS1 _
                .Select(AddressOf ReadScan) _
                .ToArray
        }
    End Function

    Private Function safeParseClassType() As FileApplicationClass
        Return Strings _
            .Trim(pack.ReadText(".etc/app.cls")) _
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

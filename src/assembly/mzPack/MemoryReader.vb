Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' read <see cref="mzPack"/> data in-memory
''' </summary>
Public Class MemoryReader : Implements IMzPackReader

    ReadOnly data As mzPack
    ReadOnly scan_index As Index(Of String)

    Public ReadOnly Property EnumerateIndex As IEnumerable(Of String) Implements IMzPackReader.EnumerateIndex
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return scan_index.Objects
        End Get
    End Property

    Public ReadOnly Property source As String Implements IMzPackReader.source
    Public ReadOnly Property rtmax As Double Implements IMzPackReader.rtmax

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(raw As mzPack)
        data = raw
        rtmax = data.MS.OrderByDescending(Function(s) s.rt).First.rt
        scan_index = data.MS.Select(Function(s) s.scan_id).Indexing
        source = raw.source
    End Sub

    Public Sub ReadChromatogramTick(scanId As String, <Out> ByRef scan_time As Double, <Out> ByRef BPC As Double, <Out> ByRef TIC As Double) Implements IMzPackReader.ReadChromatogramTick
        Dim scan As ScanMS1 = ReadScan(scanId)

        If scan Is Nothing Then
            scan_time = -1
            BPC = -1
            TIC = -1
        Else
            scan_time = scan.rt
            BPC = scan.BPC
            TIC = scan.TIC
        End If
    End Sub

    Public Function ReadScan(scan_id As String, Optional skipProducts As Boolean = False) As ScanMS1 Implements IMzPackReader.ReadScan
        Dim i As Integer = scan_index.IndexOf(scan_id)

        If i < 0 Then
            Return Nothing
        Else
            Return data.MS(i)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMetadata(id As String) As Dictionary(Of String, String) Implements IMzPackReader.GetMetadata
        Return ReadScan(scan_id:=id)?.meta
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function hasMs2(Optional sampling As Integer = 64) As Boolean Implements IMzPackReader.hasMs2
        Return data.MS.Any(Function(scan1) scan1.products.Any)
    End Function
End Class
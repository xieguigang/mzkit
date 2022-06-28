Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Public Class Reader : Inherits LibraryFile
    Implements IDisposable

    Dim index As BlockSearchFunction(Of MassIndex)
    Dim disposedValue As Boolean

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As String, massDiff As Double)
        Call Me.New(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True), massDiff)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As Stream, massDiff As Double)
        Call Me.New(New ZipArchive(file, ZipArchiveMode.Read), massDiff)
    End Sub

    Sub New(file As ZipArchive, massDiff As Double)
        Me.file = file

        Dim rawIndex = LibraryFile _
            .LoadIndex(file) _
            .ToArray

        Me.index = New BlockSearchFunction(Of MassIndex)(
            data:=rawIndex,
            eval:=Function(i) i.mz,
            tolerance:=massDiff,
            factor:=5
        )
    End Sub

    Public Function GetSpectrums(spectrumBlockId As String) As Spectrum()
        Dim spectrumName As String = $"{spectrumBlockId.Substring(0, 2)}/{spectrumBlockId}.mat"
        Dim pack As ZipArchiveEntry = file.Entries _
            .Where(Function(i) i.FullName = spectrumName) _
            .FirstOrDefault

        Using msBuffer As Stream = pack.Open
            Return MsgPackSerializer.Deserialize(Of Spectrum())(msBuffer)
        End Using
    End Function

    Public Iterator Function QueryByMz(mz As Double, mzdiff As Tolerance) As IEnumerable(Of Metabolite)
        Dim data As Metabolite
        Dim query As IEnumerable(Of MassIndex) = index.Search(New MassIndex With {.mz = mz})

        For Each index As MassIndex In query
            If mzdiff(mz, index.mz) Then
                For Each key As String In index.referenceIds
                    Dim fullName As String = $"{LibraryFile.annotationPath}/{key.Substring(0, 2)}/{key}.dat"
                    Dim pack As ZipArchiveEntry = file.Entries _
                        .Where(Function(i) i.FullName = fullName) _
                        .FirstOrDefault

                    data = MsgPackSerializer.Deserialize(Of Metabolite)(pack.Open)
                    data.spectrumBlockId = key

                    Yield data
                Next
            End If
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call file.Dispose()
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

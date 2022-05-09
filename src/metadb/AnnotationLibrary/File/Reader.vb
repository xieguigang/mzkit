Imports System.IO
Imports System.IO.Compression
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Public Class Reader : Inherits LibraryFile
    Implements IDisposable

    Dim index As MassIndex()
    Dim disposedValue As Boolean

    Sub New(file As String)
        Call Me.New(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Sub

    Sub New(file As Stream)
        Call Me.New(New ZipArchive(file, ZipArchiveMode.Read))
    End Sub

    Sub New(file As ZipArchive)
        Me.file = file
        Me.index = LibraryFile _
            .LoadIndex(file) _
            .ToArray
    End Sub

    Public Iterator Function QueryByMz(mz As Double, mzdiff As Tolerance) As IEnumerable(Of Metabolite)
        For Each index As MassIndex In Me.index
            If mzdiff(mz, index.mz) Then
                For Each key As String In index.referenceIds
                    Dim fullName As String = $"{key.Substring(0, 2)}/{key}.dat"
                    Dim pack As ZipArchiveEntry = file.Entries _
                        .Where(Function(i) i.FullName = fullName) _
                        .FirstOrDefault
                    Dim data As Metabolite = MsgPackSerializer.Deserialize(Of Metabolite)(pack.Open)

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

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

''' <summary>
''' A data pack of the reference spectrum data which 
''' is indexed via the formula data
''' </summary>
Public Class SpectrumPack : Implements IDisposable

    ''' <summary>
    ''' Each block is a collection of the metabolite spectrum
    ''' </summary>
    ReadOnly treePack As New List(Of BlockNode)
    ReadOnly massSet As New Dictionary(Of String, MassIndex)
    ReadOnly file As StreamPack

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Me.file = New StreamPack(file, meta_size:=64 * 1024 * 1024)
    End Sub

    Public Sub Push(uid As String, mass As Double, spectrum As PeakMs2)

    End Sub

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

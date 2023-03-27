
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace PoolData

    Public MustInherit Class PoolFs : Implements IDisposable

        Private disposedValue As Boolean

        Public ReadOnly Property level As Double
        Public ReadOnly Property split As Integer
        Public ReadOnly Property splitDelta As Double

        Public MustOverride Function GetTreeChilds(path As String) As IEnumerable(Of String)
        Public MustOverride Function LoadMetadata(path As String) As MetadataProxy
        Public MustOverride Sub CommitMetadata(path As String, data As MetadataProxy)
        Public MustOverride Function FindRootId(path As String) As String
        Public MustOverride Sub SetRootId(path As String, id As String)
        Public MustOverride Sub Add(spectrum As PeakMs2)
        Public MustOverride Function GetScore(x As String, y As String) As Double
        Public MustOverride Function ReadSpectrum(p As Metadata) As PeakMs2
        Public MustOverride Function WriteSpectrum(spectral As PeakMs2) As Metadata

        Friend Sub SetLevel(level As Double, split As Integer)
            _level = level
            _split = split
            _splitDelta = level / split
        End Sub

        Protected MustOverride Sub Close()

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call Close()
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
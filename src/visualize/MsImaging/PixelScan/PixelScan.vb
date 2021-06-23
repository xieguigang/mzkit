Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public MustInherit Class PixelScan : Implements IDisposable

        Dim disposedValue As Boolean

        Public MustOverride ReadOnly Property X As Integer
        Public MustOverride ReadOnly Property Y As Integer

        Public MustOverride Function GetMs() As ms2()

        Public MustOverride Function HasAnyMzIon(mz As Double(), tolerance As Tolerance) As Boolean

        Public Overrides Function ToString() As String
            Return $"[{X},{Y}]"
        End Function

        Protected Friend MustOverride Sub release()

        Private Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call release()
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
End Namespace
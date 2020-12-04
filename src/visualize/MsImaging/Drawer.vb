Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML

Public Class Drawer : Implements IDisposable

    Dim disposedValue As Boolean
    Dim ibd As ibdReader
    Dim pixels As ScanData()

    Sub New(imzML As String)
        ibd = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        pixels = XML.LoadScans(file:=imzML).ToArray
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call ibd.Dispose()
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

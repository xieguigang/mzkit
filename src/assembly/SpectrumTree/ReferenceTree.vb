Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Public Class ReferenceTree : Implements IDisposable

    ReadOnly tree As New List(Of BlockNode)
    ReadOnly spectrum As BinaryDataWriter

    Private disposedValue As Boolean

    Public Const Magic As String = "BioDeep/ReferenceTree"

    Sub New(file As Stream)
        spectrum = New BinaryDataWriter(file, Encodings.ASCII) With {
            .ByteOrder = ByteOrder.LittleEndian
        }
        spectrum.Write(Magic)
        ' jump point to tree
        spectrum.Write(0&)
    End Sub

    Public Sub Push(data As PeakMs2)
        If tree.Count = 0 Then
            ' add root node
            tree.Add(New BlockNode With {
                .Block = WriteSpectrum(data),
                .childs = New Integer(9) {},
                .Id = data.lib_guid,
                .Members = New List(Of Integer)
            })
        End If
    End Sub

    Private Function WriteSpectrum(data As PeakMs2) As BufferRegion
        Dim start As Long = spectrum.Position
        Dim scan As ScanMS2 = data.
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call spectrum.Flush()
                Call spectrum.Dispose()
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

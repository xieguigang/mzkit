Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Public Class TreeSearch : Implements IDisposable

    ReadOnly bin As BinaryDataReader
    ReadOnly tree As BlockNode()
    ReadOnly da As Tolerance
    ReadOnly intocutoff As RelativeIntensityCutoff

    Dim disposedValue As Boolean

    Sub New(stream As Stream)
        bin = New BinaryDataReader(stream, encoding:=Encodings.ASCII) With {
            .ByteOrder = ByteOrder.LittleEndian
        }
        Dim magic = Encoding.ASCII.GetString(bin.ReadBytes(ReferenceTree.Magic.Length))

        If magic <> ReferenceTree.Magic Then
            Throw New NotImplementedException
        End If

        Dim jump = bin.ReadInt64

        bin.Seek(jump, SeekOrigin.Begin)

        Dim nsize = bin.ReadInt32

        tree = New BlockNode(nsize - 1) {}

        For i As Integer = 0 To nsize - 1
            tree(i) = NodeBuffer.Read(bin)
        Next

        da = Tolerance.DeltaMass(0.3)
        intocutoff = 0.05
    End Sub

    Public Function Centroid(matrix As ms2()) As ms2()
        Return matrix.Centroid(da, intocutoff).ToArray
    End Function

    Public Function Search(centroid As ms2())

    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call bin.Dispose()
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

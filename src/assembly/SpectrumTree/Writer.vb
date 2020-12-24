Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Public Class Writer : Implements IDisposable

    ReadOnly outfile As BinaryDataWriter

    Dim disposedValue As Boolean

    Const magic As String = "spectrum_tree"

    Sub New(output As Stream)
        outfile = New BinaryDataWriter(output, Encodings.ASCII)
    End Sub

    Public Sub Write(tree As SpectrumTreeCluster)
        Dim bin As BinaryTree(Of PeakMs2, PeakMs2) = tree.getRoot

        Call outfile.Write(magic, BinaryStringFormat.NoPrefixOrTermination)
        Call write(bin)
        Call outfile.Flush()
    End Sub

    Private Sub write(node As BinaryTree(Of PeakMs2, PeakMs2))
        Dim pos As Long
        Dim size As Long

        Call outfile.Write(CByte(1))

        ' write current cluster
        ' id key of current cluster
        Call outfile.Write(node.Key.ToString, BinaryStringFormat.ZeroTerminated)
        ' spectrum counts
        Call outfile.Write(node.Members.Length)
        Call outfile.Flush()

        pos = outfile.BaseStream.Position

        ' data size
        Call outfile.Write(0&)

        ' write spectra data
        For Each spectra As PeakMs2 In node.Members
            size += write(spectra)
        Next

        Using outfile.TemporarySeek(pos)
            Call outfile.Write(size)
        End Using

        ' write left node
        If node.Left Is Nothing Then
            Call outfile.Write(CByte(0))
        Else
            Call write(node.Left)
        End If

        ' write right node
        If node.Right Is Nothing Then
            Call outfile.Write(CByte(0))
        Else
            Call write(node.Right)
        End If

        Call outfile.Flush()
    End Sub

    Private Function write(spectra As PeakMs2) As Integer
        Using buf As New MemoryStream, writer As New BinaryDataWriter(buf, Encodings.ASCII)
            Call writer.Write(spectra.lib_guid, BinaryStringFormat.ZeroTerminated)
            Call writer.Write(spectra.file, BinaryStringFormat.ZeroTerminated)
            Call writer.Write(spectra.scan)
            Call writer.Write(spectra.precursor_type)
            Call writer.Write(spectra.activation, BinaryStringFormat.ZeroTerminated)
            Call writer.Write(spectra.collisionEnergy)
            Call writer.Write(spectra.mz)
            Call writer.Write(spectra.rt)
            Call writer.Write(spectra.meta.Count)

            For Each item In spectra.meta
                Call writer.Write(item.Key, BinaryStringFormat.ZeroTerminated)
                Call writer.Write(item.Value, BinaryStringFormat.ZeroTerminated)
            Next

            Call writer.Write(spectra.mzInto.Length)

            For Each product In spectra.mzInto
                Call writer.Write(product.mz)
                Call writer.Write(product.intensity)
                Call writer.Write(product.quantity)
                Call writer.Write(product.Annotation, BinaryStringFormat.ZeroTerminated)
            Next

            Call writer.Flush()
            Call outfile.Write(buf)

            Return buf.Length
        End Using
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
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

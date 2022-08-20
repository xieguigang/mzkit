Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Public Class TreeSearch : Implements IDisposable

    ReadOnly bin As BinaryDataReader
    ReadOnly tree As BlockNode()
    ReadOnly da As Tolerance
    ReadOnly intocutoff As RelativeIntensityCutoff
    ReadOnly is_binary As Boolean

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
        is_binary = tree.All(Function(i) i.childs.TryCount <= 2)
    End Sub

    Public Overrides Function ToString() As String
        If is_binary Then
            Return "binary_spectrum_tree"
        Else
            Return "spectrum_cluster_tree"
        End If
    End Function

    Public Function Centroid(matrix As ms2()) As ms2()
        Return matrix.Centroid(da, intocutoff).ToArray
    End Function

    Public Function Search(centroid As ms2(), Optional maxdepth As Integer = 1024) As ClusterHit
        If tree.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim node As BlockNode = tree(Scan0)
        Dim depth As Integer = 0
        Dim max = (score:=0.0, raw:=(0.0, 0.0), node)

        Do While True
            Dim score = GlobalAlignment.TwoDirectionSSM(centroid, node.centroid, da)
            Dim min = stdNum.Min(score.forward, score.reverse)
            Dim index As Integer

            If is_binary Then
                index = BlockNode.GetBinaryIndex(min)

                ' translate index from binary comparision
                ' to normal index
                If index = 0 Then
                    index = -1
                ElseIf index = -1 Then
                    index = 0
                End If
            Else
                index = BlockNode.GetIndex(min)
            End If

            If min > max.score Then
                max = (min, score, node)
            End If

            If index = -1 Then
                ' is current node cluster member
                Return reportClusterHit(centroid, hit:=node, score:=score)
            Else
                node = tree(node.childs(index))
            End If

            depth += 1

            If depth > maxdepth Then
                Exit Do
            End If
        Loop

        If max.score > 0 Then
            Return reportClusterHit(centroid, hit:=max.node, score:=max.raw)
        Else
            Return Nothing
        End If
    End Function

    Private Function reportClusterHit(centroid As ms2(), hit As BlockNode, score As (forward#, reverse#)) As ClusterHit
        Dim cluster = hit.Members.Select(Function(i) tree(i)).ToArray
        Dim alignments = cluster.Select(Function(c) GlobalAlignment.TwoDirectionSSM(centroid, c.centroid, da)).ToArray
        Dim forward = alignments.Select(Function(a) a.forward).ToArray
        Dim reverse = alignments.Select(Function(a) a.reverse).ToArray
        Dim rt As Double() = cluster.Select(Function(c) c.rt).ToArray
        Dim jaccard As Double() = cluster.Select(Function(c) GlobalAlignment.JaccardIndex(c.centroid, centroid, da)).ToArray

        Return New ClusterHit With {
            .Id = hit.Id,
            .forward = score.forward,
            .reverse = score.reverse,
            .jaccard = GlobalAlignment.JaccardIndex(hit.centroid, centroid, da),
            .representive = GlobalAlignment.CreateAlignment(centroid, hit.centroid, da).ToArray,
            .ClusterId = { .Id}.JoinIterates(cluster.Select(Function(c) c.Id)).ToArray,
            .ClusterForward = {score.forward}.JoinIterates(forward).ToArray,
            .ClusterReverse = {score.reverse}.JoinIterates(reverse).ToArray,
            .ClusterRt = {hit.rt}.JoinIterates(rt).ToArray,
            .ClusterJaccard = { .jaccard}.JoinIterates(jaccard).ToArray
        }
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

#Region "Microsoft.VisualBasic::7d002fba8879b9c81986b5778ff43eb0, G:/mzkit/src/assembly/SpectrumTree//Query/TreeSearch.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 261
    '    Code Lines: 178
    ' Comment Lines: 45
    '   Blank Lines: 38
    '     File Size: 10.26 KB


    '     Class TreeSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: QueryByMz, reportClusterHit, (+2 Overloads) Search, SetCutoff, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace Query

    Public Class TreeSearch : Inherits Ms2Search
        Implements IDisposable

        ReadOnly bin As BinaryDataReader
        ReadOnly tree As BlockNode()
        ReadOnly is_binary As Boolean
        ReadOnly mzIndex As MzIonSearch

        ''' <summary>
        ''' cutoff of the cos similarity
        ''' </summary>
        Dim dotcutoff As Double = 0.6
        Dim disposedValue As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="cutoff">
        ''' cutoff value for cos similarity
        ''' </param>
        Sub New(stream As Stream, Optional cutoff As Double = 0.6)
            Call MyBase.New

            dotcutoff = cutoff
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
            Dim mzset As New List(Of IonIndex)

            tree = New BlockNode(nsize - 1) {}

            For i As Integer = 0 To nsize - 1
                tree(i) = NodeBuffer.Read(bin)
#Disable Warning
                If Not tree(i).isLeaf Then
                    Call mzset.AddRange(tree(i).mz _
                        .Select(Function(mzi)
                                    Return New IonIndex With {
                                        .mz = mzi,
                                        .node = i
                                    }
                                End Function))
                End If
#Enable Warning
            Next

            is_binary = tree.All(Function(i) i.childs.TryCount <= 2)
            ' see dev notes about the mass tolerance in 
            ' MSSearch module
            mzIndex = New MzIonSearch(mzset, da)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <DebuggerStepThrough>
        Public Function SetCutoff(cutoff As Double) As TreeSearch
            dotcutoff = cutoff
            Return Me
        End Function

        ''' <summary>
        ''' query the spectrum reference tree nodes via parent m/z matched
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <returns></returns>
        Public Function QueryByMz(mz As Double) As BlockNode()
            Dim ions As IonIndex() = mzIndex.QueryByMz(mz).ToArray
            Dim result As BlockNode()

            If ions.Length = 0 Then
                Return {}
            Else
                result = ions _
                    .Select(Function(d) tree(d.node)) _
                    .GroupBy(Function(d) d.Id) _
                    .Select(Function(g)
                                Return g.First
                            End Function) _
                    .ToArray
            End If

            Return result
        End Function

        Public Overrides Function ToString() As String
            If is_binary Then
                Return "binary_spectrum_tree"
            Else
                Return "spectrum_cluster_tree"
            End If
        End Function

        ''' <summary>
        ''' populate the top cluster
        ''' </summary>
        ''' <param name="centroid">the query spectrum matrix data should be processed in centroid mode</param>
        ''' <param name="mz1">the parent m/z of the target unknown metabolite</param>
        ''' <returns></returns>
        Public Overrides Iterator Function Search(centroid As ms2(), mz1 As Double) As IEnumerable(Of ClusterHit)
            Dim candidates As BlockNode() = QueryByMz(mz1)
            Dim max = (score:=0.0, raw:=(0.0, 0.0), node:=candidates.ElementAtOrNull(Scan0))

            For Each hit As BlockNode In candidates
                Dim score = GlobalAlignment.TwoDirectionSSM(centroid, hit.centroid, da)
                Dim min = stdNum.Min(score.forward, score.reverse)

                If min > max.score Then
                    max = (min, score, hit)
                End If
            Next

            If max.score > dotcutoff Then
                Yield reportClusterHit(centroid, hit:=max.node, score:=max.raw)
            End If
        End Function

        ''' <summary>
        ''' search by tree query
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="maxdepth"></param>
        ''' <returns></returns>
        Public Overloads Function Search(centroid As ms2(), Optional maxdepth As Integer = 1024) As ClusterHit
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

            If max.score > dotcutoff Then
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
            Dim jaccard As Double() = cluster _
                .Select(Function(c) GlobalAlignment.JaccardIndex(c.centroid, centroid, da)) _
                .ToArray
            Dim entropy_data = cluster _
                .Select(Function(c)
                            Return SpectralEntropy.calculate_entropy_similarity(centroid, c.centroid, da)
                        End Function) _
                .ToArray

            ' the first score element is always the current
            ' tree node representive spectrum alignment
            ' result
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
                .ClusterJaccard = { .jaccard}.JoinIterates(jaccard).ToArray,
                .entropy = SpectralEntropy.calculate_entropy_similarity(centroid, hit.centroid, da),
                .ClusterEntropy = { .entropy}.JoinIterates(entropy_data).ToArray
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
End Namespace

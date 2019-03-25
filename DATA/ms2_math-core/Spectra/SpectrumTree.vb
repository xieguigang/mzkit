#Region "Microsoft.VisualBasic::ce86bd4bf145c4f7ed465f9e299c9784, ms2_math-core\Spectra\SpectrumTree.vb"

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

    '     Class SpectrumCluster
    ' 
    '         Properties: cluster, Representative
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator
    ' 
    '     Class SpectrumTreeCluster
    ' 
    '         Properties: allMs2Scans
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: doCluster, ms2Compares, PopulateClusters
    ' 
    '         Sub: clusterInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Math.Spectra
Imports sys = System.Math

Namespace Spectra

    Public Class SpectrumCluster : Implements IEnumerable(Of PeakMs2)

        Public Property Representative As PeakMs2
        ''' <summary>
        ''' 在这个属性之中也会通过包含有<see cref="Representative"/>代表质谱图
        ''' </summary>
        ''' <returns></returns>
        Public Property cluster As PeakMs2()

        Public Overrides Function ToString() As String
            Return Representative.ToString & $"  with {cluster.Length} cluster members."
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of PeakMs2) Implements IEnumerable(Of PeakMs2).GetEnumerator
            For Each spectrum As PeakMs2 In cluster
                Yield spectrum
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' 因为在标准品实验之中仅包含有标准品以及流动相物质
    ''' 标准品的质谱碎片理论上应该是数量最多的,所以在这里
    ''' 通过二叉树对所有的质谱图进行聚类,取出成员最多的簇
    ''' 作为候选的标准品质谱图
    ''' </summary>
    Public Class SpectrumTreeCluster

        Dim tolerance As Tolerance = Tolerance.PPM(20)
        Dim tree As AVLTree(Of PeakMs2, PeakMs2)
        Dim showReport As Boolean
        Dim equalsScore#
        Dim gtScore#

        Public ReadOnly Property allMs2Scans As New List(Of PeakMs2)

        Sub New(showReport As Boolean, Optional equalsScore# = 0.85, Optional gtScore# = 0.6)
            Me.showReport = showReport
            Me.equalsScore = equalsScore
            Me.gtScore = gtScore

            If equalsScore < 0 OrElse equalsScore > 1 Then
                Throw New InvalidConstraintException("Scores for spectra equals is invalid, it should be in range (0, 1].")
            End If
            If gtScore < 0 OrElse gtScore > 1 OrElse gtScore > equalsScore Then
                Throw New InvalidConstraintException("Scores for x < y should be in range (0, 1] and its value is also less than score for spectra equals!")
            End If
        End Sub

        Private Function ms2Compares(x As PeakMs2, y As PeakMs2) As Integer
            Dim score = GlobalAlignment.TwoDirectionSSM(x.mzInto.ms2, y.mzInto.ms2, tolerance)
            Dim min = sys.Min(score.forward, score.reverse)

            If min >= equalsScore Then
                Return 0
            ElseIf min >= gtScore Then
                Return 1
            Else
                Return -1
            End If
        End Function

        Public Iterator Function PopulateClusters() As IEnumerable(Of SpectrumCluster)
            For Each cluster In tree.GetAllNodes
                Yield New SpectrumCluster With {
                    .Representative = cluster.Value,
                    .cluster = DirectCast(cluster!values, IEnumerable(Of PeakMs2)) _
                        .OrderBy(Function(x) x.rt) _
                        .ToArray
                }
            Next
        End Function

        Private Sub clusterInternal(ms2list As PeakMs2(), tick As Action)
            Dim shrinkTolerance As Tolerance = Tolerance.DeltaMass(0.1)

            ' simple => raw
            tree = New AVLTree(Of PeakMs2, PeakMs2)(AddressOf ms2Compares, Function(x) x.ToString)

            For Each ms2 As PeakMs2 In ms2list
                Dim simple As New PeakMs2 With {
                    .mz = ms2.mz,
                    .file = ms2.file,
                    .rt = ms2.rt,
                    .scan = ms2.scan,
                    .mzInto = ms2.mzInto.Shrink(shrinkTolerance).Trim(0.05)
                }

                Call allMs2Scans.Add(ms2)
                Call tree.Add(simple, ms2)
                Call tick()
            Next
        End Sub

        Public Function doCluster(ms2list As PeakMs2()) As SpectrumTreeCluster
            Dim tickAction As Action
            Dim releaseAction As Action

            If showReport Then
                Dim progress As New ProgressBar("Create spectrum tree", 2, True)
                Dim tick As New ProgressProvider(ms2list.Length)
                Dim message$

                tickAction =
                    Sub()
                        message = $"ETA: {tick.ETA(progress.ElapsedMilliseconds).FormatTime}"
                        progress.SetProgress(tick.StepProgress, message)
                    End Sub
                releaseAction = AddressOf progress.Dispose
            Else
                tickAction = App.DoNothing
                releaseAction = App.DoNothing
            End If

            Call clusterInternal(ms2list, tickAction)
            Call releaseAction()

            Return Me
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::94615414c7b03675cf88d1cadf514e86, src\mzmath\ms2_math-core\Spectra\SpectrumTree\SpectrumTree.vb"

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

    '     Class SpectrumTreeCluster
    ' 
    '         Properties: allMs2Scans
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Best, doCluster, getRoot, PopulateClusters, SSMCompares
    ' 
    '         Sub: clusterInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Language.Default
Imports stdNum = System.Math

Namespace Spectra

    ''' <summary>
    ''' The spectral network (molecular network, MN) concept is
    ''' based on the organization And visualization of tandem MS
    ''' information via spectral similarity (homologous MS2 fragmentations).
    ''' 
    ''' Structurally related compounds usually share
    ''' similar MS/MS spectra, And GNPS groups the similar spectra
    ''' in a network-based format, allowing the visual exploration of
    ''' identical And analogous molecules.
    ''' </summary>
    ''' <remarks>
    ''' 因为在标准品实验之中仅包含有标准品以及流动相物质
    ''' 标准品的质谱碎片理论上应该是数量最多的,所以在这里
    ''' 通过二叉树对所有的质谱图进行聚类,取出成员最多的簇
    ''' 作为候选的标准品质谱图
    ''' </remarks>
    Public Class SpectrumTreeCluster

        Dim tree As AVLTree(Of PeakMs2, PeakMs2)
        Dim showReport As Boolean
        Dim Ms2Compares As Comparison(Of PeakMs2) = SSMCompares(Tolerance.PPM(20))

        ReadOnly mzWidth As Tolerance
        ReadOnly intocutoff As LowAbundanceTrimming

        Public ReadOnly Property allMs2Scans As New List(Of PeakMs2)

        Const InvalidScoreRange$ = "Scores for x < y should be in range (0, 1] and its value is also less than score for spectra equals!"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="compares">
        ''' By default is SSM method <see cref="SSMCompares(Tolerance, Double, Double, Func(Of Double, Double, Double))"/>
        ''' </param>
        ''' <param name="mzwidth">
        ''' apply for centroid algorithm
        ''' </param>
        ''' <param name="showReport">Show progress report?</param>
        Sub New(Optional compares As Comparison(Of PeakMs2) = Nothing,
                Optional mzwidth As Tolerance = Nothing,
                Optional intocutoff As LowAbundanceTrimming = Nothing,
                Optional showReport As Boolean = True)

            Me.showReport = showReport
            Me.mzWidth = mzwidth
            Me.intocutoff = intocutoff Or LowAbundanceTrimming.Default

            If Me.mzWidth Is Nothing Then
                Me.mzWidth = Tolerance.DeltaMass(0.1)
            End If
            If Not compares Is Nothing Then
                Ms2Compares = compares
            End If
        End Sub

        Public Function getRoot() As BinaryTree(Of PeakMs2, PeakMs2)
            Return tree.root
        End Function

        ''' <summary>
        ''' 默认是取forward和reverse之间的最低分
        ''' </summary>
        ''' <param name="tolerance">By default is m/z tolerance with ppm20</param>
        ''' <param name="equalsScore">判断两个质谱图是相同的所需的最低得分</param>
        ''' <param name="gtScore">将质谱图划分到二叉树的右节点的所需要的最低得分</param>
        ''' <returns></returns>
        Public Shared Function SSMCompares(Optional tolerance As Tolerance = Nothing,
                                           Optional equalsScore# = 0.85,
                                           Optional gtScore# = 0.6,
                                           Optional scoreAggregate As Func(Of Double, Double, Double) = Nothing) As Comparison(Of PeakMs2)

            Static scoreMin As New [Default](Of Func(Of Double, Double, Double))(AddressOf stdNum.Min)

            If equalsScore < 0 OrElse equalsScore > 1 Then
                Throw New InvalidConstraintException("Scores for spectra equals is invalid, it should be in range (0, 1].")
            End If
            If gtScore < 0 OrElse gtScore > 1 OrElse gtScore > equalsScore Then
                Throw New InvalidConstraintException(InvalidScoreRange)
            End If

            tolerance = tolerance Or ppm20
            scoreAggregate = scoreAggregate Or scoreMin

            Return Function(x As PeakMs2, y As PeakMs2) As Integer
                       Dim score = GlobalAlignment.TwoDirectionSSM(x.mzInto, y.mzInto, tolerance)
                       Dim scoreVal = scoreAggregate(score.forward, score.reverse)

                       If scoreVal >= equalsScore Then
                           Return 0
                       ElseIf scoreVal >= gtScore Then
                           Return 1
                       Else
                           Return -1
                       End If
                   End Function
        End Function

        ''' <summary>
        ''' 将通过二叉树所构建得到的所有聚类结果返回给调用者, 这个函数返回来的质谱图的矩阵都是未经处理的原始矩阵数据
        ''' </summary>
        ''' <returns></returns>
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

        ''' <summary>
        ''' Get the top score cluster with a given <paramref name="score"/> evaluation function.
        ''' </summary>
        ''' <param name="score"></param>
        ''' <returns></returns>
        Public Function Best(score As Func(Of SpectrumCluster, Double)) As SpectrumCluster
            If allMs2Scans.Count = 0 Then
                Return Nothing
            Else
                Return PopulateClusters _
                    .OrderByDescending(score) _
                    .First
            End If
        End Function

        Private Sub clusterInternal(ms2list As PeakMs2(), tick As Action)
            Dim matrix As ms2()
            Dim simply As PeakMs2

            ' simple => raw
            tree = New AVLTree(Of PeakMs2, PeakMs2)(Ms2Compares, Function(x) x.ToString)

            For Each ms2 As PeakMs2 In ms2list
                matrix = ms2.mzInto.Centroid(mzWidth, intocutoff).ToArray
                simply = New PeakMs2 With {
                    .mz = ms2.mz,
                    .file = ms2.file,
                    .rt = ms2.rt,
                    .scan = ms2.scan,
                    .mzInto = matrix
                }

                Call allMs2Scans.Add(ms2)
                Call tree.Add(simply, ms2)
                Call tick()
            Next
        End Sub

        Public Function doCluster(spectrum As PeakMs2()) As SpectrumTreeCluster
            Dim tickAction As Action
            Dim releaseAction As Action

            If showReport Then
                Dim progress As New ProgressBar("Create spectrum tree", 2, True)
                Dim tick As New ProgressProvider(progress, spectrum.Length)
                Dim message$

                tickAction =
                    Sub()
                        message = $"ETA: {tick.ETA().FormatTime}"
                        progress.SetProgress(tick.StepProgress, message)
                    End Sub
                releaseAction = AddressOf progress.Dispose
            Else
                tickAction = App.DoNothing
                releaseAction = App.DoNothing
            End If

            Call clusterInternal(spectrum, tickAction)
            Call releaseAction()

            Return Me
        End Function
    End Class
End Namespace

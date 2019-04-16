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

Imports System.Runtime.CompilerServices
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

        Public ReadOnly Property Length As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cluster.Length
            End Get
        End Property

        Public ReadOnly Property MID As String
            Get
                Return $"M{sys.Round(Representative.mz)}T{sys.Round(Representative.rt)}"
            End Get
        End Property

        Public ReadOnly Property RepresentativeFeature As String
            Get
                Return $"{Representative.file}#{Representative.scan}"
            End Get
        End Property

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

        Dim tree As AVLTree(Of PeakMs2, PeakMs2)
        Dim showReport As Boolean
        Dim Ms2Compares As Comparison(Of PeakMs2) = SSMCompares(Tolerance.PPM(20))

        Public ReadOnly Property allMs2Scans As New List(Of PeakMs2)

        Const InvalidScoreRange$ = "Scores for x < y should be in range (0, 1] and its value is also less than score for spectra equals!"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="compares">
        ''' By default is SSM method <see cref="SSMCompares(Tolerance, Double, Double)"/>
        ''' </param>
        ''' <param name="showReport">Show progress report?</param>
        Sub New(Optional compares As Comparison(Of PeakMs2) = Nothing, Optional showReport As Boolean = True)
            Me.showReport = showReport

            If Not compares Is Nothing Then
                Ms2Compares = compares
            End If
        End Sub

        Public Shared Function SSMCompares(tolerance As Tolerance, Optional equalsScore# = 0.85, Optional gtScore# = 0.6) As Comparison(Of PeakMs2)
            If equalsScore < 0 OrElse equalsScore > 1 Then
                Throw New InvalidConstraintException("Scores for spectra equals is invalid, it should be in range (0, 1].")
            End If
            If gtScore < 0 OrElse gtScore > 1 OrElse gtScore > equalsScore Then
                Throw New InvalidConstraintException(InvalidScoreRange)
            End If

            Return Function(x, y) As Integer
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

        Private Sub clusterInternal(ms2list As PeakMs2(), tick As Action)
            Dim shrinkTolerance As Tolerance = Tolerance.DeltaMass(0.1)
            Dim matrix As LibraryMatrix
            Dim simply As PeakMs2

            ' simple => raw
            tree = New AVLTree(Of PeakMs2, PeakMs2)(Ms2Compares, Function(x) x.ToString)

            For Each ms2 As PeakMs2 In ms2list
                matrix = ms2.mzInto _
                    .Shrink(shrinkTolerance) _
                    .Trim(0.05)
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

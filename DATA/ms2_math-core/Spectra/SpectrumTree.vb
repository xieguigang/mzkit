Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Math.Spectra
Imports sys = System.Math

Namespace Spectra

    ''' <summary>
    ''' 因为在标准品实验之中仅包含有标准品以及流动相物质
    ''' 标准品的质谱碎片理论上应该是数量最多的,所以在这里
    ''' 通过二叉树对所有的质谱图进行聚类,取出成员最多的簇
    ''' 作为候选的标准品质谱图
    ''' </summary>
    Public Class SpectrumTree

        Dim tolerance As Tolerance = Tolerance.PPM(20)
        Dim tree As AVLTree(Of PeakMs2, PeakMs2)
        Dim report As Boolean

        Public ReadOnly Property allMs2Scans As New List(Of PeakMs2)

        Sub New(showReport As Boolean)
            report = showReport
        End Sub

        Private Function ms2Compares(x As PeakMs2, y As PeakMs2) As Integer
            Dim score = GlobalAlignment.TwoDirectionSSM(x.mzInto.ms2, y.mzInto.ms2, tolerance)
            Dim min = Sys.Min(score.forward, score.reverse)

            If min >= 0.85 Then
                Return 0
            ElseIf min >= 0.6 Then
                Return 1
            Else
                Return -1
            End If
        End Function

        Public Iterator Function PopulateClusters() As IEnumerable(Of PeakMs2())
            For Each cluster In tree.GetAllNodes
                Yield DirectCast(cluster!values, IEnumerable(Of PeakMs2)) _
                    .OrderBy(Function(x) x.rt) _
                    .ToArray
            Next
        End Function

        Public Function doCluster(ms2list As PeakMs2()) As SpectrumTree
            Dim shrinkTolerance As Tolerance = Tolerance.DeltaMass(0.1)
            Dim tickAction As Action
            Dim releaseAction As Action

            ' simple => raw
            tree = New AVLTree(Of PeakMs2, PeakMs2)(AddressOf ms2Compares, Function(x) x.ToString)

            If report Then
                Dim progress As New ProgressBar("Create spectrum tree", 2, True)
                Dim tick As New ProgressProvider(ms2list.Length)

                tickAction = Sub()
                                 Call progress.SetProgress(tick.StepProgress, $"ETA: {tick.ETA(progress.ElapsedMilliseconds).FormatTime}")
                             End Sub
                releaseAction = AddressOf progress.Dispose
            Else
                tickAction = Sub()

                             End Sub
                releaseAction = Sub()

                                End Sub
            End If

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
                Call tickAction()
            Next

            Call releaseAction()

            Return Me
        End Function
    End Class
End Namespace
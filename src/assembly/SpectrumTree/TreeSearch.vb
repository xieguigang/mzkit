Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Math.Distributions
Imports stdNum = System.Math

Public Class TreeSearch : Implements IDisposable

    ReadOnly tree As Reader

    Dim disposedValue As Boolean

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="bundle">
    ''' 这里应该是同一个feature的质谱图数据
    ''' </param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    Public Iterator Function Search(bundle As PeakMs2(), Optional cutoff As Double = 0.6) As IEnumerable(Of AlignmentOutput)
        Dim alignments As AlignmentOutput() = AlignEach(bundle, tree.rootCluster).ToArray
        Dim averages As Double = alignments.Select(Function(a) stdNum.Min(a.forward, a.reverse)).TabulateMode

        If averages >= cutoff Then

        End If
    End Function

    Private Iterator Function AlignEach(bundle As PeakMs2(), cluster As PeakMs2()) As IEnumerable(Of AlignmentOutput)

    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call tree.Dispose()
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

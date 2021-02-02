Imports System.Runtime.CompilerServices
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Namespace LinearQuantitative.Linear

    Public Module SampleWorker

        ''' <summary>
        ''' 默认将``-KB``和``-BLK``结尾的文件都判断为实验空白
        ''' </summary>
        Friend ReadOnly defaultBlankNames As New [Default](Of Func(Of String, Boolean))(
            Function(basename)
                Return InStr(basename, "-KB") > 0 OrElse InStr(basename, "-BLK") > 0
            End Function)

        ''' <summary>
        ''' 对单个原始数据文件做定量计算
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="ions"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SampleQuantify(model As StandardCurve(), ions As TargetPeakPoint(), fileName As String) As QuantifyScan

        End Function

        ''' <summary>
        ''' 对单个原始数据文件做定量计算
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function SampleQuantifyScan(result As ContentResult(Of IonPeakTableRow)(), fileName As String) As QuantifyScan
            Dim MRMPeakTable As New List(Of IonPeakTableRow)

            For Each metabolite As ContentResult(Of IonPeakTableRow) In result
                MRMPeakTable += metabolite.Peaktable
            Next

            If result.Length = 0 Then
                Call $"[NO_DATA] {fileName.ToFileURL} found nothing!".Warning
                Return Nothing
            End If

            ' 这个是浓度结果数据
            Dim quantify As New DataSet With {
                .ID = fileName.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.Content
                                    End Function)
            }

            ' 这个是峰面积比 AIS/At 数据
            Dim X As New DataSet With {
                .ID = fileName.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.X
                                    End Function)
            }

            Return New QuantifyScan With {
                .ionPeaks = MRMPeakTable,
                .quantify = quantify,
                .rawX = X
            }
        End Function
    End Module
End Namespace
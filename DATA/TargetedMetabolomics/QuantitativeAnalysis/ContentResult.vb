Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.MassSpectrum.Math.Chromatogram

''' <summary>
''' 客户的实验样本数据之中的某一种目标代谢物质的浓度计算结果
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class ContentResult(Of T As IROI) : Implements INamedValue

    Public Property Name As String Implements IKeyedEntity(Of String).Key
    Public Property Content As Double

    ''' <summary>
    ''' 目标物质的峰面积
    ''' </summary>
    ''' <returns></returns>
    Public Property Peaktable As T

    ''' <summary>
    ''' 是``AIS/A``的结果，即X轴的数据
    ''' </summary>
    ''' <returns></returns>
    Public Property X As Double

    Public Overrides Function ToString() As String
        Return $"Dim {Name} As [{Peaktable.rtmin},{Peaktable.rtmax}] = {Content}"
    End Function
End Class
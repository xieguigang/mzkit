Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.MassSpectrum.Math.MRM.Models

''' <summary>
''' 标准曲线模型
''' </summary>
Public Class FitModel : Implements INamedValue

    Public Property Name As String Implements IKeyedEntity(Of String).Key
    Public Property [IS] As [IS]

    ''' <summary>
    ''' 该代谢物的线性回归模型
    ''' </summary>
    ''' <returns></returns>
    Public Property LinearRegression As IFitted

    ''' <summary>
    ''' 在进行线性回归计算的时候是否需要内标校正？
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RequireISCalibration As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Not [IS] Is Nothing AndAlso Not [IS].ID.StringEmpty AndAlso [IS].CIS > 0
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{Name}] {LinearRegression}"
    End Function

    ''' <summary>
    ''' 对标准曲线进行线性回归建模
    ''' 
    ''' + ``<paramref name="weighted"/> = TRUE``: <see cref="WeightedFit"/>
    ''' + ``<paramref name="weighted"/> = FALSE``: <see cref="FitResult"/>
    ''' 
    ''' </summary>
    ''' <param name="line"></param>
    ''' <returns></returns>
    Public Shared Function CreateLinearRegression(line As PointF(), weighted As Boolean) As IFitted
        ' X是实验值，可能会因为标准曲线溶液配制的问题出现，所以这个可能会需要使用异常点检测
        Dim X As Vector = line.X.AsVector
        ' Y是从文件之中读取出来的浓度梯度信息，认为这个除非文件录入有错，否则将不会出现异常点
        Dim Y As Vector = line.Y.AsVector
        Dim fit As IFitted

        With X.OrderSequenceOutlierIndex.RemovesOutlier(X, Y)
            X = .X
            Y = .Y
        End With

        If weighted Then
            Dim W As Vector = 1 / X ^ 2
            fit = WeightedLinearRegression.Regress(X, Y, W, 1)
        Else
            fit = LeastSquares.LinearFit(X, Y)
        End If

        Return fit
    End Function
End Class
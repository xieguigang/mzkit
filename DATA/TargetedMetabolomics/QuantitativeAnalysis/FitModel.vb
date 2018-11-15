Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports SMRUCC.MassSpectrum.Math.MRM.Models

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

End Class
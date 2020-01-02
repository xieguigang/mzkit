Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports SMRUCC.MassSpectrum.Math.MRM.Data

''' <summary>
''' The linear model of the targeted metabolism model data.(标准曲线模型)
''' </summary>
Public Class StandardCurve : Implements INamedValue

    ''' <summary>
    ''' The metabolite name or database id
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' 该代谢物的线性回归模型
    ''' </summary>
    ''' <returns></returns>
    Public Property linear As IFitted
    Public Property points As MRMStandards()

    Public Property blankControls As Double()

    ''' <summary>
    ''' The internal standards
    ''' </summary>
    ''' <returns></returns>
    Public Property [IS] As [IS]

    ''' <summary>
    ''' This linear model is required calibration by internal standards or not?
    ''' (在进行线性回归计算的时候是否需要内标校正？)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property requireISCalibration As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Not [IS] Is Nothing AndAlso Not [IS].ID.StringEmpty AndAlso [IS].CIS > 0
        End Get
    End Property

    Public ReadOnly Property isWeighted As Boolean
        Get
            Return TypeOf linear Is WeightedFit
        End Get
    End Property

    Public ReadOnly Property isValid As Boolean
        Get
            Return Not linear.CorrelationCoefficient.IsNaNImaginary
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{name}] {linear}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CreateLinearRegression(points As IEnumerable(Of PointF), weighted As Boolean) As IFitted
        Return points.AutoPointDeletion(weighted)
    End Function

End Class
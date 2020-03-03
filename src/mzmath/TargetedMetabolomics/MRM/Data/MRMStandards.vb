Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Namespace MRM.Data

    ''' <summary>
    ''' 表示标准曲线上面的一个实验数据点
    ''' </summary>
    Public Class MRMStandards

        Public Property ID As String
        Public Property Name As String

        ''' <summary>
        ''' 内标峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property AIS As Double
        ''' <summary>
        ''' 当前试验点的标准品峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property Ati As Double
        ''' <summary>
        ''' 内标浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property cIS As Double
        ''' <summary>
        ''' 当前试验点的标准品浓度(Py)
        ''' </summary>
        ''' <returns></returns>
        Public Property Cti As Double

        Public ReadOnly Property Px As Double
            Get
                If AIS = 0.0 Then
                    Return Ati
                Else
                    Return Ati / AIS
                End If
            End Get
        End Property

        Public Property yfit As Double
        Public ReadOnly Property [error] As Double
            Get
                Return stdNum.Abs(stdNum.Round(yfit - Cti, 4))
            End Get
        End Property

        Public ReadOnly Property [variant] As Double
            Get
                Return stdNum.Round([error] / Cti, 4)
            End Get
        End Property

        ''' <summary>
        ''' If the value of this property is false, then it means 
        ''' current reference point is removed from linear modelling.
        ''' </summary>
        ''' <returns></returns>
        Public Property valid As Boolean

        ''' <summary>
        ''' 浓度梯度水平的名称，例如：``L1, L2, L3, ...``
        ''' </summary>
        ''' <returns></returns>
        Public Property level As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"Dim {Name} As {ID} = {Cti}"
        End Function
    End Class
End Namespace
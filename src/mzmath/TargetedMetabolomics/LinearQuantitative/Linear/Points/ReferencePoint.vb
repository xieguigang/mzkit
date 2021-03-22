#Region "Microsoft.VisualBasic::e2f804200a6586a26243dc9826b05339, src\mzmath\TargetedMetabolomics\LinearQuantitative\Linear\Points\ReferencePoint.vb"

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

    '     Class ReferencePoint
    ' 
    '         Properties: [error], [variant], AIS, Ati, cIS
    '                     Cti, ID, level, Name, Px
    '                     valid, yfit
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports stdNum = System.Math

Namespace LinearQuantitative.Linear

    ''' <summary>
    ''' 表示标准曲线上面的一个实验数据点
    ''' </summary>
    Public Class ReferencePoint

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

        <Ignored>
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

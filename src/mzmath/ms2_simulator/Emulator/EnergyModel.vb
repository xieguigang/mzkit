#Region "Microsoft.VisualBasic::bca4bccb8400fe83d6c81b0724f2281c, src\mzmath\ms2_simulator\Emulator\EnergyModel.vb"

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

    ' Class EnergyModel
    ' 
    '     Properties: MaxEnergy, MinEnergy
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Percentage, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Calculus
Imports stdNum = System.Math

''' <summary>
''' 分子的能量分布模型
''' </summary>
Public Class EnergyModel

    Dim model As Func(Of Double, Double)
    Dim energyRange As DoubleRange

    Public ReadOnly Property MinEnergy As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return energyRange.Min
        End Get
    End Property

    Public ReadOnly Property MaxEnergy As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return energyRange.Max
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dist">
    ''' 能量分布函数:
    ''' 
    ''' ```
    ''' energy = f(x,y)
    ''' ```
    ''' </param>
    ''' <param name="lower">能量积分的下限</param>
    ''' <param name="upper">能量积分的上限</param>
    Sub New(dist As Func(Of Double, Double), lower#, upper#)
        energyRange = {lower, upper}
        model = dist
    End Sub

    Public Function Percentage(energy As Double) As Double
        Return model(energy)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return energyRange.ToString
    End Function
End Class

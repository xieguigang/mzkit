#Region "Microsoft.VisualBasic::8e57528ae5d42a648cd160bfbd4a30fe, src\mzmath\ms2_simulator\Emulator\Emulator.vb"

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

' Module Emulator
' 
'     Function: BreakBonds, CalculateMZ, FillBoundEnergy, (+2 Overloads) MolecularFragment
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports NetworkNode = Microsoft.VisualBasic.Data.visualize.Network.Graph.Node
Imports stdNum = System.Math

''' <summary>
''' Generate insilicon MS/MS data based on the GA and graph theory.
''' </summary>
Public Module Emulator

    ''' <summary>
    ''' 会将能量写入<see cref="EdgeData.weight"/>属性之中
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="energyTable"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FillBoundEnergy(model As NetworkGraph, energyTable As BoundEnergyFinder) As NetworkGraph
        For Each bound As Edge In model.graphEdges
            ' edge weight is the bound energy
            Dim atom1 = bound.U.data.label
            Dim atom2 = bound.V.data.label
            ' The bounds number that parsed from KCF model is store in weight value
            Dim energy As Double = energyTable.FindByKCFAtoms(atom1, atom2, bound.data.weight)

            ' The higher of bound energy, the harder for break this bound 
            bound.data.weight = energy
        Next

        Return model
    End Function

    ' 需要一个电压到能量的转换函数
    ' 这部分的转换过程对应的是质谱仪器设备平台相关的模型参数

    ''' <summary>
    ''' 质谱模拟计算
    ''' </summary>
    ''' <param name="molecule"></param>
    ''' <param name="energy">
    ''' 能量的范围值是从碰撞电压转换过来的
    ''' 这个能量参数表示某一个给定的电压下所产生的能量分布
    ''' </param>
    ''' <param name="nintervals">计算能量分布的间隔区间数量</param>
    ''' <param name="precision"></param>
    ''' <param name="intoCutoff">``[0, 1]``, zero or negative value means no cutoff.</param>
    ''' <returns></returns>
    <Extension>
    Public Function MolecularFragment(molecule As NetworkGraph, energy As EnergyModel,
                                      Optional nintervals% = 100,
                                      Optional precision% = 4,
                                      Optional intoCutoff# = -1) As LibraryMatrix

        Dim de# = (energy.MaxEnergy - energy.MinEnergy) / nintervals
        ' {mz, quantity}
        Dim quantity As New Dictionary(Of Double, Double)
        Dim mzlist As New Dictionary(Of String, List(Of Double))

        For e As Double = energy.MinEnergy To energy.MaxEnergy Step de

            ' 将所有能量值低于e的化学键都打断
            ' 则完整的分子图会分裂为多个子图碎片

            ' 使用定积分求出分子能量的分布密度
            ' 分子的能量越高，高于这个能量的分子的百分比应该是越少的？
            Dim percentage# = 1 - energy.PercentageLess(e)
            Dim fragmentModel As NetworkGraph = molecule.BreakBonds(energy:=e)
            Dim fragments = IteratesSubNetworks(Of NetworkNode, Edge, NetworkGraph)(fragmentModel, singleNodeAsGraph:=True).ToArray

            Call $"Break into {fragments.Length} fragments under collision energy {e}".__DEBUG_ECHO
            Call $"Quantile percentage is {(percentage * 100).ToString("F2")}%".__DEBUG_ECHO

            For Each fragment As NetworkGraph In fragments
                Dim mz As Double = fragment.CalculateMZ

                Call $"  -> {mz.ToString("F2")} (m/z)".__DEBUG_ECHO

                With stdNum.Round(mz, precision).ToString
                    If Not quantity.ContainsKey(.ByRef) Then
                        quantity.Add(.ByRef, 0)
                        mzlist.Add(.ByRef, New List(Of Double))
                    End If

                    quantity(.ByRef) = quantity(.ByRef) + percentage
                    mzlist(.ByRef).Add(mz)
                End With
            Next
        Next

        Dim matrix As New LibraryMatrix With {
            .name = "Insilicons Ms/Ms matrix",
            .ms2 = quantity _
                .Select(Function(frag)
                            Dim mz# = mzlist(frag.Key).Average

                            Return New ms2 With {
                                .mz = mz,
                                .quantity = frag.Value
                            }
                        End Function) _
                .ToArray
        }

        ' 进行归一化计算出每一个分子碎片的相对响应度百分比
        matrix = (matrix / Max(matrix)) * 100

        If intoCutoff > 0 Then
            matrix = matrix(matrix!intensity >= intoCutoff).ToArray
        End If

        Return matrix
    End Function

    ''' <summary>
    ''' 所有的节点的质量的总和除以分子碎片电荷量
    ''' </summary>
    ''' <param name="fragment"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CalculateMZ(fragment As NetworkGraph) As Double
        Dim mass# = Aggregate atom As NetworkNode
                    In fragment.vertex
                    Into Sum(atom.data.mass)

        ' 如果计算原子基团的charge电荷量？
        Dim charge# = fragment.AtomGroupCharge

        ' 如果charge值是零的时候该怎么计算？
        ' 中性粒子？
        If charge = 0.0 Then
            ' 默认是丢失一个电子的自身电离化的
            ' [M]+或者[M]-的数据
            Return mass
        Else
            ' 计算出m/z比值结果
            Return mass / charge
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MolecularFragment(molecule As Model.KCF, energy As EnergyModel, Optional step% = 100) As LibraryMatrix
        Return molecule _
            .CreateGraph _
            .FillBoundEnergy(New BoundEnergyFinder) _
            .MolecularFragment(energy, [step])
    End Function

    ''' <summary>
    ''' 将所有的断裂能量值小于<paramref name="energy"/>的边连接都删除掉
    ''' </summary>
    ''' <param name="molecule"></param>
    ''' <param name="energy#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BreakBonds(molecule As NetworkGraph, energy#) As NetworkGraph
        ' 需要做一次复制，因为class是按内存地址引用的，否则后面的计算都会乱的
        Dim copy As NetworkGraph = molecule.Copy

        ' 将键能低于能量值的边链接都删除掉
        ' 因为他们都被打断了
        For Each edge As Edge In copy.graphEdges.ToArray
            If edge.data.weight <= energy Then
                ' 在这里得到化学键的数量
                ' 计算电荷量
                Dim bounds As Integer = edge.data!bounds
                Dim a As NetworkNode = edge.U
                Dim b As NetworkNode = edge.V

                ' 计算电荷量的变化
                ' 电子轰击之后，因为化学键的断裂的能量的来源是具有能量的电子给与的
                ' 两个原子之间产生化学键是因为各自离子状态下会多出或者缺少电子,二者在一起刚好互相补充电子的缺失或者过饱和
                ' 化学键的数量就是这些互补的电子的数量
                ' 当化学键断裂之后, 电子是给了分开的各自的基团对象？
                '
                ' charge assign
                ' 根据人为的定义来填充电子的缺失情况

                a.data!charge = Val(a.data!charge) + bounds
                b.data!charge = Val(b.data!charge) + bounds

                Call copy.RemoveEdge(edge)
            End If
        Next

        Return copy
    End Function
End Module

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.Chemistry
Imports SMRUCC.Chemistry.Model.Graph
Imports SMRUCC.MassSpectrum.Math.MSMS

''' <summary>
''' Generate insilicon MS/MS data based on the GA and graph theory.
''' </summary>
Public Module Emulator

    ''' <summary>
    ''' 质谱模拟计算
    ''' </summary>
    ''' <param name="molecule"></param>
    ''' <param name="energy"></param>
    ''' <param name="step%"></param>
    ''' <param name="precision%"></param>
    ''' <param name="intoCutoff">``[0, 1]``, zero or negative value means no cutoff.</param>
    ''' <returns></returns>
    <Extension>
    Public Function MolecularFragment(molecule As NetworkGraph, energy As EnergyModel,
                                      Optional step% = 100,
                                      Optional precision% = 4,
                                      Optional intoCutoff# = -1) As LibraryMatrix

        Dim de# = (energy.MaxEnergy - energy.MinEnergy) / [step]
        Dim quantity As New Dictionary(Of Double, Double) ' {mz, quantity}
        Dim mzlist As New Dictionary(Of String, List(Of Double))

        For e As Double = energy.MinEnergy To energy.MaxEnergy Step de

            ' 将所有能量值低于e的化学键都打断
            ' 则完整的分子图会分裂为多个子图碎片

            ' 使用定积分求出分子能量的分布密度
            Dim percentage# = energy.PercentageLess(e)
            Dim fragmentModel As NetworkGraph = molecule.BreakBonds(energy:=e)

            For Each fragment As NetworkGraph In {} ' fragmentModel.IteratesSubNetworks
                Dim mz# = fragment.CalculateMZ

                With Math.Round(mz, precision).ToString
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
        Dim mass# = Aggregate atom As Node
                    In fragment.nodes
                    Into Sum(atom.Data.mass)
        Dim charge#

        Return mass / charge
    End Function

    <Extension>
    Public Function MolecularFragment(molecule As Model.KCF, energy As EnergyModel, Optional step% = 100) As LibraryMatrix
        Return molecule.Graph.MolecularFragment(energy, [step])
    End Function

    ''' <summary>
    ''' 将所有的断裂能量值小于<paramref name="energy"/>的边连接都删除掉
    ''' </summary>
    ''' <param name="molecule"></param>
    ''' <param name="energy#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BreakBonds(molecule As NetworkGraph, energy#) As NetworkGraph
        Dim copy As NetworkGraph = molecule.Copy  ' 需要做一次复制，因为class是按内存地址引用的，否则后面的计算都会乱的

        ' 将键能低于能量值的边链接都删除掉
        ' 因为他们都被打断了
        For Each edge As Edge In copy.edges.ToArray
            If edge.Data.weight <= energy Then
                Call copy.RemoveEdge(edge)
            End If
        Next

        Return copy
    End Function
End Module

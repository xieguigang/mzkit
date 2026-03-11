Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Formula

    Public Module ChemicalCalculator

        ''' <summary>
        ''' 1. 构建元素查找表
        ''' </summary>
        ReadOnly elementDb As Dictionary(Of String, Atom) = Atom.DefaultElements.ToDictionary(Function(a) a.label)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalculatePossibleCharges(formula As String) As IEnumerable(Of (Charge As Double, Probability As Double))
            Static chargePool As New Dictionary(Of String, List(Of (Charge As Double, Probability As Double)))

            Return chargePool.ComputeIfAbsent(
                key:=formula,
                lazyValue:=Function(formula_str)
                               Return FormulaScanner.ScanFormula(formula_str).CountsByElement _
                                   .CalculatePossibleCharges
                           End Function)
        End Function

        ''' <summary>
        ''' 计算化学式可能的电荷数及其概率
        ''' </summary>
        ''' <param name="formula">化学式组成字典，如 {"N":1, "H":4}</param>
        ''' <returns>返回按概率降序排列的列表，包含电荷数和概率值</returns>
        <Extension>
        Public Function CalculatePossibleCharges(formula As Dictionary(Of String, Integer)) As List(Of (Charge As Double, Probability As Double))
            ' 临时存储结构：Key=电荷数, Value=最小的惩罚分数
            ' 我们只保留产生该电荷数的“最低成本”路径
            Dim chargeCostMap As New Dictionary(Of Integer, Integer)
            ' 2. 递归/迭代生成所有组合并计算成本
            ' 初始状态：电荷为0，成本为0
            Dim currentStates As New List(Of (Charge As Integer, Cost As Integer)) From {(0, 0)}

            For Each kvp As KeyValuePair(Of String, Integer) In formula
                Dim symbol As String = kvp.Key
                Dim count As Integer = kvp.Value

                If Not elementDb.ContainsKey(symbol) Then
                    Throw New Exception($"元素 {symbol} 未在数据库中定义。")
                End If

                Dim atom As Atom = elementDb(symbol)
                Dim valences As Integer() = atom.valence

                Dim nextStates As New List(Of (Charge As Integer, Cost As Integer))

                ' 遍历当前已有的状态
                For Each state In currentStates
                    ' 遍历当前元素的所有可能化合价
                    For i As Integer = 0 To valences.Length - 1
                        Dim v As Integer = valences(i)

                        ' 计算新的电荷数
                        Dim newCharge As Integer = state.Charge + (v * count)

                        ' 计算新的成本：旧成本 + (原子个数 * 当前化合价索引)
                        ' 索引 i 越小，成本越低，优先级越高
                        Dim newCost As Integer = state.Cost + (count * i)

                        nextStates.Add((newCharge, newCost))
                    Next
                Next

                ' 更新状态机
                currentStates = nextStates
            Next

            ' 3. 合并结果：对于相同的电荷数，只保留最小的成本
            For Each state In currentStates
                If Not chargeCostMap.ContainsKey(state.Charge) Then
                    chargeCostMap.Add(state.Charge, state.Cost)
                Else
                    ' 如果当前路径的成本更低，则更新
                    If state.Cost < chargeCostMap(state.Charge) Then
                        chargeCostMap(state.Charge) = state.Cost
                    End If
                End If
            Next

            ' 4. 将成本转换为概率
            ' 公式：Weight = 1 / (Cost + 1)
            Dim results As New List(Of (Charge As Double, Probability As Double))
            Dim totalWeight As Double = 0.0

            ' 计算总权重
            Dim tempWeights As New List(Of (Charge As Integer, Weight As Double))

            For Each kvp As KeyValuePair(Of Integer, Integer) In chargeCostMap
                Dim cost As Integer = kvp.Value
                ' 成本为0时，权重为1；成本越大，权重越小
                Dim weight As Double = 1.0 / (cost + 1.0)
                tempWeights.Add((kvp.Key, weight))
                totalWeight += weight
            Next

            ' 归一化概率
            For Each item In tempWeights
                Dim prob As Double = item.Weight / totalWeight
                results.Add((item.Charge, prob))
            Next

            ' 5. 按概率降序排序
            Return results.OrderByDescending(Function(r) r.Probability).ToList()
        End Function

    End Module
End Namespace
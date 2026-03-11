Namespace Formula

    Public Module ChemicalCalculator

        ''' <summary>
        ''' 1. 构建元素查找表
        ''' </summary>
        ReadOnly elementDb As Dictionary(Of String, Atom) = Atom.DefaultElements.ToDictionary(Function(a) a.label)

        ''' <summary>
        ''' 计算化学式可能的电荷数集合
        ''' </summary>
        ''' <param name="formula_composition">解析后的元素字典，如 {"N":1, "H":4}</param>
        ''' <returns>可能的电荷数列表</returns>
        Public Function CalculatePossibleCharges(formula_composition As Dictionary(Of String, Integer)) As IEnumerable(Of Integer)
            ' 用于存储所有可能的电荷总和
            ' 初始化为 0，作为累加的基础
            Dim possibleCharges As New List(Of Integer) From {0}

            ' 2. 遍历化学式中的每一个元素
            For Each kvp As KeyValuePair(Of String, Integer) In formula_composition
                Dim symbol As String = kvp.Key
                Dim count As Integer = kvp.Value

                ' 检查元素是否存在于数据库中
                If Not elementDb.ContainsKey(symbol) Then
                    Throw New Exception($"元素 {symbol} 未在数据库中定义。")
                End If

                Dim atom As Atom = elementDb(symbol)
                Dim atomValences As Integer() = atom.valence

                ' 临时列表存储本轮计算后的所有可能结果
                Dim nextPossibleCharges As New List(Of Integer)

                ' 3. 组合运算 (笛卡尔积逻辑)
                ' 当前的每一种可能电荷值 * 当前元素的每一种可能化合价 * 原子个数
                For Each currentTotal As Integer In possibleCharges
                    For Each v As Integer In atomValences
                        nextPossibleCharges.Add(currentTotal + (v * count))
                    Next
                Next

                ' 更新结果列表，进入下一轮循环
                possibleCharges = nextPossibleCharges
            Next

            ' 4. 去重并排序
            Return possibleCharges.Distinct().OrderBy(Function(x) x)
        End Function

    End Module
End Namespace
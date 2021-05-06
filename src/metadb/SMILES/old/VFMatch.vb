Public Class VFMatch

    ''' <summary>
    ''' query和db图   dbG=databaseGraph
    ''' </summary>
    Private quG As ChemicalFormula
    Private dbG As ChemicalFormula

    ''' <summary>
    ''' Ms用于存放空间状态点，也应该是个Map才对
    ''' 由于匹配过程，一一对应，不会出现一对多，所以mapde 键是元素是可以的
    ''' </summary>
    Private Ms As IDictionary(Of ChemicalElement, ChemicalElement)

    Public Sub New(quG As ChemicalFormula, dbG As ChemicalFormula)
        Me.quG = quG
        Me.dbG = dbG
    End Sub

    ''' <param name="state">  临时的态 </param>
    ''' <returns>         boolean </returns>
    Public Function Match(state As Dictionary(Of ChemicalElement, ChemicalElement)) As Boolean
        Dim flag = False
        'Mqu 已经在state中的query元素  Mdb 已经在state中的db元素
        Dim quG_Mid0 As ISet(Of ChemicalElement) = state.Keys
        Dim Mqu As List(Of ChemicalElement) = New List(Of ChemicalElement)(quG_Mid0)
        Dim dbG_Mid0 = state.Values
        Dim Mdb As List(Of ChemicalElement) = New List(Of ChemicalElement)(dbG_Mid0)
        'IF M（S）cover all the nodes of G2
        If Mqu.Count <> 0 AndAlso Mqu.Count = quG.formula.Count Then
            Console.WriteLine("到了最后+++++++++++++++++++++++++++++++++++++")
            flag = True
        Else
            'compute the candidate of inclusion of Ms
            Dim P As IDictionary(Of Integer?, List(Of ChemicalElement)) = New Dictionary(Of Integer?, List(Of ChemicalElement))()
            CandidateP(P, Mqu, Mdb)
            Console.WriteLine("计算完成候选的结果")
            Dim entry As ISet(Of Integer?) = P.Keys
            'Foreach p in P:
            For Each number As Integer In entry
                Dim que As ChemicalElement = P.GetValueOrNull(CType(number, Integer?)).[Get](0) 'query
                Dim dbe As ChemicalElement = P.GetValueOrNull(CType(number, Integer?)).[Get](1) 'db
                'IF the feasibility rules succeed for inclusion of p in Ms
                If FeasibilityRules(que, dbe, Mqu, Mdb, state) Then
                    'compute the state s'
                    state(que) = dbe '添加进去
                    'call match(s')
                    flag = Match(state)
                    state.Remove(que) '回溯，要删除之前的状态
                End If

                If flag = True Then
                    Exit For '找到一个解，就返回解，不再探索
                End If
            Next
            '从这里出来，就是说明没有那走到最后，所以返回
        End If

        Return flag
    End Function

    ''' <summary>
    ''' 搜索的空间P Candidate Pair Set </summary>
    ''' <param name="result"> 所有可能性集合容器 </param>
    ''' <param name="Mqu">  M_query </param>
    ''' <param name="Mdb">  M_database     </param>
    Public Sub CandidateP(result As IDictionary(Of Integer?, List(Of ChemicalElement)), Mqu As List(Of ChemicalElement), Mdb As List(Of ChemicalElement))
        Dim count = 0

        For Each ce In quG.formula

            If Not Mqu.Contains(ce) Then 'query元素没被选的元素
                For Each qe In dbG.formula

                    If Not Mdb.Contains(qe) Then 'db元素，没被选的元素
                        Console.Write(ce.label & "-" & qe.label & " ")
                        Dim res As List(Of ChemicalElement) = New List(Of ChemicalElement)()
                        res.Add(ce)
                        res.Add(qe)
                        result(count) = res
                        count += 1
                    End If
                Next
            End If
        Next
    End Sub

    ''' <param name="quG"> query </param>
    ''' <param name="dbG"> dataBase </param>
    ''' <param name="Mqu">  已经加入的结点集合 </param>
    ''' <param name="Mdb"> </param>
    ''' <param name="match"> 已经匹配好的对象
    ''' @return </param>
    Private Shared Function FeasibilityRules(quG As ChemicalElement, dbG As ChemicalElement, Mqu As List(Of ChemicalElement), Mdb As List(Of ChemicalElement), match As IDictionary(Of ChemicalElement, ChemicalElement)) As Boolean
        If Not quG.label.Equals(dbG.label) Then '两个元素名称是否一致
            Return False
        End If

        If quG.environments_2.Count > dbG.environments_2.Count Then 'quG的度要小于等于dbG的度
            Return False
        End If

        If Mqu.Count = 0 Then '如果是第一对元素，
            Console.WriteLine("   是第一个元素")
            Return True
        End If
        '接下来检测，M中是否已经有跟quG相连的元素，如果相连，必须满足条件：
        '1）quG_vID和dbG_vID与已经match的那些节点对中的【至少】一对(quVid,dbVid)分别相邻（quG_vID和dbG_vID分别是已经match的节点quVid和dbVid的"neighbor节点"）
        '2）如果存在多个相邻对(quVid,dbVid)，则必须要求【所有的】邻接边对( edge(quG_vID,quVid), edge(dbG_vID,dbVid) )的label一样

        For Each quG_Mid In Mqu

            If quG_Mid.environments_2.Contains(quG) Then 'quG_Mid是quG的连接元素
                Dim dbG_Mid = match.GetValueOrNull(quG_Mid) '映射的元素

                If Not dbG_Mid.environments_2.Contains(dbG) Then '映射元素dbG_Mid与dbG之间没有连接
                    Return False
                End If

                ' *
                ' 下面是需要改正的，键对象创建的太多了。
                ' 需要把键当成元素的属性，目前是元素是键的属性，用一个map比较好

                If Not quG.getKeyy(quG_Mid).key.Equals(dbG.getKeyy(dbG_Mid).key) Then '同一个键对象
                    Return False
                End If
            End If
        Next
        '从这里出来，
        '1)可能是不存在与Mqu相连接的情况，因此 return true
        '2) 存在于Mqu相连接的情况，且验证符合
        Return True
    End Function
End Class
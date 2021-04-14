
''' <summary>
''' The SMILES string parser
''' </summary>
''' <remarks>
''' https://blog.csdn.net/tortelee/article/details/85782913
''' </remarks>
Public Class ParseChain

    ''' <summary>
    ''' 待解析的字符串对象
    ''' </summary>
    Dim smiles As String

    ''' <summary>
    ''' 解析字符串的位置标记
    ''' </summary>
    Dim position As Integer

    ''' <summary>
    ''' 解析后存放的位置
    ''' cmf：化学式存放
    ''' MyElements_2 以前的list存放
    ''' </summary>
    Public ReadOnly Property myElements As List(Of ChemicalElement)
    Public ReadOnly Property cmf As ChemicalFormula

    ''' <summary>
    ''' MyElement 要删除的
    ''' 构造函数 </summary>
    ''' <param name="smiles"> </param>
    Public Sub New(smiles As String)
        Me.smiles = smiles.ToUpper()
        myElements = New List(Of ChemicalElement)()
        cmf = New ChemicalFormula()
    End Sub

    Public Function Parsing() As ChemicalElement
        Dim smilesList = smiles.StringSplit("", True) '将字符串转成数组
        '第一步，完成图示解C步骤
        Dim st = subParsing(smilesList, Nothing, Nothing)
        '第二步，从C步骤，完成B步骤，对数字相同的进行拼凑；断键合并
        Combine()
        cmf.formula = myElements '解析完，将元素存到化学键中
        Return st
    End Function

    Private Function subParsing(smileList As String(), formerElement As ChemicalElement, formerKey As ChemicalKey) As ChemicalElement
        '头元素，记录信息
        Dim start As ChemicalElement = Nothing

        While position < smileList.Length
            Console.Write(smileList(position))
            '是否是数字，表示断键
            If position < smileList.Length AndAlso Char.IsDigit(smileList(position)(0)) Then
                formerElement.mark = Microsoft.VisualBasic.AscW(smileList(position)(0)) '旧-将数字加进去
                formerElement.disconnectKey = formerKey '将断键的类型加上去
                formerKey = New ChemicalKey("-") '将标记键置为默认
                position += 1
                '是键
            ElseIf isKey(smileList(position)) Then
                formerKey = New ChemicalKey(smileList(position)) '将键对象保存
                position += 1
                '是元素
            ElseIf isElement(smileList(position)) Then
                Dim current As ChemicalElement = Nothing

                If smileList(position).Equals("[") Then '离子情形
                    Dim Ion As List(Of String) = New List(Of String)()

                    For i = position To smileList.Length - 1

                        If smileList(position).Equals("]") Then '结束的情形
                            position += 1
                            Exit For
                        End If

                        If smileList(position).Equals("[") OrElse smileList(position).Equals("+") OrElse smileList(position).ToUpper().Equals("H") Then
                            Continue For
                        End If

                        Ion.Add(smileList(position))
                    Next

                    current = New ChemicalElement(Ion.ToString(), 0)
                Else
                    current = New ChemicalElement(smileList(position), 0) '旧-将元素添加进去
                End If

                myElements.Add(current) '新-将元素添加到MyElements_2

                If start Is Nothing Then '第一个元素
                    start = current
                End If

                If formerElement Is Nothing Then '旧-前面是空的，第一个元素情形;此时，键也应该是空的
                    formerElement = current
                    formerKey = New ChemicalKey("-")
                Else

                    '新 添加元素
                    formerKey.setTarget(current, formerElement) '键 ---(m,n)
                    formerElement.addToKeys(formerKey, current) '元素---添加键
                    current.addToKeys(formerKey, formerElement)

                    '旧-更新前一个元素；更新键值
                    formerElement = current
                    formerKey = New ChemicalKey("-")
                End If

                position += 1
                '括号内（） 迭代，返回头元素
            ElseIf smileList(position).Equals("(") Then
                '旧-此时第一个元素应该被外面的吸纳,其他的循环就好；此时前置的元素依然不能变，即使出现了）符号，这个也是不能变的。
                position += 1
                '判断后面的元素是不是键，如果是，要提前存起来，更新。
                If isKey(smileList(position)) Then
                    formerKey = New ChemicalKey(smileList(position))
                End If

                Dim ce = subParsing(smileList, Nothing, Nothing)

                If formerElement IsNot Nothing Then '括号前面的部分处理

                    '新的添加方式
                    formerKey.setTarget(ce, formerElement)
                    formerElement.addToKeys(formerKey, ce)
                    ce.addToKeys(formerKey, formerElement)
                Else
                    formerElement = ce
                    formerKey = New ChemicalKey("-")
                End If
                '括号后面部分的处理
                formerKey = New ChemicalKey("-")
                position += 1
            ElseIf smileList(position).Equals(")") Then
                ' 这里不对位置进行操作 留在109处理
                ' 但是要对formerkey做处理 留在109 循环出来后
                Return start
            End If
        End While

        Return start
    End Function

    ''' <summary>
    ''' 判断是否是键
    ''' </summary>
    Friend allKeys As String() = ChemicalKey.allKeys

    Public Function isKey(element As String) As Boolean
        For Each c In allKeys

            If element.Equals(c) Then
                '  System.out.println("是键");
                Return True
            End If
        Next

        Return False
    End Function
    '判断是否是元素  
    '两位的元素没搞，比如Br Cl
    Friend elements As String() = ChemicalElement.allElement

    Public Function isElement(element As String) As Boolean
        For Each s In elements

            If element.Equals(s) Then
                '   System.out.println("是元素");
                Return True
            End If

            If element.Equals("[") Then
                Return True
            End If
        Next

        Return False
    End Function

    '将断开的键合起来
    ' 断键信息1111 这种形式标记的 没搞
    Private Sub Combine()
        Dim maxNumber = 0

        For Each ce In myElements

            If ce.mark > maxNumber Then
                maxNumber = ce.mark
            End If
        Next

        For i = 0 To myElements.Count - 1

            If myElements(i).mark <> 0 Then
                For j = i + 1 To myElements.Count - 1

                    If myElements(i).mark = myElements(j).mark Then
                        myElements(i).disconnectKey.setTarget(myElements(i), myElements(j))
                        myElements(i).addToKeys(myElements(i).disconnectKey, myElements(j))
                        myElements(j).addToKeys(myElements(i).disconnectKey, myElements(i))
                        Exit For
                    End If
                Next
            End If
        Next
    End Sub
End Class

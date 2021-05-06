Public Class ChemicalElement

    ''' <summary>
    ''' 周围的元素和相应的键
    ''' </summary>
    Dim Keys As ChemicalKey()

    ''' <summary>
    ''' 获得元素名称
    ''' @return
    ''' </summary>
    Public ReadOnly Property label As String

    ''' <summary>
    ''' 设置断键的数字，这里假设一个原SMILES表示最多一个断键
    ''' </summary>
    ''' <returns></returns>
    Public Property mark As Integer

    ''' <summary>
    ''' 是元素的个数
    ''' </summary>
    Public count As Integer = 0

    ''' <summary>
    ''' 返回，list,周围的元素
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property environments_2 As List(Of ChemicalElement)

    ''' <summary>
    ''' 返回所有的键
    ''' @return
    ''' </summary>
    Public ReadOnly Property keys_2 As List(Of ChemicalKey)

    ''' <summary>
    ''' 设置断键 断开的键的类型
    ''' </summary>
    ''' <returns></returns>
    Public Property disconnectKey As ChemicalKey

    Public Shared ReadOnly Property allElement As String()
        Get
            Dim allElements = New String(99) {}
            allElements(0) = "C"
            allElements(1) = "N"
            allElements(2) = "O"
            allElements(3) = "F"
            allElements(4) = "Br"
            allElements(5) = "Cl"
            allElements(6) = "H"
            allElements(7) = "S"

            Return allElements
        End Get
    End Property

    Public Sub New(label As String, mark As Integer)
        Me.label = label
        Me.mark = mark
        Keys = New ChemicalKey(3) {}
        keys_2 = New List(Of ChemicalKey)()
        environments_2 = New List(Of ChemicalElement)()
    End Sub

    ''' <summary>
    ''' 添加key的方法，同时把键连接的元素也添加进去 </summary>
    ''' <param name="key"> </param>
    Public Sub addToKeys(key As ChemicalKey, ce As ChemicalElement)
        keys_2.Add(key)
        environments_2.Add(ce)
    End Sub

    ''' <summary>
    ''' 获得与之相连元素的键对象
    ''' </summary>
    ''' <param name="ce"></param>
    ''' <returns></returns>
    Public Function getKeyy(ce As ChemicalElement) As ChemicalKey
        For Each ck As ChemicalKey In keys_2
            If ck.chemicalElements.Contains(ce) Then
                Return ck
            End If
        Next

        Return Nothing
    End Function

    Public Overrides Function ToString() As String
        Return label
    End Function
End Class

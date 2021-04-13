Imports System
Imports System.Collections.Generic

Namespace com.Demo2
    Public Class ChemicalElement
        '元素名称
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private label_Renamed As String
        '元素的整体序号
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private mark_Renamed As Integer
        '周围的元素和相应的键

        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private environments_2_Renamed As List(Of ChemicalElement)
        Private Keys As ChemicalKey()
        Private Keys_2Field As List(Of ChemicalKey)
        '是元素的个数
        Public count As Integer = 0
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private Shared allElement_Renamed As String()
        '断开的键的类型
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private disconnectKey_Renamed As ChemicalKey

        Public Sub New(ByVal label As String, ByVal mark As Integer)
            label_Renamed = label
            mark_Renamed = mark
            Keys = New ChemicalKey(3) {}
            Keys_2Field = New List(Of)()
            environments_2_Renamed = New List(Of)()
        End Sub




        '设置断键
        Public Overridable Property disconnectKey As ChemicalKey
            Set(ByVal value As ChemicalKey)
                disconnectKey_Renamed = value
            End Set
            Get
                Return disconnectKey_Renamed
            End Get
        End Property
        '设置断键的数字，这里假设一个原SMILES表示最多一个断键
        Public Overridable Property mark As Integer
            Set(ByVal value As Integer)
                mark_Renamed = value
            End Set
            Get
                Return mark_Renamed
            End Get
        End Property
        ''' <summary>
        ''' 添加key的方法，同时把键连接的元素也添加进去 </summary>
        ''' <paramname="key"> </param>
        Public Overridable Sub addToKeys(ByVal key As ChemicalKey, ByVal ce As ChemicalElement)
            Keys_2Field.Add(key)
            environments_2_Renamed.Add(ce)
        End Sub
        '返回，list,周围的元素
        Public Overridable ReadOnly Property environments_2 As List(Of ChemicalElement)
            Get
                Return environments_2_Renamed
            End Get
        End Property

        ''' <summary>
        ''' 返回所有的键
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property keys_2 As List(Of ChemicalKey)
            Get
                Return Keys_2Field
            End Get
        End Property

        ''' <summary>
        ''' 获得元素名称
        ''' @return
        ''' </summary>
        Public Overridable ReadOnly Property label As String
            Get
                Return label_Renamed
            End Get
        End Property
        '获得与之相连元素的键对象
        Public Overridable Function getKeyy(ByVal ce As ChemicalElement) As ChemicalKey
            For Each ck In Keys_2Field

                If ck.chemicalElements.Contains(ce) Then '某个键，包含这个元素,返回这个键的对象
                    Console.WriteLine("!!!!!检测到元素在键:" & ck.key & "中")
                    Return ck
                End If
            Next

            Return Nothing
        End Function

        Public Shared Sub Main(ByVal args As String())
            Dim c1 As ChemicalElement = New ChemicalElement("H", 0)
        End Sub

        Private Shared Sub SetAllElement()
            allElement_Renamed = New String(99) {}
            allElement_Renamed(0) = "C"
            allElement_Renamed(1) = "N"
            allElement_Renamed(2) = "O"
            allElement_Renamed(3) = "F"
            allElement_Renamed(4) = "Br"
            allElement_Renamed(5) = "Cl"
            allElement_Renamed(6) = "H"
            allElement_Renamed(7) = "S"
        End Sub

        Public Shared ReadOnly Property allElement As String()
            Get
                Call SetAllElement()
                Return allElement_Renamed
            End Get
        End Property
    End Class
End Namespace

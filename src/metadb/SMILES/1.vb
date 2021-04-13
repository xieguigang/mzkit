Imports System
Imports System.Collections.Generic

Namespace com.Demo2
    Public Class ChemicalKey
        '键的名称
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private key_Renamed As String
        '键的指向的对象
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Private chemicalElements_Renamed As List(Of ChemicalElement)
        'JAVA TO C# CONVERTER CRACKED BY X-CRACKER NOTE: Fields cannot have the same name as methods:
        Public Shared allKeys_Renamed As String()
        '构造函数
        Public Sub New(ByVal key As String)
            key_Renamed = key
            chemicalElements_Renamed = New List(Of ChemicalElement)(2)
        End Sub

        ''' <summary>
        ''' 键,两边的元素
        ''' @param
        ''' </summary>
        Public Overridable Sub setTarget(ByVal target1 As ChemicalElement, ByVal target2 As ChemicalElement)
            Try
                chemicalElements_Renamed.Add(target1)
                chemicalElements_Renamed.Add(target2)
            Catch __unusedException1__ As Exception
                Console.WriteLine("键连接的元素超过2个了！")
            End Try
        End Sub
        '获得键两边元素
        Public Overridable ReadOnly Property chemicalElements As List(Of ChemicalElement)
            Get
                Return chemicalElements_Renamed
            End Get
        End Property

        Public Overridable Sub print()
            For i = 0 To 2 - 1
                Console.Write(chemicalElements_Renamed(i).label)
                Console.Write("-"c)
            Next

            Console.WriteLine()
        End Sub
        '把一些常见的键，存起来，String形式，不是对象
        Private Shared Sub SetterAllKeys()
            allKeys_Renamed = New String(5) {}
            allKeys_Renamed(0) = "."
            allKeys_Renamed(1) = "-"
            allKeys_Renamed(2) = "="
            allKeys_Renamed(3) = "#"
            allKeys_Renamed(4) = "/"
            allKeys_Renamed(5) = "\\"
        End Sub

        Public Overridable ReadOnly Property key As String
            Get
                Return key_Renamed
            End Get
        End Property

        ''' <summary>
        ''' 获得所有的Key字符
        ''' @return
        ''' </summary>
        Public Shared ReadOnly Property allKeys As String()
            Get
                Call SetterAllKeys()
                Return allKeys_Renamed
            End Get
        End Property
    End Class
End Namespace

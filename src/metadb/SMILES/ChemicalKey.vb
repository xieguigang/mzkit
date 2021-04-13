
Public Class ChemicalKey

    ''' <summary>
    ''' 键的名称
    ''' </summary>
    Public Overridable ReadOnly Property key As String

    ''' <summary>
    ''' 获得键两边元素
    ''' </summary>
    ''' <returns>键的指向的对象</returns>
    Public Overridable ReadOnly Property chemicalElements As List(Of ChemicalElement)

    ''' <summary>
    ''' 获得所有的Key字符
    ''' @return
    ''' </summary>
    ''' <remarks>
    ''' 把一些常见的键，存起来，String形式，不是对象
    ''' </remarks>
    Public Shared ReadOnly Property allKeys As String()
        Get
            Dim allKeys_Renamed = New String(5) {}
            allKeys_Renamed(0) = "."
            allKeys_Renamed(1) = "-"
            allKeys_Renamed(2) = "="
            allKeys_Renamed(3) = "#"
            allKeys_Renamed(4) = "/"
            allKeys_Renamed(5) = "\\"
            Return allKeys_Renamed
        End Get
    End Property

    Public Sub New(key As String)
        Me.key = key
        Me.chemicalElements = New List(Of ChemicalElement)(2)
    End Sub

    ''' <summary>
    ''' 键,两边的元素
    ''' @param
    ''' </summary>
    Public Overridable Sub setTarget(target1 As ChemicalElement, target2 As ChemicalElement)
        Try
            chemicalElements.Add(target1)
            chemicalElements.Add(target2)
        Catch __unusedException1__ As Exception
            Console.WriteLine("键连接的元素超过2个了！")
        End Try
    End Sub

    Public Overridable Sub print()
        For i = 0 To 2 - 1
            Console.Write(chemicalElements(i).label)
            Console.Write("-"c)
        Next

        Console.WriteLine()
    End Sub
End Class
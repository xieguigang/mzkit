Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Public Class ChemicalElement : Inherits Node

    Public Property elementName As String

    ''' <summary>
    ''' 与当前的这个元素连接的化学键的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Keys As Integer
        Get
            Return degree.In + degree.Out
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(element As String)
        Me.label = App.GetNextUniqueName($"{element}_")
        Me.elementName = element
    End Sub

End Class
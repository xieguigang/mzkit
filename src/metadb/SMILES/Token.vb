Imports Microsoft.VisualBasic.Scripting.TokenIcer

Public Class Token : Inherits CodeToken(Of ElementTypes)

    Sub New(name As ElementTypes, text As String)
        Call MyBase.New(name, text)
    End Sub
End Class

Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace NaturalProduct

    Public Class Token : Inherits CodeToken(Of NameTokens)

        Public Sub New(name As NameTokens, value As String)
            MyBase.New(name, value)
        End Sub
    End Class

    Public Enum NameTokens
        ''' <summary>
        ''' invalid component name scanner token
        ''' </summary>
        na
        ''' <summary>
        ''' glycosyl
        ''' </summary>
        name
        ''' <summary>
        ''' (
        ''' </summary>
        open
        ''' <summary>
        ''' )
        ''' </summary>
        close
        ''' <summary>
        ''' <see cref="QuantityPrefix"/>
        ''' </summary>
        number
    End Enum
End Namespace
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class ReactionClass : Implements INamedValue

    Public Property rId As String Implements INamedValue.Key
    Public Property category As Integer
    Public Property from As String
    Public Property [to] As String
    Public Property define As String

End Class

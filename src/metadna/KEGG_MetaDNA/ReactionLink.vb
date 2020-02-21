Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class ReactionLink : Implements INamedValue
    Public Property rxnID As String Implements INamedValue.Key
    Public Property name As String
    Public Property define As String
    Public Property reactants As String()
    Public Property products As String()
End Class

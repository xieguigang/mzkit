Namespace Network.htmlwidget

    Public Class JSON
        Public Property x As NetGraph
    End Class

    Public Class NetGraph
        Public Property links As Links
        Public Property nodes As Nodes
        Public Property options As Options
    End Class

    Public Class Options
        Public Property NodeID As String
        Public Property Group As String
        Public Property colourScale As String
        Public Property fontSize As Integer
        Public Property fontFamily As String
        Public Property clickTextSize As Integer
        Public Property linkDistance As Integer
        Public Property linkWidth As String
        Public Property charge As Integer
        Public Property opacity As Double
        Public Property zoom As Boolean
        Public Property legend As Boolean
        Public Property nodesize As Boolean
        Public Property radiusCalculation As String
        Public Property bounded As Boolean
        Public Property opacityNoHover As Double
        Public Property clickAction As String
    End Class

    Public Class Links
        Public Property source As Integer()
        Public Property target As Integer()
        Public Property colour As String()
    End Class

    Public Class Nodes
        Public Property name As String()
        Public Property group As Integer()
    End Class
End Namespace
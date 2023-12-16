Public Class GroupMass

    ''' <summary>
    ''' 5' or 3'
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [end] As Integer
    Public ReadOnly Property name As String
    Public ReadOnly Property mass As Double

    Sub New(ends As Integer, name As String, mass As Double)
        Me.[end] = ends
        Me.name = name
        Me.mass = mass
    End Sub

    Public Shared Iterator Function MonoisotopicMass() As IEnumerable(Of GroupMass)
        Yield New GroupMass(5, "p-", 79.966331)
        Yield New GroupMass(5, "HO-", 17.00274)
        Yield New GroupMass(5, "Cap-", 484.0635)
        Yield New GroupMass(3, "-H", 1.007825)
    End Function

    Public Shared Iterator Function AverageMass() As IEnumerable(Of GroupMass)
        Yield New GroupMass(5, "p-", 79.979902)
        Yield New GroupMass(5, "HO-", 17.00734)
        Yield New GroupMass(5, "Cap-", 484.2764)
        Yield New GroupMass(3, "-H", 1.00794)
    End Function

End Class

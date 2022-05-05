Public Class Chain

    Public Property tag As String
    Public Property carbons As Integer
    Public Property doubleBonds As Integer
    Public Property position As BondPosition()
    Public Property groups As Group()

    Public ReadOnly Property hasStructureInfo As Boolean
        Get
            Return Not (position.IsNullOrEmpty AndAlso groups.IsNullOrEmpty AndAlso tag.StringEmpty)
        End Get
    End Property

    Public Overrides Function ToString() As String
        If hasStructureInfo Then
            Throw New NotImplementedException
        Else
            Return $"{carbons}:{doubleBonds}"
        End If
    End Function

    Friend Shared Function ParseName(components As String) As Chain
        Dim overview As String() = components.Match(".+[:].+").Split(":"c)
        Dim carbons As Integer
        Dim tag As String = Nothing

        components = components.Replace(overview.JoinBy(":"), "")

        If overview(Scan0).IsInteger Then
            carbons = Integer.Parse(overview(Scan0))
        Else
            tag = overview(Scan0).StringReplace("\d+", "")
            carbons = Integer.Parse(overview(Scan0).Match("\d+"))
        End If

        Dim DBes As Integer = Integer.Parse(overview(1))
        Dim bonds As BondPosition() = BondPosition.ParseStructure(components).ToArray

        Return New Chain With {
            .carbons = carbons,
            .tag = tag,
            .doubleBonds = DBes,
            .groups = (From b As BondPosition
                       In bonds
                       Where TypeOf b Is Group
                       Select DirectCast(b, Group)).ToArray,
            .position = bonds _
                .Where(Function(b) Not TypeOf b Is Group) _
                .ToArray
        }
    End Function
End Class

Public Class Group : Inherits BondPosition

    Public Property groupName As String

End Class

Public Class BondPosition

    Public Property index As Integer
    ''' <summary>
    ''' E/Z
    ''' </summary>
    ''' <returns></returns>
    Public Property [structure] As Char

    Friend Shared Iterator Function ParseStructure(components As String) As IEnumerable(Of BondPosition)
        If components = "" Then
            Return
        End If

    End Function
End Class
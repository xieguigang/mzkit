Public NotInheritable Class DoubleBondIndeterminateState
    Public Shared ReadOnly Property AllPositions As DoubleBondIndeterminateState = New DoubleBondIndeterminateState(State.PositionExclude, Nothing)
    Public Shared ReadOnly Property AllCisTransIsomers As DoubleBondIndeterminateState = New DoubleBondIndeterminateState(State.CisTransIsomerExclude, Nothing)
    Public Shared ReadOnly Property Identity As DoubleBondIndeterminateState = New DoubleBondIndeterminateState(State.None, Nothing)

    Private ReadOnly _state As State
    Private ReadOnly _positions As Integer()

    Private Sub New(state As State, positions As Integer())
        _state = state
        _positions = positions
    End Sub

    Public Function Indeterminate(infos As IReadOnlyCollection(Of IDoubleBondInfo)) As IReadOnlyCollection(Of IDoubleBondInfo)
        If infos Is Nothing Then
            Throw New ArgumentNullException(NameOf(infos))
        End If

        Select Case _state
            Case State.PositionInclude
                If _positions Is Nothing Then
                    Return infos
                End If
                Return infos.Where(Function(info) Not _positions.Contains(info.Position)).ToList()
            Case State.PositionExclude
                If _positions Is Nothing Then
                    Return New IDoubleBondInfo(-1) {}
                End If
                Return infos.Where(Function(info) _positions.Contains(info.Position)).ToList()
            Case State.CisTransIsomerInclude
                If _positions Is Nothing OrElse infos.All(Function(info) Not info.Determined) Then
                    Return infos
                End If
                Return infos.[Select](Function(info) If(_positions.Contains(info.Position) AndAlso info.Determined, DoubleBondInfo.Create(info.Position), info)).ToArray()
            Case State.CisTransIsomerExclude
                If infos.All(Function(info) Not info.Determined) Then
                    Return infos
                End If
                If _positions Is Nothing Then
                    Return infos.[Select](Function(info) If(info.Determined, DoubleBondInfo.Create(info.Position), info)).ToArray()
                End If
                Return infos.[Select](Function(info) If(Not _positions.Contains(info.Position) AndAlso info.Determined, DoubleBondInfo.Create(info.Position), info)).ToArray()
        End Select
        Return infos
    End Function

    Public Function Include(position As Integer) As DoubleBondIndeterminateState
        Select Case _state
            Case State.PositionInclude, State.CisTransIsomerInclude
                If _positions Is Nothing Then
                    Return New DoubleBondIndeterminateState(_state, {position})
                End If
                If Not _positions.Contains(position) Then
                    Return New DoubleBondIndeterminateState(_state, _positions.Append(position).ToArray())
                End If
            Case State.PositionExclude, State.CisTransIsomerExclude
                If _positions IsNot Nothing AndAlso _positions.Contains(position) Then
                    Return New DoubleBondIndeterminateState(_state, _positions.Where(Function(p) p <> position).ToArray())
                End If
        End Select
        Return Me
    End Function

    Public Function Exclude(position As Integer) As DoubleBondIndeterminateState
        Select Case _state
            Case State.PositionInclude, State.CisTransIsomerInclude
                If _positions IsNot Nothing AndAlso _positions.Contains(position) Then
                    Return New DoubleBondIndeterminateState(_state, _positions.Where(Function(p) p <> position).ToArray())
                End If
            Case State.PositionExclude, State.CisTransIsomerExclude
                If _positions Is Nothing Then
                    Return New DoubleBondIndeterminateState(_state, {position})
                End If
                If Not _positions.Contains(position) Then
                    Return New DoubleBondIndeterminateState(_state, _positions.Append(position).ToArray())
                End If
        End Select
        Return Me
    End Function

    Public Function AsVisitor() As IVisitor(Of IDoubleBond, IDoubleBond)
        Return New DoubleBondIndeterminateVisitor(Me)
    End Function

    Public Shared Function Positions(ParamArray pPositions As Integer()) As DoubleBondIndeterminateState
        Return New DoubleBondIndeterminateState(State.PositionInclude, pPositions)
    End Function

    Public Shared Function PositionsExclude(ParamArray positions As Integer()) As DoubleBondIndeterminateState
        Return New DoubleBondIndeterminateState(State.PositionExclude, positions)
    End Function

    Public Shared Function CisTransIsomer(ParamArray positions As Integer()) As DoubleBondIndeterminateState
        Return New DoubleBondIndeterminateState(State.CisTransIsomerInclude, positions)
    End Function

    Public Shared Function CisTransIsomerExclude(ParamArray positions As Integer()) As DoubleBondIndeterminateState
        Return New DoubleBondIndeterminateState(State.CisTransIsomerExclude, positions)
    End Function

    Friend Enum State
        None
        PositionInclude
        PositionExclude
        CisTransIsomerInclude
        CisTransIsomerExclude
    End Enum

    Friend NotInheritable Class DoubleBondIndeterminateVisitor
        Implements IVisitor(Of IDoubleBond, IDoubleBond)
        Private ReadOnly _state As DoubleBondIndeterminateState

        Public Sub New(state As DoubleBondIndeterminateState)
            _state = state
        End Sub

        Private Function Visit(item As IDoubleBond) As IDoubleBond Implements IVisitor(Of IDoubleBond, IDoubleBond).Visit
            Return item.Indeterminate(_state)
        End Function
    End Class
End Class


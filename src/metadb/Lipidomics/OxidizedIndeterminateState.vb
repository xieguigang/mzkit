Imports CompMs.Common.DataStructure
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public NotInheritable Class OxidizedIndeterminateState
        Public Shared ReadOnly Property AllPositions As OxidizedIndeterminateState = New OxidizedIndeterminateState(State.PositionExclude, Nothing)
        Public Shared ReadOnly Property Identity As OxidizedIndeterminateState = New OxidizedIndeterminateState(State.None, Nothing)

        Private ReadOnly _state As State
        Private ReadOnly _positions As Integer()

        Private Sub New(ByVal state As State, ByVal positions As Integer())
            _state = state
            _positions = positions
        End Sub

        Public Function Indeterminate(ByVal infos As IReadOnlyCollection(Of Integer)) As IReadOnlyCollection(Of Integer)
            If infos Is Nothing Then
                Throw New ArgumentNullException(NameOf(infos))
            End If

            Select Case _state
                Case State.PositionInclude
                    If _positions Is Nothing Then
                        Return infos
                    End If
                    Return infos.Where(Function(info) Not _positions.Contains(info)).ToArray()
                Case State.PositionExclude
                    If _positions Is Nothing Then
                        Return New Integer(-1) {}
                    End If
                    Return infos.Where(Function(info) _positions.Contains(info)).ToArray()
            End Select
            Return infos
        End Function

        Public Function Include(ByVal position As Integer) As OxidizedIndeterminateState
            Select Case _state
                Case State.PositionInclude
                    If _positions Is Nothing Then
                        Return New OxidizedIndeterminateState(_state, {position})
                    ElseIf Not _positions.Contains(position) Then
                        Return New OxidizedIndeterminateState(_state, _positions.Append(position).ToArray())
                    End If
                Case State.PositionExclude
                    If _positions IsNot Nothing AndAlso _positions.Contains(position) Then
                        Return New OxidizedIndeterminateState(_state, _positions.Where(Function(p) p <> position).ToArray())
                    End If
            End Select
            Return Me
        End Function

        Public Function Exclude(ByVal position As Integer) As OxidizedIndeterminateState
            Select Case _state
                Case State.PositionInclude
                    If _positions IsNot Nothing AndAlso _positions.Contains(position) Then
                        Return New OxidizedIndeterminateState(_state, _positions.Where(Function(p) p <> position).ToArray())
                    End If
                Case State.PositionExclude
                    If _positions Is Nothing Then
                        Return New OxidizedIndeterminateState(_state, {position})
                    ElseIf _positions IsNot Nothing AndAlso Not _positions.Contains(position) Then
                        Return New OxidizedIndeterminateState(_state, _positions.Append(position).ToArray())
                    End If
            End Select
            Return Me
        End Function

        Public Function AsVisitor() As IVisitor(Of IOxidized, IOxidized)
            Return New OxidizedIndeterminateVisitor(Me)
        End Function

        Public Shared Function Positions(ParamArray pPositions As Integer()) As OxidizedIndeterminateState
            Return New OxidizedIndeterminateState(State.PositionInclude, pPositions)
        End Function

        Public Shared Function PositionsExclude(ParamArray positions As Integer()) As OxidizedIndeterminateState
            Return New OxidizedIndeterminateState(State.PositionExclude, positions)
        End Function

        Friend Enum State
            None
            PositionInclude
            PositionExclude
        End Enum

        Friend NotInheritable Class OxidizedIndeterminateVisitor
            Implements IVisitor(Of IOxidized, IOxidized)
            Private ReadOnly _state As OxidizedIndeterminateState

            Public Sub New(ByVal state As OxidizedIndeterminateState)
                _state = state
            End Sub

            Private Function Visit(ByVal item As IOxidized) As IOxidized Implements IVisitor(Of IOxidized, IOxidized).Visit
                Dim ox = _state.Indeterminate(item.Oxidises)
                Return New Oxidized(item.Count, If(TryCast(ox, IList(Of Integer)), ox.ToArray()))
            End Function
        End Class
    End Class
End Namespace

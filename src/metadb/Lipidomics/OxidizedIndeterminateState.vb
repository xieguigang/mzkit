#Region "Microsoft.VisualBasic::de0e560a5bd9ca089c1cc2ce04f10f67, G:/mzkit/src/metadb/Lipidomics//OxidizedIndeterminateState.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 96
    '    Code Lines: 83
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 4.25 KB


    ' Class OxidizedIndeterminateState
    ' 
    '     Properties: AllPositions, Identity
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: AsVisitor, Exclude, Include, Indeterminate, Positions
    '               PositionsExclude
    '     Enum State
    ' 
    '         None, PositionExclude, PositionInclude
    ' 
    ' 
    ' 
    '     Class OxidizedIndeterminateVisitor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Visit
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public NotInheritable Class OxidizedIndeterminateState
    Public Shared ReadOnly Property AllPositions As OxidizedIndeterminateState = New OxidizedIndeterminateState(State.PositionExclude, Nothing)
    Public Shared ReadOnly Property Identity As OxidizedIndeterminateState = New OxidizedIndeterminateState(State.None, Nothing)

    Private ReadOnly _state As State
    Private ReadOnly _positions As Integer()

    Private Sub New(state As State, positions As Integer())
        _state = state
        _positions = positions
    End Sub

    Public Function Indeterminate(infos As IReadOnlyCollection(Of Integer)) As IReadOnlyCollection(Of Integer)
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

    Public Function Include(position As Integer) As OxidizedIndeterminateState
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

    Public Function Exclude(position As Integer) As OxidizedIndeterminateState
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

        Public Sub New(state As OxidizedIndeterminateState)
            _state = state
        End Sub

        Private Function Visit(item As IOxidized) As IOxidized Implements IVisitor(Of IOxidized, IOxidized).Visit
            Dim ox = _state.Indeterminate(item.Oxidises)
            Return New Oxidized(item.Count, If(TryCast(ox, IList(Of Integer)), ox.ToArray()))
        End Function
    End Class
End Class


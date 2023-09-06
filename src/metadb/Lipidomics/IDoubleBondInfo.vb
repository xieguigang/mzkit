Imports System
Imports System.Collections.Generic


Public Interface IDoubleBondInfo
        Inherits IEquatable(Of IDoubleBondInfo)
        ReadOnly Property Position As Integer
        ReadOnly Property State As DoubleBondState
        ReadOnly Property Determined As Boolean
        Function Includes(ByVal info As IDoubleBondInfo) As Boolean
    End Interface

    Public Enum DoubleBondState
        Unknown
        E
        Z
    End Enum

    Public Class DoubleBondInfo
        Implements IDoubleBondInfo
        Private Shared ReadOnly CACHE As Dictionary(Of (Integer, DoubleBondState), DoubleBondInfo) = New Dictionary(Of (Integer, DoubleBondState), DoubleBondInfo)()
        Private Shared ReadOnly LOCKOBJ As Object = New Object()

        Public Shared Function Create(ByVal position As Integer, ByVal Optional state As DoubleBondState = DoubleBondState.Unknown) As DoubleBondInfo
            Dim info As DoubleBondInfo
            SyncLock LOCKOBJ
                If Not CACHE.TryGetValue((position, state), info) Then
                    Return CSharpImpl.__Assign(CACHE((position, state)), New DoubleBondInfo(position, state))
                End If
            End SyncLock
            Return info
        End Function

        Private Sub New(ByVal position As Integer, ByVal state As DoubleBondState)
            Me.Position = position
            Me.State = state
        End Sub

        Public ReadOnly Property Position As Integer Implements IDoubleBondInfo.Position
        Public ReadOnly Property State As DoubleBondState Implements IDoubleBondInfo.State

        Public ReadOnly Property Determined As Boolean Implements IDoubleBondInfo.Determined
            Get
                Return State <> DoubleBondState.Unknown
            End Get
        End Property

        Public Shared Function E(ByVal position As Integer) As DoubleBondInfo
            Return Create(position, DoubleBondState.E)
        End Function

        Public Shared Function Z(ByVal position As Integer) As DoubleBondInfo
            Return Create(position, DoubleBondState.Z)
        End Function

        Public Overrides Function ToString() As String
            Select Case State
                Case DoubleBondState.E, DoubleBondState.Z
                    Return $"{Position}{State}"
                Case Else
                    Return $"{Position}"
            End Select
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim info As DoubleBondInfo = Nothing
            Return CSharpImpl.__Assign(info, TryCast(obj, DoubleBondInfo)) IsNot Nothing AndAlso Position = info.Position AndAlso State = info.State
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (Position, State).GetHashCode()
        End Function

        Public Function Includes(ByVal info As IDoubleBondInfo) As Boolean Implements IDoubleBondInfo.Includes
            Return Position = info.Position AndAlso (State = DoubleBondState.Unknown OrElse State = info.State)
        End Function

    Public Overloads Function Equals(ByVal other As IDoubleBondInfo) As Boolean Implements IEquatable(Of IDoubleBondInfo).Equals
        Return Position = other.Position AndAlso State = other.State
    End Function
End Class


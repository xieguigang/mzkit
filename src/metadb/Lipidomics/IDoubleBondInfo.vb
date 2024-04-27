#Region "Microsoft.VisualBasic::91d2806c3f7f7a64ca071c068a5959f3, G:/mzkit/src/metadb/Lipidomics//IDoubleBondInfo.vb"

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

    '   Total Lines: 79
    '    Code Lines: 65
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 3.03 KB


    ' Interface IDoubleBondInfo
    ' 
    '     Properties: Determined, Position, State
    ' 
    '     Function: Includes
    ' 
    ' Enum DoubleBondState
    ' 
    '     E, Unknown, Z
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class DoubleBondInfo
    ' 
    '     Properties: Determined, Position, State
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Create, E, (+2 Overloads) Equals, GetHashCode, Includes
    '               ToString, Z
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IDoubleBondInfo
    Inherits IEquatable(Of IDoubleBondInfo)
    ReadOnly Property Position As Integer
    ReadOnly Property State As DoubleBondState
    ReadOnly Property Determined As Boolean
    Function Includes(info As IDoubleBondInfo) As Boolean
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

    Public Shared Function Create(position As Integer, Optional state As DoubleBondState = DoubleBondState.Unknown) As DoubleBondInfo
        Dim info As DoubleBondInfo = Nothing
        SyncLock LOCKOBJ
            If Not CACHE.TryGetValue((position, state), info) Then
                CACHE((position, state)) = New DoubleBondInfo(position, state)
                Return CACHE((position, state))
            End If
        End SyncLock
        Return info
    End Function

    Private Sub New(position As Integer, state As DoubleBondState)
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

    Public Shared Function E(position As Integer) As DoubleBondInfo
        Return Create(position, DoubleBondState.E)
    End Function

    Public Shared Function Z(position As Integer) As DoubleBondInfo
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

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim info As DoubleBondInfo = TryCast(obj, DoubleBondInfo)
        Return info IsNot Nothing AndAlso Position = info.Position AndAlso State = info.State
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (Position, State).GetHashCode()
    End Function

    Public Function Includes(info As IDoubleBondInfo) As Boolean Implements IDoubleBondInfo.Includes
        Return Position = info.Position AndAlso (State = DoubleBondState.Unknown OrElse State = info.State)
    End Function

    Public Overloads Function Equals(other As IDoubleBondInfo) As Boolean Implements IEquatable(Of IDoubleBondInfo).Equals
        Return Position = other.Position AndAlso State = other.State
    End Function
End Class



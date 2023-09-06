Imports CompMs.Common.DataStructure
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq


Public Interface IOxidized
        Inherits IVisitableElement, IEquatable(Of IOxidized)
        ReadOnly Property Count As Integer
        ReadOnly Property DecidedCount As Integer
        ReadOnly Property UnDecidedCount As Integer

        ReadOnly Property Oxidises As ReadOnlyCollection(Of Integer)

        Function Includes(ByVal oxidized As IOxidized) As Boolean
    End Interface

    Public NotInheritable Class Oxidized
        Implements IOxidized
        Public Sub New(ByVal count As Integer, ByVal oxidises As IList(Of Integer))
            Me.Count = count
            Me.Oxidises = New ReadOnlyCollection(Of Integer)(oxidises)
        End Sub

        Public Sub New(ByVal count As Integer, ParamArray oxidises As Integer())
            Me.Count = count
            Me.Oxidises = New ReadOnlyCollection(Of Integer)(oxidises)
        End Sub

        Public ReadOnly Property Count As Integer Implements IOxidized.Count

        Public ReadOnly Property DecidedCount As Integer Implements IOxidized.DecidedCount
            Get
                Return Oxidises.Count
            End Get
        End Property

        Public ReadOnly Property UnDecidedCount As Integer Implements IOxidized.UnDecidedCount
            Get
                Return Count - DecidedCount
            End Get
        End Property

        Public ReadOnly Property Oxidises As ReadOnlyCollection(Of Integer) Implements IOxidized.Oxidises

        Public Overrides Function ToString() As String
            If Count = 0 Then
                Return ""
            ElseIf UnDecidedCount = 0 Then
                ' return ";" + string.Join(",", Oxidises.Select(o => o.ToString() + "OH")); 
                Return "(" & String.Join(",", Oxidises.[Select](Function(o) o.ToString() & "OH")) & ")" ' fix 20221201 MT
            ElseIf Count = 1 Then
                Return ";O"
            Else
                'return $";O{Count}";
                Return $";O{Count}"
            End If
        End Function

        Public Shared Function CreateFromPosition(ParamArray oxidises As Integer()) As Oxidized
            Return New Oxidized(oxidises.Length, oxidises)
        End Function

        Public Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim concrete As IDecomposer(Of TResult, Oxidized) = Nothing

            If CSharpImpl.__Assign(concrete, TryCast(decomposer, IDecomposer(Of TResult, Oxidized))) IsNot Nothing Then
                Return concrete.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Public Function Includes(ByVal oxidized As IOxidized) As Boolean Implements IOxidized.Includes
            Return Count = oxidized.Count AndAlso DecidedCount <= oxidized.DecidedCount AndAlso Oxidises.All(New Func(Of Integer, Boolean)(AddressOf oxidized.Oxidises.Contains))
        End Function

        Public Function Equals(ByVal other As IOxidized) As Boolean Implements IEquatable(Of IOxidized).Equals
            Return Count = other.Count AndAlso DecidedCount = other.DecidedCount AndAlso Oxidises.All(Function(ox) other.Oxidises.Any(New Func(Of Integer, Boolean)(AddressOf ox.Equals)))
        End Function

End Class


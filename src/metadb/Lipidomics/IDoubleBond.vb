Imports CompMs.Common.DataStructure
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Interface IDoubleBond
        Inherits IVisitableElement, IEquatable(Of IDoubleBond)
        ReadOnly Property Count As Integer
        ReadOnly Property DecidedCount As Integer
        ReadOnly Property UnDecidedCount As Integer

        ReadOnly Property Bonds As ReadOnlyCollection(Of IDoubleBondInfo)

        Function Includes(ByVal bond As IDoubleBond) As Boolean

        Function Add(ParamArray infos As IDoubleBondInfo()) As IDoubleBond
        Function Decide(ParamArray infos As IDoubleBondInfo()) As IDoubleBond
        Function Indeterminate(ByVal indeterminateState As DoubleBondIndeterminateState) As IDoubleBond
    End Interface

    Public NotInheritable Class DoubleBond
        Implements IDoubleBond
        Public Sub New(ByVal count As Integer, ByVal bonds As IList(Of IDoubleBondInfo))
            Me.Count = count
            Me.Bonds = New ReadOnlyCollection(Of IDoubleBondInfo)(bonds)
        End Sub

        Public Sub New(ByVal count As Integer, ByVal bonds As IEnumerable(Of IDoubleBondInfo))
            Me.New(count, If(TryCast(bonds, IList(Of IDoubleBondInfo)), bonds.ToArray()))

        End Sub

        Public Sub New(ByVal count As Integer, ParamArray bonds As IDoubleBondInfo())
            Me.New(count, CType(bonds, IList(Of IDoubleBondInfo)))

        End Sub

        Public Sub New(ParamArray bonds As IDoubleBondInfo())
            Me.New(bonds.Length, CType(bonds, IList(Of IDoubleBondInfo)))

        End Sub

        Public ReadOnly Property Count As Integer Implements IDoubleBond.Count

        Public ReadOnly Property DecidedCount As Integer Implements IDoubleBond.DecidedCount
            Get
                Return Bonds.Count
            End Get
        End Property

        Public ReadOnly Property UnDecidedCount As Integer Implements IDoubleBond.UnDecidedCount
            Get
                Return Count - DecidedCount
            End Get
        End Property

        Public ReadOnly Property Bonds As ReadOnlyCollection(Of IDoubleBondInfo) Implements IDoubleBond.Bonds

        Public Function Add(ParamArray infos As IDoubleBondInfo()) As IDoubleBond Implements IDoubleBond.Add
            Return New DoubleBond(Count + infos.Length, Bonds.Concat(infos).OrderBy(Function(x) x.Position).ToArray())
        End Function

        Public Function Decide(ParamArray infos As IDoubleBondInfo()) As IDoubleBond Implements IDoubleBond.Decide
            If Bonds.Count + infos.Length > Count Then
                Return Nothing
            End If
            Return New DoubleBond(Count, Bonds.Concat(infos).OrderBy(Function(x) x.Position).ToArray())
        End Function

        Public Function Indeterminate(ByVal indeterminateState As DoubleBondIndeterminateState) As IDoubleBond Implements IDoubleBond.Indeterminate
            Return New DoubleBond(Count, indeterminateState.Indeterminate(Bonds))
        End Function

        Public Shared Function CreateFromPosition(ParamArray positions As Integer()) As DoubleBond
            Return New DoubleBond(positions.Length, positions.[Select](Function(p) DoubleBondInfo.Create(p)).ToArray())
        End Function

        Public Overrides Function ToString() As String
            If DecidedCount >= 1 Then
                Return $"{Count}({String.Join(",", Bonds)})"
            Else
                Return Count.ToString()
            End If
        End Function

        Public Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim concrete As IDecomposer(Of TResult, DoubleBond) = Nothing

            If CSharpImpl.__Assign(concrete, TryCast(decomposer, IDecomposer(Of TResult, DoubleBond))) IsNot Nothing Then
                Return concrete.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Public Function Includes(ByVal bond As IDoubleBond) As Boolean Implements IDoubleBond.Includes
            Return Count = bond.Count AndAlso DecidedCount <= bond.DecidedCount AndAlso Bonds.All(Function(bd) bond.Bonds.Any(New Func(Of IDoubleBondInfo, Boolean)(AddressOf bd.Includes)))
        End Function

        Public Function Equals(ByVal other As IDoubleBond) As Boolean Implements IEquatable(Of IDoubleBond).Equals
            Return Count = other.Count AndAlso DecidedCount = other.DecidedCount AndAlso Bonds.All(Function(bond) other.Bonds.Any(New Func(Of IDoubleBondInfo, Boolean)(AddressOf bond.Equals)))
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace

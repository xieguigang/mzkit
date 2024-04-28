#Region "Microsoft.VisualBasic::621cb9ffd07f5a553958d807fadff17e, E:/mzkit/src/metadb/Lipidomics//IDoubleBond.vb"

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

    '   Total Lines: 101
    '    Code Lines: 75
    ' Comment Lines: 0
    '   Blank Lines: 26
    '     File Size: 4.29 KB


    ' Interface IDoubleBond
    ' 
    '     Properties: Bonds, Count, DecidedCount, UnDecidedCount
    ' 
    '     Function: Add, Decide, Includes, Indeterminate
    ' 
    ' Class DoubleBond
    ' 
    '     Properties: Bonds, Count, DecidedCount, UnDecidedCount
    ' 
    '     Constructor: (+4 Overloads) Sub New
    '     Function: Accept, Add, CreateFromPosition, Decide, Equals
    '               Includes, Indeterminate, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel


Public Interface IDoubleBond
    Inherits IVisitableElement, IEquatable(Of IDoubleBond)
    ReadOnly Property Count As Integer
    ReadOnly Property DecidedCount As Integer
    ReadOnly Property UnDecidedCount As Integer

    ReadOnly Property Bonds As ReadOnlyCollection(Of IDoubleBondInfo)

    Function Includes(bond As IDoubleBond) As Boolean

    Function Add(ParamArray infos As IDoubleBondInfo()) As IDoubleBond
    Function Decide(ParamArray infos As IDoubleBondInfo()) As IDoubleBond
    Function Indeterminate(indeterminateState As DoubleBondIndeterminateState) As IDoubleBond
End Interface

Public NotInheritable Class DoubleBond
    Implements IDoubleBond
    Public Sub New(count As Integer, bonds As IList(Of IDoubleBondInfo))
        Me.Count = count
        Me.Bonds = New ReadOnlyCollection(Of IDoubleBondInfo)(bonds)
    End Sub

    Public Sub New(count As Integer, bonds As IEnumerable(Of IDoubleBondInfo))
        Me.New(count, If(TryCast(bonds, IList(Of IDoubleBondInfo)), bonds.ToArray()))

    End Sub

    Public Sub New(count As Integer, ParamArray bonds As IDoubleBondInfo())
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

    Public Function Indeterminate(indeterminateState As DoubleBondIndeterminateState) As IDoubleBond Implements IDoubleBond.Indeterminate
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

    Public Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim concrete As IDecomposer(Of TResult, DoubleBond) = TryCast(decomposer, IDecomposer(Of TResult, DoubleBond))

        If concrete IsNot Nothing Then
            Return concrete.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

    Public Function Includes(bond As IDoubleBond) As Boolean Implements IDoubleBond.Includes
        Return Count = bond.Count AndAlso DecidedCount <= bond.DecidedCount AndAlso Bonds.All(Function(bd) bond.Bonds.Any(New Func(Of IDoubleBondInfo, Boolean)(AddressOf bd.Includes)))
    End Function

    Public Overloads Function Equals(other As IDoubleBond) As Boolean Implements IEquatable(Of IDoubleBond).Equals
        Return Count = other.Count AndAlso DecidedCount = other.DecidedCount AndAlso Bonds.All(Function(bond) other.Bonds.Any(New Func(Of IDoubleBondInfo, Boolean)(AddressOf bond.Equals)))
    End Function

End Class

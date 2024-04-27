#Region "Microsoft.VisualBasic::e050bcc7f2cad2382428bbf457460c1d, G:/mzkit/src/metadb/Lipidomics//IOxidized.vb"

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

    '   Total Lines: 78
    '    Code Lines: 59
    ' Comment Lines: 2
    '   Blank Lines: 17
    '     File Size: 3.11 KB


    ' Interface IOxidized
    ' 
    '     Properties: Count, DecidedCount, Oxidises, UnDecidedCount
    ' 
    '     Function: Includes
    ' 
    ' Class Oxidized
    ' 
    '     Properties: Count, DecidedCount, Oxidises, UnDecidedCount
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Accept, CreateFromPosition, Equals, Includes, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel


Public Interface IOxidized
    Inherits IVisitableElement, IEquatable(Of IOxidized)
    ReadOnly Property Count As Integer
    ReadOnly Property DecidedCount As Integer
    ReadOnly Property UnDecidedCount As Integer

    ReadOnly Property Oxidises As ReadOnlyCollection(Of Integer)

    Function Includes(oxidized As IOxidized) As Boolean
End Interface

Public NotInheritable Class Oxidized
    Implements IOxidized
    Public Sub New(count As Integer, oxidises As IList(Of Integer))
        Me.Count = count
        Me.Oxidises = New ReadOnlyCollection(Of Integer)(oxidises)
    End Sub

    Public Sub New(count As Integer, ParamArray oxidises As Integer())
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

    Public Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim concrete As IDecomposer(Of TResult, Oxidized) = TryCast(decomposer, IDecomposer(Of TResult, Oxidized))

        If concrete IsNot Nothing Then
            Return concrete.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

    Public Function Includes(oxidized As IOxidized) As Boolean Implements IOxidized.Includes
        Return Count = oxidized.Count AndAlso DecidedCount <= oxidized.DecidedCount AndAlso Oxidises.All(New Func(Of Integer, Boolean)(AddressOf oxidized.Oxidises.Contains))
    End Function

    Public Overloads Function Equals(other As IOxidized) As Boolean Implements IEquatable(Of IOxidized).Equals
        Return Count = other.Count AndAlso DecidedCount = other.DecidedCount AndAlso Oxidises.All(Function(ox) other.Oxidises.Any(New Func(Of Integer, Boolean)(AddressOf ox.Equals)))
    End Function

End Class

Imports CompMs.Common.DataStructure
Imports CompMs.Common.FormulaGenerator.DataObj
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class AcylChain
        Implements IChain
        Public Sub New(ByVal carbonCount As Integer, ByVal doubleBond As IDoubleBond, ByVal oxidized As IOxidized)
            Me.CarbonCount = carbonCount
            Me.DoubleBond = If(doubleBond, CSharpImpl.__Throw(Of IDoubleBond)(New ArgumentNullException(NameOf(doubleBond))))
            Me.Oxidized = If(oxidized, CSharpImpl.__Throw(Of IOxidized)(New ArgumentNullException(NameOf(oxidized))))
        End Sub

        Public ReadOnly Property DoubleBond As IDoubleBond Implements IChain.DoubleBond

        Public ReadOnly Property Oxidized As IOxidized Implements IChain.Oxidized

        Public ReadOnly Property CarbonCount As Integer Implements IChain.CarbonCount

        Public ReadOnly Property DoubleBondCount As Integer Implements IChain.DoubleBondCount
            Get
                Return DoubleBond.Count
            End Get
        End Property

        Public ReadOnly Property OxidizedCount As Integer Implements IChain.OxidizedCount
            Get
                Return Oxidized.Count
            End Get
        End Property

        Public ReadOnly Property Mass As Double Implements IChain.Mass
            Get
                Return CalculateAcylMass(CarbonCount, DoubleBondCount, OxidizedCount)
            End Get
        End Property

        Public Function GetCandidates(ByVal generator As IChainGenerator) As IEnumerable(Of IChain) Implements IChain.GetCandidates
            Return generator.Generate(Me)
        End Function

        Public Overrides Function ToString() As String
            Return $"{CarbonCount}:{FormatDoubleBond(DoubleBond)}{Oxidized}"
        End Function

        Private Shared Function FormatDoubleBond(ByVal doubleBond As IDoubleBond) As String
            If doubleBond.DecidedCount >= 1 Then
                Return $"{doubleBond.Count}({String.Join(",", doubleBond.Bonds)})"
            Else
                Return doubleBond.Count.ToString()
            End If
        End Function

        Private Shared Function CalculateAcylMass(ByVal carbon As Integer, ByVal doubleBond As Integer, ByVal oxidize As Integer) As Double
            If carbon = 0 AndAlso doubleBond = 0 AndAlso oxidize = 0 Then
                Return HydrogenMass
            End If
            Return carbon * CarbonMass + (2 * carbon - 2 * doubleBond - 1) * HydrogenMass + (1 + oxidize) * OxygenMass
        End Function

        Public Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim concrete As IDecomposer(Of TResult, AcylChain) = Nothing

            If CSharpImpl.__Assign(concrete, TryCast(decomposer, IDecomposer(Of TResult, AcylChain))) IsNot Nothing Then
                Return concrete.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Public Function Includes(ByVal chain As IChain) As Boolean Implements IChain.Includes
            Return TypeOf chain Is AcylChain AndAlso chain.CarbonCount = CarbonCount AndAlso chain.DoubleBondCount = DoubleBondCount AndAlso chain.OxidizedCount = OxidizedCount AndAlso DoubleBond.Includes(chain.DoubleBond) AndAlso Oxidized.Includes(chain.Oxidized)
        End Function

        Public Function Equals(ByVal other As IChain) As Boolean Implements IEquatable(Of IChain).Equals
            Return TypeOf other Is AcylChain AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBond.Equals(other.DoubleBond) AndAlso Oxidized.Equals(other.Oxidized)
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class

    Public Class AlkylChain
        Implements IChain
        Public Sub New(ByVal carbonCount As Integer, ByVal doubleBond As IDoubleBond, ByVal oxidized As IOxidized)
            Me.CarbonCount = carbonCount
            Me.DoubleBond = If(doubleBond, CSharpImpl.__Throw(Of IDoubleBond)(New ArgumentNullException(NameOf(doubleBond))))
            Me.Oxidized = If(oxidized, CSharpImpl.__Throw(Of IOxidized)(New ArgumentNullException(NameOf(oxidized))))
        End Sub
        Public ReadOnly Property DoubleBond As IDoubleBond Implements IChain.DoubleBond

        Public ReadOnly Property Oxidized As IOxidized Implements IChain.Oxidized

        Public ReadOnly Property CarbonCount As Integer Implements IChain.CarbonCount

        Public ReadOnly Property DoubleBondCount As Integer Implements IChain.DoubleBondCount
            Get
                Return DoubleBond.Count
            End Get
        End Property

        Public ReadOnly Property OxidizedCount As Integer Implements IChain.OxidizedCount
            Get
                Return Oxidized.Count
            End Get
        End Property

        Public ReadOnly Property Mass As Double Implements IChain.Mass
            Get
                Return CalculateAlkylMass(CarbonCount, DoubleBondCount, OxidizedCount)
            End Get
        End Property

        Private Shared Function CalculateAlkylMass(ByVal carbon As Integer, ByVal doubleBond As Integer, ByVal oxidize As Integer) As Double
            Return carbon * CarbonMass + (2 * carbon - 2 * doubleBond + 1) * HydrogenMass + oxidize * OxygenMass
        End Function

        Public Function GetCandidates(ByVal generator As IChainGenerator) As IEnumerable(Of IChain) Implements IChain.GetCandidates
            Return generator.Generate(Me)
        End Function

        Public Overrides Function ToString() As String
            If IsPlasmalogen Then
                Return $"P-{CarbonCount}:{FormatDoubleBondWhenPlasmalogen(DoubleBond)}{Oxidized}"
            Else
                Return $"O-{CarbonCount}:{FormatDoubleBond(DoubleBond)}{Oxidized}"
            End If
        End Function

        Public ReadOnly Property IsPlasmalogen As Boolean
            Get
                Return DoubleBond.Bonds.Any(Function(b) b.Position = 1)
            End Get
        End Property

        Private Shared Function FormatDoubleBond(ByVal doubleBond As IDoubleBond) As String
            If doubleBond.DecidedCount >= 1 Then
                Return $"{doubleBond.Count}({String.Join(",", doubleBond.Bonds)})"
            Else
                Return doubleBond.Count.ToString()
            End If
        End Function

        Private Shared Function FormatDoubleBondWhenPlasmalogen(ByVal doubleBond As IDoubleBond) As String
            If doubleBond.DecidedCount > 1 Then
                Return $"{doubleBond.Count - 1}({String.Join(",", doubleBond.Bonds.Where(Function(b) b.Position <> 1))})"
            ElseIf doubleBond.DecidedCount = 1 Then
                Return $"{doubleBond.Count - 1}"
            Else
                Throw New ArgumentException("Plasmalogens must have more than 1 double bonds.")
            End If
        End Function

        Public Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim concrete As IDecomposer(Of TResult, AlkylChain) = Nothing

            If CSharpImpl.__Assign(concrete, TryCast(decomposer, IDecomposer(Of TResult, AlkylChain))) IsNot Nothing Then
                Return concrete.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Public Function Includes(ByVal chain As IChain) As Boolean Implements IChain.Includes
            Return TypeOf chain Is AlkylChain AndAlso chain.CarbonCount = CarbonCount AndAlso chain.DoubleBondCount = DoubleBondCount AndAlso chain.OxidizedCount = OxidizedCount AndAlso DoubleBond.Includes(chain.DoubleBond) AndAlso Oxidized.Includes(chain.Oxidized)
        End Function

        Public Function Equals(ByVal other As IChain) As Boolean Implements IEquatable(Of IChain).Equals
            Return TypeOf other Is AlkylChain AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBond.Equals(other.DoubleBond) AndAlso Oxidized.Equals(other.Oxidized)
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class

    Public Class SphingoChain
        Implements IChain
        Public Sub New(ByVal carbonCount As Integer, ByVal doubleBond As IDoubleBond, ByVal oxidized As IOxidized)
            If oxidized Is Nothing Then
                Throw New ArgumentNullException(NameOf(oxidized))
            End If
            'if (!oxidized.Oxidises.Contains(1) || !oxidized.Oxidises.Contains(3))
            '{
            'if (!oxidized.Oxidises.Contains(1))
            '{
            '    throw new ArgumentException(nameof(oxidized));
            '}

            Me.CarbonCount = carbonCount
            Me.DoubleBond = If(doubleBond, CSharpImpl.__Throw(Of IDoubleBond)(New ArgumentNullException(NameOf(doubleBond))))
            Me.Oxidized = oxidized
        End Sub

        Public ReadOnly Property CarbonCount As Integer Implements IChain.CarbonCount

        Public ReadOnly Property DoubleBond As IDoubleBond Implements IChain.DoubleBond

        Public ReadOnly Property Oxidized As IOxidized Implements IChain.Oxidized

        Public ReadOnly Property DoubleBondCount As Integer Implements IChain.DoubleBondCount
            Get
                Return DoubleBond.Count
            End Get
        End Property

        Public ReadOnly Property OxidizedCount As Integer Implements IChain.OxidizedCount
            Get
                Return Oxidized.Count
            End Get
        End Property

        Public ReadOnly Property Mass As Double Implements IChain.Mass
            Get
                Return CalculateSphingosineMass(CarbonCount, DoubleBondCount, OxidizedCount)
            End Get
        End Property

        Private Shared Function CalculateSphingosineMass(ByVal carbon As Integer, ByVal doubleBond As Integer, ByVal oxidize As Integer) As Double
            Return carbon * CarbonMass + (2 * carbon - 2 * doubleBond + 2) * HydrogenMass + oxidize * OxygenMass + NitrogenMass
        End Function

        Public Function GetCandidates(ByVal generator As IChainGenerator) As IEnumerable(Of IChain) Implements IChain.GetCandidates
            Return generator.Generate(Me)
        End Function

        Public Function Accept(Of TResult)(ByVal visitor As IAcyclicVisitor, ByVal decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim concrete As IDecomposer(Of TResult, SphingoChain) = Nothing

            If CSharpImpl.__Assign(concrete, TryCast(decomposer, IDecomposer(Of TResult, SphingoChain))) IsNot Nothing Then
                Return concrete.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

        Public Overrides Function ToString() As String
            Return $"{CarbonCount}:{DoubleBond}{Oxidized}"
            'var OxidizedcountString = OxidizedCount == 1 ? ";O" : ";O" + OxidizedCount.ToString();
            'return $"{CarbonCount}:{DoubleBond}{OxidizedcountString}";
        End Function

        Public Function Includes(ByVal chain As IChain) As Boolean Implements IChain.Includes
            Return TypeOf chain Is SphingoChain AndAlso chain.CarbonCount = CarbonCount AndAlso chain.DoubleBondCount = DoubleBondCount AndAlso chain.OxidizedCount = OxidizedCount AndAlso DoubleBond.Includes(chain.DoubleBond) AndAlso Oxidized.Includes(chain.Oxidized)
        End Function

        Public Function Equals(ByVal other As IChain) As Boolean Implements IEquatable(Of IChain).Equals
            Return TypeOf other Is SphingoChain AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBond.Equals(other.DoubleBond) AndAlso Oxidized.Equals(other.Oxidized)
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class
    End Class
End Namespace

Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.DataStructure
Imports CompMs.Common.Enum
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices


<Flags>
    Public Enum LipidDescription
        None = 0
        [Class] = 1
        Chain = 2
        SnPosition = 4
        DoubleBondPosition = 8
        EZ = 16
    End Enum

    Public Module LipidDescriptionExtensions
        <Extension()>
        Public Function Has(description As LipidDescription, other As LipidDescription) As Boolean
            Return (description And other) <> LipidDescription.None
        End Function
    End Module

    Public Interface ILipid
        Inherits IEquatable(Of ILipid), IVisitableElement
        ReadOnly Property Name As String
        ReadOnly Property LipidClass As LbmClass
        ReadOnly Property Mass As Double ' TODO: Formula class maybe better.
        ReadOnly Property AnnotationLevel As Integer
        ReadOnly Property Description As LipidDescription
        ReadOnly Property ChainCount As Integer
        ReadOnly Property Chains As ITotalChain

        Function Includes(lipid As ILipid) As Boolean

        Function Generate(generator As ILipidGenerator) As IEnumerable(Of ILipid)
        Function GenerateSpectrum(generator As ILipidSpectrumGenerator, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty
    End Interface

    Public Class Lipid
        Implements ILipid
        Public Sub New(lipidClass As LbmClass, mass As Double, chains As ITotalChain)
            Me.LipidClass = lipidClass
            Me.Mass = mass
            AnnotationLevel = GetAnnotationLevel(chains)
            Me.Chains = If(chains, CSharpImpl.__Throw(Of ITotalChain)(New ArgumentNullException(NameOf(chains))))
            Description = chains.Description
        End Sub

        Public ReadOnly Property Name As String Implements ILipid.Name
            Get
                Return ToString()
            End Get
        End Property
        Public ReadOnly Property LipidClass As LbmClass Implements ILipid.LipidClass
        Public ReadOnly Property Mass As Double Implements ILipid.Mass
        Public ReadOnly Property AnnotationLevel As Integer = 1 Implements ILipid.AnnotationLevel
        Public ReadOnly Property Description As LipidDescription = LipidDescription.None Implements ILipid.Description

        Public ReadOnly Property ChainCount As Integer Implements ILipid.ChainCount
            Get
                Return Chains.CarbonCount
            End Get
        End Property
        Public ReadOnly Property Chains As ITotalChain Implements ILipid.Chains

        Public Function Includes(lipid As ILipid) As Boolean Implements ILipid.Includes
            If LipidClass <> lipid.LipidClass OrElse Math.Abs(Mass - lipid.Mass) >= 1e-6 OrElse (Description And lipid.Description) <> Description OrElse AnnotationLevel > lipid.AnnotationLevel Then
                Return False
            End If

            Return Chains.Includes(lipid.Chains)
        End Function

        Public Function Generate(generator As ILipidGenerator) As IEnumerable(Of ILipid) Implements ILipid.Generate
            Return generator.Generate(Me)
        End Function

        Public Function GenerateSpectrum(generator As ILipidSpectrumGenerator, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipid.GenerateSpectrum
            Return generator.Generate(Me, adduct, molecule)
        End Function

        ' temporary ToString method.
        Public Overrides Function ToString() As String
            Return $"{LipidClassDictionary.Default.LbmItems(LipidClass).DisplayName} {Chains}"
        End Function

        Private Shared Function GetAnnotationLevel(chains As ITotalChain) As Integer
                        ''' Cannot convert SwitchStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax'.
'''    在 System.Linq.Enumerable.<CastIterator>d__97`1.MoveNext()
'''    在 Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SeparatedList[TNode](IEnumerable`1 nodes)
'''    在 ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.ConvertSwitchSection(SwitchSectionSyntax section)
'''    在 System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()
'''    在 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
'''    在 System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)
'''    在 ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.VisitSwitchStatement(SwitchStatementSyntax node)
'''    在 Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    在 ICSharpCode.CodeConverter.VB.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)
''' 
''' Input:
'''             switch (chains) {
                case CompMs.Common.Lipidomics.TotalChain _:
                    return 1;
                case CompMs.Common.Lipidomics.MolecularSpeciesLevelChains _:
                    return 2;
                case CompMs.Common.Lipidomics.PositionLevelChains _:
                    return 3;
                default:
                    return 0;
            }

''' 
        End Function

        Public Function Equals(other As ILipid) As Boolean Implements IEquatable(Of ILipid).Equals
            Return LipidClass = other.LipidClass AndAlso AnnotationLevel = other.AnnotationLevel AndAlso Description = other.Description AndAlso Chains.Equals(other.Chains)
        End Function

        Public Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
            Dim decomposer_ As IDecomposer(Of TResult, ILipid) = Nothing

            If CSharpImpl.__Assign(decomposer_, TryCast(decomposer, IDecomposer(Of TResult, ILipid))) IsNot Nothing Then
                Return decomposer_.Decompose(visitor, Me)
            End If
            Return Nothing
        End Function

End Class


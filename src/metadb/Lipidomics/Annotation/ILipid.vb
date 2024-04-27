#Region "Microsoft.VisualBasic::c9b0beb5fc08f49b54b8a53e4f03c4c9, G:/mzkit/src/metadb/Lipidomics//Annotation/ILipid.vb"

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

    '   Total Lines: 120
    '    Code Lines: 98
    ' Comment Lines: 1
    '   Blank Lines: 21
    '     File Size: 4.79 KB


    ' Enum LipidDescription
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' Module LipidDescriptionExtensions
    ' 
    '     Function: Has
    ' 
    ' Interface ILipid
    ' 
    '     Properties: AnnotationLevel, ChainCount, Chains, Description, LipidClass
    '                 Mass, Name
    ' 
    '     Function: Generate, GenerateSpectrum, Includes
    ' 
    ' Class Lipid
    ' 
    '     Properties: AnnotationLevel, ChainCount, Chains, Description, LipidClass
    '                 Mass, Name
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Accept, Equals, Generate, GenerateSpectrum, GetAnnotationLevel
    '               Includes, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports std = System.Math


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
        If chains Is Nothing Then
            Throw New ArgumentNullException(NameOf(chains))
        End If

        Me.LipidClass = lipidClass
        Me.Mass = mass
        AnnotationLevel = GetAnnotationLevel(chains)
        Me.Chains = chains
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
        If LipidClass <> lipid.LipidClass OrElse std.Abs(Mass - lipid.Mass) >= 0.000001 OrElse (Description And lipid.Description) <> Description OrElse AnnotationLevel > lipid.AnnotationLevel Then
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
        Select Case chains.GetType
            Case GetType(TotalChain)
                Return 1
            Case GetType(MolecularSpeciesLevelChains)
                Return 2
            Case GetType(PositionLevelChains)
                Return 3
            Case Else
                Return 0
        End Select
    End Function

    Public Overloads Function Equals(other As ILipid) As Boolean Implements IEquatable(Of ILipid).Equals
        Return LipidClass = other.LipidClass AndAlso AnnotationLevel = other.AnnotationLevel AndAlso Description = other.Description AndAlso Chains.Equals(other.Chains)
    End Function

    Public Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, ILipid) = TryCast(decomposer, IDecomposer(Of TResult, ILipid))

        If decomposer_ IsNot Nothing Then
            Return decomposer_.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

End Class



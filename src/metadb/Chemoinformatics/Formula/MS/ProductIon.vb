
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Formula.MS

    ''' <summary>
    ''' This class is the storage of product ion assignment used in MS-FINDER program.
    ''' </summary>
    Public Class ProductIon

        Public Property Mass As Double

        Public Property Intensity As Double

        Public Property Formula As Formula

        Public Property IonMode As IonModes

        Public Property Smiles As String

        Public Property MassDiff As Double

        Public Property IsotopeDiff As Double

        Public Property Comment As String

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As New List(Of String)()

        Public Property Frequency As Double

        Public Property CandidateOntologies As New List(Of String)()

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(formula As String, name As String, comment As String)
            Call Me.New(FormulaScanner.ScanFormula(formula).CountsByElement, name, comment)
        End Sub

        Sub New(formula As IDictionary(Of String, Integer), name As String, comment As String)
            _Formula = New Formula(formula)
            _Mass = _Formula.ExactMass
            _Intensity = 1
            _IonMode = IonModes.Unknown
            _Comment = comment
            _Name = name
            _ShortName = name
            _Frequency = 1
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Name} ({Formula.EmpiricalFormula}); m/z: {Mass}"
        End Function
    End Class

End Namespace
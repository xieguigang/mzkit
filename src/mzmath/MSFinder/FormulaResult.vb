Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class FormulaResult
    Public Sub New()

    End Sub

#Region "properties"
    Public Property Formula As Formula = New Formula()

    Public Property MatchedMass As Double
    Public Property MassDiff As Double
    Public Property M1IsotopicDiff As Double
    Public Property M2IsotopicDiff As Double
    Public Property M1IsotopicIntensity As Double
    Public Property M2IsotopicIntensity As Double
    Public Property MassDiffScore As Double
    Public Property IsotopicScore As Double
    Public Property ProductIonScore As Double
    Public Property NeutralLossHits As Integer
    Public Property NeutralLossScore As Double
    Public Property ProductIonHits As Integer
    Public Property ProductIonNum As Integer
    Public Property NeutralLossNum As Integer
    Public Property ResourceScore As Double
    Public Property ResourceNames As String = String.Empty
    Public Property ResourceRecords As Integer
    Public Property TotalScore As Double
    Public Property IsSelected As Boolean
    Public Property ProductIonResult As List(Of ProductIon) = New List(Of ProductIon)()
    Public Property NeutralLossResult As List(Of NeutralLoss) = New List(Of NeutralLoss)()
    Public Property AnnotatedIonResult As List(Of AnnotatedIon) = New List(Of AnnotatedIon)()
    Public Property PubchemResources As List(Of Integer) = New List(Of Integer)()
    Public Property ChemicalOntologyDescriptions As List(Of String) = New List(Of String)()
    Public Property ChemicalOntologyIDs As List(Of String) = New List(Of String)()
    Public Property ChemicalOntologyScores As List(Of Double) = New List(Of Double)()
    Public Property ChemicalOntologyRepresentativeInChIKeys As List(Of String) = New List(Of String)()

#End Region
End Class

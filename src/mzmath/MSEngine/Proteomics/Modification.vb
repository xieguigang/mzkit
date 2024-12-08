Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

'proteinNterm modification is allowed only once.
'proteinCterm modification is allowed only once.
'anyCterm modification is allowed only once.
'anyNterm modification is allowed only once.

Public Class Modification

    Public Property Title As String

    Public Property Description As String

    Public Property CreateDate As String

    Public Property LastModifiedDate As String

    Public Property User As String

    Public Property ReporterCorrectionM2 As Integer

    Public Property ReporterCorrectionM1 As Integer

    Public Property ReporterCorrectionP1 As Integer

    Public Property ReporterCorrectionP2 As Integer

    Public Property ReporterCorrectionType As Boolean

    Public Property Composition As Formula ' only derivative moiety 

    Public Property ModificationSites As List(Of ModificationSite) = New List(Of ModificationSite)()

    Public Property Position As String ' anyCterm, anyNterm, anywhere, notCterm, proteinCterm, proteinNterm

    Public Property Type As String ' Standard, Label, IsobaricLabel, Glycan, AaSubstitution, CleavedCrosslink, NeuCodeLabel

    Public Property TerminusType As String

    Public Property IsSelected As Boolean

    Public Property IsVariable As Boolean
End Class


Public Class ModificationSite

    Public Property Site As String

    Public Property DiagnosticIons As List(Of ProductIon) = New List(Of ProductIon)()

    Public Property DiagnosticNLs As List(Of NeutralLoss) = New List(Of NeutralLoss)()
End Class

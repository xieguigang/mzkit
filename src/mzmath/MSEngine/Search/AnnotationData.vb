Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class AnnotationData(Of T As ICrossReference)
    Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider
    Implements GenericCompound

    ''' <summary>
    ''' A unique database reference id of current metabolite data object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ID As String Implements IReadOnlyId.Identity
    Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
    Public ReadOnly Property CommonName As String Implements ICompoundNameProvider.CommonName
    Public ReadOnly Property Formula As String Implements IFormulaProvider.Formula

    ''' <summary>
    ''' the external database cross reference
    ''' </summary>
    ''' <returns></returns>
    Public Property Xref As T

    ''' <summary>
    ''' MSDIAL score
    ''' </summary>
    ''' <returns></returns>
    Public Property Score As MsScanMatchResult
    ''' <summary>
    ''' Spectrum alignment result
    ''' </summary>
    ''' <returns></returns>
    Public Property Alignment As AlignmentOutput

End Class

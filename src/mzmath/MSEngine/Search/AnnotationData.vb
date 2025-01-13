Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the annotation result dataset
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks>
''' the report table row will be generates from this object
''' </remarks>
Public Class AnnotationData(Of T As ICrossReference)
    Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider
    Implements GenericCompound
    Implements IMetabolite(Of T)

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
    Public Property Xref As T Implements IMetabolite(Of T).CrossReference

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

    Public Property kingdom As String Implements ICompoundClass.kingdom
    Public Property super_class As String Implements ICompoundClass.super_class
    Public Property [class] As String Implements ICompoundClass.class
    Public Property sub_class As String Implements ICompoundClass.sub_class
    Public Property molecular_framework As String Implements ICompoundClass.molecular_framework
End Class

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO

Namespace PoolData

    ''' <summary>
    ''' metadata of the spectrum object
    ''' </summary>
    Public Class Metadata : Implements INamedValue
        Implements IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider

        Public Property guid As String Implements INamedValue.Key
        Public Property mz As Double
        Public Property rt As Double
        Public Property intensity As Double
        Public Property source_file As String

        ''' <summary>
        ''' the spectrum data is store in mzpack <see cref="ScanMs2"/> format
        ''' </summary>
        ''' <returns></returns>
        Public Property block As BufferRegion

        ''' <summary>
        ''' blood, urine, etc
        ''' </summary>
        ''' <returns></returns>
        Public Property sample_source As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property organism As String
        Public Property instrument As String = "Thermo Scientific Q Exactive"
        Public Property name As String Implements ICompoundNameProvider.CommonName
        Public Property biodeep_id As String Implements IReadOnlyId.Identity
        Public Property formula As String Implements IFormulaProvider.Formula
        Public Property adducts As String
        Public Property project As String

        Private ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return FormulaScanner.EvaluateExactMass(formula)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(copy As Metadata)
            Me.adducts = copy.adducts
            Me.sample_source = copy.sample_source
            Me.source_file = copy.source_file
            Me.rt = copy.rt
            Me.biodeep_id = copy.biodeep_id
            Me.block = New BufferRegion(copy.block)
            Me.formula = copy.formula
            Me.guid = copy.guid
            Me.instrument = copy.instrument
            Me.intensity = copy.intensity
            Me.mz = copy.mz
            Me.name = copy.name
            Me.organism = copy.organism
            Me.project = copy.project
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{guid}] {name} {mz.ToString("F4")}@{(rt / 60).ToString("F2")}min"
        End Function

    End Class
End Namespace
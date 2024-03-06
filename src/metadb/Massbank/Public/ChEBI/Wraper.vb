Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace ChEBI

    Public Class Wraper : Implements GenericCompound

        Public ReadOnly Property Identity As String Implements IReadOnlyId.Identity
            Get
                Return chebi.chebiId
            End Get
        End Property

        Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return FormulaScanner.EvaluateExactMass(Formula)
            End Get
        End Property

        Public ReadOnly Property CommonName As String Implements ICompoundNameProvider.CommonName
            Get
                Return chebi.chebiAsciiName
            End Get
        End Property

        Public ReadOnly Property Formula As String Implements IFormulaProvider.Formula
            Get
                Return chebi.Formulae.data
            End Get
        End Property

        Public ReadOnly Property chebi As ChEBIEntity

        Public Overrides Function ToString() As String
            Return $"({Identity}) {CommonName}"
        End Function

    End Class
End Namespace
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' object model wrapper for the KEGG compound in order to apply of the generic ms search engine
''' </summary>
Public Structure KEGGCompound : Implements IReadOnlyId, IExactmassProvider

    Public ReadOnly Property ExactMass As Double Implements IExactmassProvider.ExactMass
        Get
            Return KEGG.exactMass
        End Get
    End Property

    Public ReadOnly Property kegg_id As String Implements IReadOnlyId.Identity
        Get
            Return KEGG.entry
        End Get
    End Property

    Dim KEGG As Compound

    Public Overrides Function ToString() As String
        Return KEGG.ToString
    End Function

End Structure

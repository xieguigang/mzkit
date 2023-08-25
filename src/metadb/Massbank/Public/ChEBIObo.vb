Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo

Public Module ChEBIObo

    Public Iterator Function ImportsMetabolites(chebi As OBOFile) As IEnumerable(Of metadata)
        For Each term As RawTerm In chebi.GetRawTerms

        Next
    End Function

End Module

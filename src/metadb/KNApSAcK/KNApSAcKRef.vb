Imports BioNovoGene.BioDeep.Chemoinformatics

Public Class KNApSAcKRef : Implements IExactmassProvider

    Public Property xrefId As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double Implements IExactmassProvider.ExactMass
    Public Property CAS As String
    Public Property SMILES As String
    Public Property InChI As String
    Public Property InChIKey As String

    ''' <summary>
    ''' glycosyl count n
    ''' </summary>
    ''' <returns></returns>
    Public Property glycosyl As String()

End Class

Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.Linq

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
    Public Property term As String

    Public Function CreateLossElementList() As Dictionary(Of String, Integer)
        Return glycosyl _
            .SafeQuery _
            .Select(Function(gly) gly.GetTagValue(" ", trim:=True)) _
            .ToDictionary(Function(a) a.Value,
                          Function(a)
                              Return Integer.Parse(a.Name)
                          End Function)
    End Function

End Class

Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq

Namespace Data

    Public Class InformationTable

        Public Property CID As String
        Public Property names As String()
        Public Property formula As String
        Public Property exact_mass As Double
        Public Property CAS As String()
        Public Property InChIKey As String
        Public Property InChI As String
        Public Property SMILES As String
        Public Property Biosynthesis As String

        ''' <summary>
        ''' Kingdom|Family|Species
        ''' </summary>
        ''' <returns></returns>
        Public Property origins As String()
        Public Property query As String
        Public Property glycosyl As String()

        Public Shared Function FromDetails(data As Information) As InformationTable
            Dim glycosyl As String() = data.name _
                .Select(Function(name)
                            Return name.GlycosylNameParser.ToArray
                        End Function) _
                .OrderByDescending(Function(n) n.Length) _
                .First

            Return New InformationTable With {
                .Biosynthesis = data.Biosynthesis,
                .CAS = data.CAS,
                .CID = data.CID,
                .exact_mass = FormulaScanner.EvaluateExactMass(data.formula),
                .formula = data.formula,
                .InChI = data.InChICode,
                .InChIKey = data.InChIKey,
                .names = data.name,
                .SMILES = data.SMILES,
                .origins = data.Organism _
                    .SafeQuery _
                    .Select(Function(o)
                                Return $"{o.Kingdom}+{o.Family}+{o.Species}"
                            End Function) _
                    .ToArray,
                .query = data.query,
                .glycosyl = glycosyl
            }
        End Function

    End Class
End Namespace
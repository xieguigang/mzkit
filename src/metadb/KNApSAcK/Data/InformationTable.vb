Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct
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

        Public Shared Function FromDetails(data As Information, solver As GlycosylNameSolver, Optional ByRef chemicalName As String = Nothing) As InformationTable
            Dim glycosyl As (String, String()) = data.name _
                .Select(Function(name)
                            Return (name, solver.GlycosylNameParser(name.ToLower.Replace(data.query.ToLower, "")).ToArray)
                        End Function) _
                .Where(Function(n) n.Item2.Length > 0) _
                .Where(Function(n)
                           If n.Item2.Length = 1 Then
                               Return n.name.ToLower <> n.Item2(Scan0)
                           Else
                               Return True
                           End If
                       End Function) _
                .OrderByDescending(Function(n) n.Item2.Length) _
                .FirstOrDefault

            chemicalName = glycosyl.Item1

            If chemicalName.StringEmpty Then
                chemicalName = data.name.First
            End If

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
                .glycosyl = glycosyl.Item2
            }
        End Function

    End Class
End Namespace
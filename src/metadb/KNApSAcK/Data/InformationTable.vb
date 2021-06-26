#Region "Microsoft.VisualBasic::9a3b0dc6c96cc69929d1e4ed2c218aa9, src\metadb\KNApSAcK\Data\InformationTable.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Class InformationTable
    ' 
    '         Properties: Biosynthesis, CAS, CID, exact_mass, formula
    '                     glycosyl, InChI, InChIKey, names, origins
    '                     query, SMILES
    ' 
    '         Function: FromDetails
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

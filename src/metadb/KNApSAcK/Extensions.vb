#Region "Microsoft.VisualBasic::a47c0eb0591723f26eeb2512f66b289f, metadb\KNApSAcK\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 33
    '    Code Lines: 29 (87.88%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 1.18 KB


    ' Module Extensions
    ' 
    '     Function: CreateReference
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.Massbank.KNApSAcK.Data
Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct
Imports Microsoft.VisualBasic.Linq

Public Module Extensions

    <Extension>
    Public Function CreateReference(data As Information, solver As GlycosylNameSolver) As KNApSAcKRef
        Dim chemicalName As String = Nothing
        Dim table As InformationTable = InformationTable.FromDetails(data, solver, chemicalName)
        Dim glycosyl As String() = table.glycosyl _
            .SafeQuery _
            .GroupBy(Function(n) n) _
            .Select(Function(n) $"{n.Count} {n.Key}") _
            .ToArray
        Dim ref As New KNApSAcKRef With {
            .CAS = table.CAS.FirstOrDefault,
            .exact_mass = table.exact_mass,
            .formula = table.formula,
            .InChI = table.InChI,
            .InChIKey = table.InChIKey,
            .SMILES = table.SMILES,
            .name = chemicalName,
            .xrefId = table.CID,
            .glycosyl = glycosyl,
            .term = data.query
        }

        Return ref
    End Function

End Module

#Region "Microsoft.VisualBasic::0baa7b0cc8c4aa6c329ad26981f790e5, metadb\SMILES\ChemicalMarkup.vb"

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

    '   Total Lines: 44
    '    Code Lines: 32 (72.73%)
    ' Comment Lines: 5 (11.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (15.91%)
    '     File Size: 1.43 KB


    ' Module ChemicalMarkup
    ' 
    '     Function: AsCML
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.ChemicalMarkupLanguage

Public Module ChemicalMarkup

    ''' <summary>
    ''' cast the smiles graph as chemical markup language model data
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsCML(graph As ChemicalFormula) As MarkupFile
        Dim data As New molecule With {
            .id = graph.id
        }
        Dim atoms As New List(Of ChemicalMarkupLanguage.atom)
        Dim bonds As New List(Of ChemicalMarkupLanguage.bond)

        For Each element As ChemicalElement In graph.AllElements
            Call atoms.Add(New ChemicalMarkupLanguage.atom With {
                .elementType = element.elementName,
                .hydrogenCount = element.hydrogen,
                .id = element.label
            })
        Next

        For Each key As ChemicalKey In graph.AllBonds
            Call bonds.Add(New bond With {
                .id = key.ID,
                .order = 1,
                .atomRefs2 = {key.U.label, key.V.label}
            })
        Next

        data.atomArray = New atomArray("atoms", atoms)
        data.bondArray = New bondArray("bonds", bonds)

        Return New MarkupFile With {
            .molecule = data,
            .title = graph.name
        }
    End Function

End Module


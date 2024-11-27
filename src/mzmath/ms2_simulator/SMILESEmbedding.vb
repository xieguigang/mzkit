#Region "Microsoft.VisualBasic::80738f9a8663dd936c094d5e48accd13, mzmath\ms2_simulator\SMILESEmbedding.vb"

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

    '   Total Lines: 63
    '    Code Lines: 39 (61.90%)
    ' Comment Lines: 13 (20.63%)
    '    - Xml Docs: 38.46%
    ' 
    '   Blank Lines: 11 (17.46%)
    '     File Size: 2.45 KB


    ' Module SMILESEmbedding
    ' 
    '     Function: BuildDocuments, StructureDocuments
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module SMILESEmbedding

    ''' <summary>
    ''' Generates the molecule structure documents
    ''' </summary>
    ''' <param name="mols"></param>
    ''' <returns></returns>
    Public Function StructureDocuments(mols As IEnumerable(Of NamedValue(Of ChemicalFormula))) As IEnumerable(Of NamedCollection(Of String))
        Dim documents As New Dictionary(Of String, List(Of String))
        Dim group_label As String

        For Each smiles As NamedValue(Of ChemicalFormula) In mols
            If smiles.Value Is Nothing Then
                ' ignores of the error graph object
                Continue For
            End If

            For Each key As ChemicalKey In smiles.Value.AsEnumerable
                ' 20240820
                '
                ' the chemical key is the node link 
                ' in a chemical graph
                ' use the atom groups between two chemical key for embedding the graph structure
                ' use atom group table only gets the element composition
                ' atom group table missing the graph structure information
                For Each v As ChemicalElement In key.AtomGroups
                    group_label = v.group

                    If v.aromatic Then
                        group_label = $"(aromatic){v.elementName}-"
                    End If

                    If Not documents.ContainsKey(group_label) Then
                        documents(group_label) = New List(Of String)
                    End If

                    Call documents(group_label).Add(smiles.Name)
                Next
            Next
        Next

        Return documents.BuildDocuments
    End Function

    <Extension>
    Private Iterator Function BuildDocuments(tokens As Dictionary(Of String, List(Of String))) As IEnumerable(Of NamedCollection(Of String))
        For Each atom_group In tokens
            Dim mols = atom_group.Value _
                .GroupBy(Function(id) id) _
                .OrderByDescending(Function(id) id.Count) _
                .Select(Function(a) a.First) _
                .ToArray

            Yield New NamedCollection(Of String)(atom_group.Key, mols)
        Next
    End Function

End Module

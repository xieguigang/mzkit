#Region "Microsoft.VisualBasic::8e5fe5089de0cda9d0f335493588680a, metadb\FormulaSearch.Extensions\SMILESGraph.vb"

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

    '   Total Lines: 37
    '    Code Lines: 26
    ' Comment Lines: 5
    '   Blank Lines: 6
    '     File Size: 1.28 KB


    ' Module SMILESGraph
    ' 
    '     Function: AsGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module SMILESGraph

    ''' <summary>
    ''' convert the smiles chemical formula graph data to a network graph object
    ''' </summary>
    ''' <param name="f"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AsGraph(f As ChemicalFormula) As NetworkGraph
        Dim g As New NetworkGraph

        For Each atom As ChemicalElement In f.AllElements
            Call g.CreateNode(atom.label, New NodeData With {
                .label = atom.label,
                .origID = atom.label,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, atom.group}
                }
            })
        Next

        For Each key As ChemicalKey In f.AllBonds
            Call g.CreateEdge(key.U.label, key.V.label, weight:=1, data:=New EdgeData With {
                .label = key.ID,
                .Properties = New Dictionary(Of String, String)
            })
        Next

        Return g
    End Function

End Module

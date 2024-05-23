#Region "Microsoft.VisualBasic::7446a585bebb6869ed77392a2a3f6558, visualize\SDF.Extensions\SMILES2KCF.vb"

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

    '   Total Lines: 46
    '    Code Lines: 41 (89.13%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (10.87%)
    '     File Size: 1.49 KB


    ' Module SMILES2KCF
    ' 
    '     Function: ToKCF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Atom = BioNovoGene.BioDeep.Chemistry.Model.Atom
Imports Bound = BioNovoGene.BioDeep.Chemistry.Model.Bound

Public Module SMILES2KCF

    <Extension>
    Public Function ToKCF(smiles As ChemicalFormula) As KCF
        Dim atoms As New List(Of Atom)
        Dim bonds As Bound() = smiles.graphEdges _
            .Select(Function(l)
                        Return New Bound With {
                            .bounds = l.bond,
                            .from = l.U.ID,
                            .[to] = l.V.ID
                        }
                    End Function) _
            .ToArray

        For Each e In smiles.vertex
            atoms += New Atom With {
                .Atom = e.elementName,
                .Atom2D_coordinates = New Coordinate With {
                    .ID = e.ID,
                    .X = e.coordinate(0),
                    .Y = e.coordinate(1)
                },
                .Index = e.ID
            }
        Next

        Return New KCF With {
            .Entry = New Entry With {
                .Type = "Compound",
                .Id = smiles.ToString
            },
            .Atoms = atoms.ToArray,
            .Bounds = bonds
        }
    End Function

End Module

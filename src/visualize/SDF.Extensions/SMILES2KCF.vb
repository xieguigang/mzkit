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

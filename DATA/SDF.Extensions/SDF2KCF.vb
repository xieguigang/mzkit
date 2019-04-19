#Region "Microsoft.VisualBasic::6f9a3c67a45391dd8c8df92367c86f0b, SDF.Extensions\SDF2KCF.vb"

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

    ' Module SDF2KCF
    ' 
    '     Function: KCFAtom, KCFBound, ToKCF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports KCF_atom = BioNovoGene.BioDeep.Chemistry.Model.Atom
Imports KCF_bound = BioNovoGene.BioDeep.Chemistry.Model.Bound

Public Module SDF2KCF

    ''' <summary>
    ''' Convert the 3D mol structure as the <see cref="KCF"/> KEGG molecule 2D model
    ''' </summary>
    ''' <param name="sdf"></param>
    ''' <returns></returns>
    <Extension> Public Function ToKCF(sdf As SDF) As KCF
        Dim mol As [Structure] = sdf.Structure
        Dim atoms As KCF_atom() = mol.Atoms _
            .Select(AddressOf KCFAtom) _
            .ToArray
        Dim bounds As KCF_bound() = mol.Bounds _
            .Select(AddressOf KCFBound) _
            .ToArray

        Return New KCF With {
            .Entry = New Entry With {
                .Id = sdf.ID,
                .Type = sdf.Comment
            },
            .Atoms = atoms,
            .Bounds = bounds
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function KCFBound(b As Bound) As KCF_bound
        Return New KCF_bound With {
            .bounds = b.Type,
            .from = b.i,
            .to = b.j,
            .dimentional_levels = b.Stereo.ToString
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function KCFAtom(a As Atom, i%) As KCF_atom
        Return New KCF_atom With {
            .Atom = a.Atom,
            .Atom2D_coordinates = New Coordinate With {
                .X = a.Coordination.X,
                .Y = a.Coordination.Y,
                .ID = i
            },
            .Index = i,
            .KEGGAtom = KegAtomType.GetAtom(a.Atom)
        }
    End Function
End Module

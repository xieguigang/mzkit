#Region "Microsoft.VisualBasic::f82a0ff0a31e32bf83125e1464831c17, Rscript\Library\mzkit_app\src\mzkit\math\SMILESTool.vb"

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

    '   Total Lines: 131
    '    Code Lines: 86
    ' Comment Lines: 33
    '   Blank Lines: 12
    '     File Size: 5.77 KB


    ' Module SMILESTool
    ' 
    '     Function: asFormula, asGraph, atomGroups, atomLinks, atoms_table
    '               parseSMILES
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Embedding
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports list = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

''' <summary>
''' ### Simplified molecular-input line-entry system
''' 
''' The simplified molecular-input line-entry system (SMILES) is a specification in the 
''' form of a line notation for describing the structure of chemical species using short
''' ASCII strings. SMILES strings can be imported by most molecule editors for conversion
''' back into two-dimensional drawings or three-dimensional models of the molecules.
'''
''' The original SMILES specification was initiated In the 1980S. It has since been 
''' modified And extended. In 2007, an open standard called OpenSMILES was developed In
''' the open source chemistry community.
''' </summary>
<Package("SMILES", Category:=APICategories.UtilityTools)>
Module SMILESTool

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(ChemicalFormula), AddressOf atoms_table)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function atoms_table(smiles As ChemicalFormula, args As list, env As Environment) As RDataframe
        Return atomGroups(smiles)
    End Function


    ''' <summary>
    ''' Parse the SMILES molecule structre string
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <param name="strict"></param>
    ''' <returns>
    ''' A chemical graph object that could be used for build formula or structure analysis
    ''' </returns>
    ''' <remarks>
    ''' SMILES denotes a molecular structure as a graph with optional chiral 
    ''' indications. This is essentially the two-dimensional picture chemists
    ''' draw to describe a molecule. SMILES describing only the labeled
    ''' molecular graph (i.e. atoms and bonds, but no chiral or isotopic 
    ''' information) are known as generic SMILES.
    ''' </remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("parse")>
    <RApiReturn(GetType(ChemicalFormula))>
    Public Function parseSMILES(SMILES As String, Optional strict As Boolean = True) As ChemicalFormula
        Return ParseChain.ParseGraph(SMILES, strict)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.formula")>
    <RApiReturn(GetType(Formula))>
    Public Function asFormula(SMILES As ChemicalFormula, Optional canonical As Boolean = True) As Formula
        Return SMILES.GetFormula(canonical)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.graph")>
    Public Function asGraph(smiles As ChemicalFormula) As NetworkGraph
        Return smiles.AsGraph
    End Function

    ''' <summary>
    ''' get atoms table from the SMILES structure data
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <returns></returns>
    <ExportAPI("atoms")>
    Public Function atomGroups(SMILES As ChemicalFormula) As RDataframe
        Dim elements As SmilesAtom() = SMILES.GetAtomTable.ToArray
        Dim rowKeys As String() = elements.Select(Function(a) a.id).ToArray
        Dim atoms As String() = elements.Select(Function(a) a.atom).ToArray
        Dim groups As String() = elements.Select(Function(a) a.group).ToArray
        Dim ionCharge As Integer() = elements.Select(Function(a) a.ion_charge).ToArray
        Dim links As Integer() = elements.Select(Function(a) a.links).ToArray
        Dim partners As String() = elements.Select(Function(a) a.connected.JoinBy("; ")).ToArray

        Return New RDataframe With {
            .rownames = rowKeys,
            .columns = New Dictionary(Of String, Array) From {
                {"atom", atoms},
                {"group", groups},
                {"ion_charge", ionCharge},
                {"links", links},
                {"connected", partners}
            }
        }
    End Function

    <ExportAPI("links")>
    Public Function atomLinks(SMILES As ChemicalFormula,
                              Optional kappa As Double = 2,
                              Optional normalize_size As Boolean = False) As RDataframe

        Dim links As AtomLink() = SMILES.GraphEmbedding(kappa, normalize_size).ToArray
        Dim atom1 As String() = links.Select(Function(l) l.atom1).ToArray
        Dim atom2 As String() = links.Select(Function(l) l.atom2).ToArray
        Dim weight As Double() = links.Select(Function(l) l.score).ToArray
        Dim vk As Double() = links.Select(Function(l) l.vk).ToArray
        Dim v0 As Double() = links.Select(Function(l) l.v0).ToArray
        Dim vertex As String() = links _
            .Select(Function(l) l.vertex.ToBEncodeString) _
            .ToArray

        Return New RDataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"atom1", atom1},
                {"atom2", atom2},
                {"weight", weight},
                {"vk", vk},
                {"v0", v0},
                {"vertex", vertex}
            }
        }
    End Function
End Module

﻿#Region "Microsoft.VisualBasic::e6e31c8dba5c4ee1294e190407aa7fc1, metadb\Massbank\Public\NCBI\PubChem\Web\Query\MetadataConvertor.vb"

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

    '   Total Lines: 67
    '    Code Lines: 62 (92.54%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (7.46%)
    '     File Size: 2.77 KB


    '     Module MetadataConvertor
    ' 
    '         Function: CreateMetadata, stringArray
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CommonNames
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.Linq
Imports any = Microsoft.VisualBasic.Scripting
Imports Metadata2 = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace NCBI.PubChem.Web

    Module MetadataConvertor

        <Extension>
        Public Function CreateMetadata(q As QueryXml) As Metadata2
            Dim descriptor As New ChemicalDescriptor With {
                .AtomDefStereoCount = q.definedatomstereocnt,
                .AtomUdefStereoCount = q.undefinedatomstereocnt,
                .BondDefStereoCount = q.definedbondstereocnt,
                .BondUdefStereoCount = q.undefinedbondstereocnt,
                .Complexity = q.complexity,
                .ExactMass = q.exactmass,
                .FormalCharge = q.charge,
                .HeavyAtoms = q.heavycnt,
                .HydrogenAcceptor = q.hbondacc,
                .HydrogenDonors = q.hbonddonor,
                .RotatableBonds = q.rotbonds,
                .XLogP3 = q.xlogp,
                .CovalentlyBonded = q.covalentunitcnt,
                .TopologicalPolarSurfaceArea = q.polararea,
                .IsotopicAtomCount = q.isotopeatomcnt
            }
            Dim xrefs As New xref With {
                .pubchem = q.cid,
                .SMILES = If(q.canonicalsmiles, q.isosmiles),
                .MeSH = stringArray(q.meshheadings).JoinBy("; "),
                .InChI = q.inchi,
                .InChIkey = q.inchikey
            }
            Dim name As String = NameRanking.Ranking(stringArray(q.cmpdname)).FirstOrDefault

            Return New Metadata2 With {
                .chemical = descriptor,
                .description = stringArray(q.annotation).JoinBy("; "),
                .exact_mass = q.monoisotopicmass,
                .formula = q.mf,
                .ID = q.cid,
                .IUPACName = q.iupacname,
                .name = If(name, q.iupacname),
                .synonym = stringArray(q.cmpdsynonym).ToArray,
                .xref = xrefs
            }
        End Function

        Private Function stringArray(obj As Object) As String()
            If obj Is Nothing Then
                Return New String() {}
            ElseIf obj.GetType.IsArray Then
                Return DirectCast(obj, Array) _
                    .AsObjectEnumerator _
                    .Select(Function(o) any.ToString(o)) _
                    .ToArray
            Else
                Return {any.ToString(obj)}
            End If
        End Function
    End Module
End Namespace

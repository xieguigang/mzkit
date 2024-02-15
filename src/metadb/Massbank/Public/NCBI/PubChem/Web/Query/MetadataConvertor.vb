Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
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

            Return New Metadata2 With {
                .chemical = descriptor,
                .description = stringArray(q.annotation).JoinBy("; "),
                .exact_mass = q.monoisotopicmass,
                .formula = q.mf,
                .ID = q.cid,
                .IUPACName = q.iupacname,
                .name = NameRanking.Ranking(stringArray(q.cmpdname)).First,
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
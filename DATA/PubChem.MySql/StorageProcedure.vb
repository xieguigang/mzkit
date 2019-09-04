Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.MassSpectrum.DATA.File
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Public Module StorageProcedure

    Public Sub CreateMySqlDatabase(repository$, mysql$)
        Call SDF.MoleculePopulator(directory:=repository, takes:=3) _
            .PopulateData _
            .DoCall(Sub(data)
                        Call LinqExports.ProjectDumping(data, EXPORT:=mysql)
                    End Sub)
    End Sub

    <Extension>
    Private Iterator Function PopulateData(source As IEnumerable(Of SDF)) As IEnumerable(Of MySQLTable)
        For Each molecule As SDF In source
            Dim descriptor As ChemicalDescriptor = molecule.ChemicalProperties

            Yield New mysql.descriptor With {
                .atom_def_stereo_count = 0,
                .cid = molecule.CID,
                .complexity = descriptor.Complexity,
                .hbond_acceptor = descriptor.HydrogenAcceptor,
                .hbond_donor = descriptor.HydrogenDonors,
                .xlogp3_aa = descriptor.XLogP3_AA,
                .tpsa = descriptor.TopologicalPolarSurfaceArea
            }
        Next
    End Function
End Module

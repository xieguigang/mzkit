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

        Next
    End Function
End Module

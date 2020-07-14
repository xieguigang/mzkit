
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry.NCBI
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("pubchem_kit")>
Module PubChemToolKit

    ''' <summary>
    ''' query of the pubchem database
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cache$"></param>
    ''' <returns></returns>
    <ExportAPI("query")>
    Public Function queryPubChem(<RRawVectorArgument> id As Object, Optional cache$ = "./", Optional env As Environment = Nothing) As list
        Dim ids As String() = REnv.asVector(Of String)(id)
        Dim cid As String()
        Dim query As New Dictionary(Of String, PugViewRecord)
        Dim result As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each term As String In ids
            query = PubChem.QueryPugViews(term, cacheFolder:=cache)
            cid = query.Keys.ToArray

            For Each id2 As String In cid
                Call ImageFly _
                    .GetImage(id, 500, 500, doBgTransparent:=False) _
                    .SaveAs($"{cache}/{id2}.png")
                Call Thread.Sleep(1000)
            Next

            result.slots.Add(term, query)
        Next

        Return result
    End Function
End Module

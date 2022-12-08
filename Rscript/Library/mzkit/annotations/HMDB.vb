
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.Repository
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' toolkit for handling of the hmdb database
''' </summary>
<Package("hmdb_kit")>
Module HMDBTools

    ''' <summary>
    ''' open a reader for read hmdb database
    ''' </summary>
    ''' <param name="xml">
    ''' the file path of the hmdb metabolite database xml file
    ''' </param>
    ''' <returns>
    ''' this function populate a collection of the hmdb metabolites data
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("read.hmdb")>
    Public Function readHMDB(xml As String) As pipeline
        Return TMIC.HMDB _
            .LoadXML(xml) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' save the hmdb database as a csv table file
    ''' </summary>
    ''' <param name="hmdb"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("export.hmdb_table")>
    Public Function exportTable(hmdb As pipeline, file As String, Optional env As Environment = Nothing) As Object
        Using buffer As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call TMIC.HMDB.MetaDb.WriteTable(hmdb.populates(Of TMIC.HMDB.metabolite)(env), out:=buffer)
            Call buffer.Flush()
        End Using

        Return True
    End Function

    <ExportAPI("chemical_taxonomy")>
    Public Function chemical_taxonomy(metabolite As TMIC.HMDB.metabolite) As String()
        If metabolite.taxonomy Is Nothing Then
            Return {"noclass"}
        Else
            Return {
                metabolite.taxonomy.kingdom,
                metabolite.taxonomy.super_class,
                metabolite.taxonomy.class,
                metabolite.taxonomy.sub_class,
                metabolite.taxonomy.molecular_framework,
                metabolite.taxonomy.direct_parent
            }
        End If
    End Function

    ''' <summary>
    ''' split the hmdb database by biospecimen locations
    ''' </summary>
    ''' <param name="hmdb"></param>
    ''' <param name="locations"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("biospecimen_slicer")>
    Public Function biospecimen_slicer(hmdb As pipeline, locations As BioSamples, Optional env As Environment = Nothing) As Object
        Dim locationIndex As Index(Of String) = locations.GetSampleLocations.Indexing

        Return hmdb _
            .populates(Of TMIC.HMDB.metabolite)(env) _
            .Where(Function(metabolite)
                       If metabolite.biological_properties Is Nothing OrElse
                          metabolite.biological_properties _
                                    .biospecimen_locations _
                                    .biospecimen Is Nothing Then

                           Return False
                       Else
                           Return metabolite.biological_properties _
                               .biospecimen_locations _
                               .biospecimen _
                               .Any(Function(loc)
                                        Return loc Like locationIndex
                                    End Function)
                       End If
                   End Function) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    Public Function tissue_slicer(hmdb As pipeline)

    End Function

    Public Function subCellular_slicer(hmdb As pipeline)

    End Function
End Module

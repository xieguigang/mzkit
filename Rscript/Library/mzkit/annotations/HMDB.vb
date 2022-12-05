
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry
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

    <ExportAPI("biospecimen_slicer")>
    Public Function biospecimen_slicer(hmdb As pipeline, locations As String(), Optional env As Environment = Nothing) As Object
        Dim locationIndex As Index(Of String) = locations.Indexing

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

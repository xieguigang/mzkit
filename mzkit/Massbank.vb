Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.MassSpectrum.DATA.TMIC
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports ChEBIRepo = SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.DATA
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.Invokes.base

<Package("mzkit.massbank")>
Module Massbank

    <ExportAPI("chebi.secondary2main.mapping")>
    Public Function chebiSecondary2Main(repository As String) As Dictionary(Of String, String())
        Return ChEBIRepo.ScanEntities(repository) _
            .GroupBy(Function(c) c.chebiId) _
            .Select(Function(c) c.First) _
            .ToDictionary(Function(c) c.chebiId,
                          Function(c)
                              Return c.SecondaryChEBIIds _
                                  .SafeQuery _
                                  .ToArray
                          End Function)
    End Function

    <ExportAPI("hmdb.secondary2main.mapping")>
    Public Function hmdbSecondary2Main(repository As String) As Dictionary(Of String, String())
        Return HMDB.metabolite _
            .Load(repository) _
            .ToDictionary(Function(a) a.accession,
                          Function(a)
                              If a.secondary_accessions.accession.IsNullOrEmpty Then
                                  Return {a.accession}
                              Else
                                  Return a.secondary_accessions.accession
                              End If
                          End Function)
    End Function

    ''' <summary>
    ''' Create SecondaryIDSolver object from mapping file or mapping dictionary object data.
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("secondary2main.mapping")>
    Public Function createIdMapping(mapping As Object, Optional envir As Environment = Nothing) As RReturn
        If mapping Is Nothing Then
            Return REnv.stop("No mapping data provided!", envir)
        ElseIf mapping.GetType Is GetType(String) Then
            mapping = DirectCast(mapping, String).LoadJSON(Of Dictionary(Of String, String()))
        End If

        Return SecondaryIDSolver.FromMaps(mapping).AsRReturn
    End Function

    <ExportAPI("save.mapping")>
    Public Function saveIDMapping(mapping As Dictionary(Of String, String()), file$, Optional envir As Environment = Nothing) As Object
        If mapping Is Nothing Then
            Return REnv.stop("No mapping data provided!", envir)
        ElseIf file.StringEmpty Then
            Return REnv.stop("Missing file parameter!", envir)
        End If

        Return mapping.GetJson.SaveTo(file)
    End Function
End Module

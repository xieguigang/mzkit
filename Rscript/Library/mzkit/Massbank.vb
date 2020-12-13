#Region "Microsoft.VisualBasic::f2a61a439ce17ab6bc0f71cd410ae72e, Rscript\Library\mzkit\Massbank.vb"

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

' Module Massbank
' 
'     Function: chebiSecondary2Main, createIdMapping, hmdbSecondary2Main, KEGGPathwayCoverages, readMoNA
'               saveIDMapping
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports ChEBIRepo = SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.DATA
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.Invokes.base

''' <summary>
''' Metabolite annotation database toolkit
''' </summary>
<Package("massbank")>
Module Massbank

    ''' <summary>
    ''' read MoNA database file.
    ''' </summary>
    ''' <param name="rawfile"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a linq pipeline for populate the spectrum data from the MoNA database.
    ''' </returns>
    <ExportAPI("read.MoNA")>
    Public Function readMoNA(rawfile As String,
                             Optional skipSpectraInfo As Boolean = False,
                             Optional is_gcms As Boolean = False,
                             Optional env As Environment = Nothing) As pipeline

        Select Case rawfile.ExtensionSuffix.ToLower
            Case "sdf"
                Return SDFReader _
                    .ParseFile(
                        path:=rawfile,
                        skipSpectraInfo:=skipSpectraInfo,
                        isGcms:=is_gcms
                    ) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)
            Case Else
                Return Internal.debug.stop(New NotSupportedException(rawfile.ExtensionSuffix), env)
        End Select
    End Function

    ''' <summary>
    ''' read metabolite data in a given sdf data file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.SDF")>
    Public Function readSDF(file As String, Optional env As Environment = Nothing) As pipeline
        If Not file.FileExists Then
            Return Internal.debug.stop({$"the required file is not exists on your file system!", $"file: {file}"}, env)
        Else
            Return SDF.IterateParser(file).DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

    Public Function KEGGPathwayCoverages() As Object
        Throw New NotImplementedException
    End Function

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

        Return SecondaryIDSolver.FromMaps(DirectCast(mapping, Dictionary(Of String, String()))).AsRReturn
    End Function

    ''' <summary>
    ''' Save id mapping file in json file format
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <param name="file">``*.json`` file name.</param>
    ''' <param name="envir"></param>
    ''' <returns></returns>
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

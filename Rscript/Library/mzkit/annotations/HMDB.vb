#Region "Microsoft.VisualBasic::716473ea2ff8400bc315a27c7955c012, mzkit\Rscript\Library\mzkit\annotations\HMDB.vb"

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

    '   Total Lines: 108
    '    Code Lines: 69
    ' Comment Lines: 27
    '   Blank Lines: 12
    '     File Size: 3.94 KB


    ' Module HMDBTools
    ' 
    '     Function: biospecimen_slicer, chemical_taxonomy, exportTable, readHMDB, subCellular_slicer
    '               tissue_slicer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.Repository
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
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
    ''' <param name="file">
    ''' this function will returns a huge metabolite table
    ''' if this parameter value default null
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("export.hmdb_table")>
    Public Function exportTable(hmdb As pipeline, Optional file As Object = Nothing, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return TMIC.HMDB.MetaDb.PopulateTable(hmdb.populates(Of TMIC.HMDB.metabolite)(env)).ToArray
        End If

        Dim con = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If con Like GetType(Message) Then
            Return con.TryCast(Of Message)
        End If

        Using buffer As Stream = con.TryCast(Of Stream)
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

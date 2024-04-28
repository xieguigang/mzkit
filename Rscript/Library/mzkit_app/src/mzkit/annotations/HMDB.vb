#Region "Microsoft.VisualBasic::561b33b2a1e34cdd02ee0c4fbb790869, E:/mzkit/Rscript/Library/mzkit_app/src/mzkit//annotations/HMDB.vb"

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

    '   Total Lines: 291
    '    Code Lines: 177
    ' Comment Lines: 85
    '   Blank Lines: 29
    '     File Size: 12.51 KB


    ' Module HMDBTools
    ' 
    '     Function: biospecimen_slicer, chemical_taxonomy, Convert, exportTable, getHMDB
    '               readHMDB, readHMDBSpectrals, subCellular_slicer, tissue_slicer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports BioNovoGene.BioDeep.Chemistry.TMIC.HMDB
Imports BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.Repository
Imports BioNovoGene.BioDeep.Chemistry.TMIC.HMDB.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' toolkit for handling of the hmdb database
''' 
''' The Human Metabolome Database (HMDB) is a comprehensive, high-quality, freely accessible, 
''' online database of small molecule metabolites found in the human body. It bas been created 
''' by the Human Metabolome Project funded by Genome Canada and is one of the first dedicated
''' metabolomics databases. The HMDB facilitates human metabolomics research, including the 
''' identification and characterization of human metabolites using NMR spectroscopy, GC-MS
''' spectrometry and LC/MS spectrometry. To aid in this discovery process, the HMDB contains 
''' three kinds of data: 1) chemical data, 2) clinical data, and 3) molecular biology/biochemistry 
''' data. The chemical data includes 41,514 metabolite structures with detailed descriptions 
''' along with nearly 10,000 NMR, GC-MS and LC/MS spectra.
'''
''' The clinical data includes information On >10,000 metabolite-biofluid concentrations And 
''' metabolite concentration information On more than 600 different human diseases. The biochemical 
''' data includes 5,688 protein (And DNA) sequences And more than 5,000 biochemical reactions that 
''' are linked To these metabolite entries. Each metabolite entry In the HMDB contains more than 110 
''' data fields With 2/3 Of the information being devoted To chemical/clinical data And the other 
''' 1/3 devoted To enzymatic Or biochemical data. Many data fields are hyperlinked To other 
''' databases (KEGG, MetaCyc, PubChem, Protein Data Bank, ChEBI, Swiss-Prot, And GenBank) And a 
''' variety Of Structure And pathway viewing applets. The HMDB database supports extensive text, 
''' sequence, spectral, chemical Structure And relational query searches. It has been widely used
''' In metabolomics, clinical chemistry, biomarker discovery And general biochemistry education.
'''
''' Four additional databases, DrugBank, T3DB, SMPDB And FooDB are also part Of the HMDB suite Of 
''' databases. DrugBank contains equivalent information On ~1,600 drug And drug metabolites, T3DB 
''' contains information On 3,100 common toxins And environmental pollutants, SMPDB contains pathway 
''' diagrams For 700 human metabolic And disease pathways, While FooDB contains equivalent 
''' information On ~28,000 food components And food additives.
''' </summary>
<Package("hmdb_kit")>
<RTypeExport("hmdb_metabolite", GetType(HMDB.metabolite))>
Module HMDBTools

    ''' <summary>
    ''' read hmdb spectral data collection
    ''' </summary>
    ''' <param name="repo">
    ''' A directory path to the hmdb spectral data files
    ''' </param>
    ''' <param name="hmdbRaw"></param>
    ''' <param name="lazy"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.hmdb_spectrals")>
    <RApiReturn(GetType(PeakMs2), GetType(SpectraFile))>
    Public Function readHMDBSpectrals(repo As String,
                                      Optional hmdbRaw As Boolean = False,
                                      Optional lazy As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        Dim scan = TMIC.HMDB.Spectra.PopulateSpectras(repo)

        If hmdbRaw Then
            If lazy Then
                Return pipeline.CreateFromPopulator(scan)
            Else
                Return New list With {
                    .slots = scan _
                        .ToDictionary(Function(s) s.Name,
                                      Function(s)
                                          Return CObj(s.Value)
                                      End Function)
                }
            End If
        Else
            ' converts to peak_ms2 object
            Dim println = env.WriteLineHandler
            Dim converts As IEnumerable(Of PeakMs2) = scan _
                .Select(Function(si)
                            Try
                                Return Convert(si)
                            Catch ex As Exception
                                Call println(ex.Message)
                                Call env.AddMessage(ex.Message, MSG_TYPES.WRN)

                                Return Nothing
                            End Try
                        End Function) _
                .Where(Function(si) Not si Is Nothing)

            If lazy Then
                Return pipeline.CreateFromPopulator(converts)
            Else
                Return converts.ToArray
            End If
        End If
    End Function

    Private Function Convert(msms As NamedValue(Of SpectraFile)) As PeakMs2
        Dim file As MSMS = msms.Value
        Dim libname As String = file.database_id.value
        Dim mode As IonModes = ParseIonMode(file.ionization_mode.value)
        Dim hmdbId As String = file.database_id.value

        If Not file.references.IsNullOrEmpty Then
            Dim ref0 As Spectra.reference = file.references(Scan0)
            Dim xref_id As String = ref0.database_id

            ' biodeepMSMS package use the delimiter | symbol
            ' to seperate the reference id
            If xref_id.StringEmpty Then
                libname = "spectrum_" & file.GetHashCode.ToHexString
            ElseIf Not ref0.database.StringEmpty Then
                libname = $"{xref_id}|{ref0.database}"
            Else
                libname = xref_id
            End If

            libname = hmdbId & "|" & libname
        End If

        Return New PeakMs2 With {
            .mzInto = file.peakList _
                .Select(Function(p)
                            Return New ms2 With {
                                .mz = p.mass_charge,
                                .intensity = p.intensity
                            }
                        End Function) _
                .ToArray,
            .lib_guid = libname,
            .file = msms.Name,
            .scan = hmdbId,
            .precursor_type = If(mode = IonModes.Positive, "[M]+", "[M]-")
        }
    End Function

    ''' <summary>
    ''' get metabolite via a given hmdb id from the hmdb.ca online web services
    ''' </summary>
    ''' <param name="id">
    ''' the given hmdb id 
    ''' </param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("get_hmdb")>
    <RApiReturn(GetType(HMDB.metabolite), GetType(HMDB.MetaDb))>
    Public Function getHMDB(id As String,
                            Optional cache_dir As String = "./hmdb/",
                            Optional tabular As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim q = HMDB.Repository.GetMetabolite(id, cache_dir)

        If tabular AndAlso q IsNot Nothing Then
            Return HMDB.MetaDb.FromMetabolite(q)
        Else
            Return q
        End If
    End Function

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
    <RApiReturn(GetType(HMDB.metabolite))>
    Public Function readHMDB(xml As String, Optional convert_std As Boolean = False) As pipeline
        Dim pull As IEnumerable(Of metabolite) = TMIC.HMDB.LoadXML(xml)

        If convert_std Then
            Return pull _
                .ConvertInternal _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return pipeline.CreateFromPopulator(pull)
        End If
    End Function

    ''' <summary>
    ''' save the hmdb database as a csv table file
    ''' </summary>
    ''' <param name="hmdb">A collection of the HMDB <see cref="TMIC.HMDB.metabolite"/>.</param>
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

    ''' <summary>
    ''' Extract the chemical taxonomy data
    ''' </summary>
    ''' <param name="metabolite">the HMDB metabolite data</param>
    ''' <returns>
    ''' A character vector that contains the <see cref="TMIC.HMDB.taxonomy"/> information from the <see cref="TMIC.HMDB.metabolite"/>
    ''' </returns>
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
    <RApiReturn(GetType(TMIC.HMDB.metabolite))>
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

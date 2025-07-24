﻿#Region "Microsoft.VisualBasic::cac80d88839d01af49f030b44065ccce, Rscript\Library\mzkit_app\src\mzkit\assembly\data.vb"

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

'   Total Lines: 1005
'    Code Lines: 644 (64.08%)
' Comment Lines: 236 (23.48%)
'    - Xml Docs: 94.92%
' 
'   Blank Lines: 125 (12.44%)
'     File Size: 43.85 KB


' Module data
' 
'     Function: createPeakMs2, getAlignmentReference, getIntensity, getIonsSummaryTable, getMSMSTable
'               getRawXICSet, getScantime, getXICPoints, groupBy_ROI, libraryMatrix
'               LibraryTable, linearMatrix, makeAlignmentString, makeROInames, MsdataFromDf
'               nfragments, rawXIC, readMatrix, representative_spectrum, RtSlice
'               simpleSearch, (+2 Overloads) splashId, TICTable, toString, unionPeaks
'               XIC, XICGroups, XICTable
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports rDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

''' <summary>
''' Provides core functionality for mass spectrometry data processing and analysis within the mzkit framework.
''' </summary>
''' <remarks>
''' This module contains operations for:
''' 
''' 1. Mass spectral data manipulation (MS1 and MS2 level)
''' 2. Chromatographic data processing (XIC/TIC generation)
''' 3. Spectral similarity calculations and alignment
''' 4. Data format conversions between native objects and R dataframe
''' 5. Spectral library operations and metadata handling
''' 
''' Key features include:
''' 
''' - SPLASH ID generation for spectral uniqueness verification
''' - ROI-based spectral grouping and analysis
''' - Raw data centroiding and intensity normalization
''' - Cross-sample spectral alignment and matching
''' - Mass tolerance-aware operations (ppm/DA)
''' </remarks>
''' <example>
''' ```
''' # Get XIC from raw data
''' rawdata &lt;- open.mzpack("sample.mzPack")
''' xic &lt;- data::XIC(rawdata, mz=438.3251, tolerance="ppm:20")
''' 
''' # Create spectral library entry
''' peaks &lt;- data::libraryMatrix(
'''     mz=c(438.3251, 512.3987, 615.4872),
'''     intensity=c(15000, 8700, 4300),
'''     title="My Compound"
''' )
''' 
''' # Generate SPLASH ID
''' splash &lt;- data::splash_id(peaks)
''' ```
''' </example>
<Package("data")>
Module data

    <RInitialize>
    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ms1_scan()), AddressOf XICTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PeakMs2()), AddressOf getIonsSummaryTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(LibraryMatrix), AddressOf LibraryTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ChromatogramTick()), AddressOf TICTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ms2()), AddressOf getMSMSTable)
    End Sub

    Private Function TICTable(TIC As ChromatogramTick(), args As list, env As Environment) As rDataframe
        Dim table As New rDataframe With {.columns = New Dictionary(Of String, Array)}

        table.columns("time") = TIC.Select(Function(t) t.Time).ToArray
        table.columns("intensity") = TIC.Select(Function(t) t.Intensity).ToArray

        Return table
    End Function

    ''' <summary>
    ''' converts the spectrum peaks as dataframe
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <RGenericOverloads("as.data.frame")>
    <Extension>
    Private Function getMSMSTable(matrix As ms2(), args As list, env As Environment) As rDataframe
        Dim table As New rDataframe With {.columns = New Dictionary(Of String, Array)}

        table.columns("mz") = matrix.Select(Function(m) m.mz).ToArray
        table.columns("intensity") = matrix.Select(Function(m) m.intensity).ToArray
        table.columns("info") = matrix.Select(Function(m) m.Annotation).ToArray

        Return table
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function LibraryTable(matrix As LibraryMatrix, args As list, env As Environment) As rDataframe
        Return getMSMSTable(matrix.ms2, args, env)
    End Function

    Private Function XICTable(XIC As ms1_scan(), args As list, env As Environment) As rDataframe
        Dim table As New rDataframe With {.columns = New Dictionary(Of String, Array)}

        table.columns("mz") = XIC.Select(Function(a) a.mz).ToArray
        table.columns("scan_time") = XIC.Select(Function(a) a.scan_time).ToArray
        table.columns("intensity") = XIC.Select(Function(a) a.intensity).ToArray

        Return table
    End Function

    Private Function getIonsSummaryTable(peaks As PeakMs2(), args As list, env As Environment) As rDataframe
        Dim df As New rDataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add(NameOf(PeakMs2.lib_guid), From i In peaks Select i.lib_guid)
        Call df.add(NameOf(PeakMs2.scan), From i In peaks Select i.scan)
        Call df.add(NameOf(PeakMs2.file), From i In peaks Select i.file)
        Call df.add(NameOf(PeakMs2.mz), From i In peaks Select i.mz)
        Call df.add(NameOf(PeakMs2.rt), From i In peaks Select i.rt)
        Call df.add(NameOf(PeakMs2.precursor_type), From i In peaks Select i.precursor_type)
        Call df.add(NameOf(PeakMs2.collisionEnergy), From i In peaks Select i.collisionEnergy)
        Call df.add(NameOf(PeakMs2.activation), From i In peaks Select i.activation)
        Call df.add(NameOf(PeakMs2.fragments), From i In peaks Select i.fragments)
        Call df.add(NameOf(PeakMs2.intensity), From i In peaks Select i.intensity)
        Call df.add(NameOf(PeakMs2.Ms2Intensity), From i In peaks Select i.Ms2Intensity)

        Dim ionsMs2 = peaks _
            .Select(Function(i)
                        Return i.mzInto _
                            .OrderByDescending(Function(m) m.intensity) _
                            .ToArray
                    End Function) _
            .ToArray

        Call df.add("basePeak", From i In ionsMs2 Select toString(i(Scan0)))
        Call df.add("top2", From i In ionsMs2 Select toString(i.ElementAtOrNull(1)))
        Call df.add("top3", From i In ionsMs2 Select toString(i.ElementAtOrNull(2)))
        Call df.add("top4", From i In ionsMs2 Select toString(i.ElementAtOrNull(3)))
        Call df.add("top5", From i In ionsMs2 Select toString(i.ElementAtOrNull(4)))

        Return df
    End Function

    Private Function toString(i As ms2) As String
        If i Is Nothing Then
            Return ""
        Else
            Return $"{i.mz.ToString("F4")}:{i.intensity.ToString("G3")}"
        End If
    End Function

    ''' <summary>
    ''' Calculates the SPLASH identifier for the given spectrum data.
    ''' </summary>
    ''' <param name="spec">The spectrum data, which can be a single spectrum object, a list, or an array of spectra.</param>
    ''' <param name="type">The type of spectrum (default is MS).</param>
    ''' <param name="env">The R environment for error handling.</param>
    ''' <returns>A SPLASH ID string or an array/list of SPLASH IDs if input is multiple spectra.</returns>
    ''' <remarks>
    ''' The SPLASH is an unambiguous, database-independent spectral identifier, 
    ''' just as the InChIKey is designed to serve as a unique identifier for 
    ''' chemical structures. It contains separate blocks that define different 
    ''' layers of information, separated by dashes. For example, the full SPLASH 
    ''' of a caffeine mass spectrum above is splash10-0002-0900000000-b112e4e059e1ecf98c5f.
    ''' The first block is the SPLASH identifier, the second and third are 
    ''' summary blocks, and the fourth is the unique hash block.
    ''' 
    ''' The SPLASH began As the MoNA (Massbank Of North America) hash, designed To 
    ''' identify duplicate spectra within the database. This idea developed further 
    ''' during the 2015 Metabolomics conference, where the SPLASH collaboration 
    ''' was formed. Currently, the specification has been formalized For mass 
    ''' spectrometry data. Additional specifications For IR, UV And NMR spectrometry
    ''' are planned.
    ''' </remarks>
    <ExportAPI("splash_id")>
    Public Function splashId(<RRawVectorArgument>
                             spec As Object,
                             Optional type As SpectrumType = SpectrumType.MS,
                             Optional env As Environment = Nothing) As Object

        Dim hash As New Splash(SpectrumType.MS)

        If TypeOf spec Is vector Then
            spec = DirectCast(spec, vector).data
        End If

        If TypeOf spec Is list Then
            Dim id As New list With {.slots = New Dictionary(Of String, Object)}

            For Each spec_item In DirectCast(spec, list).slots
                Call id.add(spec_item.Key, splashId(spec_item.Value, hash, env))

                If Program.isException(id(spec_item.Key)) Then
                    Return id(spec_item.Key)
                End If
            Next

            Return id
        ElseIf spec.GetType.IsArray Then
            Dim list As Array = spec
            Dim id As String() = New String(list.Length - 1) {}

            For i As Integer = 0 To id.Length - 1
                id(i) = splashId(list.GetValue(i), hash, env)
            Next

            Return id
        Else
            Return splashId(spec, hash, env)
        End If
    End Function

    ''' <summary>
    ''' calculate the splash id of a single spectrum object
    ''' </summary>
    ''' <param name="spec"></param>
    ''' <param name="hash"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function splashId(spec As Object, hash As Splash, env As Environment) As Object
        If TypeOf spec Is LibraryMatrix Then
            Return hash.CalcSplashID(DirectCast(spec, LibraryMatrix))
        ElseIf TypeOf spec Is PeakMs2 Then
            Return hash.CalcSplashID(DirectCast(spec, PeakMs2))
        Else
            Return Message.InCompatibleType(GetType(LibraryMatrix), spec.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' Union and merge the given multiple spectrum data into one single spectrum
    ''' </summary>
    ''' <param name="peaks">A collection of the <see cref="PeakMs2"/> spectrum object that going to merge into single one</param>
    ''' <param name="matrix">
    ''' this parameter will affects the data type of the value returns of this function:
    ''' 
    ''' 1. default false, returns a peak ms2 data object
    ''' 2. true, returns a library matrix data object
    ''' </param>
    ''' <param name="massDiff">the mass error for merge two spectra peak</param>
    ''' <param name="aggreate_sum">
    ''' default false means use the max intensity for the union merged peaks, 
    ''' or use the sum value of the intensity if this parameter value is set as TRUE.
    ''' </param>
    ''' <returns>
    ''' a single ms spectrum data object, its data type depeneds on the <paramref name="matrix"/> parameter.
    ''' </returns>
    <ExportAPI("unionPeaks")>
    <RApiReturn(GetType(PeakMs2), GetType(LibraryMatrix))>
    Public Function unionPeaks(peaks As PeakMs2(),
                               Optional norm As Boolean = False,
                               Optional matrix As Boolean = False,
                               Optional massDiff As Double = 0.1,
                               Optional aggreate_sum As Boolean = False) As Object

        Dim fragments As ms2() = peaks _
            .Select(Function(i) As IEnumerable(Of ms2)
                        If (Not i.mzInto.IsNullOrEmpty) AndAlso norm Then
                            Dim maxinto As Double = i.mzInto.Max(Function(a) a.intensity)
                            Dim normInto = i.mzInto.Select(Function(a) New ms2(a.mz, a.intensity / maxinto, a.Annotation))
                            Return normInto
                        Else
                            Return i.mzInto
                        End If
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(i) i.mz, offsets:=massDiff) _
            .Select(Function(i)
                        Dim mz As Double = i.OrderByDescending(Function(x) x.intensity).First.mz
                        Dim into As Double = If(
                            aggreate_sum,
                            i.Sum(Function(x) x.intensity),
                            i.Max(Function(x) x.intensity)
                        )

                        Return New ms2 With {
                            .mz = mz,
                            .intensity = into
                        }
                    End Function) _
            .ToArray

        If matrix Then
            Return New LibraryMatrix With {
                .ms2 = fragments,
                .centroid = True,
                .parentMz = peaks _
                    .OrderByDescending(Function(i) i.intensity) _
                    .First _
                    .mz
            }
        Else
            Return New PeakMs2 With {
                .file = peaks.Select(Function(i) i.file).Distinct.JoinBy("; "),
                .intensity = peaks.Sum(Function(i) i.intensity),
                .mzInto = fragments,
                .rt = peaks.Average(Function(i) i.rt)
            }
        End If
    End Function

    ''' <summary>
    ''' Generates a representative spectrum by aggregating (sum or mean) input spectra.
    ''' </summary>
    ''' <param name="x">Input spectra (PeakMs2 or LibraryMatrix collection).</param>
    ''' <param name="mean">If true, uses mean intensity; otherwise sums intensities.</param>
    ''' <param name="centroid">Mass tolerance for centroiding peaks.</param>
    ''' <param name="intocutoff">Relative intensity cutoff (0-1) to filter weak peaks.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>A LibraryMatrix representing the aggregated spectrum.</returns>
    <ExportAPI("representative")>
    <RApiReturn(GetType(LibraryMatrix))>
    Public Function representative_spectrum(<RRawVectorArgument> x As Object,
                                            Optional mean As Boolean = True,
                                            Optional centroid As Double = 0.1,
                                            Optional intocutoff As Double = 0.05,
                                            Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(x, env, suppress:=True)
        Dim representative As LibraryMatrix

        If Not pull.isError Then
            representative = pull.populates(Of PeakMs2)(env).SpectrumSum(centroid, average:=mean)
        Else
            pull = pipeline.TryCreatePipeline(Of LibraryMatrix)(x, env, suppress:=True)

            If Not pull.isError Then
                representative = pull.populates(Of LibraryMatrix)(env).SpectrumSum(centroid, average:=mean)
            Else
                Return pull.getError
            End If
        End If

        If intocutoff > 0 Then
            representative.ms2 = New RelativeIntensityCutoff(intocutoff).Trim(representative.ms2)
        End If

        Return representative
    End Function

    ''' <summary>
    ''' Gets the number of fragments in a spectrum object.
    ''' </summary>
    ''' <param name="matrix">A LibraryMatrix or PeakMs2 object.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>Integer count of fragments.</returns>
    <ExportAPI("nsize")>
    <RApiReturn(GetType(Integer))>
    Public Function nfragments(matrix As Object, Optional env As Environment = Nothing) As Object
        If matrix Is Nothing Then
            Return 0
        ElseIf TypeOf matrix Is LibraryMatrix Then
            Return DirectCast(matrix, LibraryMatrix).Length
        ElseIf TypeOf matrix Is PeakMs2 Then
            Return DirectCast(matrix, PeakMs2).fragments
        Else
            Return Message.InCompatibleType(GetType(LibraryMatrix), matrix.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' search the target query spectra against a reference mzpack data file
    ''' </summary>
    ''' <param name="q">The target spectra data, mz and into data fields must 
    ''' be included inside if this parameter value is a dataframe.</param>
    ''' <param name="refer">A mzpack data object that contains the reference 
    ''' spectrum dataset. The spectra dataset inside this mzpack data object
    ''' must be already been centroid processed!</param>
    ''' <returns></returns>
    <ExportAPI("search")>
    <RApiReturn(GetType(AlignmentOutput))>
    Public Function simpleSearch(q As Object, refer As mzPack,
                                 Optional tolerance As Object = "da:0.3",
                                 Optional intocutoff As Double = 0.05,
                                 Optional similarity_cutoff As Double = 0.3,
                                 Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)
        Dim spectra = getSpectrum(q, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        ElseIf spectra Like GetType(Message) Then
            Return spectra.TryCast(Of Message)
        ElseIf refer Is Nothing OrElse refer.MS Is Nothing Then
            Return RInternal.debug.stop("the required reference data should not be nothing!", env)
        End If

        Dim mzdiff As Tolerance = mzErr.TryCast(Of Tolerance)
        Dim cutoff As New RelativeIntensityCutoff(intocutoff)
        Dim query As LibraryMatrix = spectra.TryCast(Of LibraryMatrix).CentroidMode(mzdiff, cutoff)
        Dim cos As New CosAlignment(mzdiff, cutoff)
        Dim alignments As AlignmentOutput() = refer.MS _
            .AsParallel _
            .Select(Function(ms1)
                        Return ms1.products _
                            .SafeQuery _
                            .Select(Function(scan2)
                                        Dim align As AlignmentOutput = cos.CreateAlignment(query.ms2, scan2.GetMs.ToArray)

                                        align.query = New Meta With {.id = query.name}
                                        align.reference = New Meta With {.id = scan2.scan_id}

                                        Return align
                                    End Function)
                    End Function) _
            .IteratesALL _
            .OrderByDescending(Function(a) (a.forward + a.reverse + a.jaccard + a.entropy) / 4) _
            .Where(Function(a) (a.forward + a.reverse + a.jaccard + a.entropy) / 4 > similarity_cutoff) _
            .ToArray

        Return alignments
    End Function

    ''' <summary>
    ''' get alignment result tuple: query and reference
    ''' </summary>
    ''' <param name="align">AlignmentOutput object from spectral matching.</param>
    ''' <param name="query">Name label for query spectrum.</param>
    ''' <param name="reference">Name label for reference spectrum.</param>
    ''' <returns>
    ''' a tuple list object that contains spectrum alignment result:
    ''' 
    ''' 1. query - spectrum of sample query
    ''' 2. reference - spectrum of library reference
    ''' </returns>
    <ExportAPI("alignment_ref")>
    <RApiReturn("query", "reference")>
    Public Function getAlignmentReference(align As AlignmentOutput, Optional query$ = "Query", Optional reference$ = "Reference") As Object
        Dim tuple = align.GetAlignmentMirror
        Dim list As New list(
            slot("query") = tuple.query,
            slot("reference") = tuple.ref
        )

        tuple.query.name = query
        tuple.ref.name = reference

        Return list
    End Function

    ''' <summary>
    ''' Creates a formatted string representation of aligned peaks.
    ''' </summary>
    ''' <param name="mz">Array of m/z values for aligned peaks.</param>
    ''' <param name="query">Array of query intensities.</param>
    ''' <param name="reference">Array of reference intensities.</param>
    ''' <param name="annotation">Optional annotations for peaks.</param>
    ''' <returns>Formatted string showing alignment details.</returns>
    <ExportAPI("alignment_str")>
    Public Function makeAlignmentString(<RRawVectorArgument> mz As Object,
                                        <RRawVectorArgument> query As Object,
                                        <RRawVectorArgument> reference As Object,
                                        <RRawVectorArgument>
                                        Optional annotation As Object = Nothing) As String

        Return AlignmentOutput.CreateLinearMatrix(
            mz:=CLRVector.asNumeric(mz),
            query:=CLRVector.asNumeric(query),
            ref:=CLRVector.asNumeric(reference),
            annotation_str:=CLRVector.asCharacter(annotation)
        ).JoinBy(" ")
    End Function

    ''' <summary>
    ''' Constructs a PeakMs2 object from spectral data.
    ''' </summary>
    ''' <param name="precursor">Precursor m/z value.</param>
    ''' <param name="rt">Retention time in seconds.</param>
    ''' <param name="mz">Array of fragment m/z values.</param>
    ''' <param name="into">Array of fragment intensities.</param>
    ''' <param name="totalIons">Total ion current (optional).</param>
    ''' <param name="file">Source file identifier.</param>
    ''' <param name="libname">Library identifier.</param>
    ''' <param name="precursor_type">Precursor adduct type.</param>
    ''' <param name="meta">Metadata list for the peak.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>A PeakMs2 object containing the spectral data.</returns>
    <ExportAPI("peakMs2")>
    Public Function createPeakMs2(precursor As Double, rt As Double, mz As Double(), into As Double(),
                                  Optional totalIons As Double = 0,
                                  Optional file As String = Nothing,
                                  Optional libname As String = Nothing,
                                  Optional precursor_type As String = Nothing,
                                  <RListObjectArgument>
                                  Optional meta As list = Nothing,
                                  Optional env As Environment = Nothing) As PeakMs2

        Return New PeakMs2 With {
            .mz = precursor,
            .intensity = totalIons,
            .mzInto = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = into(i)
                            }
                        End Function) _
                .ToArray,
            .rt = rt,
            .file = file,
            .meta = meta.AsGeneric(Of String)(env),
            .lib_guid = libname,
            .precursor_type = precursor_type
        }
    End Function

    ''' <summary>
    ''' make a tuple list via grouping of the spectrum data via the ROI id inside the metadata list
    ''' </summary>
    ''' <param name="peakms2">a collection of the spectrum data to make spectrum data grouping.</param>
    ''' <param name="default">the default ROI id for make the data groups if the metadata 
    ''' is null or the ``ROI`` id tag is missing from the spectrum object metadata.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A tuple list object that contains the spectrum data grouping by the ROI id.
    ''' 
    ''' The key of the tuple list is the ROI id, and the value is a list of spectrum data
    ''' that belongs to this ROI id.
    ''' 
    ''' If no ROI id was assigned, a warning message will be added to the runtime environment.
    ''' </returns>
    <ExportAPI("groupBy_ROI")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function groupBy_ROI(<RRawVectorArgument> peakms2 As Object,
                                Optional default$ = "Not_Assigned",
                                Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(peakms2, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim ROI_groups = pull.populates(Of PeakMs2)(env) _
            .GroupBy(Function(s)
                         If s.meta Is Nothing Then
                             Return [default]
                         Else
                             Return If(s.meta.ContainsKey("ROI"), s.meta!ROI, [default])
                         End If
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CObj(a.ToArray)
                          End Function)

        If ROI_groups.Count = 1 AndAlso ROI_groups.Keys.First = [default] Then
            Call env.AddMessage("No peak ROI id was assigned with the given spectrum data!")
        End If

        Return New list With {.slots = ROI_groups}
    End Function

    ''' <summary>
    ''' Create a library matrix object
    ''' </summary>
    ''' <param name="matrix">
    ''' for a dataframe object, should contains column data:
    ''' mz, into and annotation.
    ''' </param>
    ''' <param name="title"></param>
    ''' <param name="env"></param>
    ''' <returns>A simple mzkit spectrum peak list object</returns>
    ''' <example>
    ''' libraryMatrix(mz = [100 101], intensity = [100 35]);
    ''' 
    ''' # or construct from a dataframe
    ''' let data = data.frame(mz = [100 101], into = [100 35]);
    ''' let msPeaks = libraryMatrix(data);
    ''' </example>
    <ExportAPI("libraryMatrix")>
    Public Function libraryMatrix(<RRawVectorArgument>
                                  Optional matrix As Object = Nothing,
                                  Optional title$ = "MS Matrix",
                                  Optional parentMz As Double = -1,
                                  Optional centroid As Boolean = False,
                                  <RListObjectArgument>
                                  Optional args As list = Nothing,
                                  Optional env As Environment = Nothing) As Object
        Dim MS As ms2()

        If matrix Is Nothing Then
            Dim mz As Double() = CLRVector.asNumeric(args.getBySynonyms("mz", "MZ", "m/z"))
            Dim into As Double() = CLRVector.asNumeric(args.getBySynonyms("into", "intensity"))

            If mz.IsNullOrEmpty OrElse into.IsNullOrEmpty Then
                Return RInternal.debug.stop("No mass spectrum peaks data was assigned!", env)
            ElseIf mz.Length <> into.Length Then
                Return RInternal.debug.stop($"The vector data size of mz({mz.Length}) is mis-matched with the intensity vector({into.Length})!", env)
            End If

            MS = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {.mz = mzi, .intensity = into(i)}
                        End Function) _
                .ToArray
        ElseIf TypeOf matrix Is rDataframe Then
            MS = DirectCast(matrix, rDataframe).MsdataFromDf.ToArray
        Else
            Dim data As pipeline = pipeline.TryCreatePipeline(Of ms2)(matrix, env)

            If data.isError Then
                Return data.getError
            End If

            MS = data.populates(Of ms2)(env).ToArray
        End If

        Return New LibraryMatrix With {
            .name = title,
            .ms2 = MS,
            .parentMz = parentMz,
            .centroid = centroid
        }
    End Function

    <Extension>
    Private Function MsdataFromDf(ms2 As rDataframe) As IEnumerable(Of ms2)
        Dim mz As Double() = ms2.getVector(Of Double)("mz", "m/z")
        Dim into As Double() = ms2.getVector(Of Double)("into", "intensity")
        Dim annotation As String() = ms2.getVector(Of String)("annotation")

        If annotation Is Nothing Then
            annotation = {}
        End If

        Return mz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = into(i),
                            .Annotation = annotation.ElementAtOrDefault(i)
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' Groups MS1 scans into XIC (Extracted Ion Chromatogram) groups by m/z.
    ''' </summary>
    ''' <param name="ms1">Input MS1 data (mzPack or array of scans).</param>
    ''' <param name="tolerance">Mass tolerance for grouping m/z values.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>A list of XIC groups, each containing scans with similar m/z.</returns>
    <ExportAPI("XIC_groups")>
    Public Function XICGroups(<RRawVectorArgument>
                              ms1 As Object,
                              Optional tolerance As Object = "ppm:20",
                              Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)
        Dim mzdiff As Tolerance

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        Else
            mzdiff = mzErr.TryCast(Of Tolerance)
        End If

        If TypeOf ms1 Is mzPack Then
            Return DirectCast(ms1, mzPack).getRawXICSet(mzdiff)
        ElseIf TypeOf ms1 Is mzPack() Then
            Dim list As mzPack() = DirectCast(ms1, mzPack())

            If list.Length = 1 Then
                Return list(Scan0).getRawXICSet(mzdiff)
            Else
                Dim output As New list With {
                    .slots = New Dictionary(Of String, Object)
                }

                For i As Integer = 0 To list.Length - 1
                    Call output.add(list(i).source Or $"#{i + 1}".AsDefault, list(i).getRawXICSet(mzdiff))
                Next

                Return output
            End If
        Else
            Return pipeline _
                .TryCreatePipeline(Of IMs1)(ms1, env, suppress:=True) _
                .populates(Of IMs1)(env) _
                .getXICPoints(mzdiff)
        End If
    End Function

    <Extension>
    Private Function getXICPoints(Of T As IMs1)(ms1_scans As IEnumerable(Of T), mzdiff As Tolerance) As list
        Dim mzgroups = ms1_scans.GroupBy(Function(x) x.mz, mzdiff).ToArray
        Dim xic As New list With {.slots = New Dictionary(Of String, Object)}

        For Each mzi As NamedCollection(Of T) In mzgroups
            xic.add(Val(mzi.name).ToString("F4"), mzi.OrderBy(Function(ti) ti.rt).ToArray)
        Next

        Return xic
    End Function

    <Extension>
    Private Function getRawXICSet(raw As mzPack, tolerance As Tolerance) As list
        Dim allPoints = raw.GetAllScanMs1().ToArray
        Dim pack = allPoints.getXICPoints(tolerance)

        Return pack
    End Function

    ''' <summary>
    ''' Extracts chromatogram data for a specific m/z from MS1 scans.
    ''' </summary>
    ''' <param name="ms1">Input MS1 data (mzPack, PeakSet, or scan array).</param>
    ''' <param name="mz">Target m/z value to extract.</param>
    ''' <param name="tolerance">Mass tolerance for m/z matching.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>ChromatogramTick array or ChromatogramOverlap object.</returns>
    <ExportAPI("XIC")>
    <RApiReturn(GetType(ms1_scan), GetType(ChromatogramTick), GetType(ChromatogramOverlap))>
    Public Function XIC(<RRawVectorArgument> ms1 As Object, mz#,
                        Optional tolerance As Object = "ppm:20",
                        Optional env As Environment = Nothing) As Object

        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of IMs1)(ms1, env, suppress:=True)
        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim mzdiff As Tolerance = mzErr.TryCast(Of Tolerance)

        If ms1_scans.isError Then
            If TypeOf ms1 Is mzPack Then
                Return DirectCast(ms1, mzPack).rawXIC(mz, mzdiff)
            ElseIf TypeOf ms1 Is mzPack() Then
                Dim all = DirectCast(ms1, mzPack())

                If all.Length = 1 Then
                    Return all(Scan0).rawXIC(mz, mzdiff)
                Else
                    Dim list As New list With {.slots = New Dictionary(Of String, Object)}

                    For i As Integer = 0 To all.Length - 1
                        Call list.add($"#{i + 1}", all(i).rawXIC(mz, mzdiff))
                    Next

                    Return list
                End If
            ElseIf TypeOf ms1 Is PeakSet Then
                Dim pkset As PeakSet = ms1
                Dim da As Double = mzdiff.GetErrorDalton
                Dim peaks = pkset.FilterMz(mz, mzdiff:=da).ToArray
                Dim overlaps As New ChromatogramOverlap
                Dim sample_names As String() = peaks.Split(peaks.Length / 8) _
                    .AsParallel _
                    .Select(Function(a) a.PropertyNames) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
                Dim peak_rt = peaks _
                    .GroupBy(Function(a) a.rt, offsets:=0.5) _
                    .OrderBy(Function(a) Val(a.name)) _
                    .ToArray
                Dim rt As Double() = peak_rt _
                    .Select(Function(a) Val(a.name)) _
                    .ToArray

                For Each name As String In sample_names
                    Dim tic As Double() = peak_rt _
                        .Select(Function(a)
                                    Return Aggregate ti As xcms2 In a Into Sum(ti(name))
                                End Function) _
                        .ToArray
                    Dim bpc As Double() = peak_rt _
                        .Select(Function(a)
                                    Return Aggregate ti As xcms2 In a Into Max(ti(name))
                                End Function) _
                        .ToArray

                    overlaps.overlaps(name) = New Chromatogram With {
                        .BPC = bpc,
                        .scan_time = rt,
                        .TIC = tic
                    }
                Next

                Return overlaps
            Else
                Return ms1_scans.getError
            End If
        Else
        End If

        Dim xicFilter As Array = ms1_scans _
            .populates(Of IMs1)(env) _
            .Where(Function(pt) mzdiff(pt.mz, mz)) _
            .OrderBy(Function(a) a.rt) _
            .Select(Function(a) CObj(a)) _
            .ToArray

        Return REnv.TryCastGenericArray(xicFilter, env)
    End Function

    <Extension>
    Private Function rawXIC(ms1 As mzPack, mz As Double, mzdiff As Tolerance) As ChromatogramTick()
        Return ms1.MS _
            .Select(Function(scan)
                        Dim i As Double = scan.GetIntensity(mz, mzdiff)
                        Dim tick As New ChromatogramTick With {
                            .Intensity = i,
                            .Time = scan.rt
                        }

                        Return tick
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Filters MS1 scans within a specified retention time window.
    ''' </summary>
    ''' <param name="ms1">Input MS1 scan data.</param>
    ''' <param name="rtmin">Minimum retention time.</param>
    ''' <param name="rtmax">Maximum retention time.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>Array of MS1 scans within the RT window.</returns>
    <ExportAPI("rt_slice")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function RtSlice(<RRawVectorArgument>
                            ms1 As Object,
                            rtmin#, rtmax#,
                            Optional env As Environment = Nothing) As Object

        Dim ms1_scans As pipeline = pipeline.TryCreatePipeline(Of IMs1)(ms1, env)

        If ms1_scans.isError Then
            Return ms1_scans.getError
        End If

        Dim xicFilter As Array = ms1_scans _
            .populates(Of IMs1)(env) _
            .Where(Function(pt) pt.rt >= rtmin AndAlso pt.rt <= rtmax) _
            .OrderBy(Function(a) a.rt) _
            .Select(Function(a) CObj(a)) _
            .ToArray

        Return REnv.TryCastGenericArray(xicFilter, env)
    End Function

    ''' <summary>
    ''' Extracts intensity values from MS1 scans or PeakMs2 spectra.
    ''' </summary>
    ''' <param name="ticks">Input scans or spectra.</param>
    ''' <param name="mz">Optional m/z to extract specific intensity.</param>
    ''' <param name="mzdiff">Mass tolerance for m/z matching.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>Numeric vector of intensity values.</returns>
    <ExportAPI("intensity")>
    <RApiReturn(GetType(Double))>
    Public Function getIntensity(<RRawVectorArgument>
                                 ticks As Object,
                                 Optional mz As Double = -1,
                                 Optional mzdiff As Object = "da:0.3",
                                 Optional env As Environment = Nothing) As Object

        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env, suppress:=True)
        Dim tolerance = Math.getTolerance(mzdiff, env)

        If scans.isError Then
            scans = pipeline.TryCreatePipeline(Of PeakMs2)(ticks, env)

            If scans.isError Then
                Return scans.getError
            End If

            If mz > 0 AndAlso tolerance Like GetType(Message) Then
                Return tolerance.TryCast(Of Message)
            End If

            Dim mzErr As Tolerance = tolerance.TryCast(Of Tolerance)

            Return scans _
                .populates(Of PeakMs2)(env) _
                .Select(Function(x)
                            If mz > 0 Then
                                Return x.GetIntensity(mz, mzErr)
                            Else
                                Return x.intensity
                            End If
                        End Function) _
                .DoCall(AddressOf vector.asVector)
        End If

        Return scans _
            .populates(Of ms1_scan)(env) _
            .Select(Function(x) x.intensity) _
            .DoCall(AddressOf vector.asVector)
    End Function

    ''' <summary>
    ''' Extracts retention times from MS1 scans or PeakMs2 spectra.
    ''' </summary>
    ''' <param name="ticks">Input scans or spectra.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>Numeric vector of retention times.</returns>
    <ExportAPI("scan_time")>
    <RApiReturn(GetType(Double))>
    Public Function getScantime(<RRawVectorArgument>
                                ticks As Object,
                                Optional env As Environment = Nothing) As Object

        Dim scans As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(ticks, env)

        If scans.isError Then
            scans = pipeline.TryCreatePipeline(Of PeakMs2)(ticks, env)

            If scans.isError Then
                Return scans.getError
            Else
                Return scans.populates(Of PeakMs2)(env) _
                    .Select(Function(x) x.rt) _
                    .DoCall(AddressOf vector.asVector)
            End If
        End If

        Return scans.populates(Of ms1_scan)(env) _
            .Select(Function(x) x.scan_time) _
            .DoCall(AddressOf vector.asVector)
    End Function

    ''' <summary>
    ''' Generates unique ROI (Region of Interest) IDs for spectra.
    ''' </summary>
    ''' <param name="ROIlist">Input PeakMs2 spectra or list with mz/rt vectors.</param>
    ''' <param name="name_chrs">If true, returns ROI IDs as strings; otherwise updates PeakMs2 metadata.</param>
    ''' <param name="prefix">Prefix for ROI IDs.</param>
    ''' <param name="env">R environment for error handling.</param>
    ''' <returns>String array of ROI IDs or modified PeakMs2 objects array.</returns>
    <ExportAPI("make.ROI_names")>
    <RApiReturn(GetType(PeakMs2), GetType(String))>
    Public Function makeROInames(<RRawVectorArgument> ROIlist As Object,
                                 Optional name_chrs As Boolean = False,
                                 Optional prefix As String = "",
                                 Optional env As Environment = Nothing) As Object

        If TypeOf ROIlist Is list AndAlso {"mz", "rt"}.All(AddressOf DirectCast(ROIlist, list).hasName) Then
            Dim mz As Double() = DirectCast(ROIlist, list).getValue(Of Double())("mz", env)
            Dim rt As Double() = DirectCast(ROIlist, list).getValue(Of Double())("rt", env)

            Return xcms_id(mz, rt, prefix:=prefix)
        End If

        Dim dataList As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ROIlist, env)

        If dataList.isError Then
            Return dataList.getError
        End If

        Dim allData As PeakMs2() = dataList.populates(Of PeakMs2)(env).ToArray
        Dim uniques As String() = xcms_id(
            mz:=allData.Select(Function(p) p.mz).ToArray,
            rt:=allData.Select(Function(p) p.rt).ToArray,
            prefix:=prefix
        )

        If name_chrs Then
            Return uniques
        Else
            For i As Integer = 0 To allData.Length - 1
                allData(i).lib_guid = uniques(i)
                Call Algorithm.SimpleSetROI(allData(i), uniques(i))
            Next

            Return allData
        End If
    End Function

    ''' <summary>
    ''' Reads a spectral matrix from a CSV file.
    ''' </summary>
    ''' <param name="file">Path to CSV file containing spectral data.</param>
    ''' <returns>Array of Library objects parsed from the file.</returns>
    <ExportAPI("read.MsMatrix")>
    Public Function readMatrix(file As String) As Spectra.Library()
        Return file.LoadCsv(Of Spectra.Library)
    End Function

    ''' <summary>
    ''' Generates a string representation of top intensity ions from spectra.
    ''' </summary>
    ''' <param name="data">Input PeakMs2 spectra.</param>
    ''' <param name="topIons">Number of top ions to include per spectrum.</param>
    ''' <returns>String array formatted as "mz:intensity" for top ions.</returns>
    <ExportAPI("linearMatrix")>
    Public Function linearMatrix(data As PeakMs2(), Optional topIons As Integer = 5) As String()
        Dim da = Tolerance.DeltaMass(0.3)
        Dim into As RelativeIntensityCutoff = 0.0

        Return data _
            .Select(Function(ms2)
                        Return ms2.mzInto _
                            .Centroid(da, into) _
                            .OrderByDescending(Function(a) a.intensity) _
                            .Take(topIons) _
                            .Select(Function(a)
                                        Return $"{a.mz.ToString("F4")}:{a.intensity.ToString("G5")}"
                                    End Function) _
                            .JoinBy("; ")
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' use log foldchange for compares two spectrum
    ''' </summary>
    ''' <param name="spec1"></param>
    ''' <param name="spec2"></param>
    ''' <returns></returns>
    <ExportAPI("logfc")>
    Public Function logfc_f(spec1 As LibraryMatrix, spec2 As LibraryMatrix,
                            Optional da As Double = 0.03,
                            Optional lb1 As String = Nothing,
                            Optional lb2 As String = Nothing) As Object

        Dim label1 = If(spec1.name.StringEmpty(, True), NameOf(spec1), spec1.name)
        Dim label2 = If(spec2.name.StringEmpty(, True), NameOf(spec2), spec2.name)

        label1 = If(lb1.StringEmpty(, True), label1, lb1)
        label2 = If(lb2.StringEmpty(, True), label2, lb2)

        If label1 = label2 Then
            label1 = $"{label1}_1"
            label2 = $"{label2}_2"
        End If

        Dim s1 = spec1.ms2.Select(Function(a) New ms2(a, label1)).ToArray
        Dim s2 = spec2.ms2.Select(Function(a) New ms2(a, label2)).ToArray
        Dim merge = s1.JoinIterates(s2).GroupBy(Function(m) m.mz, da).ToArray
        Dim mz As Double() = merge.Select(Function(i) Val(i.name)).ToArray
        Dim i1 As Double() = merge.Select(Function(i) i.Where(Function(m) m.Annotation = label1).Sum(Function(a) a.intensity)).ToArray
        Dim i2 As Double() = merge.Select(Function(i) i.Where(Function(m) m.Annotation = label2).Sum(Function(a) a.intensity)).ToArray
        Dim logfc As Double() = i1 _
            .Select(Function(into1, i)
                        Dim into2 As Double = i2(i)

                        If into1 <= 0.0 Then
                            Return 0
                        ElseIf into2 <= 0 Then
                            Return Double.PositiveInfinity
                        Else
                            Return Double.Log(into1 / into2, 2)
                        End If
                    End Function) _
            .ToArray

        Return New rDataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mz},
                {label1, i1},
                {label2, i2},
                {"logfc", logfc},
                {"abs_logfc", logfc.Select(Function(a) std.Abs(a)).ToArray}
            }
        }
    End Function

    <ExportAPI("msn_matrix")>
    <RApiReturn(GetType(MzMatrix))>
    Public Function msn_matrix(<RRawVectorArgument> raw As Object,
                               Optional mzdiff As Double = 0.01,
                               Optional q As Double = 0.01,
                               Optional env As Environment = Nothing) As Object

        Dim rawdata As pipeline = pipeline.TryCreatePipeline(Of mzPack)(raw, env)

        If rawdata.isError Then
            Return rawdata.getError
        End If

        Dim pooldata As MSnFragmentProvider() = rawdata.populates(Of mzPack)(env) _
            .Select(Function(s) New MSnFragmentProvider(s)) _
            .ToArray

        Return MassFragmentPool.CreateMatrix(pooldata, mzdiff, q)
    End Function
End Module

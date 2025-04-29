#Region "Microsoft.VisualBasic::15a2867a85f409d6f0d30e6232ca1ebb, Rscript\Library\mzkit_app\src\mzquant\mzDeco.vb"

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

'   Total Lines: 1376
'    Code Lines: 912 (66.28%)
' Comment Lines: 317 (23.04%)
'    - Xml Docs: 88.33%
' 
'   Blank Lines: 147 (10.68%)
'     File Size: 57.63 KB


' Module mzDeco
' 
'     Function: adjust_to_seconds, convertDataframeToXcmsPeaks, create_peakset, Deconv, dumpPeaks
'               expression, get_ionPeak, getIonPeak, ms1Scans, mz_deco
'               mz_groups, peakAlignment, peaksetMatrix, peaksSetMatrix, peakSubset
'               peaktable, pull_xic, read_rtshifts, readPeakData, readPeaktable
'               readSamples, readXcmsFeaturePeaks, readXcmsPeaks, readXcmsTableFile, readXIC
'               RI_batch_join, RI_calc, RI_reference, to_matrix, writePeaktable
'               writeSamples, writeXcmsPeaktable, writeXIC, writeXIC1, xcms_peak
'               xic_deco, xic_dtw_list, xic_matrix_list, XICpool_func
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Tasks
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports deco_math = BioNovoGene.Analytical.MassSpectrometry.Math.Extensions
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math
Imports vec = SMRUCC.Rsharp.Runtime.Internal.Object.vector

''' <summary>
''' Extract peak and signal data from rawdata
''' 
''' Data processing is the computational process of converting raw LC-MS 
''' data to biological knowledge and involves multiple processes including 
''' raw data deconvolution and the chemical identification of metabolites.
''' 
''' The process of data deconvolution, sometimes called peak picking, is 
''' in itself a complex process caused by the complexity of the data and 
''' variation introduced during the process of data acquisition related to 
''' mass-to-charge ratio, retention time and chromatographic peak area.
''' </summary>
<Package("mzDeco")>
<RTypeExport("peak_feature", GetType(PeakFeature))>
<RTypeExport("mz_group", GetType(MzGroup))>
<RTypeExport("peak_set", GetType(PeakSet))>
<RTypeExport("xcms2", GetType(xcms2))>
<RTypeExport("rt_shift", GetType(RtShift))>
<RTypeExport("RI_refer", GetType(RIRefer))>
Module mzDeco

    Sub Main()
        Call RInternal.Object.Converts.addHandler(GetType(PeakFeature()), AddressOf peaktable)
        Call RInternal.Object.Converts.addHandler(GetType(xcms2()), AddressOf peaksetMatrix)
        Call RInternal.Object.Converts.addHandler(GetType(PeakSet), AddressOf peaksSetMatrix)

        Call generic.add("readBin.mz_group", GetType(Stream), AddressOf readXIC)
        Call generic.add("readBin.peak_feature", GetType(Stream), AddressOf readSamples)
        Call generic.add("readBin.peak_set", GetType(Stream), AddressOf readPeaktable)

        Call generic.add("writeBin", GetType(MzGroup), AddressOf writeXIC1)
        Call generic.add("writeBin", GetType(MzGroup()), AddressOf writeXIC)
        Call generic.add("writeBin", GetType(PeakFeature()), AddressOf writeSamples)
        Call generic.add("writeBin", GetType(PeakSet), AddressOf writePeaktable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function peaksSetMatrix(peaks As PeakSet, args As list, env As Environment) As dataframe
        Return peaksetMatrix(peaks.peaks, args, env)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writePeaktable(table As PeakSet, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveXcms.DumpSample(table, con)
        Call con.Flush()
        Return True
    End Function

    <RGenericOverloads("readBin")>
    Private Function readPeaktable(file As Stream, args As list, env As Environment) As Object
        Return SaveXcms.ReadSample(file)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeSamples(samples As PeakFeature(), args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveSample.DumpSample(samples, con)
        Call con.Flush()
        Return True
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeXIC1(xic As MzGroup, args As list, env As Environment) As Object
        Return writeXIC({xic}, args, env)
    End Function

    <RGenericOverloads("writeBin")>
    Private Function writeXIC(xic As MzGroup(), args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveXIC.DumpSample(xic, con)
        Call con.Flush()
        Return True
    End Function

    <RGenericOverloads("readBin")>
    Private Function readSamples(file As Stream, args As list, env As Environment) As Object
        Return SaveSample.ReadSample(file).ToArray
    End Function

    <RGenericOverloads("readBin")>
    Private Function readXIC(file As Stream, args As list, env As Environment) As Object
        Return SaveXIC.ReadSample(file).ToArray
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function peaksetMatrix(peakset As xcms2(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
           .columns = New Dictionary(Of String, Array)
        }
        Dim allsampleNames = peakset _
            .Select(Function(i) i.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(a) a) _
            .ToArray

        table.rownames = peakset _
            .Select(Function(p) p.ID) _
            .ToArray

        Call table.add(NameOf(xcms2.mz), peakset.Select(Function(a) a.mz))
        Call table.add(NameOf(xcms2.mzmin), peakset.Select(Function(a) a.mzmin))
        Call table.add(NameOf(xcms2.mzmax), peakset.Select(Function(a) a.mzmax))
        Call table.add(NameOf(xcms2.rt), peakset.Select(Function(a) a.rt))
        Call table.add(NameOf(xcms2.rtmin), peakset.Select(Function(a) a.rtmin))
        Call table.add(NameOf(xcms2.rtmax), peakset.Select(Function(a) a.rtmax))
        Call table.add(NameOf(xcms2.RI), peakset.Select(Function(a) a.RI))
        Call table.add(NameOf(xcms2.npeaks), peakset.Select(Function(a) a.npeaks))

        For Each name As String In allsampleNames
            Call table.add(name, peakset.Select(Function(i) i(name)))
        Next

        Return table
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function peaktable(x As PeakFeature(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        table.add(NameOf(PeakFeature.mz), x.Select(Function(a) a.mz))
        table.add(NameOf(PeakFeature.rt), x.Select(Function(a) a.rt))
        table.add(NameOf(PeakFeature.RI), x.Select(Function(a) a.RI))
        table.add(NameOf(PeakFeature.rtmin), x.Select(Function(a) a.rtmin))
        table.add(NameOf(PeakFeature.rtmax), x.Select(Function(a) a.rtmax))
        table.add(NameOf(PeakFeature.maxInto), x.Select(Function(a) a.maxInto))
        table.add(NameOf(PeakFeature.baseline), x.Select(Function(a) a.baseline))
        table.add(NameOf(PeakFeature.integration), x.Select(Function(a) a.integration))
        table.add(NameOf(PeakFeature.area), x.Select(Function(a) a.area))
        table.add(NameOf(PeakFeature.noise), x.Select(Function(a) a.noise))
        table.add(NameOf(PeakFeature.nticks), x.Select(Function(a) a.nticks))
        table.add(NameOf(PeakFeature.snRatio), x.Select(Function(a) a.snRatio))
        table.add(NameOf(PeakFeature.rawfile), x.Select(Function(a) a.rawfile))

        table.rownames = x.Select(Function(a) a.xcms_id).ToArray

        Return table
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <param name="features_mz"></param>
    ''' <param name="errors"></param>
    ''' <param name="rtRange"></param>
    ''' <param name="baseline"></param>
    ''' <param name="joint"></param>
    ''' <param name="dtw"></param>
    ''' <param name="parallel"></param>
    ''' <returns>a vector of <see cref="xcms2"/></returns>
    <Extension>
    Private Function xic_deco(pool As XICPool, features_mz As Double(),
                              errors As Tolerance,
                              rtRange As DoubleRange,
                              baseline As Double,
                              joint As Boolean,
                              dtw As Boolean,
                              parallel As Boolean) As Object

        VectorTask.n_threads = App.CPUCoreNumbers

        If features_mz.Length = 1 Then
            ' extract the aligned data
            Return xic_deco_task.ExtractAlignedPeaks(
                pool.DtwXIC(features_mz(0), errors).ToArray,
                rtRange:=rtRange,
                baseline:=baseline,
                joint:=joint, xic_align:=True, rt_shifts:=Nothing)
        Else
            Dim task As New xic_deco_task(pool, features_mz, errors, rtRange, baseline, joint, dtw)

            If parallel Then
                Call task.Run()
            Else
                Call task.Solve()
            End If

            Dim result = xcms2.MakeUniqueId(task.out).ToArray
            Dim vec As New vec(result, RType.GetRSharpType(GetType(xcms2)))
            Dim rt_diff As RtShift() = task.rt_shifts.ToArray

            Call vec.setAttribute("rt.shift", rt_diff)

            Return vec
        End If
    End Function

    ''' <summary>
    ''' read the peaktable file that in xcms2 output format
    ''' </summary>
    ''' <param name="file">should be the file path to the peaktable csv/txt file.</param>
    ''' <param name="make_unique">
    ''' set this parameter to value TRUE will ensure that the xcms reference id is always unique
    ''' </param>
    ''' <returns>A collection set of the <see cref="xcms2"/> peak features data object</returns>
    ''' <keywords>read data</keywords>
    <ExportAPI("read.xcms_peaks")>
    <RApiReturn(GetType(PeakSet))>
    Public Function readXcmsPeaks(file As Object,
                                  Optional tsv As Boolean = False,
                                  Optional general_method As Boolean = False,
                                  Optional make_unique As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            Return RInternal.debug.stop("the required file connection for read the xcms peaktable file should not be nothing!", env)
        End If

        If TypeOf file Is String Then
            Return readXcmsTableFile(file, general_method, tsv, make_unique)
        ElseIf TypeOf file Is AnnotationWorkspace Then
            Return New PeakSet(DirectCast(file, AnnotationWorkspace).LoadPeakTable)
        Else
            Return Message.InCompatibleType(GetType(String), file.GetType, env)
        End If
    End Function

    Private Function readXcmsTableFile(file As String, general_method As Boolean, tsv As Boolean, make_unique As Boolean) As Object
        If file.ExtensionSuffix("dat", "xcms") Then
            ' read binary file
            Using buf As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return SaveXcms.ReadSample(buf)
            End Using
        End If

        If Not general_method Then
            Return SaveXcms.ReadTextTable(file, tsv, make_unique)
        Else
            Return New PeakSet With {
                .peaks = file.LoadCsv(Of xcms2)(mute:=True).ToArray
            }
        End If
    End Function

    ''' <summary>
    ''' cast peaktable to expression matrix object
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    ''' <keywords>ctype data</keywords>
    <ExportAPI("to_expression")>
    Public Function expression(x As PeakSet) As Matrix
        Dim sampleNames As String() = x.sampleNames
        Dim features As New List(Of DataFrameRow)

        For Each ion As xcms2 In x.peaks
            Call features.Add(New DataFrameRow With {
                .geneID = ion.ID,
                .experiments = ion(sampleNames)
            })
        Next

        Return New Matrix With {
            .sampleID = sampleNames,
            .tag = "peaktable",
            .expression = features.ToArray
        }
    End Function

    ''' <summary>
    ''' cast peaktable to mzkit expression matrix
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    ''' <keywords>ctype data</keywords>
    <ExportAPI("to_matrix")>
    Public Function to_matrix(x As PeakSet) As MzMatrix
        Dim mz As Double() = x.peaks.Select(Function(a) a.mz).ToArray
        Dim mzmin As Double() = x.peaks.Select(Function(a) a.mzmin).ToArray
        Dim mzmax As Double() = x.peaks.Select(Function(a) a.mzmax).ToArray
        Dim samples As New List(Of PixelData)
        Dim table As xcms2() = x.peaks

        For Each name As String In x.sampleNames
            Call samples.Add(New PixelData With {
                .label = name,
                .intensity = table _
                    .Select(Function(a) a(name)) _
                    .ToArray
            })
        Next

        Return New MzMatrix With {
            .matrixType = FileApplicationClass.LCMS,
            .mz = mz,
            .mzmin = mzmin,
            .mzmax = mzmax,
            .tolerance = 0,
            .matrix = samples.ToArray
        }
    End Function

    ''' <summary>
    ''' save mzkit peaktable object to csv table file
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="file">the file path to the target csv table file</param>
    ''' <returns></returns>
    <ExportAPI("write.xcms_peaks")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function writeXcmsPeaktable(x As PeakSet, file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return RInternal.debug.stop("the required file connection for save the xcms peaktable data should not be nothing!", env)
        End If

        If TypeOf file Is String Then
            Return x.peaks.SaveTo(CStr(file), silent:=True)
        ElseIf TypeOf file Is Stream Then
            Return SaveXcms.DumpSample(x, DirectCast(file, Stream))
        ElseIf TypeOf file Is AnnotationWorkspace Then
            Call DirectCast(file, AnnotationWorkspace).SetPeakTable(x.peaks)
            Return True
        Else
            Return Message.InCompatibleType(GetType(String), file.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' cast dataset to mzkit peaktable object
    ''' </summary>
    ''' <param name="x">should be a data collection of the peaks data, value could be:
    ''' 
    ''' 1. a collection of the <see cref="xcms2"/> ROI peaks data
    ''' 2. an actual <see cref="PeakSet"/> object, then this function will make value copy of this object
    ''' 3. a dataframe object that contains the peaks data for make the data conversion
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' for make data object conversion from a R# runtime dataframe object, that these data 
    ''' fields is required for creates the xcms peaks object:
    ''' 
    ''' 1. mz, mzmin, mzmax: the ion m/z value of the xcms peak
    ''' 2. rt, rtmin, rtmax: the ion retention time of the xcms peak data, should be in time unit seconds
    ''' 3. RI: the ion retention index value that evaluated based on the RT value
    ''' 4. all of the other data fields in the dataframe will be treated as the sample peak area data.
    ''' </remarks>
    ''' <keywords>ctype data</keywords>
    <ExportAPI("as.peak_set")>
    <RApiReturn(GetType(PeakSet))>
    Public Function create_peakset(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        Dim pull = pipeline.TryCreatePipeline(Of xcms2)(x, env, suppress:=True)
        Dim peaks As New List(Of xcms2)

        If pull.isError Then
            ' deal with dataframe?
            If TypeOf x Is dataframe Then
                Call peaks.AddRange(convertDataframeToXcmsPeaks(DirectCast(x, dataframe)))
            ElseIf TypeOf x Is PeakSet Then
                ' make peakset data copy
                Return New PeakSet(DirectCast(x, PeakSet).peaks)
            Else
                Return pull.getError
            End If
        Else
            Call peaks.AddRange(pull.populates(Of xcms2)(env))
        End If

        Return New PeakSet(peaks)
    End Function

    ''' <summary>
    ''' extract the peak data from the given dataframe
    ''' </summary>
    ''' <param name="df"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the unique id of each peak object extract from the dataframe row names
    ''' </remarks>
    Private Iterator Function convertDataframeToXcmsPeaks(df As dataframe) As IEnumerable(Of xcms2)
        Dim mz As Double() = CLRVector.asNumeric(df!mz)
        Dim mzmin As Double() = CLRVector.asNumeric(df!mzmin)
        Dim mzmax As Double() = CLRVector.asNumeric(df!mzmax)
        Dim rt As Double() = CLRVector.asNumeric(df!rt)
        Dim rtmin As Double() = CLRVector.asNumeric(df!rtmin)
        Dim rtmax As Double() = CLRVector.asNumeric(df!rtmax)
        Dim RI As Double() = CLRVector.asNumeric(df!RI)
        Dim ID As String() = df.getRowNames.UniqueNames
        Dim npeaks As Integer() = CLRVector.asInteger(df!npeaks)
        Dim RImin As Double() = CLRVector.asNumeric(df!RImin)
        Dim RImax As Double() = CLRVector.asNumeric(df!RImax)
        Dim groups As Integer() = CLRVector.asInteger(df!groups)

        ' 20241029 for avoid the unexpected data updates from the 
        ' R# runtime symbols, we should make a data copy at here
        df = New dataframe(df)
        df.delete("ID", "mz", "mzmin", "mzmax", "rt", "rtmin", "rtmax",
                  "RI", "npeaks",
                  "xcms_id", "into", "RImin", "RImax", "groups")

        Dim offset As Integer
        Dim v As Dictionary(Of String, Double)
        Dim matrix As NamedCollection(Of Double)() = df.columns _
            .Select(Function(i)
                        Return New NamedCollection(Of Double)(i.Key, CLRVector.asNumeric(i.Value))
                    End Function) _
            .ToArray
        Dim ion As xcms2
        Dim no_sample As Boolean = matrix.IsNullOrEmpty AndAlso Not npeaks Is Nothing
        Dim no_npeaks As Boolean = npeaks.IsNullOrEmpty
        Dim no_mz_range As Boolean = mzmin.IsNullOrEmpty OrElse mzmax.IsNullOrEmpty
        Dim no_rt_range As Boolean = rtmin.IsNullOrEmpty OrElse rtmax.IsNullOrEmpty
        Dim no_ri As Boolean = RI.IsNullOrEmpty

        For i As Integer = 0 To mz.Length - 1
            If no_sample Then
                If no_npeaks Then
                    ion = New xcms2()
                Else
                    ion = New xcms2(npeaks(i))
                End If
            Else
                offset = i
                v = matrix.ToDictionary(Function(a) a.name, Function(a) a(offset))
                ion = New xcms2(v)
            End If

            With ion
                .ID = ID(i)
                .mz = mz(i)
                .groups = groups.ElementAtOrDefault(i)

                If no_mz_range Then
                    .mzmax = .mz
                    .mzmin = .mz
                Else
                    .mzmax = mzmax(i)
                    .mzmin = mzmin(i)
                End If
                If no_ri Then
                    ' do nothing
                Else
                    .RI = RI(i)
                    .RImin = RImin.ElementAtOrDefault(i, [default]:= .RI)
                    .RImax = RImax.ElementAtOrDefault(i, [default]:= .RI)
                End If

                .rt = rt(i)

                If no_rt_range Then
                    .rtmin = .rt
                    .rtmax = .rt
                Else
                    .rtmax = rtmax(i)
                    .rtmin = rtmin(i)
                End If
            End With

            Yield ion
        Next
    End Function

    ''' <summary>
    ''' Try to cast the dataframe to the mzkit peak feature object set
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <keywords>read data</keywords>
    <ExportAPI("read.xcms_features")>
    <RApiReturn(GetType(PeakFeature))>
    Public Function readXcmsFeaturePeaks(file As dataframe) As Object
        Dim mz As Double() = CLRVector.asNumeric(file.getVector("mz", True))
        Dim mzmin As Double() = CLRVector.asNumeric(file.getVector("mzmin", True))
        Dim mzmax As Double() = CLRVector.asNumeric(file.getVector("mzmax", True))
        Dim rt As Double() = CLRVector.asNumeric(file.getVector("rt", True))
        Dim rtmin As Double() = CLRVector.asNumeric(file.getVector("rtmin", True))
        Dim rtmax As Double() = CLRVector.asNumeric(file.getVector("rtmax", True))
        Dim into As Double() = CLRVector.asNumeric(file.getVector("into", True))
        Dim maxo As Double() = CLRVector.asNumeric(file.getVector("maxo", True))
        Dim sn As Double() = CLRVector.asNumeric(file.getVector("sn", True))
        Dim xcms_id As String() = mz.Select(Function(mzi, i) $"M{CInt(mzi)}T{CInt(rt(i))}").UniqueNames

        Return xcms_id _
            .Select(Function(id, i)
                        Return New PeakFeature With {
                            .xcms_id = xcms_id(i),
                            .rt = rt(i),
                            .area = into(i),
                            .baseline = 0,
                            .integration = 1,
                            .maxInto = maxo(i),
                            .mz = mz(i),
                            .noise = 0,
                            .nticks = 0,
                            .rawfile = Nothing,
                            .RI = 0,
                            .rtmax = rtmax(i),
                            .rtmin = rtmin(i)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' make sample column projection
    ''' </summary>
    ''' <param name="peaktable">A xcms liked peaktable object, is a collection 
    ''' of the <see cref="xcms2"/> peak feature data.</param>
    ''' <param name="sampleNames">A character vector of the sample names for make 
    ''' the peaktable projection.</param>
    ''' <returns>A sub-table of the input original peaktable data</returns>
    <ExportAPI("peak_subset")>
    <RApiReturn(GetType(PeakSet))>
    Public Function peakSubset(peaktable As PeakSet, sampleNames As String()) As Object
        Return peaktable.Subset(sampleNames)
    End Function

    ''' <summary>
    ''' Create a xcms peak data object
    ''' </summary>
    ''' <param name="id">the unique referene id of the peak data</param>
    ''' <param name="mz"></param>
    ''' <param name="mz_range"></param>
    ''' <param name="rt"></param>
    ''' <param name="rt_range"></param>
    ''' <param name="RI"></param>
    ''' <param name="samples"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("xcms_peak")>
    Public Function xcms_peak(id As String, mz As Double, mz_range As Double(), rt As Double, rt_range As Double(), RI As Double,
                              <RListObjectArgument>
                              samples As list,
                              Optional env As Environment = Nothing) As xcms2

        Return New xcms2 With {
            .ID = id,
            .mz = mz,
            .mzmin = mz_range.Min,
            .mzmax = mz_range.Max,
            .RI = RI,
            .rt = rt,
            .rtmax = rt_range.Max,
            .rtmin = rt_range.Min,
            .Properties = samples.AsGeneric(Of Double)(env)
        }
    End Function

    ''' <summary>
    ''' helper function for find ms1 peaks based on the given mz/rt tuple data
    ''' </summary>
    ''' <param name="peaktable">the peaktable object, is a collection of the <see cref="xcms2"/> object.</param>
    ''' <param name="mz">target ion m/z</param>
    ''' <param name="rt">target ion rt in seconds.</param>
    ''' <param name="mzdiff">the mass tolerance error in data unit delta dalton, 
    ''' apply for matches between the peaktable precursor m/z and the given ion mz value.</param>
    ''' <param name="rt_win">the rt window size for matches the rt. should be in data unit seconds.</param>
    ''' <returns>data is re-ordered via the tolerance error</returns>
    <ExportAPI("find_xcms_ionPeaks")>
    <RApiReturn(GetType(xcms2))>
    Public Function get_ionPeak(peaktable As PeakSet, mz As Double, rt As Double,
                                Optional mzdiff As Double = 0.01,
                                Optional rt_win As Double = 90,
                                Optional find_RI As Boolean = False) As Object

        Return peaktable.FindIonSet(mz, rt, mzdiff, rt_win) _
            .OrderBy(Function(a)
                         Return std.Abs(a.mz - mz + 0.0001) *
                             std.Abs(a.rt - rt + 0.0001)
                     End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' make filter of the noise spectrum data
    ''' </summary>
    ''' <remarks>
    ''' this function will filter the noise spectrum data from the given 
    ''' msn level spectrum data collection
    ''' </remarks>
    ''' <param name="peaktable">the peaktable object, is a collection of the <see cref="xcms2"/> object.</param>
    ''' <param name="ions">a collection of the msn level spectrum data</param>
    ''' <param name="mzdiff">the mass tolerance error in data unit delta dalton, 
    ''' apply for matches between the peaktable precursor m/z and the given ion mz value.</param>
    ''' <param name="rt_win">the rt window size for matches the rt. should be in data unit seconds.</param>
    ''' <returns>
    ''' return a vector of clean spectrum object that could find any peak in ms1 table.
    ''' additionally, the noise spectrum data will be set to the attribute named "noise" 
    ''' of the return vector value.
    ''' 
    ''' the return value is a vector of <see cref="PeakMs2"/> object, and the noise
    ''' spectrum data is set to the attribute named "noise" of the return value.
    ''' </returns>
    ''' <example>
    ''' let cleandata = filter_noise_spectrum(peaktable, ions, mzdiff=0.1, rt_win=30);
    ''' # get the noise spectrum data
    ''' let noise = cleandata$noise;
    ''' </example>
    <ExportAPI("filter_noise_spectrum")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function filter_noise_spectrum(<RRawVectorArgument> ions As Object, peaktable As PeakSet,
                                          Optional mzdiff As Double = 0.1,
                                          Optional rt_win As Double = 30,
                                          Optional env As Environment = Nothing) As Object

        Dim filterData As New List(Of PeakMs2)
        Dim noiseData As New List(Of PeakMs2)
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ions, env)

        If pull.isError Then
            Return pull.getError
        End If

        For Each ion As PeakMs2 In pull.populates(Of PeakMs2)(env)
            If peaktable.FindIonSet(ion.mz, ion.rt, mzdiff, rt_win).Any Then
                Call filterData.Add(ion)
            Else
                Call noiseData.Add(ion)
            End If
        Next

        Return New vec(filterData.ToArray, RType.GetRSharpType(GetType(PeakMs2))) _
            .setAttribute("noise", noiseData.ToArray)
    End Function

    ''' <summary>
    ''' get ion peaks via the unique reference id
    ''' </summary>
    ''' <param name="peaktable"></param>
    ''' <param name="id">a character vector of the unique reference id of the ion peaks</param>
    ''' <param name="drop">
    ''' if the given id set contains a single id value, just returns the single xcms ion peak clr object,
    ''' instead of a tuple list with single element? Default is not, which means this function 
    ''' always returns the tuple list data by default.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>a tuple list of the xcms peaks object</returns>
    <ExportAPI("get_xcms_ionPeaks")>
    <RApiReturn(GetType(xcms2))>
    Public Function getIonPeak(peaktable As PeakSet, <RRawVectorArgument> id As Object,
                               Optional drop As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim idset As String() = CLRVector.asCharacter(id)

        If idset.Length = 1 And drop Then
            Return peaktable.GetById(idset(0))
        End If

        Dim pull As list = list.empty

        For Each id_str As String In idset
            Call pull.add(id_str, peaktable.GetById(id_str))
        Next

        Return pull
    End Function

    ''' <summary>
    ''' adjust the reteintion time data to unit seconds
    ''' </summary>
    ''' <param name="rt_data"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("adjust_to_seconds")>
    Public Function adjust_to_seconds(<RRawVectorArgument> rt_data As Object, Optional env As Environment = Nothing) As Object
        Dim pull_data As pipeline = pipeline.TryCreatePipeline(Of IRetentionTime)(rt_data, env)

        If pull_data.isError Then
            Return pull_data.getError
        End If

        Dim ds As New List(Of IRetentionTime)

        For Each xi As IRetentionTime In pull_data.populates(Of IRetentionTime)(env)
            xi.rt *= 60
            ds.Add(xi)
        Next

        Return ds.ToArray
    End Function

    ''' <summary>
    ''' Create RI reference dataset.
    ''' </summary>
    ''' <returns>a collection of the mzkit ri reference object model 
    ''' which is matched via the xcms peaktable.</returns>
    <ExportAPI("RI_reference")>
    <RApiReturn(GetType(RIRefer))>
    Public Function RI_reference(xcms_id As String(),
                                 mz As Double(),
                                 rt As Double(),
                                 ri As Double(),
                                 Optional names As String() = Nothing,
                                 Optional reference_mz As Double() = Nothing,
                                 Optional reference_rt As Double() = Nothing) As Object
        Return xcms_id _
            .Select(Function(id, i)
                        Return New RIRefer() With {
                            .xcms_id = id,
                            .mz = mz(i),
                            .rt = rt(i),
                            .RI = ri(i),
                            .name = names.ElementAtOrNull(i),
                            .reference_mz = reference_mz.ElementAtOrDefault(i, Double.NaN),
                            .reference_rt = reference_rt.ElementAtOrDefault(i, Double.NaN)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' RI calculation of a speicifc sample data
    ''' </summary>
    ''' <param name="peakdata">should be a collection of the peak data from a single sample file.</param>
    ''' <param name="RI">should be a collection of the <see cref="RIRefer"/> data.</param>
    ''' <param name="C">
    ''' the number of carbon atoms for kovats retention index
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("RI_cal")>
    Public Function RI_calc(peakdata As PeakFeature(),
                            <RRawVectorArgument>
                            Optional RI As Object = Nothing,
                            Optional ppm As Double = 20,
                            Optional dt As Double = 15,
                            Optional rawfile As String = Nothing,
                            Optional by_id As Boolean = False,
                            Optional C As list = Nothing,
                            Optional safe_wrap_missing As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim refer_points As New List(Of PeakFeature)
        Dim map_RI_id As Dictionary(Of String, String) = Nothing

        If RI Is Nothing Then
            ' ri reference from the peakdata which has RI value assigned
            Call refer_points.AddRange(From pk As PeakFeature In peakdata.SafeQuery Where pk.RI > 0 Order By pk.RI)
        Else
            Dim RIrefers As pipeline = pipeline.TryCreatePipeline(Of RIRefer)(RI, env)

            If RIrefers.isError Then
                Return RIrefers.getError
            Else
                map_RI_id = New Dictionary(Of String, String)
            End If

            Dim ri_refers As RIRefer() = RIrefers.populates(Of RIRefer)(env) _
                .OrderBy(Function(i) i.rt) _
                .ToArray
            Dim ppmErr As Tolerance = Tolerance.PPM(ppm)

            For i As Integer = 0 To ri_refers.Length - 1
                If Not ri_refers(i).name.StringEmpty(, True) Then
                    map_RI_id(ri_refers(i).name) = ri_refers(i).xcms_id
                End If
            Next

            If by_id Then
                ' the RI is already has been assigned the peak id
                ' get peak feature data by its id directly!
                Dim peak1Index = peakdata.ToDictionary(Function(p1) p1.xcms_id)
                Dim target As PeakFeature

                For Each refer As RIRefer In ri_refers
                    If peak1Index.ContainsKey(refer.xcms_id) Then
                        target = peak1Index(refer.xcms_id)
                    ElseIf safe_wrap_missing Then
                        ' create a fake peak feature at here
                        Call $"Missing the required RI reference peak feature: {refer.xcms_id}, a fake peak feature is generated as placeholder at here".Warning

                        target = New PeakFeature With {
                            .xcms_id = refer.xcms_id,
                            .mz = refer.mz,
                            .rt = refer.rt,
                            .rawfile = "Missing Feature"
                        }
                    Else
                        Return RInternal.debug.stop($"Missing the required RI reference peak feature: {refer.xcms_id}, please check of your peaktable input!", env)
                    End If

                    target.RI = refer.RI
                    refer_points.Add(target)
                Next
            Else
                ' find a ri reference point at first
                ' find a set of the candidate points
                For Each refer As RIRefer In ri_refers
                    Dim target As PeakFeature = peakdata _
                        .Where(Function(pi) ppmErr(pi.mz, refer.mz) AndAlso std.Abs(pi.rt - refer.rt) <= dt) _
                        .OrderByDescending(Function(pi) pi.maxInto) _
                        .FirstOrDefault

                    If target Is Nothing Then
                        Return RInternal.debug.stop({
                            $"the required retention index reference point({refer.ToString}) could not be found! please check the rt window parameter(dt) is too small?",
                            $"retention_index_reference: {ri_refers.GetJson}",
                            $"rawfile tag: {rawfile}",
                            $"ms1_pars: {ppm} PPM, rt_win {dt} sec"
                        }, env)
                    End If

                    target.RI = refer.RI
                    refer_points.Add(target)
                Next
            End If
        End If

        ' order raw data by rt
        peakdata = peakdata.OrderBy(Function(i) i.rt).ToArray
        refer_points = refer_points.OrderBy(Function(i) i.rt).AsList
        ' add a fake point
        refer_points.Add(New PeakFeature With {
            .RI = refer_points.Last.RI + 100,
            .rt = peakdata.Last.rt,
            .xcms_id = peakdata.Last.xcms_id
        })

        Dim a As (rt As Double, ri As Double)
        Dim b As (rt As Double, ri As Double)
        Dim offset As Integer = 0
        Dim id_a, id_b As String
        Dim c_atoms As Dictionary(Of String, Integer) = Nothing

        If Not C Is Nothing Then
            c_atoms = C.AsGeneric(Of Integer)(env)

            If Not map_RI_id.IsNullOrEmpty Then
                ' some reference data peak maybe missing from the lcms experiment data.
                c_atoms = c_atoms _
                    .Where(Function(t) map_RI_id.ContainsKey(t.Key)) _
                    .ToDictionary(Function(t) map_RI_id(t.Key),
                                  Function(t)
                                      Return t.Value
                                  End Function)
            End If

            If Not c_atoms.ContainsKey(peakdata(0).xcms_id) Then
                Call c_atoms.Add(peakdata(0).xcms_id, 0)
            End If
            If Not c_atoms.ContainsKey(peakdata.Last.xcms_id) Then
                Call c_atoms.Add(peakdata.Last.xcms_id, c_atoms.Values.Max + 1)
            End If
        End If

        If peakdata(0).RI > 0 Then
            a = (peakdata(0).rt, peakdata(0).RI)
            id_a = peakdata(0).xcms_id
            offset = 1
            b = (refer_points(1).rt, refer_points(1).RI)
            id_b = refer_points(1).xcms_id
        Else
            a = (peakdata(0).rt, 0)
            b = (refer_points(0).rt, refer_points(0).RI)
            id_a = peakdata(0).xcms_id
            id_b = refer_points(0).xcms_id
        End If

        For i As Integer = offset To peakdata.Length - 1
            peakdata(i).rawfile = If(rawfile, peakdata(i).rawfile)

            If peakdata(i).RI = 0 Then
                If c_atoms Is Nothing Then
                    peakdata(i).RI = deco_math.RetentionIndex(peakdata(i), a, b)
                Else
                    peakdata(i).RI = deco_math.KovatsRI(c_atoms(id_a), c_atoms(id_b), peakdata(i).rt, a.rt, b.rt)
                End If
            Else
                a = b
                id_a = id_b
                offset += 1
                b = (refer_points(offset).rt, refer_points(offset).RI)
                id_b = refer_points(offset).xcms_id
            End If
        Next

        ' and then evaluate the ri for each peak points
        Return peakdata
    End Function

    ''' <summary>
    ''' Chromatogram data deconvolution
    ''' </summary>
    ''' <param name="ms1">
    ''' a collection of the ms1 data or the mzpack raw data object, this parameter could also be
    ''' a XIC pool object which contains a collection of the ion XIC data for run deconvolution.
    ''' </param>
    ''' <param name="tolerance">the mass tolerance for extract the XIC data for run deconvolution.</param>
    ''' <param name="feature">
    ''' a numeric vector of target feature ion m/z value for extract the XIC data.
    ''' </param>
    ''' <param name="parallel">
    ''' run peak detection algorithm on mutliple xic data in parallel mode?
    ''' </param>
    ''' <returns>a vector of the peak deconvolution data,
    ''' in format of xcms peak table liked or mzkit <see cref="PeakFeature"/>
    ''' data object.
    ''' 
    ''' the result data vector may contains the rt shift data result, where you can get this shift
    ''' value via the ``rt.shift`` attribute name, rt shift data model is clr type: <see cref="RtShift"/>.
    ''' </returns>
    ''' <example>
    ''' require(mzkit);
    ''' 
    ''' imports "mzDeco" from "mz_quantify";
    ''' 
    ''' let rawdata = open.mzpack("/path/to/rawdata.mzXML");
    ''' let ms1 = rawdata |> ms1_scans();
    ''' let peaks = mz_deco(ms1, tolerance = "da:0.01", peak.width = [3,30], 
    '''    dtw = TRUE);
    ''' 
    ''' write.peaks(peaks, file = "/data/save_debug.dat");
    ''' </example>
    <ExportAPI("mz_deco")>
    <RApiReturn(GetType(PeakFeature), GetType(xcms2))>
    Public Function mz_deco(<RRawVectorArgument>
                            ms1 As Object,
                            Optional tolerance As Object = "ppm:20",
                            Optional baseline# = 0.65,
                            <RRawVectorArgument>
                            Optional peak_width As Object = "3,15",
                            Optional joint As Boolean = False,
                            Optional parallel As Boolean = False,
                            Optional dtw As Boolean = False,
                            <RRawVectorArgument>
                            Optional feature As Object = Nothing,
                            Optional rawfile As String = Nothing,
                            Optional sn_threshold As Double = 1,
                            Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim rtRange = ApiArgumentHelpers.GetDoubleRange(peak_width, env, [default]:="3,15")

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        ElseIf rtRange Like GetType(Message) Then
            Return rtRange.TryCast(Of Message)
        End If

        ' 1. processing for XIC pool
        If TypeOf ms1 Is XICPool Then
            Dim pool As XICPool = DirectCast(ms1, XICPool)
            Dim features_mz As Double() = CLRVector.asNumeric(feature)

            If features_mz.IsNullOrEmpty Then
                Return RInternal.debug.stop("no ion m/z feature was provided!", env)
            Else
                Return pool.xic_deco(features_mz,
                                     errors.TryCast(Of Tolerance),
                                     rtRange.TryCast(Of DoubleRange),
                                     baseline, joint, dtw, parallel)
            End If
        ElseIf TypeOf ms1 Is list Then
            ' 2. processing for a set of the xic data
            Dim ls_xic = DirectCast(ms1, list) _
                .AsGeneric(Of MzGroup)(env) _
                .Select(Function(a) New NamedValue(Of MzGroup)(a.Key, a.Value)) _
                .ToArray

            If Not ls_xic.All(Function(a) a.Value Is Nothing) Then
                If dtw Then
                    ls_xic = XICPool.DtwXIC(rawdata:=ls_xic).ToArray
                End If

                Return xic_deco_task.ExtractAlignedPeaks(
                    ls_xic,
                    rtRange:=rtRange.TryCast(Of DoubleRange),
                    baseline:=baseline,
                    joint:=joint, xic_align:=True, rt_shifts:=Nothing)
            Else
                GoTo extract_ms1
            End If
        ElseIf TypeOf ms1 Is ChromatogramOverlapList Then
            Return DirectCast(ms1, ChromatogramOverlapList) _
                .GetPeakGroups(rtRange.TryCast(Of DoubleRange), quantile:=baseline, sn_threshold, joint, [single]:=False) _
                .ToArray
        Else
            Dim pull_xic As pipeline = pipeline.TryCreatePipeline(Of MzGroup)(ms1, env, suppress:=True)

            If Not pull_xic.isError Then
                Return pull_xic _
                    .populates(Of MzGroup)(env) _
                    .DecoMzGroups(
                        peakwidth:=rtRange.TryCast(Of DoubleRange),
                        quantile:=baseline,
                        parallel:=parallel,
                        joint:=joint,
                        source:=rawfile,
                        sn:=sn_threshold
                    ) _
                    .ToArray
            End If

extract_ms1:
            Dim source As String = Nothing
            Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1, source)

            source = If(rawfile, source)

            If Not source.StringEmpty Then
                Call VBDebugger.EchoLine($"run peak feature finding for rawdata: {source}.")
            End If

            ' usually used for make extract features
            ' for a single sample file
            Return ms1_scans _
                .GetMzGroups(mzdiff:=errors) _
                .DecoMzGroups(
                    peakwidth:=rtRange.TryCast(Of DoubleRange),
                    quantile:=baseline,
                    parallel:=parallel,
                    joint:=joint,
                    source:=source,
                    sn:=sn_threshold
                ) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' A debug function for test peak finding method 
    ''' </summary>
    ''' <param name="raw">the file path of a single rawdata file.</param>
    ''' <param name="massdiff"></param>
    ''' <returns></returns>
    <ExportAPI("MS1deconv")>
    Public Function Deconv(raw As String,
                           Optional massdiff As Double = 0.01,
                           <RRawVectorArgument(TypeCodes.double)>
                           Optional peak_width As Object = "3,12",
                           Optional q As Double = 0.65,
                           Optional sn_threshold As Double = 1,
                           Optional nticks As Integer = 6,
                           Optional joint As Boolean = True) As PeakFeature()

        Dim pack As mzPack = mzPack.ReadAll(raw.Open, skipMsn:=True)
        Dim scanPoints As ms1_scan() = pack.GetAllScanMs1().ToArray
        Dim massGroups = scanPoints.GetMzGroups(mzdiff:=DAmethod.DeltaMass(massdiff)).ToArray
        Dim peak_win As Double() = CLRVector.asNumeric(peak_width)
        Dim source_tag As String = raw.BaseName
        Dim features = massGroups.DecoMzGroups(
            peakwidth:=peak_win,
            quantile:=q,
            sn:=sn_threshold,
            nticks:=nticks,
            joint:=joint,
            source:=source_tag
        ) _
        .ToArray

        Return features
    End Function

    <ExportAPI("read.rt_shifts")>
    Public Function read_rtshifts(file As String) As RtShift()
        Return file.LoadCsv(Of RtShift)(mute:=True).ToArray
    End Function

    ''' <summary>
    ''' write peak debug data
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>save data</keywords>
    <ExportAPI("write.peaks")>
    Public Function dumpPeaks(<RRawVectorArgument> peaks As Object, file As Object, Optional env As Environment = Nothing) As Object
        Dim peakSet = pipeline.TryCreatePipeline(Of PeakFeature)(peaks, env)
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If peakSet.isError Then
            Return peakSet.getError
        ElseIf buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Call SaveSample.DumpSample(peakSet.populates(Of PeakFeature)(env), buf.TryCast(Of Stream))

        If TypeOf file Is String Then
            Call buf.TryCast(Of Stream).Dispose()
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' read the peak feature table data
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="readBin">
    ''' does the given data file is in binary format not a csv table file, 
    ''' and this function should be parsed as a binary data file?
    ''' </param>
    ''' <returns></returns>
    ''' <keywords>read data</keywords>
    <ExportAPI("read.peakFeatures")>
    <RApiReturn(GetType(PeakFeature))>
    Public Function readPeakData(file As String, Optional readBin As Boolean = False) As Object
        If readBin Then
            Using buf As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return SaveSample.ReadSample(buf).ToArray
            End Using
        Else
            Return file.LoadCsv(Of PeakFeature)(mute:=True).ToArray
        End If
    End Function

    ''' <summary>
    ''' Do COW peak alignment and export peaktable
    ''' 
    ''' Correlation optimized warping (COW) based on the total ion 
    ''' current (TIC) is a widely used time alignment algorithm 
    ''' (COW-TIC). This approach works successfully on chromatograms 
    ''' containing few compounds and having a well-defined TIC.
    ''' </summary>
    ''' <param name="samples">should be a set of sample file data, which could be extract from the ``mz_deco`` function.</param>
    ''' <param name="mzdiff"></param>
    ''' <param name="norm">do total ion sum normalization after peak alignment and the peaktable object has been exported?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let peaksdata = lapply(files, function(ms1) {
    '''     mz_deco(ms1, tolerance = "da:0.01", 
    '''         peak.width = [3,30]);
    ''' });
    ''' let peaktable = peak_alignment(samples = peaksdata);
    ''' 
    ''' write.csv(peaktable, 
    '''     file = "/path/to/peaktable.csv", 
    '''     row.names = TRUE);
    ''' </example>
    <ExportAPI("peak_alignment")>
    <RApiReturn(GetType(xcms2))>
    Public Function peakAlignment(<RRawVectorArgument>
                                  samples As Object,
                                  Optional mzdiff As Double = 0.01,
                                  Optional ri_win As Double = 10,
                                  Optional norm As Boolean = False,
                                  Optional ri_alignment As Boolean = False,
                                  Optional max_intensity_ion As Boolean = False,
                                  Optional cow_alignment As Boolean = False,
                                  Optional aggregate As Aggregates = Aggregates.Sum,
                                  Optional env As Environment = Nothing) As Object

        Dim sampleData As NamedCollection(Of PeakFeature)() = Nothing

        If TypeOf samples Is list Then
            Dim ls = DirectCast(samples, list).AsGeneric(Of PeakFeature())(env)

            If ls.All(Function(a) a.Value Is Nothing) Then
            Else
                sampleData = ls _
                    .Select(Function(a) New NamedCollection(Of PeakFeature)(a.Key, a.Value)) _
                    .ToArray
            End If
        End If

        If sampleData.IsNullOrEmpty Then
            Dim samplePeaks = pipeline.TryCreatePipeline(Of PeakFeature)(samples, env)

            If samplePeaks.isError Then
                Return samplePeaks.getError
            End If

            sampleData = samplePeaks _
                .populates(Of PeakFeature)(env) _
                .GroupBy(Function(a) a.rawfile) _
                .Select(Function(i)
                            Return New NamedCollection(Of PeakFeature)(i.Key, i.ToArray)
                        End Function) _
                .ToArray
        End If

        Dim peaktable As xcms2()
        Dim rt_shifts As New List(Of RtShift)

        If ri_alignment Then
            peaktable = sampleData _
                .RIAlignment(rt_shifts,
                             mzdiff:=mzdiff,
                             ri_offset:=ri_win,
                             top_ion:=max_intensity_ion,
                             aggregate:=aggregate) _
                .ToArray
        ElseIf cow_alignment Then
            peaktable = sampleData _
                .CowAlignment() _
                .ToArray
        Else
            Dim ions = peak_align_task.MakeIonGroups(sampleData, mzdiff).ToArray
            Dim task As peak_align_task = New peak_align_task(ions, ri_win).Solve()

            peaktable = task.out.ToArray
            rt_shifts.AddRange(task.rt_shifts)
        End If

        Dim id As String() = peaktable.Select(Function(i) i.ID).UniqueNames
        Dim sampleNames As String() = sampleData.Keys.ToArray

        For i As Integer = 0 To id.Length - 1
            Dim peak As xcms2 = peaktable(i)

            peak.ID = id(i)

            For Each sample_id As String In sampleNames
                If Not peak.Properties.ContainsKey(sample_id) Then
                    peak(sample_id) = 0.0
                End If
            Next
        Next

        If norm Then
            For Each name As String In sampleNames
                Dim v = peaktable.Select(Function(i) i(name)).AsVector
                v = v / v.Sum * (10 ^ 8)

                For i As Integer = 0 To peaktable.Length - 1
                    peaktable(i)(name) = v(i)
                Next
            Next
        End If

        Dim vec As New vec(peaktable, RType.GetRSharpType(GetType(xcms2)))
        Call vec.setAttribute("rt.shift", rt_shifts.ToArray)
        Return vec
    End Function

    ''' <summary>
    ''' make peaktable join of two batch data via (mz,RI)
    ''' </summary>
    ''' <param name="batch1"></param>
    ''' <param name="batch2"></param>
    ''' <returns>
    ''' the ROI merge result across two sample batch data.
    ''' </returns>
    <ExportAPI("RI_batch_join")>
    Public Function RI_batch_join(batch1 As PeakSet, batch2 As PeakSet,
                                  Optional mzdiff As Double = 0.01,
                                  Optional ri_win As Double = 10,
                                  Optional max_intensity_ion As Boolean = False,
                                  Optional aggregate As Aggregates = Aggregates.Sum) As Object

        Dim allpeaks = batch1.ToFeatures _
            .JoinIterates(batch2.ToFeatures) _
            .GroupBy(Function(a) a.rawfile) _
            .Select(Function(s) New NamedCollection(Of PeakFeature)(s.Key, s)) _
            .ToArray
        Dim rt_shifts As New List(Of RtShift)
        Dim peaktable As xcms2() = allpeaks _
            .RIAlignment(rt_shifts,
                        mzdiff:=mzdiff,
                        ri_offset:=ri_win,
                        top_ion:=max_intensity_ion,
                        aggregate:=aggregate) _
            .ToArray
        Dim id As String() = peaktable.Select(Function(i) i.ID).UniqueNames
        Dim sampleNames As String() = allpeaks.Keys.ToArray

        For i As Integer = 0 To id.Length - 1
            Dim peak As xcms2 = peaktable(i)

            peak.ID = id(i)

            For Each sample_id As String In sampleNames
                If Not peak.Properties.ContainsKey(sample_id) Then
                    peak(sample_id) = 0.0
                End If
            Next
        Next

        Dim vec As New vec(peaktable, RType.GetRSharpType(GetType(xcms2)))
        Call vec.setAttribute("rt.shift", rt_shifts.ToArray)
        Return vec
    End Function

    ''' <summary>
    ''' do ``m/z`` grouping under the given tolerance
    ''' </summary>
    ''' <param name="ms1">
    ''' a LCMS mzpack rawdata object or a collection of the ms1 point data
    ''' </param>
    ''' <param name="mzdiff">the mass tolerance error for extract the XIC from the rawdata set</param>
    ''' <param name="rtwin">the rt tolerance window size for merge data points</param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' create a list of XIC dataset for run downstream deconv operation
    ''' </returns>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "/path/to/rawdata.mzpack");
    ''' let xic = mz.groups(ms1 = rawdata, mzdiff = "ppm:20");
    ''' 
    ''' # export the XIC data as binary data file.
    ''' writeBin(xic, con = "/path/to/xic_data.dat");
    ''' </example>
    ''' <remarks>
    ''' the ion mz value is generated via the max intensity point in each ion 
    ''' feature group, and the xic data has already been re-order via the 
    ''' time asc.
    ''' </remarks>
    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup))>
    Public Function mz_groups(<RRawVectorArgument>
                              ms1 As Object,
                              Optional mzdiff As Object = "ppm:20",
                              Optional rtwin As Double = 0.05,
                              Optional env As Environment = Nothing) As Object

        Return ms1Scans(ms1) _
            .GetMzGroups(mzdiff:=Math.getTolerance(mzdiff, env), rtwin:=rtwin) _
            .ToArray
    End Function

    ''' <summary>
    ''' Make peaks data group merge by rt directly
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <param name="dt"></param>
    ''' <param name="ppm"></param>
    ''' <returns></returns>
    <ExportAPI("rt_groups")>
    <RApiReturn(GetType(xcms2))>
    Public Function rt_groups_merge(peaks As xcms2(),
                                    Optional dt As Double = 3,
                                    Optional ppm As Double = 20,
                                    Optional aggregate As Aggregates = Aggregates.Sum) As Object

        Dim ions = peaks.GroupBy(Function(i) i.mz, Function(a, b) PPMmethod.PPM(a, b) <= ppm).ToArray
        Dim merge As New List(Of xcms2)
        Dim f As Func(Of Double, Double, Double) = aggregate.GetAggregateFunction2

        For Each ion_group As NamedCollection(Of xcms2) In TqdmWrapper.Wrap(ions)
            If ion_group.Length > 1 Then
                Dim rt_groups = ion_group.GroupBy(Function(i) i.rt, Function(a, b) std.Abs(a - b) <= dt)

                For Each ion As NamedCollection(Of xcms2) In rt_groups
                    If ion.Length = 1 Then
                        ' is a unique ion
                        ' merge into the result list directly
                        Call merge.AddRange(ion)
                    Else
                        Call merge.Add(xcms2.Merge(ion, f))
                    End If
                Next
            Else
                Call merge.AddRange(ion_group)
            End If
        Next

        Return merge.ToArray
    End Function

    Private Function ms1Scans(ms1 As Object, Optional ByRef source As String = Nothing) As IEnumerable(Of IMs1Scan)
        If ms1 Is Nothing Then
            Return {}
        ElseIf ms1.GetType Is GetType(ms1_scan()) Then
            Return DirectCast(ms1, ms1_scan()).Select(Function(t) DirectCast(t, IMs1Scan))
        ElseIf TypeOf ms1 Is mzPack Then
            source = DirectCast(ms1, mzPack).source
            Return DirectCast(ms1, mzPack) _
                .GetAllScanMs1 _
                .Select(Function(t) DirectCast(t, IMs1Scan))
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' Load xic sample data files
    ''' </summary>
    ''' <param name="files">a character vector of a collection of the xic data files.</param>
    ''' <returns></returns>
    <ExportAPI("xic_pool")>
    <RApiReturn(GetType(XICPool))>
    Public Function XICpool_func(files As String()) As Object
        Dim pool As New XICPool
        Dim group As MzGroup()

        For Each file As String In files
            Using s As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                group = SaveXIC.ReadSample(s).ToArray
                pool.Add(file.BaseName, group)
            End Using
        Next

        Return pool
    End Function

    ''' <summary>
    ''' extract a collection of xic data for a specific ion feature
    ''' 
    ''' this function is debug used only
    ''' </summary>
    ''' <param name="pool">should be type of <see cref="XICPool"/> or peak collection <see cref="PeakSet"/> object.</param>
    ''' <param name="mz">the ion feature m/z value</param>
    ''' <param name="dtw">this parameter will not working when the data pool type is clr type <see cref="PeakSet"/></param>
    ''' <param name="mzdiff"></param>
    ''' <returns>
    ''' a tuple list object that contains the xic data across
    ''' multiple sample data files for a speicifc ion feature
    ''' m/z.
    ''' </returns>
    ''' <example>
    ''' require(mzkit);
    '''
    ''' imports "mzDeco" from "mz_quantify";
    ''' imports "visual" from "mzplot";
    ''' 
    ''' let files = list.files("/path/to/debug_data_dir/", pattern = "*.xic");
    ''' let pool = xic_pool(files);
    ''' let dtw_xic = pull_xic(pool, mz = 100.0011, dtw = TRUE);
    ''' 
    ''' bitmap(file = "/path/to/save_image.png") {
    '''     raw_snapshot3D(dtw_xic);
    ''' }
    '''
    ''' dtw_xic
    ''' |> mz_deco(joint = TRUE, peak.width = [3,60])
    ''' |> write.csv(file = "/path/to/export_peakstable.csv")
    ''' ;
    ''' </example>
    <ExportAPI("pull_xic")>
    Public Function pull_xic(pool As Object, mz As Double,
                             Optional dtw As Boolean = True,
                             Optional mzdiff As Double = 0.01,
                             Optional strict As Boolean = False,
                             Optional env As Environment = Nothing) As Object
        If pool Is Nothing Then
            Return Message.NullOrStrict(strict, NameOf(pool), env)
        End If

        If TypeOf pool Is XICPool Then
            If dtw Then
                Return DirectCast(pool, XICPool).xic_dtw_list(mz, mzdiff)
            Else
                Return DirectCast(pool, XICPool).xic_matrix_list(mz, mzdiff)
            End If
        ElseIf TypeOf pool Is PeakSet Then
            Return DirectCast(pool, PeakSet) _
                .FilterMz(mz, mzdiff) _
                .OrderBy(Function(i) i.rt) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(XICPool), pool.GetType, env)
        End If
    End Function

    <Extension>
    Private Function xic_dtw_list(pool As XICPool, mz As Double, mzdiff As Double) As list
        Return New list With {
            .slots = pool _
                .DtwXIC(mz, Tolerance.DeltaMass(mzdiff)) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return CObj(a.Value)
                              End Function)
        }
    End Function

    <Extension>
    Private Function xic_matrix_list(pool As XICPool, mz As Double, mzdiff As Double) As list
        Return New list With {
            .slots = pool _
                .GetXICMatrix(mz, Tolerance.DeltaMass(mzdiff)) _
                .ToDictionary(Function(a) a.Name,
                                Function(a)
                                    Return CObj(a.Value)
                                End Function)
        }
    End Function
End Module

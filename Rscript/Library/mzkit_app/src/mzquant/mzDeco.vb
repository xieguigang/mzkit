#Region "Microsoft.VisualBasic::0cb17be81195d2e5c3ded08472c9430e, mzkit\Rscript\Library\mzkit.quantify\mzDeco.vb"

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

'   Total Lines: 140
'    Code Lines: 106
' Comment Lines: 15
'   Blank Lines: 19
'     File Size: 5.35 KB


' Module mzDeco
' 
'     Function: ms1Scans, mz_deco, mz_groups, peakAlignment, readPeakData
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' Extract peak and signal data from rawdata
''' </summary>
<Package("mzDeco")>
<RTypeExport("peak_feature", GetType(PeakFeature))>
<RTypeExport("mz_group", GetType(MzGroup))>
Module mzDeco

    Sub Main()
        Call Internal.Object.Converts.addHandler(GetType(PeakFeature()), AddressOf peaktable)
        Call Internal.Object.Converts.addHandler(GetType(xcms2()), AddressOf peaksetMatrix)

        Call generic.add("readBin.mz_group", GetType(Stream), AddressOf readXIC)
        Call generic.add("readBin.peak_feature", GetType(Stream), AddressOf readSamples)

        Call generic.add("writeBin", GetType(MzGroup), AddressOf writeXIC1)
        Call generic.add("writeBin", GetType(MzGroup()), AddressOf writeXIC)
        Call generic.add("writeBin", GetType(PeakFeature()), AddressOf writeSamples)
    End Sub

    Private Function writeSamples(samples As PeakFeature(), args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveSample.DumpSample(samples, con)
        Call con.Flush()
        Return True
    End Function

    Private Function writeXIC1(xic As MzGroup, args As list, env As Environment) As Object
        Return writeXIC({xic}, args, env)
    End Function

    Private Function writeXIC(xic As MzGroup(), args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveXIC.DumpSample(xic, con)
        Call con.Flush()
        Return True
    End Function

    Private Function readSamples(file As Stream, args As list, env As Environment) As Object
        Return SaveSample.ReadSample(file).ToArray
    End Function

    Private Function readXIC(file As Stream, args As list, env As Environment) As Object
        Return SaveXIC.ReadSample(file).ToArray
    End Function

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

        table.add(NameOf(xcms2.mz), peakset.Select(Function(a) a.mz))
        table.add(NameOf(xcms2.mzmin), peakset.Select(Function(a) a.mzmin))
        table.add(NameOf(xcms2.mzmax), peakset.Select(Function(a) a.mzmax))
        table.add(NameOf(xcms2.rt), peakset.Select(Function(a) a.rt))
        table.add(NameOf(xcms2.rtmin), peakset.Select(Function(a) a.rtmin))
        table.add(NameOf(xcms2.rtmax), peakset.Select(Function(a) a.rtmax))
        table.add(NameOf(xcms2.npeaks), peakset.Select(Function(a) a.npeaks))

        For Each name As String In allsampleNames
            Call table.add(name, peakset.Select(Function(i) i(name)))
        Next

        Return table
    End Function

    Private Function peaktable(x As PeakFeature(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        table.add(NameOf(PeakFeature.mz), x.Select(Function(a) a.mz))
        table.add(NameOf(PeakFeature.rt), x.Select(Function(a) a.rt))
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

    <Extension>
    Private Function xic_deco(pool As XICPool, features_mz As Double(),
                              errors As Tolerance,
                              rtRange As DoubleRange,
                              baseline As Double,
                              joint As Boolean) As xcms2()

        If features_mz.Length = 1 Then
            ' extract the aligned data
            Return pool.DtwXIC(features_mz(0), errors) _
                .ToArray _
                .extractAlignedPeaks(
                    rtRange:=rtRange,
                    baseline:=baseline,
                    joint:=joint, xic_align:=True)
        Else
            ' run parallel task for extract the aligned xic data
            Throw New NotImplementedException
        End If
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
    ''' <returns>a vector of the peak deconvolution data,
    ''' in format of xcms peak table liked or mzkit <see cref="PeakFeature"/>
    ''' data object.</returns>
    ''' <example>
    ''' require(mzkit);
    ''' 
    ''' imports "mzDeco" from "mz_quantify";
    ''' 
    ''' let rawdata = open.mzpack("/path/to/rawdata.mzXML");
    ''' let ms1 = rawdata |> ms1_scans();
    ''' let peaks = mz_deco(ms1, tolerance = "da:0.01", peak.width = [3,30]);
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
                            Optional peak_width As Object = "3,20",
                            Optional joint As Boolean = False,
                            Optional parallel As Boolean = False,
                            <RRawVectorArgument>
                            Optional feature As Object = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)
        Dim rtRange = ApiArgumentHelpers.GetDoubleRange(peak_width, env, [default]:="3,20")

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        ElseIf rtRange Like GetType(Message) Then
            Return rtRange.TryCast(Of Message)
        End If

        If TypeOf ms1 Is XICPool Then
            Dim pool As XICPool = DirectCast(ms1, XICPool)
            Dim features_mz As Double() = CLRVector.asNumeric(feature)

            If features_mz.IsNullOrEmpty Then
                Return Internal.debug.stop("no ion m/z feature was provided!", env)
            Else
                Return pool.xic_deco(features_mz,
                                     errors.TryCast(Of Tolerance),
                                     rtRange.TryCast(Of DoubleRange),
                                     baseline, joint)
            End If
        ElseIf TypeOf ms1 Is list Then
            Dim ls_xic = DirectCast(ms1, list) _
                .AsGeneric(Of MzGroup)(env) _
                .Select(Function(a) New NamedValue(Of MzGroup)(a.Key, a.Value)) _
                .ToArray

            If Not ls_xic.All(Function(a) a.Value Is Nothing) Then
                Return ls_xic.extractAlignedPeaks(
                    rtRange:=rtRange.TryCast(Of DoubleRange),
                    baseline:=baseline,
                    joint:=joint, xic_align:=True)
            Else
                GoTo extract_ms1
            End If
        Else
extract_ms1:
            Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1)

            ' usually used for make extract features
            ' for a single sample file
            Return ms1_scans _
                .GetMzGroups(mzdiff:=errors) _
                .DecoMzGroups(
                    peakwidth:=rtRange.TryCast(Of DoubleRange),
                    quantile:=baseline,
                    parallel:=parallel,
                    joint:=joint
                ) _
                .ToArray
        End If
    End Function

    <Extension>
    Private Function extractAlignedPeaks(dtw_aligned As NamedValue(Of MzGroup)(), rtRange As DoubleRange, baseline As Double, joint As Boolean, xic_align As Boolean) As xcms2()
        ' and then export the peaks and area data
        Dim peaksSet As NamedCollection(Of PeakFeature)() = dtw_aligned _
            .Select(Function(sample)
                        Dim peaks = sample.Value.GetPeakGroups(
                                peakwidth:=rtRange,
                                quantile:=baseline,
                                sn_threshold:=0,
                                joint:=joint) _
                            .ExtractFeatureGroups _
                            .ToArray

                        Return New NamedCollection(Of PeakFeature)(sample.Name, peaks)
                    End Function) _
            .ToArray
        Dim xcms As xcms2()

        If xic_align Then
            xcms = peaksSet _
                .XicTable(rtwin:=rtRange.Max) _
                .ToArray
        Else
            xcms = peaksSet.XcmsTable.ToArray
        End If

        Return xcms
    End Function

    ''' <summary>
    ''' write peak debug data
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    '''     mz_deco(ms1, tolerance = "da:0.01", peak.width = [3,30]);
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
                                  Optional mzdiff As Object = "da:0.001",
                                  Optional norm As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(mzdiff, env, [default]:="da:0.001")
        Dim sampleData As NamedCollection(Of PeakFeature)() = Nothing

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

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

        Dim peaktable As xcms2() = sampleData _
            .CreateMatrix() _
            .ToArray
        Dim id As String() = peaktable.Select(Function(i) i.ID).uniqueNames
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

        Return peaktable
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
    ''' let XIC = mz.groups(ms1 = rawdata, mzdiff = "ppm:20");
    ''' 
    ''' </example>
    ''' <remarks>
    ''' the ion mz value is generated via the max intensity point in each ion 
    ''' feature group, and the xic data has already been re-order via the 
    ''' time asc.
    ''' </remarks>
    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup()))>
    Public Function mz_groups(<RRawVectorArgument>
                              ms1 As Object,
                              Optional mzdiff As Object = "ppm:20",
                              Optional rtwin As Double = 0.05,
                              Optional env As Environment = Nothing) As Object

        Return ms1Scans(ms1) _
            .GetMzGroups(mzdiff:=Math.getTolerance(mzdiff, env), rtwin:=rtwin) _
            .ToArray
    End Function

    Private Function ms1Scans(ms1 As Object) As IEnumerable(Of IMs1Scan)
        If ms1 Is Nothing Then
            Return {}
        ElseIf ms1.GetType Is GetType(ms1_scan()) Then
            Return DirectCast(ms1, ms1_scan()).Select(Function(t) DirectCast(t, IMs1Scan))
        ElseIf TypeOf ms1 Is mzPack Then
            Return DirectCast(ms1, mzPack) _
                .GetAllScanMs1 _
                .Select(Function(t) DirectCast(t, IMs1Scan))
        Else
            Throw New NotImplementedException
        End If
    End Function

    <ExportAPI("xic_pool")>
    <RApiReturn(GetType(XICPool))>
    Public Function XICpool(files As String()) As Object
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
    ''' debug used only
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <param name="mz"></param>
    ''' <param name="dtw"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    <ExportAPI("pull_xic")>
    Public Function pull_xic(pool As XICPool, mz As Double,
                             Optional dtw As Boolean = True,
                             Optional mzdiff As Double = 0.01) As Object
        If dtw Then
            Return New list With {
                .slots = pool _
                    .DtwXIC(mz, Tolerance.DeltaMass(mzdiff)) _
                    .ToDictionary(Function(a) a.Name,
                                  Function(a)
                                      Return CObj(a.Value)
                                  End Function)
            }
        Else
            Return New list With {
                .slots = pool _
                    .GetXICMatrix(mz, Tolerance.DeltaMass(mzdiff)) _
                    .ToDictionary(Function(a) a.Name,
                                  Function(a)
                                      Return CObj(a.Value)
                                  End Function)
            }
        End If
    End Function
End Module

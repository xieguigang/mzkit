#Region "Microsoft.VisualBasic::f45d574b3337099e9792d02a308cf1f1, mzkit\Rscript\Library\mzkit.quantify\GCMS.vb"

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

'   Total Lines: 238
'    Code Lines: 191
' Comment Lines: 12
'   Blank Lines: 35
'     File Size: 10.09 KB


' Module GCMSLinear
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: algorithm, ContentTable, createScanIonExtract, createSIMIonExtract, extractSampleRaw
'               FileNames2Contents, GetRawROIlist, InternalStandardMethod, quantifyIons, readRaw
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list

''' <summary>
''' the targetted GCMS sim data quantification module
''' </summary>
<Package("GCMS")>
<RTypeExport("gcms_peak", GetType(GCMSPeak))>
Module GCMSLinear

    Sub New()
        Call Internal.ConsolePrinter.AttachConsoleFormatter(Of TargetPeakPoint)(Function(pt) pt.ToString)

        Call generic.add("readBin.gcms_peak", GetType(Stream), AddressOf readGCSample)
        Call generic.add("writeBin", GetType(MzGroup), AddressOf writeSamples)
    End Sub

    Private Function readGCSample(file As Stream, args As list, env As Environment) As Object
        Return SaveSample.ReadGCSample(file).ToArray
    End Function

    Private Function writeSamples(samples As GCMSPeak(), args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call SaveSample.DumpGCMSPeaks(samples, con)
        Call con.Flush()
        Return True
    End Function

    <ExportAPI("as.quantify.ion")>
    Public Function quantifyIons(ions As MSLIon(), Optional rtwin As Double = 1) As QuantifyIon()
        Return QuantifyIon.FromIons(ions, rtwin).ToArray
    End Function

    <ExportAPI("SIMIonExtractor")>
    <RApiReturn(GetType(SIMIonExtract))>
    Public Function createSIMIonExtract(ions As QuantifyIon(),
                                        <RRawVectorArgument(GetType(Double))>
                                        Optional peakwidth As Object = "3,5",
                                        Optional centroid As Object = "da:0.3",
                                        Optional rtshift As Double = 30,
                                        Optional baselineQuantile As Double = 0.3,
                                        Optional env As Environment = Nothing) As Object

        Dim peakwin = GetDoubleRange(peakwidth, env)
        Dim ms1ppm = Math.getTolerance(centroid, env)

        If peakwin Like GetType(Message) Then
            Return peakwin.TryCast(Of Message)
        ElseIf ms1ppm Like GetType(Message) Then
            Return ms1ppm.TryCast(Of Message)
        End If

        Return New SIMIonExtract(ions, peakwin, ms1ppm, rtshift, baselineQuantile)
    End Function

    <ExportAPI("ScanIonExtractor")>
    <RApiReturn(GetType(ScanIonExtract))>
    Public Function createScanIonExtract(ions As QuantifyIon(),
                                        <RRawVectorArgument(GetType(Double))>
                                        Optional peakwidth As Object = "3,5",
                                        Optional centroid As Object = "da:0.3",
                                        Optional rtshift As Double = 30,
                                        Optional baselineQuantile As Double = 0.3,
                                        Optional env As Environment = Nothing) As Object

        Dim peakwin = GetDoubleRange(peakwidth, env)
        Dim ms1ppm = Math.getTolerance(centroid, env)

        If peakwin Like GetType(Message) Then
            Return peakwin.TryCast(Of Message)
        ElseIf ms1ppm Like GetType(Message) Then
            Return ms1ppm.TryCast(Of Message)
        End If

        Return New ScanIonExtract(ions, peakwin, ms1ppm, rtshift, baselineQuantile)
    End Function

    <ExportAPI("parseContents")>
    Public Function FileNames2Contents(<RRawVectorArgument> files As Object) As Rlist
        Dim names As String() = CLRVector.asCharacter(files)
        Dim vec As Dictionary(Of String, Object) = names _
            .ContentVector _
            .ToDictionary(Function(L) L.Key,
                          Function(L)
                              Return CObj(L.Value)
                          End Function)

        Return New Rlist(RType.GetRSharpType(GetType(Double))) With {
            .slots = vec
        }
    End Function

    <ExportAPI("contentTable")>
    <RApiReturn(GetType(ContentTable))>
    Public Function ContentTable(<RRawVectorArgument> ions As Object,
                                 <RRawVectorArgument> contentVector As Object,
                                 <RRawVectorArgument> Optional [IS] As Object = Nothing,
                                 Optional env As Environment = Nothing) As Object

        Dim MSL As pipeline = pipeline.TryCreatePipeline(Of MSLIon)(ions, env)
        Dim ISMap As Dictionary(Of String, String)

        If MSL.isError Then
            Return MSL.getError
        End If

        Dim MSLIons As MSLIon() = MSL.populates(Of MSLIon)(env).ToArray

        If TypeOf [IS] Is list Then
            ISMap = DirectCast([IS], list).AsGeneric(Of String)(env)
        ElseIf TypeOf [IS] Is String() OrElse TypeOf [IS] Is vector OrElse TypeOf [IS] Is String Then
            Dim ISnames As String() = CLRVector.asCharacter([IS])

            If ISnames.Length = 1 AndAlso ISnames(Scan0) = "IS" Then
                ISMap = New Dictionary(Of String, String)

                For Each ion As MSLIon In MSLIons
                    ISMap(ion.Name) = "IS"
                Next
            Else
                Return Internal.debug.stop(New NotImplementedException, env)
            End If
        Else
            Return Internal.debug.stop(New NotImplementedException, env)
        End If

        If TypeOf contentVector Is Rlist Then
            Dim vec As Dictionary(Of String, Double) = DirectCast(contentVector, Rlist).AsGeneric(Of Double)(env)
            Dim table As ContentTable = GCMS.ContentTable(MSLIons, vec, ISMap)

            Return table
        Else
            Return Message.InCompatibleType(GetType(Rlist), contentVector.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' read gcms rawdata
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.raw")>
    Public Function readRaw(file As String) As Raw
        Return GCMS.OpenRawAuto(file)
    End Function

    ''' <summary>
    ''' read raw peaks data
    ''' </summary>
    ''' <param name="extract"></param>
    ''' <param name="sample"></param>
    ''' <returns></returns>
    <ExportAPI("peakRaw")>
    <RApiReturn(GetType(TargetPeakPoint), GetType(ChromatogramTick))>
    Public Function extractSampleRaw(extract As QuantifyIonExtract, sample As Raw, Optional chromatogramPlot As Boolean = False) As Object
        If chromatogramPlot Then
            Dim rtmin As Double = sample.times.Min
            Dim rtmax As Double = sample.times.Max
            Dim scan As New ScanIonExtract(base:=extract)

            Return scan _
                .GetSamplePeaks(sample) _
                .ToDictionary(Function(ion) ion.Name,
                              Function(ion)
                                  Return CObj({
                                      New ChromatogramTick With {.Time = rtmin},
                                      New ChromatogramTick With {.Time = rtmax},
                                      New ChromatogramTick With {.Time = ion.Peak.window.Min},
                                      New ChromatogramTick With {.Time = ion.Peak.window.Max}
                                  }.JoinIterates(ion.Peak.ticks))
                              End Function) _
                .DoCall(Function(plot)
                            Return New Rlist With {.slots = plot}
                        End Function)
        Else
            Return extract.GetSamplePeaks(sample).ToArray
        End If
    End Function

    ''' <summary>
    ''' create linear model handler
    ''' </summary>
    ''' <param name="contents"></param>
    ''' <param name="maxDeletions"></param>
    ''' <returns></returns>
    <ExportAPI("linear_algorithm")>
    Public Function algorithm(contents As ContentTable,
                              Optional maxDeletions As Integer = 1,
                              Optional baselineQuantile As Double = 0,
                              Optional integrator As PeakAreaMethods = PeakAreaMethods.NetPeakSum) As InternalStandardMethod

        Return New InternalStandardMethod(
            contents:=contents,
            integrator:=integrator,
            baselineQuantile:=baselineQuantile,
            maxDeletions:=maxDeletions
        )
    End Function

    <ExportAPI("linears")>
    <RApiReturn(GetType(StandardCurve))>
    Public Function InternalStandardMethod(method As InternalStandardMethod,
                                           <RRawVectorArgument> reference As Object,
                                           Optional env As Environment = Nothing) As Object

        Dim points As pipeline = pipeline.TryCreatePipeline(Of TargetPeakPoint)(reference, env)

        If points.isError Then
            Return points.getError
        End If

        Return points _
            .populates(Of TargetPeakPoint)(env) _
            .DoCall(AddressOf method.ToLinears) _
            .ToArray
    End Function

    ''' <summary>
    ''' do peak detection for gc-ms rawdata
    ''' </summary>
    ''' <param name="raw">the input gcms rawdata, could be <see cref="Raw"/> for 
    ''' targetted data and <see cref="mzPack"/> for un-targetted gc-ms rawdata.
    ''' </param>
    ''' <param name="peakwidth"></param>
    ''' <param name="baseline"></param>
    ''' <param name="sn"></param>
    ''' <param name="env"></param>
    ''' <returns>a list of detected peak features</returns>
    <ExportAPI("ROIlist")>
    <RApiReturn(GetType(ROI), GetType(GCMSPeak))>
    Public Function GetRawROIlist(raw As Object,
                                  <RRawVectorArgument>
                                  Optional peakwidth As Object = "3,20",
                                  Optional baseline# = 0.65,
                                  Optional sn# = 3,
                                  Optional joint As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        Dim range = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "3,20")

        If range Like GetType(Message) Then
            Return range.TryCast(Of Message)
        End If

        If raw Is Nothing Then
            Return Nothing
        End If

        If TypeOf raw Is Raw Then
            Return DirectCast(raw, Raw) _
                .GetTIC _
                .Shadows _
                .PopulateROI(
                    peakwidth:=range.TryCast(Of DoubleRange),
                    baselineQuantile:=baseline,
                    snThreshold:=sn
                ) _
                .ToArray
        ElseIf TypeOf raw Is mzPack Then
            ' processing the un-targetted data
            Dim rawdata As mzPack = DirectCast(raw, mzPack)
            Dim scan1 As PeakMs2() = rawdata.MS _
                .SafeQuery _
                .Select(Function(s1)
                            Return New PeakMs2(s1.scan_id, s1.GetPeaks) With {
                                .rt = s1.rt,
                                .intensity = s1.TIC
                            }
                        End Function) _
                .ToArray

            Call VBDebugger.EchoLine("extract ROI peak features for GC-MS untargetted rawdata.")

            Dim peaks As GCMSPeak() = GCMSDeconv _
                .DeconvGCMSRawdata(scan1, range.TryCast(Of DoubleRange),
                                   quantile:=baseline,
                                   sn_threshold:=sn,
                                   joint:=joint) _
                .ToArray

            Return peaks
        Else
            Return Message.InCompatibleType(GetType(Raw), raw.GetType, env)
        End If
    End Function
End Module

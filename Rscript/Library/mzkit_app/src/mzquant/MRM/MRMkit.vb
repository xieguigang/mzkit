#Region "Microsoft.VisualBasic::cfdc2aa455fc234d20a1579ca5114a93, mzkit\Rscript\Library\mzkit.quantify\MRM\MRMkit.vb"

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

    '   Total Lines: 832
    '    Code Lines: 579
    ' Comment Lines: 166
    '   Blank Lines: 87
    '     File Size: 35.91 KB


    ' Module MRMkit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: asIonPair, ExtractIonData, ExtractPeakROI, GetPeakROIList, GetRTAlignments
    '               IsomerismIonPairs, Linears, MRMarguments, printIonPairs, R2
    '               readCompoundReference, readIonPairs, readIS, ROISummary, RTShiftSummary
    '               (+2 Overloads) SampleQuantify, ScanPeakTable, ScanPeakTable2, (+2 Overloads) ScanWiffRaw, WiffRawFile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports RRuntime = SMRUCC.Rsharp.Runtime
Imports std = System.Math
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.XLSX.File

''' <summary>
''' MRM Targeted Metabolomics
''' </summary>
<Package("MRMLinear", Category:=APICategories.ResearchTools, Publisher:="BioNovoGene")>
Module MRMkit

    Sub New()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of IonPair())(AddressOf printIonPairs)
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of IsomerismIonPairs)(
            Function(ion As IsomerismIonPairs)
                If ion.hasIsomerism Then
                    Return $"[{ion.index}, rt:{ion.target.rt} (sec)] {ion.target} {{{ion.ions.Select(Function(i) i.name).JoinBy(", ")}}}"
                Else
                    Return ion.target.ToString
                End If
            End Function)

        REnv.Internal.htmlPrinter.AttachHtmlFormatter(Of QCData)(AddressOf MRMQCReport.CreateHtml)
        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(RTAlignment()), AddressOf RTShiftSummary)

        Dim toolkit As AssemblyInfo = GetType(MRMkit).Assembly.FromAssembly

        Call VBDebugger.WaitOutput()
        Call toolkit.AppSummary(Nothing, Nothing, App.StdOut)
    End Sub

    Private Function RTShiftSummary(x As RTAlignment(), args As list, env As Environment) As Rdataframe
        Dim rownames = x.Select(Function(i) i.ion.target.accession).ToArray
        Dim cols As New Dictionary(Of String, Array)
        Dim name As Array = x.Select(Function(i) i.ion.target.name).ToArray
        Dim isomerism As Array = x.Select(Function(i) If(i.ion.hasIsomerism, "*", "")).ToArray
        Dim rt As Array = x _
            .Select(Function(i)
                        Dim act = std.Round(i.actualRT)
                        Dim ref = i.ion.target.rt

                        Return $"{act}/{If(ref Is Nothing, "NA", std.Round(ref.Value))}"
                    End Function) _
            .ToArray
        Dim rtshifts = x _
            .Select(Function(i)
                        Return i _
                            .CalcRtShifts _
                            .ToDictionary(Function(sample) sample.Name,
                                          Function(sample)
                                              Return sample.Value
                                          End Function)
                    End Function) _
            .ToArray
        Dim allSampleNames As String() = rtshifts _
            .Select(Function(i) i.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(s) s) _
            .ToArray
        Dim shifts As Array

        Call cols.Add(NameOf(name), name)
        Call cols.Add("rt(actual/reference)", rt)
        Call cols.Add(NameOf(isomerism), isomerism)

        For Each sampleName As String In allSampleNames
            shifts = rtshifts.Select(Function(result) result.TryGetValue(sampleName, [default]:=Double.NaN)).ToArray
            cols(sampleName) = shifts
        Next

        Return New Rdataframe With {
            .rownames = rownames,
            .columns = cols
        }
    End Function

    Private Function printIonPairs(obj As Object) As String
        Dim csv = DirectCast(obj, IonPair()).ToCsvDoc.ToMatrix.RowIterator.ToArray
        Dim printContent = csv.Print(addBorder:=False)

        Return printContent
    End Function

    ''' <summary>
    ''' Create argument object for run MRM quantification.
    ''' </summary>
    ''' <param name="tolerance"></param>
    ''' <param name="timeWindowSize#"></param>
    ''' <param name="angleThreshold#"></param>
    ''' <param name="baselineQuantile#"></param>
    ''' <param name="integratorTicks%"></param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="peakwidth"></param>
    ''' <param name="TPAFactors"></param>
    ''' <param name="sn_threshold"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MRM.arguments")>
    <RApiReturn(GetType(MRMArguments))>
    Public Function MRMarguments(Optional tolerance As Object = "da:0.3",
                                 Optional timeWindowSize# = 5,
                                 Optional angleThreshold# = 5,
                                 Optional baselineQuantile# = 0.65,
                                 Optional integratorTicks% = 5000,
                                 Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.NetPeakSum,
                                 <RRawVectorArgument>
                                 Optional peakwidth As Object = "8,30",
                                 Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                 Optional sn_threshold As Double = 3,
                                 Optional env As Environment = Nothing) As Object

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, Nothing)
        Dim mzErrors = Math.getTolerance(tolerance, env)

        If _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        ElseIf mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        End If

        Return New MRMArguments(
            TPAFactors:=TPAFactors,
            tolerance:=mzErrors,
            timeWindowSize:=timeWindowSize,
            angleThreshold:=angleThreshold,
            baselineQuantile:=baselineQuantile,
            integratorTicks:=integratorTicks,
            peakAreaMethod:=peakAreaMethod,
            peakwidth:=_peakwidth.TryCast(Of DoubleRange),
            sn_threshold:=sn_threshold
        )
    End Function

    <ExportAPI("MRM.rt_alignments")>
    Public Function GetRTAlignments(<RRawVectorArgument>
                                    cal As Object,
                                    <RRawVectorArgument>
                                    ions As Object,
                                    Optional args As MRMArguments = Nothing) As RTAlignment()

        Dim references As IEnumerable(Of String) = CLRVector.asCharacter(cal)
        Dim ionPairs As IonPair() = REnv.asVector(Of IonPair)(ions)

        If args Is Nothing Then
            args = MRM.MRMArguments.GetDefaultArguments
        End If

        Return RTAlignmentProcessor.AcquireRT(references, ionPairs, args)
    End Function

    ''' <summary>
    ''' Extract ion peaks
    ''' </summary>
    ''' <param name="mzML">A mzML raw file</param>
    ''' <param name="ionpairs">metabolite targets</param>
    ''' <returns></returns>
    <ExportAPI("extract.ions")>
    <RApiReturn(GetType(IonChromatogram))>
    Public Function ExtractIonData(mzML$, ionpairs As IonPair(), Optional tolerance As Object = "ppm:20", Optional env As Environment = Nothing) As Object
        Dim mzErrors = Math.getTolerance(tolerance, env)

        If mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        End If

        Return MRMSamples.ExtractIonData(
            ion_pairs:=IonPair.GetIsomerism(ionpairs, mzErrors),
            mzML:=mzML,
            assignName:=Function(i) i.accession,
            tolerance:=mzErrors
        ).DoCall(Function(data)
                     Return New vector With {.data = data}
                 End Function)
    End Function

    ''' <summary>
    ''' Exact ``regions of interested`` based on the given ion pair as targets.
    ''' </summary>
    ''' <param name="mzML">A mzML raw data file</param>
    ''' <param name="ionpairs">MRM ion pairs</param>
    ''' <param name="TPAFactors">Peak factors</param>
    ''' <param name="baselineQuantile#"></param>
    ''' <param name="integratorTicks%"></param>
    ''' <param name="peakAreaMethod"></param>
    ''' <returns></returns>
    <ExportAPI("extract.peakROI")>
    <RApiReturn(GetType(IonTPA))>
    Public Function ExtractPeakROI(mzML$, ionpairs As IonPair(),
                                   Optional tolerance$ = "ppm:20",
                                   Optional timeWindowSize# = 5,
                                   Optional baselineQuantile# = 0.65,
                                   Optional integratorTicks% = 5000,
                                   Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.NetPeakSum,
                                   Optional angleThreshold# = 5,
                                   <RRawVectorArgument>
                                   Optional peakwidth As Object = "8,30",
                                   Optional rtshift As Dictionary(Of String, Double) = Nothing,
                                   Optional bsplineDensity% = 100,
                                   Optional bsplineDegree% = 2,
                                   Optional sn_threshold As Double = 3,
                                   Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                   Optional env As Environment = Nothing) As Object

        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "8,30")

        If _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        End If

        Return ScanOfTPA.ScanTPA(
            raw:=mzML,
            ionpairs:=ionpairs,
            rtshifts:=rtshift,
            args:=New MRMArguments(
                TPAFactors:=TPAFactors,
                tolerance:=Ms1.Tolerance.ParseScript(tolerance),
                timeWindowSize:=timeWindowSize,
                angleThreshold:=angleThreshold,
                baselineQuantile:=baselineQuantile,
                integratorTicks:=integratorTicks,
                peakAreaMethod:=peakAreaMethod,
                peakwidth:=_peakwidth,
                sn_threshold:=sn_threshold
            )
        ).ToArray
    End Function

    ''' <summary>
    ''' Get ion pair definition data from a given table file.
    ''' </summary>
    ''' <param name="file">A csv file or xlsx Excel data sheet</param>
    ''' <param name="sheetName">The sheet name in excel tables.</param>
    ''' <returns></returns>
    <ExportAPI("read.ion_pairs")>
    Public Function readIonPairs(file$, Optional sheetName$ = "Sheet1") As IonPair()
        If file.ExtensionSuffix("xlsx") Then
            Return Xlsx.Open(path:=file) _
                .GetTable(sheetName) _
                .AsDataSource(Of IonPair)(silent:=True) _
                .ToArray
        Else
            Return file.LoadCsv(Of IonPair)(mute:=True).ToArray
        End If
    End Function

    <ExportAPI("isomerism.ion_pairs")>
    <RApiReturn(GetType(IsomerismIonPairs))>
    Public Function IsomerismIonPairs(ions As IonPair(), Optional tolerance$ = "ppm:20", Optional env As Environment = Nothing) As Object
        Dim mzErrors = Math.getTolerance(tolerance, env)

        If mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        End If

        Return IonPair.GetIsomerism(ions, mzErrors).ToArray
    End Function

    ''' <summary>
    ''' Convert any compatibale type as the ion pairs data object for MRM target selected.
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.ion_pairs")>
    <RApiReturn(GetType(IonPair()))>
    Public Function asIonPair(<RRawVectorArgument> mz As Object, Optional env As Environment = Nothing) As Object
        If mz Is Nothing Then
            Return Nothing
        End If

        Dim type As Type = mz.GetType

        If type.IsArray Then
            type = REnv.MeasureArrayElementType(mz)

            Select Case type
                Case GetType(MSLIon)
                    Return DirectCast(REnv.asVector(Of MSLIon)(mz), MSLIon()) _
                        .Select(Function(ion)
                                    Return New IonPair With {
                                        .accession = ion.Name,
                                        .name = ion.Name,
                                        .precursor = ion.MW,
                                        .product = ion.Peaks(Scan0).mz,
                                        .rt = ion.RT
                                    }
                                End Function) _
                        .ToArray
            End Select
        End If

        Return REnv.Internal.debug.stop(New NotImplementedException(mz.GetType.FullName), env)
    End Function

    ''' <summary>
    ''' Read reference points
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="sheetName$"></param>
    ''' <returns></returns>
    <ExportAPI("read.reference")>
    Public Function readCompoundReference(file$, Optional sheetName$ = "Sheet1") As Standards()
        Dim reference As Standards()

        If file.ExtensionSuffix("xlsx") Then
            reference = Xlsx.Open(path:=file) _
                .GetTable(sheetName) _
                .AsDataSource(Of Standards)(silent:=True) _
                .ToArray
        Else
            reference = file.LoadCsv(Of Standards)(mute:=True).ToArray
        End If

        Return reference
    End Function

    ''' <summary>
    ''' Read the definition of internal standards
    ''' </summary>
    ''' <param name="file">A csv file or xlsx file</param>
    ''' <param name="sheetName">
    ''' The sheet name that contains data of the IS data if the given file is a xlsx file.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.IS")>
    Public Function readIS(file$, Optional sheetName$ = "Sheet1", Optional env As Environment = Nothing) As [IS]()
        If file.ExtensionSuffix("xlsx") Then
            Dim table = Xlsx.Open(path:=file).GetTable(sheetName)

            If table Is Nothing Then
                ' probably no used of any IS for data calibration
                env.AddMessage($"No IS data was found in MRM information table file '{file.FileName}', where the sheet name is '{sheetName}'...", MSG_TYPES.WRN)
                Return {}
            Else
                Return table _
                    .AsDataSource(Of [IS])(silent:=True) _
                    .ToArray
            End If
        Else
            Return file.LoadCsv(Of [IS])(mute:=True).ToArray
        End If
    End Function

    ''' <summary>
    ''' Create model of the MRM raw files
    ''' </summary>
    ''' <param name="convertDir">
    ''' A directory data object for read MRM sample raw files. If the parameter value is
    ''' a ``list``, then it should contains at least two fields: ``samples`` and ``reference``.
    ''' The balnks raw data should be contains in reference files directory.
    ''' </param>
    ''' <param name="patternOfRef">File name pattern for filter reference data.</param>
    ''' <param name="patternOfBlank">File name pattern for filter blank controls.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("wiff.rawfiles")>
    Public Function WiffRawFile(<RRawVectorArgument>
                                convertDir As Object,
                                Optional patternOfRef$ = ".+[-]CAL[-]?\d+",
                                Optional patternOfBlank$ = "KB[-]?(\d+)?",
                                Optional env As Environment = Nothing) As Object

        If REnv.Internal.Invokes.isEmpty(convertDir) Then
            Return REnv.Internal.debug.stop("No raw files data provided!", env)
        End If

        Dim dataType As Type = convertDir.GetType

        If dataType Is GetType(String) Then
            Return RawFile.ScanDir(convertDir, patternOfRef, patternOfBlank)
        ElseIf dataType Is GetType(String()) Then
            With DirectCast(convertDir, String())
                If .Length = 1 Then
                    Return RawFile.ScanDir(.GetValue(Scan0), patternOfRef, patternOfBlank)
                Else
                    Return RawFile.ScanDir(
                        sampleDir:= .GetValue(0),
                        referenceDir:= .GetValue(1),
                        patternOfRefer:=patternOfRef,
                        patternOfBlanks:=patternOfBlank
                    )
                End If
            End With
        ElseIf dataType Is GetType(Rlist) Then
            ' samples/reference
            With DirectCast(convertDir, Rlist)
                Dim samples As String = RRuntime.getFirst(!samples)
                Dim reference As String = RRuntime.getFirst(!reference)

                Return RawFile.ScanDir(
                    sampleDir:=samples,
                    referenceDir:=reference,
                    patternOfRefer:=patternOfRef,
                    patternOfBlanks:=patternOfBlank
                )
            End With
        Else
            Return Message.InCompatibleType(GetType(String()), dataType, env)
        End If
    End Function

    ''' <summary>
    ''' Get MRM ions peaks data from a given raw data file
    ''' </summary>
    ''' <param name="mzML">the file path of the mzML raw data file</param>
    ''' <param name="ions">the ion pairs data list</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MRM.peak2")>
    <RApiReturn(GetType(DataSet))>
    Public Function ScanPeakTable2(mzML$, ions As IonPair(), args As MRMArguments,
                                   Optional rtshifts As Dictionary(Of String, Double) = Nothing,
                                   Optional env As Environment = Nothing) As Object

        If args Is Nothing Then
            Return Internal.debug.stop("the required MRM argument can not be nothing!", env)
        End If

        Return WiffRaw.ScanPeakTable(
            mzML:=mzML,
            ions:=ions,
            rtshifts:=rtshifts,
            args:=args
        )
    End Function

    ''' <summary>
    ''' Get MRM ions peaks data from a given raw data file
    ''' </summary>
    ''' <param name="mzML">the file path of the mzML raw data file</param>
    ''' <param name="ions">the ion pairs data list</param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="tolerance$"></param>
    ''' <param name="timeWindowSize#"></param>
    ''' <param name="angleThreshold#"></param>
    ''' <param name="baselineQuantile#"></param>
    ''' <param name="rtshifts"></param>
    ''' <param name="TPAFactors"></param>
    ''' <param name="peakwidth"></param>
    ''' <param name="sn_threshold"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("MRM.peaks")>
    <RApiReturn(GetType(DataSet))>
    Public Function ScanPeakTable(mzML$, ions As IonPair(),
                                  Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.NetPeakSum,
                                  Optional tolerance$ = "ppm:20",
                                  Optional timeWindowSize# = 5,
                                  Optional angleThreshold# = 5,
                                  Optional baselineQuantile# = 0.65,
                                  Optional rtshifts As Dictionary(Of String, Double) = Nothing,
                                  Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                  <RRawVectorArgument>
                                  Optional peakwidth As Object = "8,30",
                                  Optional sn_threshold As Double = 3,
                                  Optional env As Environment = Nothing) As Object

        Dim mzErrors = Math.getTolerance(tolerance, env)

        If mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        End If
        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "8,30")

        If _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        End If

        Return ScanPeakTable2(
            mzML:=mzML,
            ions:=ions,
            rtshifts:=rtshifts,
            args:=New MRMArguments(
                TPAFactors:=TPAFactors,
                tolerance:=mzErrors,
                timeWindowSize:=timeWindowSize,
                angleThreshold:=angleThreshold,
                baselineQuantile:=baselineQuantile,
                integratorTicks:=0,
                peakAreaMethod:=peakAreaMethod,
                peakwidth:=_peakwidth,
                sn_threshold:=sn_threshold
            ),
            env:=env
        )
    End Function

    <ExportAPI("wiff.scan2")>
    <RApiReturn(GetType(DataSet))>
    <Extension>
    Public Function ScanWiffRaw(wiffConverts As String(), ions As IonPair(),
                                Optional args As MRMArguments = Nothing,
                                Optional rtshifts As RTAlignment() = Nothing,
                                Optional removesWiffName As Boolean = True,
                                Optional env As Environment = Nothing) As Object

        If wiffConverts Is Nothing Then
            Return Internal.debug.stop({
                "the given wiff raw file list is nothing or empty!",
                "argument: " & NameOf(wiffConverts)
            }, env)
        ElseIf args Is Nothing Then
            Return Internal.debug.stop("the required MRM argument can not be nothing!", env)
        End If

        If rtshifts Is Nothing Then
            rtshifts = {}
        End If

        Return WiffRaw.Scan(
            mzMLRawFiles:=wiffConverts,
            ions:=ions,
            refName:=Nothing,
            removesWiffName:=removesWiffName,
            rtshifts:=rtshifts,
            args:=args
        )
    End Function

    ''' <summary>
    ''' # Scan the raw file data
    ''' 
    ''' Get the peak area data of the metabolites in each given sample 
    ''' data files
    ''' </summary>
    ''' <param name="wiffConverts">
    ''' A directory that contains the mzML files which are converts from 
    ''' the given wiff raw file.
    ''' </param>
    ''' <param name="ions">Ion pairs definition data.</param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="TPAFactors"></param>
    ''' <param name="removesWiffName"></param>
    ''' <param name="rtshifts">
    ''' For the calibration of the linear model reference points used only, 
    ''' **DO NOT apply this parameter for the user sample data!**
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("wiff.scans")>
    <RApiReturn(GetType(DataSet))>
    Public Function ScanWiffRaw(wiffConverts As String(), ions As IonPair(),
                                Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.NetPeakSum,
                                Optional tolerance$ = "ppm:20",
                                Optional angleThreshold# = 5,
                                Optional baselineQuantile# = 0.65,
                                Optional removesWiffName As Boolean = True,
                                Optional timeWindowSize# = 5,
                                Optional rtshifts As RTAlignment() = Nothing,
                                Optional bsplineDensity% = 100,
                                Optional bsplineDegree% = 2,
                                Optional resolution% = 3000,
                                <RRawVectorArgument>
                                Optional peakwidth As Object = "8,30",
                                Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                Optional sn_threshold As Double = 3,
                                Optional env As Environment = Nothing) As Object

        Dim mzErrors = Math.getTolerance(tolerance, env)

        If mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        End If
        If TPAFactors Is Nothing Then
            TPAFactors = New Dictionary(Of String, Double)
        End If

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "8,30")

        If _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        End If

        'If wiffConverts Is Nothing Then
        '    Throw New ArgumentNullException(NameOf(wiffConverts))
        'ElseIf RRuntime.isVector(Of String)(wiffConverts) Then
        '    Dim stringVec As Array = RRuntime.asVector(Of String)(wiffConverts)

        '    If stringVec.Length = 1 Then
        '        wiffConverts = stringVec.GetValue(Scan0) _
        '                .ToString _
        '                .ListFiles("*.mzML") _
        '                .ToArray _
        '                .DoCall(Function(files)
        '                            Return RawFile.WrapperForStandards(files, "CAL[-]?\d+")
        '                        End Function)
        '    Else
        '        wiffConverts = RawFile.WrapperForStandards(stringVec, "CAL[-]?\d+")
        '    End If
        'End If

        'Dim raw As RawFile = DirectCast(wiffConverts, RawFile)
        Dim errorTolerance As Tolerance = mzErrors

        Return wiffConverts.ScanWiffRaw(
            ions:=ions,
            removesWiffName:=removesWiffName,
            rtshifts:=rtshifts,
            args:=New MRMArguments(
                TPAFactors:=TPAFactors,
                tolerance:=errorTolerance,
                timeWindowSize:=timeWindowSize,
                angleThreshold:=angleThreshold,
                baselineQuantile:=baselineQuantile,
                integratorTicks:=0,
                peakAreaMethod:=peakAreaMethod,
                peakwidth:=_peakwidth,
                sn_threshold:=sn_threshold
            ),
            env:=env
        )
    End Function

    ''' <summary>
    ''' Create linear fitting based on the wiff raw scan data.
    ''' </summary>
    ''' <param name="rawScan">The wiff raw scan data which are extract by function: ``wiff.scans``.</param>
    ''' <param name="calibrates"></param>
    ''' <param name="[ISvector]"></param>
    ''' <param name="maxDeletions">
    ''' Max number of the reference points that delete automatically by 
    ''' the linear modelling program.
    ''' 
    ''' + negative value means auto
    ''' + zero means no deletion
    ''' + positive means the max allowed point numbers for auto deletion by the program
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 20200106 checked, test success
    ''' </remarks>
    <ExportAPI("linears")>
    Public Function Linears(rawScan As DataSet(), calibrates As Standards(), [ISvector] As [IS](),
                            Optional blankControls As DataSet() = Nothing,
                            Optional maxDeletions As Integer = 1,
                            Optional isWorkCurveMode As Boolean = True,
                            Optional args As MRMArguments = Nothing) As StandardCurve()

        Return rawScan.ToDictionary _
            .Regression(
                calibrates:=calibrates,
                ISvector:=ISvector,
                blankControls:=blankControls,
                maxDeletions:=maxDeletions,
                isWorkCurveMode:=isWorkCurveMode
            ) _
            .Select(Function(line)
                        line.arguments = args
                        Return line
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("R2")>
    Public Function R2(lines As StandardCurve()) As Double()
        Return lines.Select(Function(r) r.linear.R2).ToArray
    End Function

    ''' <summary>
    ''' Do sample quantify
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="file">The sample raw file its file path.</param>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    <ExportAPI("sample.quantify2")>
    <RApiReturn(GetType(QuantifyScan))>
    Public Function SampleQuantify(model As StandardCurve(), file$, ions As IonPair(), Optional env As Environment = Nothing) As Object
        If model.Any(Function(line) line.arguments Is Nothing) Then
            Return Internal.debug.stop("part of the reference line missing argument value!", env)
        End If

        Dim scan As New QuantifyScan

        For Each line As StandardCurve In model
            Dim ion As IonPair = ions.Where(Function(i) i.accession = line.name).FirstOrDefault
            Dim subscan As QuantifyScan = MRMSamples.SampleQuantify(
                model:={line},
                file:=file,
                ions:={ion},
                args:=line.arguments,
                rtshifts:=New Dictionary(Of String, Double)
            )
        Next

        Return scan
    End Function

    ''' <summary>
    ''' Do sample quantify
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="file">The sample raw file its file path.</param>
    ''' <param name="ions"></param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="TPAFactors"></param>
    ''' <returns></returns>
    <ExportAPI("sample.quantify")>
    <RApiReturn(GetType(QuantifyScan))>
    Public Function SampleQuantify(model As StandardCurve(), file$, ions As IonPair(),
                                   Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.NetPeakSum,
                                   Optional tolerance$ = "ppm:20",
                                   Optional timeWindowSize# = 5,
                                   Optional angleThreshold# = 5,
                                   Optional baselineQuantile# = 0.65,
                                   <RRawVectorArgument>
                                   Optional peakwidth As Object = "8,30",
                                   Optional TPAFactors As Dictionary(Of String, Double) = Nothing,
                                   Optional sn_threshold As Double = 3,
                                   Optional env As Environment = Nothing) As Object

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "8,30")
        Dim mzErrors = Math.getTolerance(tolerance, env)

        If mzErrors Like GetType(Message) Then
            Return mzErrors.TryCast(Of Message)
        ElseIf _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        End If

        Return MRMSamples.SampleQuantify(
            model:=model,
            file:=file,
            ions:=ions,
            args:=New MRMArguments(
                TPAFactors:=TPAFactors,
                tolerance:=mzErrors,
                timeWindowSize:=timeWindowSize,
                angleThreshold:=angleThreshold,
                baselineQuantile:=baselineQuantile,
                integratorTicks:=0,
                peakAreaMethod:=peakAreaMethod,
                peakwidth:=_peakwidth,
                sn_threshold:=sn_threshold
            ),
            rtshifts:=New Dictionary(Of String, Double)
        )
    End Function
End Module

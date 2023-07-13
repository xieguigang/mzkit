#Region "Microsoft.VisualBasic::d9b25d3eed3358524d383e2fdb072054, mzkit\Rscript\Library\mzkit\math\Math.vb"

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

'   Total Lines: 695
'    Code Lines: 477
' Comment Lines: 124
'   Blank Lines: 94
'     File Size: 27.74 KB


' Module MzMath
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: centroid, cosine, CreateMSMatrix, createTolerance, defaultPrecursors
'               exact_mass, getAlignmentTable, GetClusters, getPrecursorTable, jaccard
'               jaccardSet, mz, MzUnique, peaktable, ppm
'               precursorTypes, printCalculator, printMzTable, sequenceOrder, spectrumEntropy
'               SpectrumTreeCluster, SSMCompares, xcms_id, XICTable
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdVector = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' mass spectrometry data math toolkit
''' </summary>
<Package("math", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
Module MzMath

    Sub New()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of PrecursorInfo())(AddressOf printMzTable)
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of MzCalculator)(AddressOf printCalculator)

        Call REnv.Internal.Object.Converts.addHandler(GetType(MzGroup), AddressOf XICTable)
        Call REnv.Internal.Object.Converts.addHandler(GetType(AlignmentOutput), AddressOf getAlignmentTable)
        Call REnv.Internal.Object.Converts.addHandler(GetType(PrecursorInfo()), AddressOf getPrecursorTable)

        Call ExactMass.SetExactMassParser(Function(f) FormulaScanner.EvaluateExactMass(f))
    End Sub

    Private Function getPrecursorTable(list As PrecursorInfo(), args As list, env As Environment) As dataframe
        Dim precursor_type As String() = list.Select(Function(i) i.precursor_type).ToArray
        Dim charge As Double() = list.Select(Function(i) i.charge).ToArray
        Dim M As Double() = list.Select(Function(i) i.M).ToArray
        Dim adduct As Double() = list.Select(Function(i) i.adduct).ToArray
        Dim mz As String() = list.Select(Function(i) i.mz).ToArray
        Dim ionMode As Integer() = list.Select(Function(i) i.ionMode).ToArray

        Return New dataframe With {
            .rownames = precursor_type,
            .columns = New Dictionary(Of String, Array) From {
                {"precursor_type", precursor_type},
                {"charge", charge},
                {"M", M},
                {"adduct", adduct},
                {"m/z", mz},
                {"ionMode", ionMode}
            }
        }
    End Function

    Private Function getAlignmentTable(align As AlignmentOutput, args As list, env As Environment) As dataframe
        Dim mz As Double() = align.alignments.Select(Function(a) a.mz).ToArray
        Dim query As Double() = align.alignments.Select(Function(a) a.query).ToArray
        Dim reference As Double() = align.alignments.Select(Function(a) a.ref).ToArray
        Dim da As String() = align.alignments.Select(Function(a) a.da).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mz},
                {"query", query},
                {"ref", reference},
                {"da", da}
            }
        }
    End Function

    Private Function printCalculator(type As MzCalculator) As String
        Dim summary As New StringBuilder

        Call summary.AppendLine(type.ToString)
        Call summary.AppendLine($"adducts: {type.adducts}")
        Call summary.AppendLine($"M: {type.M}")
        Call summary.AppendLine($"charge: {type.charge}")
        Call summary.AppendLine($"ion_mode: {type.mode}")

        Return summary.ToString
    End Function

    Private Function XICTable(x As MzGroup, args As list, env As Environment) As dataframe
        Dim mz As Array = {x.mz}
        Dim into As Array = x.XIC.Select(Function(t) t.Intensity).ToArray
        Dim rt As Array = x.XIC.Select(Function(t) t.Time).ToArray
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mz},
                {"rt", rt},
                {"into", into}
            }
        }

        Return table
    End Function

    Private Function printMzTable(obj As Object) As String
        Return DirectCast(obj, PrecursorInfo()).Print(addBorder:=False)
    End Function

    ''' <summary>
    ''' evaluate all m/z for all known precursor type.
    ''' </summary>
    ''' <param name="mass">the target exact mass value</param>
    ''' <param name="mode">
    ''' this parameter could be two type of data:
    ''' 
    ''' 1. character of value ``+`` or ``-``, means evaluate all m/z for all known precursor types in given ion mode
    ''' 2. character of value in precursor type format means calculate mz for the target precursor type
    ''' 3. mzcalculator type means calculate mz for the traget precursor type
    ''' 4. a list of the mz calculator object and a list of corresponding mz value will be evaluated.
    ''' 
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("mz")>
    <RApiReturn(GetType(PrecursorInfo), GetType(Double))>
    Public Function mz(mass As Double,
                       Optional mode As Object = "+",
                       Optional env As Environment = Nothing) As Object

        If TypeOf mode Is MzCalculator Then
            Return DirectCast(mode, MzCalculator).CalcMZ(mass)
        ElseIf TypeOf mode Is list Then
            Dim err As Message = Nothing
            Dim adducts = DirectCast(mode, list).AsGeneric(Of MzCalculator)(env, err:=err)

            If Not err Is Nothing Then
                Return err
            Else
                Return New list With {
                    .slots = adducts _
                        .ToDictionary(Function(c) c.Key,
                                      Function(c)
                                          Return CObj(c.Value.CalcMZ(mass))
                                      End Function)
                }
            End If
        Else
            Dim strVal As String = any.ToString(mode, "+")

            Static supportedModes As Index(Of String) = {"+", "-", "1", "-1"}

            If strVal Like supportedModes Then
                Return MzCalculator _
                    .EvaluateAll(mass, strVal) _
                    .ToArray
            Else
                ' the given string is a string value in precursor_type
                ' format
                Return Ms1.PrecursorType _
                    .ParseMzCalculator(strVal, strVal.Last) _
                    .CalcMZ(mass)
            End If
        End If
    End Function

    ''' <summary>
    ''' evaluate all exact mass for all known precursor type.
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    <ExportAPI("exact_mass")>
    Public Function exact_mass(mz As Double, Optional mode As Object = "+") As PrecursorInfo()
        Return MzCalculator.EvaluateAll(mz, any.ToString(mode, "+"), True).ToArray
    End Function

    ''' <summary>
    ''' calculate ppm value between two mass vector
    ''' </summary>
    ''' <param name="a">mass a</param>
    ''' <param name="b">mass b</param>
    ''' <returns></returns>
    <ExportAPI("ppm")>
    Public Function ppm(<RRawVectorArgument> a As Object,
                        <RRawVectorArgument> b As Object,
                        Optional env As Environment = Nothing) As Double()

        Dim x As Double() = CLRVector.asNumeric(a)
        Dim y As Double() = CLRVector.asNumeric(b)

        Return Vectorization.BinaryCoreInternal(Of Double, Double, Double)(
            x:=x,
            y:=y,
            [do]:=Function(xi, yi, env2) PPMmethod.PPM(xi, yi),
            env:=env
        )
    End Function

    ''' <summary>
    ''' create a delegate function pointer that apply for compares spectrums theirs similarity.
    ''' </summary>
    ''' <param name="tolerance"></param>
    ''' <param name="equals_score"></param>
    ''' <param name="gt_score"></param>
    ''' <param name="score_aggregate">
    ''' ``<see cref="Func(Of Double, Double, Double)"/>``
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("spectrum.compares")>
    <RApiReturn(GetType(Comparison(Of PeakMs2)))>
    Public Function SSMCompares(Optional tolerance As Object = "da:0.1",
                                Optional equals_score# = 0.85,
                                Optional gt_score# = 0.6,
                                Optional score_aggregate As ScoreAggregates = ScoreAggregates.min,
                                Optional env As Environment = Nothing) As Object

        Dim errors = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Return Spectra.SpectrumTreeCluster.SSMCompares(errors.TryCast(Of Tolerance), Nothing, equals_score, gt_score, score_aggregate)
    End Function

    <ExportAPI("jaccard")>
    Public Function jaccard(query As Double(), ref As Double(),
                            Optional tolerance As Object = "da:0.3",
                            Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Return GlobalAlignment.JaccardIndex(query, ref, mzErr)
    End Function

    ''' <summary>
    ''' Search spectra with entropy similarity
    ''' </summary>
    ''' <param name="x">target spectral data that used for calculate the entropy value</param>
    ''' <param name="ref">
    ''' if this parameter is not nothing, then the spectral similarity score will
    ''' be evaluated from this function.
    ''' </param>
    ''' <param name="tolerance">
    ''' the mass tolerance error to make the spectrum data centroid
    ''' 
    ''' To calculate spectral entropy, the spectrum need to be centroid first. When you are
    ''' focusing on fragment ion's information, the precursor ion may need to be removed 
    ''' from the spectrum before calculating spectral entropy. If isotope peak exitsted on
    ''' the MS/MS spectrum, the isotope peak should be removed fist as the isotope peak does
    ''' not contain useful information for identifing molecule.
    ''' </param>
    ''' <param name="intocutoff">
    ''' the percentage relative intensity value that used for removes the noise ion fragment peaks
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Li, Y., Kind, T., Folz, J. et al. Spectral entropy outperforms MS/MS dot product
    ''' similarity for small-molecule compound identification. Nat Methods 18, 1524–1531
    ''' (2021). https://doi.org/10.1038/s41592-021-01331-z
    ''' 
    ''' </remarks>
    <ExportAPI("spectral_entropy")>
    Public Function spectrumEntropy(x As LibraryMatrix,
                                    Optional ref As LibraryMatrix = Nothing,
                                    Optional tolerance As Object = "da:0.3",
                                    Optional intocutoff As Double = 0.05,
                                    Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        x = x.CentroidMode(mzErr.TryCast(Of Tolerance), New RelativeIntensityCutoff(intocutoff))

        If ref IsNot Nothing Then
            ref = ref.CentroidMode(mzErr.TryCast(Of Tolerance), New RelativeIntensityCutoff(intocutoff))

            Return SpectralEntropy.calculate_entropy_similarity(
                spectrum_a:=x.ms2,
                spectrum_b:=ref.ms2,
                tolerance:=mzErr.TryCast(Of Tolerance)
            )
        Else
            Dim into As stdVector = x.intensity
            Dim ent As Double = (into / into.Sum).ShannonEntropy

            Return ent
        End If
    End Function

    <ExportAPI("jaccardSet")>
    Public Function jaccardSet(query As Double(), ref As Double(),
                               Optional tolerance As Object = "da:0.3",
                               Optional env As Environment = Nothing)

        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim union As Double() = GlobalAlignment.MzUnion(query, ref, mzErr)
        Dim intersects As Double() = GlobalAlignment.MzIntersect(query, ref, mzErr)

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"intersect", intersects},
                {"union", union}
            }
        }
    End Function

    ''' <summary>
    ''' Do evaluate the spectra cosine similarity score
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="ref"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="intocutoff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("cosine")>
    <RApiReturn(GetType(AlignmentOutput))>
    Public Function cosine(query As LibraryMatrix, ref As LibraryMatrix,
                           Optional tolerance As Object = "da:0.3",
                           Optional intocutoff As Double = 0.05,
                           Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        query = query.CentroidMode(mzErr.TryCast(Of Tolerance), New RelativeIntensityCutoff(intocutoff))
        ref = ref.CentroidMode(mzErr.TryCast(Of Tolerance), New RelativeIntensityCutoff(intocutoff))

        Dim cos As New CosAlignment(mzErr, New RelativeIntensityCutoff(intocutoff))
        Dim align As AlignmentOutput = cos.CreateAlignment(query.ms2, ref.ms2)

        align.query = New Meta With {.id = query.name}
        align.reference = New Meta With {.id = ref.name}

        Return align
    End Function

    ''' <summary>
    ''' create spectrum tree cluster based on the spectrum to spectrum similarity comparison.
    ''' </summary>
    ''' <param name="ms2list">a vector of spectrum peaks data</param>
    ''' <param name="compares">a delegate function pointer that could be generated by ``spectrum.compares`` api.</param>
    ''' <param name="tolerance">the mz tolerance threshold value</param>
    ''' <param name="intocutoff">intensity cutoff of the spectrum matrix its product ``m/z`` fragments.</param>
    ''' <param name="showReport">show progress report?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("spectrum_tree.cluster")>
    <RApiReturn(GetType(SpectrumTreeCluster))>
    Public Function SpectrumTreeCluster(<RRawVectorArgument>
                                        ms2list As Object,
                                        Optional compares As Comparison(Of PeakMs2) = Nothing,
                                        Optional tolerance As Object = "da:0.1",
                                        Optional intocutoff As Double = 0.05,
                                        Optional showReport As Boolean = False,
                                        Optional env As Environment = Nothing) As Object

        Dim spectrum As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ms2list, env)
        Dim mzrange = getTolerance(tolerance, env)
        Dim threshold As New RelativeIntensityCutoff(intocutoff)

        If spectrum.isError Then
            Return spectrum.getError
        ElseIf mzrange Like GetType(Message) Then
            Return mzrange.TryCast(Of Message)
        End If

        Return New SpectrumTreeCluster(
            compares:=compares,
            showReport:=showReport,
            mzwidth:=mzrange,
            intocutoff:=threshold
        ).doCluster(spectrum:=spectrum.populates(Of PeakMs2)(env).ToArray)
    End Function

    ''' <summary>
    ''' get all nodes from the spectrum tree cluster result
    ''' </summary>
    ''' <param name="tree">the spectra molecule networking tree</param>
    ''' <returns></returns>
    <ExportAPI("cluster.nodes")>
    Public Function GetClusters(tree As SpectrumTreeCluster) As SpectrumCluster()
        Return tree.PopulateClusters.ToArray
    End Function

    ''' <summary>
    ''' data pre-processing helper, make the spectra ion data unique
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="eq#"></param>
    ''' <param name="gt#"></param>
    ''' <param name="mzwidth$"></param>
    ''' <param name="tolerance$"></param>
    ''' <param name="precursor$"></param>
    ''' <param name="rtwidth#"></param>
    ''' <param name="trim$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("ions.unique")>
    Public Function MzUnique(<RRawVectorArgument> ions As Object,
                             Optional eq# = 0.85,
                             Optional gt# = 0.6,
                             Optional mzwidth$ = "da:0.1",
                             Optional tolerance$ = "da:0.3",
                             Optional precursor$ = "ppm:20",
                             Optional rtwidth# = 5,
                             Optional trim$ = "0.05",
                             Optional env As Environment = Nothing) As Object

        Dim data As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ions, env, suppress:=True)
        Dim ionstream As IEnumerable(Of PeakMs2)

        If data.isError Then
            data = pipeline.TryCreatePipeline(Of MGF.Ions)(ions, env)

            If data.isError Then
                Return data.getError
            End If

            ionstream = data.populates(Of MGF.Ions)(env).IonPeaks
        Else
            ionstream = data.populates(Of PeakMs2)(env)
        End If

        Return ionstream _
            .Unique(
                eq:=eq,
                gt:=gt,
                mzwidth:=mzwidth,
                tolerance:=tolerance,
                precursor:=precursor,
                rtwidth:=rtwidth,
                trim:=trim
            ) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' Converts profiles peak data to peak data in centroid mode.
    ''' 
    ''' profile and centroid in Mass Spectrometry?
    ''' 
    ''' 1. Profile means the continuous wave form in a mass spectrum.
    '''   + Number of data points Is large.
    ''' 2. Centroid means the peaks in a profile data Is changed to bars.
    '''   + location of the bar Is center of the profile peak.
    '''   + height of the bar Is area of the profile peak.
    '''   
    ''' </summary>
    ''' <param name="ions">
    ''' value of this parameter could be 
    ''' 
    ''' + a collection of peakMs2 data 
    ''' + a library matrix data 
    ''' + or a dataframe object which should contains at least ``mz`` and ``intensity`` columns.
    ''' + or just a m/z vector
    ''' + also could be a mzpack data object
    ''' 
    ''' </param>
    ''' <returns>
    ''' Peaks data in centroid mode or a new m/z vector in centroid.
    ''' </returns>
    <ExportAPI("centroid")>
    <RApiReturn(GetType(PeakMs2), GetType(LibraryMatrix), GetType(Double))>
    Public Function centroid(<RRawVectorArgument> ions As Object,
                             Optional tolerance As Object = "da:0.1",
                             Optional intoCutoff As Double = 0.05,
                             Optional parallel As Boolean = False,
                             Optional env As Environment = Nothing) As Object

        Dim inputType As Type = ions.GetType
        Dim errors As [Variant](Of Tolerance, Message) = getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        Else
            Dim mzvec As pipeline = pipeline.TryCreatePipeline(Of Double)(ions, env, suppress:=True)

            If Not mzvec.isError Then
                Return mzvec _
                    .populates(Of Double)(env) _
                    .GroupBy(errors.TryCast(Of Tolerance)) _
                    .Select(Function(d) d.Average) _
                    .ToArray
            End If
        End If

        Dim threshold As LowAbundanceTrimming = New RelativeIntensityCutoff(intoCutoff)

        If TypeOf ions Is vector Then
            ions = DirectCast(ions, vector).data
            ions = REnv.TryCastGenericArray(ions, env)
            inputType = ions.GetType
        End If

        If inputType Is GetType(pipeline) OrElse inputType Is GetType(PeakMs2()) Then
            Dim source As IEnumerable(Of PeakMs2) = If(
                inputType Is GetType(pipeline),
                DirectCast(ions, pipeline).populates(Of PeakMs2)(env),
                DirectCast(ions, PeakMs2())
            )
            Dim converter = Iterator Function() As IEnumerable(Of PeakMs2)
                                For Each peak As PeakMs2 In source
                                    ' only updates of the spectral data
                                    ' will not create new object, so the
                                    ' metadata inside the class object will
                                    ' not loess
                                    peak.mzInto = peak.mzInto _
                                        .Centroid(errors, threshold) _
                                        .ToArray

                                    Yield peak
                                Next
                            End Function

            If parallel Then
                Return New pipeline(converter().AsParallel, GetType(PeakMs2))
            Else
                Return New pipeline(converter(), GetType(PeakMs2))
            End If
        ElseIf inputType Is GetType(PeakMs2) Then
            Dim ms2Peak As PeakMs2 = DirectCast(ions, PeakMs2)

            ms2Peak.mzInto = ms2Peak.mzInto _
                .Centroid(errors, threshold) _
                .ToArray

            Return ms2Peak
        ElseIf inputType Is GetType(LibraryMatrix) Then
            Dim ms2 As LibraryMatrix = DirectCast(ions, LibraryMatrix)

            If Not ms2.centroid Then
                ms2 = ms2.CentroidMode(errors, threshold)
            End If

            Return ms2
        ElseIf inputType Is GetType(dataframe) Then
            Return DirectCast(ions, dataframe).centroidDataframe(errors, threshold, env)
        ElseIf inputType Is GetType(ScanMS1) Then
            Dim scan1 As ScanMS1 = DirectCast(ions, ScanMS1)
            Dim msdata As ms2() = scan1 _
                .GetMs _
                .ToArray _
                .Centroid(errors, threshold) _
                .ToArray

            Return New ScanMS1 With {
                .BPC = scan1.BPC,
                .into = msdata.Select(Function(a) a.intensity).ToArray,
                .meta = scan1.meta,
                .mz = msdata.Select(Function(a) a.mz).ToArray,
                .products = scan1.products,
                .rt = scan1.rt,
                .scan_id = scan1.scan_id,
                .TIC = scan1.TIC
            }
        Else
            Return Internal.debug.stop(New InvalidCastException(inputType.FullName), env)
        End If
    End Function

    Private Function centroidMzPack(data As mzPack, errors As Tolerance, threshold As LowAbundanceTrimming) As mzPack

    End Function

    <Extension>
    Private Function centroidDataframe(data As dataframe,
                                       errors As Tolerance,
                                       threshold As LowAbundanceTrimming,
                                       env As Environment) As Object

        Dim mz As Double(), into As Double()

        If data.hasName("mz") Then
            mz = CLRVector.asNumeric(data!mz)
        ElseIf data.hasName("m/z") Then
            mz = CLRVector.asNumeric(data("m/z"))
        Else
            Return Internal.debug.stop("mz column in dataframe should be 'mz' or 'm/z'!", env)
        End If

        If data.hasName("into") Then
            into = CLRVector.asNumeric(data!into)
        ElseIf data.hasName("intensity") Then
            into = CLRVector.asNumeric(data!intensity)
        Else
            Return Internal.debug.stop("intensity column in dataframe should be 'into' or 'intensity'!", env)
        End If

        Dim ms2 As New LibraryMatrix With {
            .centroid = False,
            .name = "MS-matrix from dataframe",
            .ms2 = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = into(i)
                            }
                        End Function) _
                .ToArray
        }

        Return ms2.CentroidMode(errors, threshold)
    End Function

    ''' <summary>
    ''' Create tolerance object
    ''' </summary>
    ''' <param name="threshold"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    <ExportAPI("tolerance")>
    Public Function createTolerance(threshold As Double,
                                    <RRawVectorArgument(GetType(String))>
                                    Optional method As Object = "ppm|da",
                                    Optional env As Environment = Nothing) As Object

        Dim methodVec As String() = CLRVector.asCharacter(method)

        Select Case methodVec(Scan0).ToLower
            Case "da" : Return Tolerance.DeltaMass(threshold)
            Case "ppm" : Return Tolerance.PPM(threshold)
            Case Else
                Return Internal.debug.stop({
                    $"invalid method name: '{methodVec(Scan0)}'!",
                    $"given: {methodVec(Scan0)}"
                }, env)
        End Select
    End Function

    ''' <summary>
    ''' reorder scan points into a sequence for downstream data analysis
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="mzwidth"></param>
    ''' <param name="rtwidth"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("sequenceOrder")>
    Public Function sequenceOrder(<RRawVectorArgument> scans As Object,
                                  Optional mzwidth As Object = "da:0.1",
                                  Optional rtwidth As Double = 60,
                                  Optional env As Environment = Nothing) As Object

        Dim points As pipeline = pipeline.TryCreatePipeline(Of ms1_scan)(scans, env)

        If points.isError Then
            Return points.getError
        End If

        Dim mzwindow As [Variant](Of Tolerance, Message) = getTolerance(mzwidth, env)

        If mzwindow Like GetType(Message) Then
            Return mzwindow.TryCast(Of Message)
        End If

        Return points.populates(Of ms1_scan)(env) _
            .SequenceOrder(mzwindow.TryCast(Of Tolerance), rtwidth) _
            .ToArray
    End Function

    ''' <summary>
    ''' create precursor type calculator
    ''' </summary>
    ''' <param name="types"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("precursor_types")>
    Public Function precursorTypes(<RRawVectorArgument> types As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, MzCalculator)(
            types, Function(type)
                       Return Ms1.PrecursorType.ParseMzCalculator(type, type.Last)
                   End Function)
    End Function

    ''' <summary>
    ''' returns all precursor types for a given libtype
    ''' </summary>
    ''' <param name="ionMode"></param>
    ''' <returns></returns>
    <ExportAPI("defaultPrecursors")>
    Public Function defaultPrecursors(ionMode As String) As MzCalculator()
        Return Provider.GetCalculator(ionMode).Values.ToArray
    End Function

    <ExportAPI("toMS")>
    Public Function CreateMSMatrix(isotope As IsotopeDistribution) As LibraryMatrix
        Return New LibraryMatrix With {
            .name = isotope.data(Scan0).Formula.ToString,
            .centroid = False,
            .ms2 = isotope.mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = isotope.intensity(i)
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' makes xcms_id format liked ROI unique id
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <returns></returns>
    <ExportAPI("xcms_id")>
    Public Function xcms_id(mz As Double(), rt As Double()) As String()
        Dim allId As String() = mz _
            .Select(Function(mzi, i)
                        If CInt(rt(i)) = 0 Then
                            Return $"M{CInt(mzi)}"
                        Else
                            Return $"M{CInt(mzi)}T{CInt(rt(i))}"
                        End If
                    End Function) _
            .ToArray
        Dim uniques As String() = base.makeNames(allId, unique:=True, allow_:=True)

        Return uniques
    End Function
End Module

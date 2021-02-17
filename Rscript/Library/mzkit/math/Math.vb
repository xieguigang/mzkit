#Region "Microsoft.VisualBasic::a2665978047d4d7726121b408fdef676, Library\mzkit\math\Math.vb"

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

    ' Module MzMath
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: centroid, createTolerance, exact_mass, GetClusters, ms1Scans
    '               mz, mz_deco, mz_groups, MzUnique, peaktable
    '               ppm, printMzTable, sequenceOrder, SpectrumTreeCluster, SSMCompares
    '               XICTable
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdNum = System.Math

''' <summary>
''' mass spectrometry data math toolkit
''' </summary>
<Package("math", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
Module MzMath

    Sub New()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of PrecursorInfo())(AddressOf printMzTable)

        Call REnv.Internal.Object.Converts.addHandler(GetType(PeakFeature()), AddressOf peaktable)
        Call REnv.Internal.Object.Converts.addHandler(GetType(MzGroup), AddressOf XICTable)
    End Sub

    Private Function peaktable(x As PeakFeature(), args As list, env As Environment) As dataframe
        Dim dataset = x.ToCsvDoc
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        For Each col As String() In dataset.Columns
            table.columns.Add(col(Scan0), col.Skip(1).ToArray)
        Next

        table.rownames = table.columns(NameOf(PeakFeature.xcms_id))

        Return table
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
    ''' <param name="mass"></param>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    <ExportAPI("mz")>
    Public Function mz(mass As Double, Optional mode As Object = "+") As PrecursorInfo()
        Return MzCalculator.EvaluateAll(mass, any.ToString(mode, "+")).ToArray
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
    ''' Chromatogram data deconvolution
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <ExportAPI("mz.deco")>
    <RApiReturn(GetType(PeakFeature()))>
    Public Function mz_deco(<RRawVectorArgument>
                            ms1 As Object,
                            Optional tolerance As Object = "ppm:20",
                            Optional baseline# = 0.65,
                            Optional env As Environment = Nothing) As Object

        Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1)
        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Return ms1_scans _
            .GetMzGroups(errors) _
            .DecoMzGroups(quantile:=baseline) _
            .ToArray
    End Function

    Private Function ms1Scans(ms1 As Object) As IEnumerable(Of IMs1Scan)
        If ms1 Is Nothing Then
            Return {}
        ElseIf ms1.GetType Is GetType(ms1_scan()) Then
            Return DirectCast(ms1, ms1_scan()).Select(Function(t) DirectCast(t, IMs1Scan))
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' do ``m/z`` grouping under the given tolerance
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup()))>
    Public Function mz_groups(<RRawVectorArgument> ms1 As Object, Optional tolerance As Object = "ppm:20", Optional env As Environment = Nothing) As Object
        Return ms1Scans(ms1).GetMzGroups(Math.getTolerance(tolerance, env)).ToArray
    End Function

    ''' <summary>
    ''' calculate ppm value between two mass vector
    ''' </summary>
    ''' <param name="a">mass a</param>
    ''' <param name="b">mass b</param>
    ''' <returns></returns>
    <ExportAPI("ppm")>
    Public Function ppm(<RRawVectorArgument> a As Object, <RRawVectorArgument> b As Object) As Double()
        Dim x As Double() = REnv.asVector(Of Double)(a)
        Dim y As Double() = REnv.asVector(Of Double)(b)

        Return REnv _
            .BinaryCoreInternal(Of Double, Double, Double)(x, y, Function(xi, yi) PPMmethod.PPM(xi, yi)) _
            .ToArray
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

    ''' <summary>
    ''' create spectrum tree cluster based on the spectrum to spectrum similarity comparision.
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
                                        Optional showReport As Boolean = True,
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
    ''' <param name="tree"></param>
    ''' <returns></returns>
    <ExportAPI("cluster.nodes")>
    Public Function GetClusters(tree As SpectrumTreeCluster) As SpectrumCluster()
        Return tree.PopulateClusters.ToArray
    End Function

    ''' <summary>
    ''' data pre-processing helper
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
    ''' </param>
    ''' <returns>
    ''' Peaks data in centroid mode.
    ''' </returns>
    <ExportAPI("centroid")>
    <RApiReturn(GetType(PeakMs2), GetType(LibraryMatrix))>
    Public Function centroid(<RRawVectorArgument> ions As Object,
                             Optional intoCutoff As Double = 0.05,
                             Optional tolerance As Object = "da:0.1",
                             Optional parallel As Boolean = False,
                             Optional env As Environment = Nothing) As Object

        Dim inputType As Type = ions.GetType
        Dim errors = getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim threshold As LowAbundanceTrimming = New RelativeIntensityCutoff(intoCutoff)

        If inputType Is GetType(pipeline) OrElse inputType Is GetType(PeakMs2()) Then
            Dim source As IEnumerable(Of PeakMs2) = If(inputType Is GetType(pipeline), DirectCast(ions, pipeline).populates(Of PeakMs2)(env), DirectCast(ions, PeakMs2()))
            Dim converter = Iterator Function() As IEnumerable(Of PeakMs2)
                                For Each peak As PeakMs2 In source
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
            Dim mz As Double()
            Dim into As Double()
            Dim data As dataframe = DirectCast(ions, dataframe)

            If data.hasName("mz") Then
                mz = REnv.asVector(Of Double)(data!mz)
            ElseIf data.hasName("m/z") Then
                mz = REnv.asVector(Of Double)(data("m/z"))
            Else
                Return Internal.debug.stop("mz column in dataframe should be 'mz' or 'm/z'!", env)
            End If

            If data.hasName("into") Then
                into = REnv.asVector(Of Double)(data!into)
            ElseIf data.hasName("intensity") Then
                into = REnv.asVector(Of Double)(data!intensity)
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
                                    .intensity = into(i),
                                    .quantity = .intensity
                                }
                            End Function) _
                    .ToArray
            }

            Return ms2.CentroidMode(errors, threshold)
        Else
            Return Internal.debug.stop(New InvalidCastException(inputType.FullName), env)
        End If
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

        Dim methodVec As String() = REnv.asVector(Of String)(method)

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
End Module

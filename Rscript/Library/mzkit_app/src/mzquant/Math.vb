#Region "Microsoft.VisualBasic::e2433a5c6b909d4cfb113b6ffc60db4a, Rscript\Library\mzkit_app\src\mzquant\Math.vb"

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

    '   Total Lines: 383
    '    Code Lines: 269 (70.23%)
    ' Comment Lines: 67 (17.49%)
    '    - Xml Docs: 86.57%
    ' 
    '   Blank Lines: 47 (12.27%)
    '     File Size: 15.86 KB


    ' Module QuantifyMath
    ' 
    '     Function: asChromatogram, combineVector, GetPeakROIList, impute_f, impute_knn
    '               MapSampleNames, mergeTables, removes_missing, resample
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LCMS.Preprocessing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("math")>
Module QuantifyMath

    ''' <summary>
    ''' Do resample of the chromatogram data
    ''' </summary>
    ''' <param name="TIC"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    <ExportAPI("resample")>
    Public Function resample(TIC As ChromatogramTick(),
                             Optional dt As Double = 1,
                             Optional aggregate As RFunction = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim fun As Func(Of IEnumerable(Of Double), Double) = Nothing

        If Not aggregate Is Nothing Then
            fun = Function(v) As Double
                      Dim val As Object = aggregate.Invoke(env, {New InvokeParameter(runtimeValue:=v.ToArray)})

                      If Program.isException(val) Then
                          Throw DirectCast(val, Message).ToCLRException
                      Else
                          Return CLRVector.asNumeric(val).First
                      End If
                  End Function
        End If

        Dim signal As New Signal(TIC)
        Dim reshape = New BinSampler(signal).AggregateSignal(dt, fun)
        Dim outVal = reshape _
            .Select(Function(t) New ChromatogramTick(t)) _
            .ToArray

        Return outVal
    End Function

    ''' <summary>
    ''' data matrix pre-processing before run data analysis
    ''' </summary>
    ''' <param name="x">the lcms expression data matrix</param>
    ''' <param name="sampleinfo">sample metadata for run batch correction and normalization</param>
    ''' <param name="impute">Determines the algorithm used to fill in missing values in the expression matrix. 
    ''' Common methods include HalfMin (replacing with half of the minimum positive value of the feature), 
    ''' KNN, PCA, etc. The choice of method impacts downstream statistical sensitivity and variance.</param>
    ''' <param name="normalize">Specifies the algorithm used to adjust for systematic variations in total signal intensity 
    ''' across samples, making them comparable. Options typically include TotalIonCount (TIC), PQN, 
    ''' or internal standard normalization.</param>
    ''' <param name="batch">Specifies the algorithm used to remove non-biological systematic variation (batch effects) 
    ''' between different analytical runs, ensuring that observed differences are biological rather than technical.</param>
    ''' <param name="knn">In K-Nearest Neighbors (KNN) imputation, missing values are estimated by averaging the 
    ''' values of the K most similar samples (based on Euclidean distance or other metrics). A smaller K 
    ''' may introduce noise, while a larger K may oversmooth local variations.</param>
    ''' <param name="max_missing_ratio">Features (metabolites) with extremely high missing rates provide little reliable information 
    ''' and can severely distort imputation. This sets the threshold (0.0 to 1.0) for removing such features 
    ''' before downstream analysis to ensure statistical robustness.</param>
    ''' <param name="pca">Probabilistic PCA or NIPALS-based PCA imputation reconstructs missing values using the 
    ''' specified number of principal components. This determines the dimensionality of the subspace 
    ''' used to capture the variance in the data for accurate estimation.</param>
    ''' <param name="pca_eps">Defines the threshold for the change in estimated values between iterations. The algorithm 
    ''' stops when the change falls below this tolerance, indicating that the principal components have 
    ''' successfully converged.</param>
    ''' <param name="pqn">Probabilistic Quotient Normalization (PQN) requires a reference spectrum to calculate 
    ''' normalization quotients. This parameter determines whether the median or mean spectrum across 
    ''' all samples (or QC samples) is used as that reference. The reference sample selection method for PQN normalization: "median" or "mean".</param>
    ''' <param name="loess_span">Local Polynomial Regression (LOESS/LOWESS) span controls the proportion of data points 
    ''' used for each local fit. A larger span produces smoother curves (higher bias, lower variance), 
    ''' while a smaller span captures more local signal drift (lower bias, higher variance).</param>
    ''' <param name="loess_degree">Defines the degree of the polynomial fitted locally by LOESS. Typically set to 1 (linear) 
    ''' or 2 (quadratic). A degree of 2 is common as it captures curvilinear trends in instrumental 
    ''' signal drift across injection order.</param>
    ''' <param name="combat_parametric">In the ComBat algorithm, parametric empirical Bayes assumes batch effect parameters follow 
    ''' a normal distribution, providing more robust shrinkage estimates, especially for small sample sizes. 
    ''' Non-parametric (False) uses a non-parametric prior, which can be better if the normality assumption 
    ''' is severely violated.</param>
    ''' <param name="svr_c">In Support Vector Regression (SVR), C determines the trade-off between model flatness and 
    ''' tolerance for deviations larger than epsilon. A larger C penalizes errors more heavily, leading to 
    ''' a more complex model that fits the training data closely.</param>
    ''' <param name="svr_eps">Defines the epsilon-tube within which no penalty is associated with predicted errors. 
    ''' Errors within this margin are ignored, which helps in creating a sparse model and smoothing 
    ''' out minor noise when modeling signal drift.</param>
    ''' <param name="svr_gamma">Controls the influence of a single training example in the Radial Basis Function (RBF) kernel. 
    ''' A larger gamma means a narrower Gaussian function, leading to a more complex, tighter fit to the 
    ''' training data (potential overfitting), while a smaller gamma yields a broader, smoother fit.</param>
    ''' <param name="svr_learn_rate">Determines the step size at each iteration while moving toward a minimum of the loss function 
    ''' in gradient-based SVR optimization. A learning rate that is too large may overshoot the minimum, 
    ''' while one that is too small may lead to extremely slow convergence.</param>
    ''' <param name="iteration">
    ''' Caps the number of iterations for the SVR optimization solver (e.g., Sequential Minimal Optimization). 
    ''' Prevents the algorithm from running indefinitely on non-converging datasets.
    ''' or The Nonlinear Iterative Partial Least Squares (NIPALS) algorithm iteratively computes 
    ''' principal components. This parameter sets an upper bound on iterations to prevent infinite loops 
    ''' if the algorithm fails to converge.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the normalized expression data matrix. also with two attribute tagged:
    ''' 
    ''' 1. ``result`` is the raw clr object of the pre-processing <see cref="PreprocessingResult"/> inside mzkit
    ''' 2. ``opts`` is the parameter <see cref="PreprocessingOptions"/> that input this function
    ''' </returns>
    <ExportAPI("preprocessing")>
    <RApiReturn(GetType(PeakSet))>
    Public Function impute_f(x As PeakSet,
                             <RRawVectorArgument(GetType(SampleInfo))>
                             Optional sampleinfo As Object = Nothing,
                             Optional impute As MissingValueMethod = MissingValueMethod.HalfMin,
                             Optional normalize As NormalizationMethod = NormalizationMethod.TotalIonCount,
                             Optional batch As BatchCorrectionMethod = BatchCorrectionMethod.None,
                             Optional knn As Integer = 5,
                             Optional max_missing_ratio As Double = 0.85,
                             Optional pca As Integer = 5,
                             Optional pca_eps As Double = 0.00000001,
                             <RRawVectorArgument(TypeCodes.string)>
                             Optional pqn As Object = "median|mean",
                             Optional loess_span As Double = 0.75,
                             Optional loess_degree As Integer = 2,
                             Optional combat_parametric As Boolean = True,
                             Optional svr_c As Double = 1,
                             Optional svr_eps As Double = 0.1,
                             Optional svr_gamma As Double = 0.1,
                             Optional svr_learn_rate As Double = 0.01,
                             Optional iteration As Integer = 1000,
                             Optional env As Environment = Nothing) As Object

        Dim pull_samples As pipeline = pipeline.TryCreatePipeline(Of SampleInfo)(sampleinfo, env:=env, nullPipe:=True)

        If pull_samples IsNot Nothing AndAlso pull_samples.isError Then
            Return pull_samples.getError
        End If

        Dim opts As New PreprocessingOptions With {
            .MissingValueMethod = impute,
            .EnableMissingValueFilter = False,
            .KNN_K = knn,
            .QCLabel = "QC",
            .MaxMissingRate = max_missing_ratio,
            .NormalizationMethod = normalize,
            .BatchCorrectionMethod = batch,
            .ComBat_Parametric = combat_parametric,
            .LOESS_Degree = loess_degree,
            .LOESS_Span = loess_span,
            .PCA_Components = pca,
            .PCA_MaxIterations = iteration,
            .PCA_Tolerance = pca_eps,
            .PQN_ReferenceType = CLRVector.asScalarCharacter(pqn),
            .SVR_C = svr_c,
            .SVR_Epsilon = svr_eps,
            .SVR_Gamma = svr_gamma,
            .SVR_LearningRate = svr_learn_rate,
            .SVR_MaxIterations = iteration
        }
        Dim pipe As New LCMSPreprocessor(opts)
        Dim sampleMeta As SampleInfo() = If(pull_samples Is Nothing, Nothing, pull_samples.populates(Of SampleInfo)(env).ToArray)
        Dim result As PreprocessingResult = pipe.Process(x.AsEnumerable.ToArray, sampleMeta)
        Dim result_ions As xcms2() = result.ProcessedIons

        ' 20260531 z-score normalized and pareto scaling
        ' always produce negative or zero value
        ' skip of these methods
        If normalize <> NormalizationMethod.ParetoScaling AndAlso x.sampleNames.TryCount > 1 Then
            Call $"run peak expression table post processing on {result_ions.Length} processed ions...".debug

            ' 20260604 median scaling is used for post processing of the peak expression table, which can make the data more comparable across samples and reduce the influence of extreme values.
            ' we needs to skip of median scale processing for that expression matrix data that contains only one sample, otherwise the median scale will make all values to 1, which is not expected.
            For i As Integer = 0 To result_ions.Length - 1
                result_ions(i) = result_ions(i).FillMissing.MedianScale
            Next
        End If

        Dim processed As New PeakSet With {
            .peaks = result_ions
        }
        Dim obj As New vbObject(processed)

        Call obj.setAttribute("result", result)
        Call obj.setAttribute("opts", opts)

        Return obj
    End Function

    <ExportAPI("preprocessing.knn")>
    Public Function impute_knn(x As PeakSet, Optional scale As Double = 10 ^ 8, Optional k As Integer = 3) As PeakSet
        Dim samples As Dictionary(Of String, Double()) = x.sampleNames _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Return x.SampleVector(name)
                          End Function)
        ' find knn for each sample
        Dim ksamples = samples.AsParallel _
            .Select(Function(name)
                        Dim sample As Double() = name.Value
                        Dim sortDist = samples _
                            .Where(Function(other) other.Key <> name.Key) _
                            .OrderBy(Function(other) sample.EuclideanDistance(other.Value)) _
                            .Take(k) _
                            .ToArray

                        Return (name, KNN:=sortDist)
                    End Function) _
            .ToArray
        Dim peaks As xcms2() = x.peaks.Select(Function(clone) New xcms2(clone)).ToArray

        ' fill by knn
        For Each sample In TqdmWrapper.Wrap(ksamples)
            Dim v As Double() = sample.name.Value
            Dim m As Double()() = sample.KNN.Select(Function(a) a.Value).ToArray
            Dim sample_name As String = sample.name.Key

            For i As Integer = 0 To v.Length - 1
                Dim offset As Integer = i

                If v(i) <= 0 OrElse v(i).IsNaNImaginary Then
                    Dim impute As Double() = m _
                        .Select(Function(si) si(offset)) _
                        .Where(Function(si) si > 0 AndAlso Not si.IsNaNImaginary) _
                        .ToArray

                    If impute.Length = 0 Then
                        v(i) = randf(0.5, 1)
                    Else
                        v(i) = impute.Average
                    End If
                End If

                ' update peakset matrix
                peaks(i)(sample_name) = v(i)
            Next
        Next

        Dim norm As xcms2() = xcms2.TotalPeakSum(peaks, scale).ToArray

        Return New PeakSet(peaks) With {
            .annotations = x.annotations
        }
    End Function

    ''' <summary>
    ''' removes the missing peaks
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="sampleinfo">a sample info data vector for provides the sample group information about each sample. 
    ''' if this parameter value is omit missing then the missing feature will be checked across all sample files, 
    ''' otherwise the missing will be check across the multiple sample groups</param>
    ''' <param name="percent">the missing percentage threshold</param>
    ''' <returns></returns>
    <ExportAPI("removes_missing")>
    Public Function removes_missing(x As PeakSet,
                                    Optional sampleinfo As SampleInfo() = Nothing,
                                    Optional percent As Double = 0.5) As PeakSet

        If sampleinfo.IsNullOrEmpty Then
            ' check missing based on all samples
            Dim peaks As New List(Of xcms2)
            Dim sampleNames As String() = x.peaks.Select(Function(k) k.Properties.Keys).IteratesALL.Distinct.ToArray

            For Each peak As xcms2 In x.peaks
                Dim missing As Integer = Aggregate xi In peak.Properties Where xi.Value <= 0 Into Count()

                If (missing / sampleNames.Length) < percent Then
                    Call peaks.Add(peak)
                End If
            Next

            Return New PeakSet With {.peaks = peaks.ToArray}
        Else
            ' check missing based on sample groups
            Dim peaks As New List(Of xcms2)
            Dim groups = sampleinfo _
                .GroupBy(Function(k) k.sample_info) _
                .ToDictionary(Function(k) k.Key,
                              Function(k)
                                  Return k.Select(Function(s) s.ID).ToArray
                              End Function)

            For Each peak As xcms2 In x.peaks
                Dim missing As Integer() = groups _
                    .Select(Function(group)
                                Return Aggregate xi As Double
                                       In peak(group.Value)
                                       Where xi <= 0
                                       Into Count()
                            End Function) _
                    .ToArray

                ' has one group contains value more than
                ' the given missing percentage cutoff
                ' then we keeps this feature
                If missing.Any(Function(m) m / sampleinfo.Length < percent) Then
                    Call peaks.Add(peak)
                End If
            Next

            Return New PeakSet With {.peaks = peaks.ToArray}
        End If
    End Function

    ''' <summary>
    ''' Take the top peaks
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' sort the peaks via samples in desc order and then take the top n peaks data
    ''' </remarks>
    <ExportAPI("top_peaks")>
    <RApiReturn(GetType(xcms2), GetType(PeakSet))>
    Public Function top(<RRawVectorArgument> x As Object, Optional n As Integer = 20000, Optional env As Environment = Nothing) As Object
        If TypeOf x Is PeakSet Then
            Dim topTable As New PeakSet(From ion As xcms2
                                        In DirectCast(x, PeakSet).peaks
                                        Order By ion.npeaks Descending
                                        Take n)
            Return topTable
        Else
            Dim ions As pipeline = pipeline.TryCreatePipeline(Of xcms2)(x, env)

            If ions.isError Then
                Return ions.getError
            End If

            Return (From ion As xcms2
                    In ions.populates(Of xcms2)(env)
                    Order By ion.npeaks Descending
                    Take n).ToArray
        End If
    End Function

    ''' <summary>
    ''' Create a chromatogram data from a dataframe object
    ''' </summary>
    ''' <param name="x">Should be a dataframe object that contains 
    ''' the required data field for construct the chromatogram data.
    ''' </param>
    ''' <param name="time">the column name for get the rt field vector data</param>
    ''' <param name="into">the column name for get the signal intensity field vector data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("chromatogram")>
    <RApiReturn(GetType(ChromatogramTick))>
    Public Function asChromatogram(<RRawVectorArgument> Optional x As Object = Nothing,
                                   <RRawVectorArgument> Optional time As Object = "Time",
                                   <RRawVectorArgument> Optional into As Object = "Intensity",
                                   Optional env As Environment = Nothing) As Object
        Dim timeVec As Double()
        Dim intoVec As Double()

        If x Is Nothing Then
            Return combineVector(
                CLRVector.asNumeric(time),
                CLRVector.asNumeric(into)
            )
        End If

        Dim time_str As String = CLRVector.asCharacter(time).First
        Dim into_str As String = CLRVector.asCharacter(into).First

        If TypeOf x Is Rdataframe Then
            timeVec = CLRVector.asNumeric(DirectCast(x, Rdataframe).getColumnVector(time_str))
            intoVec = CLRVector.asNumeric(DirectCast(x, Rdataframe).getColumnVector(into_str))
        ElseIf TypeOf x Is DataSet() Then
            timeVec = DirectCast(x, DataSet()).Vector(time_str)
            intoVec = DirectCast(x, DataSet()).Vector(into_str)
        Else
            Return RInternal.debug.stop($"invalid data sequence: {x.GetType.FullName}", env)
        End If

        Return combineVector(timeVec, intoVec)
    End Function

    Private Function combineVector(timeVec As Double(), intoVec As Double()) As ChromatogramTick()
        Return timeVec _
          .Select(Function(t, i)
                      Return New ChromatogramTick With {
                          .Time = t,
                          .Intensity = intoVec(i)
                      }
                  End Function) _
          .OrderBy(Function(ti) ti.Time) _
          .ToArray
    End Function

    ''' <summary>
    ''' ### Peak finding
    ''' 
    ''' Extract the peak ROI data from the chromatogram data
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <param name="baselineQuantile"></param>
    ''' <param name="angleThreshold"></param>
    ''' <param name="peakwidth"></param>
    ''' <param name="sn_threshold"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("peakROI")>
    <RApiReturn(GetType(ROI))>
    Public Function GetPeakROIList(<RRawVectorArgument(GetType(ChromatogramTick))>
                                   chromatogram As Object,
                                   Optional baselineQuantile# = 0.65,
                                   Optional angleThreshold# = 5,
                                   <RRawVectorArgument>
                                   Optional peakwidth As Object = "8,30",
                                   Optional sn_threshold As Double = 3,
                                   Optional joint As Boolean = False,
                                   Optional env As Environment = Nothing) As Object

        If chromatogram Is Nothing Then
            Return Nothing
        End If

        Dim _peakwidth = ApiArgumentHelpers.GetDoubleRange(peakwidth, env, "8,30")

        If _peakwidth Like GetType(Message) Then
            Return _peakwidth.TryCast(Of Message)
        End If

        Return DirectCast(REnv.asVector(Of ChromatogramTick)(chromatogram), ChromatogramTick()) _
            .Shadows _
            .PopulateROI(
                baselineQuantile:=baselineQuantile,
                angleThreshold:=angleThreshold,
                peakwidth:=_peakwidth,
                snThreshold:=sn_threshold,
                joint:=joint
            ) _
            .ToArray
    End Function

    ''' <summary>
    ''' merge all peakset tables into one peaktable object
    ''' </summary>
    ''' <param name="tables"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function merge two peaktable directly via the unique id reference
    ''' </remarks>
    <ExportAPI("merge_tables")>
    <RApiReturn(GetType(PeakSet))>
    Public Function mergeTables(<RRawVectorArgument> tables As Object, Optional env As Environment = Nothing) As Object
        Dim peaksets = pipeline.TryCreatePipeline(Of PeakSet)(tables, env)

        If peaksets.isError Then
            Return peaksets.getError
        End If

        Dim unions As New Dictionary(Of String, xcms2)
        Dim annos As New List(Of Dictionary(Of String, MetID))

        For Each part As PeakSet In peaksets.populates(Of PeakSet)(env)
            If Not part.annotations.IsNullOrEmpty Then
                Call annos.Add(part.annotations)
            End If

            For Each peak As xcms2 In part.peaks
                Dim datapeak As xcms2 = unions.TryGetValue(peak.ID)

                If datapeak Is Nothing Then
                    Call unions.Add(New xcms2(peak))
                Else
                    Call datapeak.AddSamples(peak.Properties)
                End If
            Next
        Next

        Return New PeakSet(unions.Values) With {
            .annotations = annos _
                .IteratesALL _
                .GroupBy(Function(a) a.Key) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First.Value
                              End Function)
        }
    End Function

    ''' <summary>
    ''' mapping sample id to sample names
    ''' </summary>
    ''' <param name="x">the sample id is used as the sample identifier</param>
    ''' <param name="samples">the mapping of sample id to sample name</param>
    ''' <returns>
    ''' the peaktable that use the sample name as the sample identifier.
    ''' </returns>
    <ExportAPI("map_samplenames")>
    Public Function MapSampleNames(x As PeakSet, samples As SampleInfo()) As PeakSet
        Dim mapIndex As Dictionary(Of String, String) = samples.ToDictionary(Function(a) a.ID, Function(a) a.sample_name)
        Dim mapNames = x.peaks _
            .Select(Function(xi)
                        Return MapIonFeatureSamples(xi, mapIndex)
                    End Function) _
            .ToArray

        Return New PeakSet(mapNames) With {.annotations = x.annotations}
    End Function

    Private Function MapIonFeatureSamples(xi As xcms2, mapIndex As Dictionary(Of String, String)) As xcms2
        ' 20260516
        ' handling of the pos/neg name mapping
        ' example as pos: XXX -> sample1
        '            neg: YYY -> sample1
        ' will generates two sample1       
        xi = New xcms2(xi)
        xi.Properties = xi.Properties _
            .GroupBy(Function(si)
                         Return If(mapIndex.ContainsKey(si.Key), mapIndex(si.Key), si.Key)
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              ' if the POS/NEG different sample id mapping
                              ' to one name, then it means always one sample is
                              ' zero
                              ' make sum of these two sample directly
                              Return Aggregate ai As KeyValuePair(Of String, Double)
                                     In a
                                     Into Sum(ai.Value)
                          End Function)
        Return xi
    End Function
End Module

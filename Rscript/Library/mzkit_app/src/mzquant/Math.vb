#Region "Microsoft.VisualBasic::3c70d033e41a988c3805a9683603a503, Rscript\Library\mzkit_app\src\mzquant\Math.vb"

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

    '   Total Lines: 232
    '    Code Lines: 157 (67.67%)
    ' Comment Lines: 47 (20.26%)
    '    - Xml Docs: 87.23%
    ' 
    '   Blank Lines: 28 (12.07%)
    '     File Size: 9.78 KB


    ' Module QuantifyMath
    ' 
    '     Function: asChromatogram, combineVector, GetPeakROIList, impute, removes_missing
    '               resample
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.KNearNeighbors
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
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("preprocessing")>
    Public Function impute_f(x As PeakSet,
                             Optional scale As Double = 10 ^ 8,
                             Optional impute As Imputation = Imputation.Min) As PeakSet

        Dim imputes As xcms2() = x.peaks _
            .AsParallel _
            .Select(Function(k)
                        If impute = Imputation.None Then
                            Return New xcms2(k)
                        Else
                            Return k.Impute(impute)
                        End If
                    End Function) _
            .ToArray
        Dim norm As xcms2() = xcms2.TotalPeakSum(imputes, scale).ToArray
        Dim peaktable As New PeakSet With {
            .peaks = norm,
            .annotations = x.annotations
        }

        Return peaktable
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
        Dim mapIndex = samples.ToDictionary(Function(a) a.ID)
        Dim mapNames = x.peaks _
            .Select(Function(xi)
                        xi = New xcms2(xi)
                        xi.Properties = xi.Properties _
                            .ToDictionary(Function(si)
                                              Return If(mapIndex.ContainsKey(si.Key), mapIndex(si.Key).sample_name, si.Key)
                                          End Function,
                                          Function(si)
                                              Return si.Value
                                          End Function)
                        Return xi
                    End Function) _
            .ToArray

        Return New PeakSet(mapNames) With {.annotations = x.annotations}
    End Function
End Module

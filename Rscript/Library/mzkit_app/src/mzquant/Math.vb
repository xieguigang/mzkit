
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("math")>
Module QuantifyMath

    ''' <summary>
    ''' Do resample of the chromatogram data
    ''' </summary>
    ''' <param name="TIC"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    <ExportAPI("resample")>
    Public Function resample(TIC As ChromatogramTick(), Optional dt As Double = 1) As Object
        Dim signal As New Signal(TIC)
        Dim reshape = New BinSampler(signal).AggregateSignal(dt)
        Dim aggregate = reshape _
            .Select(Function(t) New ChromatogramTick(t)) _
            .ToArray

        Return aggregate
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
            Return Internal.debug.stop($"invalid data sequence: {x.GetType.FullName}", env)
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
                snThreshold:=sn_threshold
            ) _
            .ToArray
    End Function
End Module

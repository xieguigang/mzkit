#Region "Microsoft.VisualBasic::5084739fa90910e578f8662741fe457c, G:/mzkit/Rscript/Library/mzkit_app/src/mzquant//Math.vb"

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

    '   Total Lines: 155
    '    Code Lines: 109
    ' Comment Lines: 28
    '   Blank Lines: 18
    '     File Size: 6.33 KB


    ' Module QuantifyMath
    ' 
    '     Function: asChromatogram, combineVector, GetPeakROIList, resample
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
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
End Module

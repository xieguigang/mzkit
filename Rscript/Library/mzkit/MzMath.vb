#Region "Microsoft.VisualBasic::f9d1979191d50bce3cf03d979c0d2c8e, Rscript\Library\mzkit\MzMath.vb"

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
'     Function: exact_mass, ms1Scans, mz, mz_deco, mz_groups
'               peaktable, ppm, printMzTable, XICTable
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports NumberAggregate = Microsoft.VisualBasic.Math.Scripting.Aggregate
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' mass spectrometry data math toolkit
''' </summary>
<Package("math", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
Module MzMath

    Sub New()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of MzReport())(AddressOf printMzTable)

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
        Return DirectCast(obj, MzReport()).Print(addBorder:=False)
    End Function

    ''' <summary>
    ''' evaluate all m/z for all known precursor type.
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    <ExportAPI("mz")>
    Public Function mz(mass As Double, Optional mode As Object = "+") As MzReport()
        Return MzCalculator.EvaluateAll(mass, Scripting.ToString(mode, "+")).ToArray
    End Function

    ''' <summary>
    ''' evaluate all exact mass for all known precursor type.
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    <ExportAPI("exact_mass")>
    Public Function exact_mass(mz As Double, Optional mode As Object = "+") As MzReport()
        Return MzCalculator.EvaluateAll(mz, Scripting.ToString(mode, "+"), True).ToArray
    End Function

    ''' <summary>
    ''' Chromatogram data deconvolution
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <ExportAPI("mz.deco")>
    <RApiReturn(GetType(PeakFeature()))>
    Public Function mz_deco(<RRawVectorArgument> ms1 As Object, Optional tolerance As Object = "ppm:20", Optional baseline# = 0.65, Optional env As Environment = Nothing) As Object
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
    ''' do mz grouping under the given tolerance
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
            .BinaryCoreInternal(Of Double, Double, Double)(x, y, Function(xi, yi) PPMmethod.ppm(xi, yi)) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
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
                                Optional score_aggregate As Object = "min",
                                Optional env As Environment = Nothing) As Object

        Dim errors = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Dim aggregate As Func(Of Double, Double, Double)
        Dim handler As IAggregate

        If score_aggregate Is Nothing Then
            Return Internal.debug.stop("you must specific a aggregate function!", env)
        ElseIf TypeOf score_aggregate Is String Then
            handler = NumberAggregate.GetAggregater(score_aggregate)
            aggregate = Function(x, y) handler({x, y})
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Func(Of Double, Double, Double)), score_aggregate.GetType, env), env)
        End If

        Return Spectra.SpectrumTreeCluster.SSMCompares(errors.TryCast(Of Tolerance), equals_score, gt_score, aggregate)
    End Function

    <ExportAPI("spectrum_tree.cluster")>
    <RApiReturn(GetType(SpectrumTreeCluster))>
    Public Function SpectrumTreeCluster(<RRawVectorArgument>
                                        ms2list As Object,
                                        Optional compares As Comparison(Of PeakMs2) = Nothing,
                                        Optional showReport As Boolean = True,
                                        Optional env As Environment = Nothing) As Object

        Dim spectrum As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ms2list, env)

        If spectrum.isError Then
            Return spectrum.getError
        End If

        Return New SpectrumTreeCluster(
            compares:=compares,
            showReport:=showReport
        ).doCluster(spectrum:=spectrum.populates(Of PeakMs2).ToArray)
    End Function
End Module

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("mzkit.math")>
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

    <ExportAPI("mz")>
    Public Function mz(mass As Double, Optional mode As Object = "+") As MzReport()
        Return MzCalculator.Calculate(mass, Scripting.ToString(mode, "+")).ToArray
    End Function

    ''' <summary>
    ''' Chromatogram data deconvolution
    ''' </summary>
    ''' <param name="ms1"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <ExportAPI("mz.deco")>
    <RApiReturn(GetType(PeakFeature()))>
    Public Function mz_deco(<RRawVectorArgument> ms1 As Object, Optional tolerance As Object = "ppm:20", Optional baseline# = 0.65) As Object
        Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1)
        Dim errors As Tolerance = getTolerance(tolerance)

        Return ms1_scans _
            .GetMzGroups(errors) _
            .DecoMzGroups(quantile:=baseline) _
            .ToArray
    End Function

    Private Function getTolerance(val As Object) As Tolerance
        If val Is Nothing Then
            Return Tolerance.DefaultTolerance
        ElseIf val.GetType.IsInheritsFrom(GetType(Tolerance)) Then
            Return val
        ElseIf val.GetType Is GetType(String) Then
            Return Tolerance.ParseScript(val)
        ElseIf val.GetType Is GetType(String()) Then
            Return Tolerance.ParseScript(DirectCast(val, String())(Scan0))
        ElseIf val.GetType Is GetType(vector) Then
            Return Tolerance.ParseScript(DirectCast(val, vector).data(Scan0))
        Else
            Throw New NotImplementedException
        End If
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

    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup()))>
    Public Function mz_groups(<RRawVectorArgument> ms1 As Object, Optional tolerance As Object = "ppm:20") As Object
        Return ms1Scans(ms1).GetMzGroups(getTolerance(tolerance)).ToArray
    End Function

    <ExportAPI("ppm")>
    Public Function ppm(<RRawVectorArgument> a As Object, <RRawVectorArgument> b As Object) As Double()
        Dim x As Double() = REnv.asVector(Of Double)(a)
        Dim y As Double() = REnv.asVector(Of Double)(b)

        Return REnv _
            .BinaryCoreInternal(Of Double, Double, Double)(x, y, Function(xi, yi) PPMmethod.ppm(xi, yi)) _
            .ToArray
    End Function
End Module

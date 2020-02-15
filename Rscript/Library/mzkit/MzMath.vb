Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

<Package("mzkit.math")>
Module MzMath

    Sub New()
        Call REnv.ConsolePrinter.AttachConsoleFormatter(Of MzReport())(AddressOf printMzTable)

        Call REnv.Object.Converts.addHandler(GetType(PeakFeature()), AddressOf peaktable)
    End Sub

    Private Function peaktable(x As PeakFeature(), env As Environment) As dataframe

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
    Public Function mz_deco(<RRawVectorArgument> ms1 As Object, Optional tolerance As Tolerance = Nothing, Optional baseline# = 0.65) As Object
        Dim ms1_scans As IEnumerable(Of IMs1Scan) = ms1Scans(ms1)

        Return ms1_scans _
            .GetMzGroups(tolerance) _
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

    <ExportAPI("mz.groups")>
    <RApiReturn(GetType(MzGroup()))>
    Public Function mz_groups(<RRawVectorArgument> ms1 As Object, Optional tolerance As Tolerance = Nothing) As Object
        Return ms1Scans(ms1).GetMzGroups(tolerance).ToArray
    End Function
End Module

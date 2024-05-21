
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Public Module FeatureSearchHandler

    ''' <summary>
    ''' run search in parallel with auto task schedule.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="rawdata"></param>
    ''' <param name="exact_mass"></param>
    ''' <param name="ppm"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MatchByExactMass(Of T As ISpectrumScanData)(rawdata As IEnumerable(Of NamedValue(Of T())),
                                                                         exact_mass As Double,
                                                                         ppm As Tolerance) As IEnumerable(Of NamedCollection(Of ParentMatch))
        Dim pool = rawdata.SafeQuery.ToArray

        If pool.Length = 1 Then
            Dim adducts = GetAdductsTuple(exact_mass)
            Dim source As String = pool(0).Name
            Dim find = pool(0).Value _
                .AsParallel _
                .Select(Function(scan) scan.MatchByExactMass(adducts, source, ppm)) _
                .IteratesALL _
                .ToArray

            Yield New NamedCollection(Of ParentMatch)(source, find)
        Else
            For Each find As NamedCollection(Of ParentMatch) In pool _
                .AsParallel _
                .Select(Function(file)
                            Return New NamedCollection(Of ParentMatch)(file.Name, file.Value.MatchByExactMass(exact_mass, file.Name, ppm))
                        End Function)

                Yield find
            Next
        End If
    End Function

    Private Function GetAdductsTuple(exact_mass As Double) As PolarityData(Of PrecursorInfo())
        Return New PolarityData(Of PrecursorInfo())(
            pos:=MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray,
            neg:=MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="exact_mass"></param>
    ''' <param name="raw">
    ''' the rawdata spectrum collection which is extract from the rawdata file.
    ''' </param>
    ''' <param name="source">the source file name of the <paramref name="raw"/></param>
    ''' <param name="ppm"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function MatchByExactMass(Of T As ISpectrumScanData)(raw As IEnumerable(Of T),
                                                                         exact_mass As Double,
                                                                         source As String,
                                                                         ppm As Tolerance) As IEnumerable(Of ParentMatch)
        ' C25H40N4O5
        Dim adducts = GetAdductsTuple(exact_mass)

        For Each scan As ISpectrumScanData In raw
            For Each match As ParentMatch In scan.MatchByExactMass(adducts, source, ppm)
                Yield match
            Next
            ' Call System.Windows.Forms.Application.DoEvents()
        Next
    End Function

    <Extension>
    Private Iterator Function MatchByExactMass(scan As ISpectrumScanData, adducts As PolarityData(Of PrecursorInfo()),
                                               source As String,
                                               ppm As Tolerance) As IEnumerable(Of ParentMatch)

        For Each mode As PrecursorInfo In adducts(scan.Polarity)
            If ppm(scan.mz, Val(mode.mz)) Then
                Yield New ParentMatch With {
                    .BPC = scan.PeaksIntensity.Max,
                    .TIC = scan.PeaksIntensity.Sum,
                    .M = mode.M,
                    .adducts = mode.adduct,
                    .precursor_type = mode.precursor_type,
                    .ppm = PPMmethod.PPM(scan.mz, Val(mode.mz)).ToString("F0"),
                    .XIC = scan.intensity,
                    .rawfile = source,
                    .da = std.Round(std.Abs(scan.mz - Val(mode.mz)), 3),
                    .scan = scan
                }
            End If
        Next
    End Function
End Module

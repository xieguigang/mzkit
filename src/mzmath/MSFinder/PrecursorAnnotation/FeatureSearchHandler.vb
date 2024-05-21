
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

        End If
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
    Public Iterator Function MatchByExactMass(Of T As ISpectrumScanData)(raw As IEnumerable(Of T), exact_mass As Double, source As String, ppm As Tolerance) As IEnumerable(Of ParentMatch)
        ' C25H40N4O5
        Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
        Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray
        Dim info As PrecursorInfo()

        For Each scan As ISpectrumScanData In raw
            If scan.Polarity > 0 Then
                info = pos
            Else
                info = neg
            End If

            For Each mode As PrecursorInfo In info
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

            ' Call System.Windows.Forms.Application.DoEvents()
        Next
    End Function

    <Extension>
    Private Iterator Function MatchByExactMass(scan As ISpectrumScanData, adducts As PolarityData(Of PrecursorInfo()),
                                               source As String,
                                               exact_mass As Double,
                                               ppm As Tolerance) As IEnumerable(Of ParentMatch)

    End Function
End Module

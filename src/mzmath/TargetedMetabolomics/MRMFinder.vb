
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports std = System.Math

''' <summary>
''' Development of a plasma pseudotargeted metabolomics method based on ultra-high-performance liquid chromatography-mass spectrometry
''' </summary>
Public Module MRMFinder

    ''' <summary>
    ''' https://github.com/zhengfj1994/MRM-Ion_Pair_Finder
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <param name="diff_MS2MS1">
    ''' The smallest difference between product ion and precusor ion. 
    ''' Mass tolerance error in delta dalton, not ppm error.
    ''' </param>
    ''' <param name="ms2_intensity">
    ''' The smallest intensity of product ion. this parameter is a 
    ''' relatve intensity cutoff, value should be between [0,1].
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MRMIonPairFinder(ms2 As IEnumerable(Of PeakMs2), ms1 As Ms1Feature(),
                                              Optional diff_MS2MS1 As Double = 0.05,
                                              Optional ms2_intensity As Double = 0.05) As IEnumerable(Of IonPair)
        Dim pool As PeakMs2() = ms2.ToArray

        For Each target As Ms1Feature In ms1.SafeQuery
            Dim ions = ms2 _
                .Where(Function(i) std.Abs(i.mz - target.mz) <= 0.1) _
                .Where(Function(i) std.Abs(i.rt - target.rt) < 6) _
                .ToArray
            Dim search = ions _
                .Select(Function(i) i.MRMIonPairFinder(diff_MS2MS1, ms2_intensity)) _
                .Where(Function(a) Not a Is Nothing) _
                .ToArray

            If search.IsNullOrEmpty Then
                Continue For
            End If

            Dim product = search _
                .GroupBy(Function(i) std.Round(i.product, 2), offsets:=diff_MS2MS1) _
                .OrderByDescending(Function(a)
                                       Return a.Count()
                                   End Function) _
                .First

            Yield New IonPair With {
                .accession = target.ID,
                .name = product.Select(Function(i) i.name).Distinct.JoinBy("; "),
                .precursor = target.mz,
                .rt = target.rt,
                .product = product _
                    .Average(Function(i)
                                 Return i.product
                             End Function)
            }
        Next
    End Function

    ''' <summary>
    ''' https://github.com/zhengfj1994/MRM-Ion_Pair_Finder
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <param name="diff_MS2MS1">
    ''' The smallest difference between product ion and precusor ion. 
    ''' Mass tolerance error in delta dalton, not ppm error.
    ''' </param>
    ''' <param name="ms2_intensity">
    ''' The smallest intensity of product ion. this parameter is a 
    ''' relatve intensity cutoff, value should be between [0,1].
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MRMIonPairFinder(ms2 As IEnumerable(Of PeakMs2),
                                              Optional diff_MS2MS1 As Double = 0.05,
                                              Optional ms2_intensity As Double = 0.05) As IEnumerable(Of IonPair)

        For Each spectrum As PeakMs2 In ms2.SafeQuery
            Dim ionpair As IonPair = spectrum.MRMIonPairFinder(diff_MS2MS1, ms2_intensity)

            If Not ionpair Is Nothing Then
                Yield ionpair
            End If
        Next
    End Function

    <Extension>
    Public Function MRMIonPairFinder(ms2 As PeakMs2, Optional diff_MS2MS1 As Double = 0.05, Optional ms2_intensity As Double = 0.05) As IonPair
        ' removes noise and precursors
        Dim products = ms2.mzInto _
            .Where(Function(m) std.Abs(m.mz - ms2.mz) > diff_MS2MS1) _
            .ToArray

        products = New RelativeIntensityCutoff(ms2_intensity).Trim(products)

        If products.IsNullOrEmpty Then
            Return Nothing
        End If

        ' use the highest intensity fragment as MRM Q3
        Dim top = products.OrderByDescending(Function(m) m.intensity).First
        Dim ion As New IonPair With {
            .accession = ms2.lib_guid,
            .name = ms2.meta.TryGetValue("name"),
            .precursor = ms2.mz,
            .product = top.mz,
            .rt = ms2.rt
        }

        Return ion
    End Function

End Module

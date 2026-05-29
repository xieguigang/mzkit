Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Module Module3

    Sub Main()
        Dim test1 As ms2() = Enumerable.Range(50, 500).Select(Function(i) New ms2(i * randf.NextDouble + 50, randf.NextDouble)).OrderBy(Function(i) i.mz).ToArray
        Dim test2 As ms2() = Enumerable.Range(50, 2000).Select(Function(i) New ms2(i * randf.NextDouble + 50, randf.NextDouble)).OrderBy(Function(i) i.mz).ToArray
        Dim test3 As ms2() = Enumerable.Range(50, 12000).Select(Function(i) New ms2(i * randf.NextDouble + 50, randf.NextDouble)).OrderBy(Function(i) i.mz).ToArray
        Dim pixels As Integer = 1200 * 950
        Dim mz As Double = test3(randf.NextInteger(test1.Length - 1)).mz
        Dim ppm20 As Tolerance = Tolerance.PPM(20)

        Call VBDebugger.benchmark(Sub() matchesBinary(test1, pixels, mz, ppm20))
        Call VBDebugger.benchmark(Sub() matchesOrdinal(test1, pixels, mz, ppm20))

        Call VBDebugger.benchmark(Sub() matchesBinary(test2, pixels, mz, ppm20))
        Call VBDebugger.benchmark(Sub() matchesOrdinal(test2, pixels, mz, ppm20))

        Call VBDebugger.benchmark(Sub() matchesBinary(test3, pixels, mz, ppm20))
        Call VBDebugger.benchmark(Sub() matchesOrdinal(test3, pixels, mz, ppm20))
    End Sub

    Private Sub matchesOrdinal(ms1 As ms2(), pixels As Integer, mz As Double, mzdiff As Tolerance)
        Call "matches ordinal...".warning
        Dim search = Enumerable.Range(0, pixels).ToArray.AsParallel.Select(Function(any)
                                                                               Return ms1.Where(Function(a) mzdiff(a.mz, mz)).FirstOrDefault
                                                                           End Function).ToArray
        Dim into = search.Where(Function(a) Not a Is Nothing).ToArray

        If into.Length = 0 Then
            Call "no matches".info
        Else
            Call Console.WriteLine(into(0).intensity)
        End If
    End Sub

    Private Sub matchesBinary(ms1 As ms2(), pixels As Integer, mz As Double, mzdiff As Tolerance)
        Call "matches binary...".warning
        Dim q As New ComparesMz(mzdiff)
        Dim search = Enumerable.Range(0, pixels).ToArray.AsParallel.Select(Function(any)
                                                                               Return ms1.BinarySearch(mz, q)
                                                                           End Function).ToArray
        Dim into = search.Where(Function(a) Not a Is Nothing).ToArray

        If into.Length = 0 Then
            Call "no matches".info
        Else
            Call Console.WriteLine(into(0).intensity)
        End If
    End Sub
End Module

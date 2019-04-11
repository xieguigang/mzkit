Imports SMRUCC.MassSpectrum.Math.Spectra

Module Module2

    Sub Main()
        Dim ref = New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 1, .quantity = 1}, New ms2 With {.mz = 200, .intensity = 0, .quantity = 0}},
            .Name = "Library"
        }

        Dim A As New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 0.8, .quantity = 0.8}, New ms2 With {.mz = 200, .intensity = 0.05, .quantity = 0.05}},
            .Name = NameOf(A)
        }

        Dim B As New LibraryMatrix With {
            .ms2 = {New ms2 With {.mz = 100, .intensity = 0.2, .quantity = 0.2}, New ms2 With {.mz = 200, .intensity = 0.98, .quantity = 0.98}},
            .Name = NameOf(B)
        }

        Call SMRUCC.MassSpectrum.Visualization.AlignMirrorPlot(A, ref).Save("./A.png")
        Call SMRUCC.MassSpectrum.Visualization.AlignMirrorPlot(B, ref).Save("./B.png")
    End Sub
End Module

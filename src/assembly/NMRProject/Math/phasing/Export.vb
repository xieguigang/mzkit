Namespace fidMath.Phasing

    Public Module Export

        Public Function phasecorrection(spectrumRaw As Double(), teta0 As Double, teta1 As Double, pivot As Integer) As Double()
            Dim spectrum = New Double(spectrumRaw.Length / 2 - 1) {}
            Console.WriteLine(teta0 + teta1 * (0 - pivot) / spectrum.Length)
            For i = 0 To spectrum.Length - 1
                spectrum(i) = spectrumRaw(i * 2) * Math.Cos(teta0 + teta1 * (i - pivot) / spectrum.Length) + spectrumRaw(i * 2 + 1) * Math.Sin(teta0 + teta1 * (i - pivot) / spectrum.Length)
            Next
            Return spectrum
        End Function
    End Module
End Namespace
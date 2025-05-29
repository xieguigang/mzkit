Imports BioNovoGene.Analytical.MassSpectrometry.Math

Module formulaTest

    Sub Main()
        Dim formulas = New String() {""}

        For Each str As String In formulas
            Call Console.WriteLine(AnthocyaninValidator.CalculateProbability(str))
        Next

        Pause()
    End Sub


End Module

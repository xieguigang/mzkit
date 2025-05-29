Imports BioNovoGene.Analytical.MassSpectrometry.Math

Module formulaTest

    Sub Main()
        Dim formulas = New String() {"[C21H21O12]+"}

        For Each str As String In formulas
            Call Console.WriteLine(str & vbTab & AnthocyaninValidator.CalculateProbability(str))
        Next

        Pause()
    End Sub


End Module

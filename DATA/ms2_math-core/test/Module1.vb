Imports System.IO
Imports System.Text
Imports SMRUCC.MassSpectrum.Math.Ms1.PrecursorType

Module Module1

    Sub Main()

        Dim mass = 853.33089

        Dim mz = Provider.Positive("2M+H").CalcMZ(mass)


        Dim html As New StringBuilder

        Using dev As New StringWriter(html)
            Call MzCalculator.CalculateMode(mass, "-").PrintTable(dev)
        End Using

        Dim display As String = html.ToString

        Call display.SaveTo("./test.html")

        Pause()
    End Sub
End Module

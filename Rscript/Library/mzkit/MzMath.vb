Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

<Package("mzkit.math")>
Module MzMath

    Sub New()
        Call REnv.AttachConsoleFormatter(Of MzReport())(AddressOf printMzTable)
    End Sub

    Private Function printMzTable(obj As Object) As String
        Return DirectCast(obj, MzReport()).Print(addBorder:=False)
    End Function

    <ExportAPI("mz")>
    Public Function mz(mass As Double, Optional mode As Object = "+") As MzReport()
        Return MzCalculator.Calculate(mass, Scripting.ToString(mode, "+")).ToArray
    End Function
End Module

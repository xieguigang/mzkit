Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' 
''' </summary>
<Package("mzkit.formula", Category:=APICategories.UtilityTools)>
Module Formula

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
    End Sub

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
    End Function

    <ExportAPI("scan.formula")>
    Public Function ScanFormula(formula$, Optional n% = 9999) As FormulaComposition
        Return FormulaScanner.ScanFormula(formula, n)
    End Function
End Module

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.DATA.MetaLib

''' <summary>
''' 
''' </summary>
<Package("mzkit.formula", Category:=APICategories.UtilityTools)>
Module Formula

    <ExportAPI("scan.formula")>
    Public Function ScanFormula(formula As String) As FormulaComposition
        Return FormulaScanner.ScanFormula(formula)
    End Function
End Module

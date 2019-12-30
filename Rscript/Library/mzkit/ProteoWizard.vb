
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' ProteoWizard helper
''' 
''' You should config the bin path of ProteoWizard at first 
''' by using ``options`` function
''' </summary>
<Package("ProteoWizard", Category:=APICategories.UtilityTools)>
Module ProteoWizard

    ''' <summary>
    ''' Convert MRM wiff file to mzMl files
    ''' </summary>
    ''' <param name="wiff">The file path of the wiff file</param>
    ''' <returns>
    ''' File path collection of the converted mzML files.
    ''' </returns>
    <ExportAPI("MRM.mzML")>
    Public Function wiffMRM(wiff As String) As Object

    End Function
End Module

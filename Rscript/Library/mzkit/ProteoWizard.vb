
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ProteoWizard.Interop

''' <summary>
''' ProteoWizard helper
''' 
''' You should config the bin path of ProteoWizard at first 
''' by using ``options`` function
''' </summary>
<Package("ProteoWizard", Category:=APICategories.UtilityTools)>
Module ProteoWizard

    ''' <summary>
    ''' Is the ``ProteoWizard`` program ready to use?
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("msconvert.ready")>
    Public Function Ready() As Boolean
        Return ProteoWizardCLI.BIN.FileExists(True)
    End Function

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

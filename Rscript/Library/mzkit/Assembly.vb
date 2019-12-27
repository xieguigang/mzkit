
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MGF

''' <summary>
''' The mass spectrum assembly file read/write library module.
''' </summary>
<Package("mzkit.assembly", Category:=APICategories.UtilityTools)>
Module Assembly

    <ExportAPI("read.mgf")>
    Public Function ReadMgfIons(file As String) As Ions()
        Return MgfReader.StreamParser(path:=file).ToArray
    End Function


End Module

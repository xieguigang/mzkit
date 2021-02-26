Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' 已经成功进行注释的代谢物信息(作为MetaDNA推断的种子)
''' </summary>
Public Class AnnotatedSeed : Implements INamedValue

    Public Property kegg_id As String
    Public Property id As String Implements INamedValue.Key
    Public Property parent As ms1_scan
    Public Property products As Dictionary(Of String, LibraryMatrix)

    Public Overrides Function ToString() As String
        Return $"{id}: {kegg_id}"
    End Function

End Class

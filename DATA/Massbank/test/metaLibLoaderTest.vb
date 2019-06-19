Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Module metaLibLoaderTest

    Sub Main()

        Dim libs = Data.LoadUltraLargeXMLDataSet(Of MetaLib)(path:="D:\Database\CID-Synonym-filtered\CID-Synonym-filtered.metlib_kegg.Xml").ToArray

        Dim libs2 = "D:\Database\CID-Synonym-filtered\CID-Synonym-filtered.metlib_cas.Xml".LoadXml(Of DataSetOfMetaLib)


        Pause()
    End Sub
End Module

Public Class DataSetOfMetaLib

    <XmlElement>
    Public Property MetaLib As MetaLib()

End Class

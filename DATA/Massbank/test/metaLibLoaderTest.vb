Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib

Module metaLibLoaderTest

    Sub Main()

        Dim libs = Data.LoadUltraLargeXMLDataSet(Of MetaLib)(path:="D:\Database\CID-Synonym-filtered\CID-Synonym-filtered.metlib_hmdb.Xml").ToArray


        Pause()
    End Sub
End Module

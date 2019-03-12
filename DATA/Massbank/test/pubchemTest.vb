Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.DATA.NCBI
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module pubchemTest

    Sub Main()

        Call fileTest()


        Dim cas$ = "345909-34-4"
        Dim result = PubChem.Query.QueryPugViews(cas)

        Pause()
    End Sub

    Sub fileTest()
        Dim file = "D:\MassSpectrum-toolkits\DATA\DATA\pubchem\CID_5957.xml"
        Dim xml = file.LoadXml(Of PugViewRecord)

        Call xml.GetJson.SaveTo(file.ChangeSuffix("json"))

        Dim meta = xml.GetMetaInfo

        Pause()
    End Sub
End Module

Imports SMRUCC.MassSpectrum.DATA.NCBI
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module pubchemTest

    Sub Main()

        Dim ddd = "D:\MassSpectrum-toolkits\DATA\Massbank\Public\NCBI\PubChem\Web\record.Xml".LoadXml(Of PugView)


        Dim cas$ = "345909-34-4"
        Dim result = PubChem.Query.QueryPugViews(cas)

        Pause()
    End Sub
End Module

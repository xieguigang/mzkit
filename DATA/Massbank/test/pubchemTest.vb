Module pubchemTest

    Sub Main()
        Dim cas$ = "345909-34-4"
        Dim result = SMRUCC.MassSpectrum.DATA.NCBI.PubChem.Query.QueryPugViews(cas)

        Pause()
    End Sub
End Module

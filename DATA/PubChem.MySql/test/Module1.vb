Module Module1

    Sub Main()
        Call PubChem.MySql.CreateMySqlDatabase("N:\pubchem\raw\SDF\uncompress", "N:\pubchem\raw\mysql", "N:\pubchem\raw\extras\metalib.Xml")
    End Sub

End Module

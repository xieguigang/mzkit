Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct
Imports Microsoft.VisualBasic.Serialization.JSON

Module nameTest

    Sub Main()

        Dim parser As New GlycosylNameSolver

        Call Console.WriteLine(parser.GlycosylNameParser("Malvidin 3,7-di-(6-malonylglucoside)").ToArray.GetJson)

        Pause()
    End Sub
End Module

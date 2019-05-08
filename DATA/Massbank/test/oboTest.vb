Imports SMRUCC.genomics.foundation.OBO_Foundry

Module oboTest

    Sub Main()
        Dim tree = New OBOFile("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT").GetRawTerms.BuildTree


        Pause()
    End Sub
End Module

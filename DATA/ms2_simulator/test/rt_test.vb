Imports ms2_simulator
Imports SMRUCC.Chemistry.Model

Module rt_test

    Sub Main()

        Call loaderTest()
        Call predictionTest()

    End Sub

    Sub predictionTest()


        Pause()
    End Sub

    Sub loaderTest()
        Dim C00047 As KCF = IO.LoadKCF("https://www.kegg.jp/dbget-bin/www_bget?-f+k+compound+C00047")
        Dim cc = C00047.KCFComposition

        Pause()
    End Sub
End Module

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML

Module read_imzML

    Sub Main()
        Call testReadIbd()
    End Sub

    Sub testReadIbd()
        Dim ibd As New ibdReader("E:\demo\HR2MSI mouse urinary bladder S096.ibd".Open([readOnly]:=True, doClear:=False))

        Pause()
    End Sub
End Module

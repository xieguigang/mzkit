Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML

Module read_imzML

    Sub Main()
        Call readmzML()
        Call testReadIbd()
    End Sub

    Sub readmzML()
        Dim scans = imzML.XML.LoadScans("E:\demo\HR2MSI mouse urinary bladder S096.imzML").First
        Dim ibd As New ibdReader("E:\demo\HR2MSI mouse urinary bladder S096.ibd".Open([readOnly]:=True, doClear:=False), Format.Processed)
        Dim data = ibd.GetMSMS(scans)


        Pause()
    End Sub

    Sub testReadIbd()
        Dim ibd As New ibdReader("E:\demo\HR2MSI mouse urinary bladder S096.ibd".Open([readOnly]:=True, doClear:=False), Format.Processed)

        Dim testMzArray = ibd.ReadArray(16, 9032, 1129)
        Dim testIntoArray = ibd.ReadArray(9048, 4516, 1129)

        Pause()
    End Sub
End Module

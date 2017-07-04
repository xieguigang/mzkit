Module Module1

    Sub Main()


        For Each m In SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.metabolite.Load("C:\Users\xieguigang\Desktop\hmdb_metabolites.xml")

            m.name.__DEBUG_ECHO

        Next

        Dim rs = SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.RecordIO.ScanLoad("D:\smartnucl_integrative\DATA\OpenData\record\Athens_Univ\")

        Pause()
    End Sub
End Module

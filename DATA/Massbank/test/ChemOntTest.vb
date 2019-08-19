Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.MassSpectrum.DATA.MetaLib

Public Module ChemOntTest

    Sub Main()
        Dim ChemOnt As New ChemOntClassify("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT")
        Dim anno = "D:\Database\CID-Synonym-filtered\class\HMDB_36_classyfire_21_annotations.csv".LoadCsv(Of ClassyfireAnnotation)
        Dim table = ClassyfireInfoTable.PopulateMolecules(anno, ChemOnt).ToArray


        Call ClassyfireInfoTable.Unique(table).ToArray.SaveTo("D:\Database\CID-Synonym-filtered\class\HMDB_36_classyfire_21_annotations_infoTable.csv")

        Pause()
    End Sub
End Module

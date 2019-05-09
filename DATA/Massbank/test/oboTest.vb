Imports SMRUCC.genomics.foundation.OBO_Foundry
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports Microsoft.VisualBasic.Data.csv.IO.Linq

Module oboTest

    Sub Main()
        Dim tree = New OBOFile("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT").GetRawTerms _
            .BuildTree _
            .Values _
            .Select(Function(node) node.GetTermsByLevel(3)) _
            .Where(Function(a) Not a.IsNullOrEmpty) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(o) o.name) _
            .ToArray


        Dim classify As New ChemOntClassify("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT")

        Dim annos = classify.FilterByLevel("C:\Users\gg.xie\Downloads\ChEBI_126_classyfire_21_annotations.csv\ChEBI_126_classyfire_21_annotations.csv".OpenHandle.AsLinq(Of ClassyfireAnnotation), 2).ToArray


        Pause()
    End Sub
End Module

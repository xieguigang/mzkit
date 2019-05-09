Imports SMRUCC.genomics.foundation.OBO_Foundry
Imports Microsoft.VisualBasic.Linq

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


        Pause()
    End Sub
End Module

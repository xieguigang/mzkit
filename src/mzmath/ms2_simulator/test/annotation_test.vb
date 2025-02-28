Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module annotation_test

    Sub Main()
        Dim formuyla As Formula = FormulaScanner.ScanFormula("C7H11N3O2")
        Dim smiles As ChemicalFormula = ChemicalFormula.ParseGraph("CN1C=NC(C[C@H](N)C(O)=O)=C1")
        Dim anno As New SMILESAnnotator(smiles, formuyla)

        Call Console.WriteLine(anno.Annotation(170.16, IonModes.Positive)) ' C7H11N3O2
        Call Console.WriteLine(anno.Annotation(170.52, IonModes.Positive)) ' C7H11N3O2
        Call Console.WriteLine(anno.Annotation(124.2, IonModes.Positive)) ' C6H6NO2
    End Sub
End Module

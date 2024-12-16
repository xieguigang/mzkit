Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports SMRUCC.genomics.SequenceModel.Polypeptides


Public Class AminoAcid

    Public Property OneLetter As Char

    Public Property ThreeLetters As String

    Public Property Formula As Formula ' original formula information


    Public Property ModifiedCode As String

    Public Property ModifiedFormula As Formula

    Public Property ModifiedComposition As Formula

    Public Property Modifications As List(Of Modification)

    Public Function IsModified() As Boolean
        Return Not ModifiedCode.StringEmpty
    End Function

    Public Function ExactMass() As Double
        If IsModified() Then Return ModifiedFormula.ExactMass
        Return Formula.ExactMass
    End Function

    Public Function Code() As String
        If IsModified() Then Return ModifiedCode
        Return OneLetter.ToString()
    End Function

    Public Function GetFormula() As Formula
        If IsModified() Then Return ModifiedFormula
        Return Formula
    End Function

    Public Sub New()

    End Sub

    Public Sub New(oneletter As Char)
        Dim char2formula = OneChar2FormulaString
        Dim char2string = OneChar2ThreeLetter
        Me.OneLetter = oneletter
        ThreeLetters = char2string(oneletter)
        Formula = FormulaScanner.Convert2FormulaObjV2(char2formula(oneletter))
    End Sub

    Public Sub New(oneletter As Char, code As String, formula As Formula)
        Me.OneLetter = oneletter
        ThreeLetters = code
        Me.Formula = formula
    End Sub

    Public Sub New(aa As AminoAcid, modifiedCode As String, modifiedComposition As Formula)
        OneLetter = aa.OneLetter
        ThreeLetters = aa.ThreeLetters
        Formula = aa.Formula

        If modifiedCode.StringEmpty Then Return

        Me.ModifiedCode = modifiedCode
        Me.ModifiedComposition = modifiedComposition
        ModifiedFormula = modifiedComposition + aa.Formula
    End Sub

    Public Sub New(aa As AminoAcid, modifiedCode As String, modifiedComposition As Formula, modifications As List(Of Modification))
        OneLetter = aa.OneLetter
        ThreeLetters = aa.ThreeLetters
        Formula = aa.Formula

        If modifiedCode.StringEmpty Then Return

        Me.ModifiedCode = modifiedCode
        Me.ModifiedComposition = modifiedComposition
        ModifiedFormula = modifiedComposition + aa.Formula
        Me.Modifications = modifications
    End Sub

    Public Shared ReadOnly OneChar2Formula As Dictionary(Of Char, Formula) = New Dictionary(Of Char, Formula) From {
        {"A"c, FormulaScanner.Convert2FormulaObjV2("C3H7O2N")},
        {"R"c, FormulaScanner.Convert2FormulaObjV2("C6H14O2N4")},
        {"N"c, FormulaScanner.Convert2FormulaObjV2("C4H8O3N2")},
        {"D"c, FormulaScanner.Convert2FormulaObjV2("C4H7O4N")},
        {"C"c, FormulaScanner.Convert2FormulaObjV2("C3H7O2NS")},
        {"E"c, FormulaScanner.Convert2FormulaObjV2("C5H9O4N")},
        {"Q"c, FormulaScanner.Convert2FormulaObjV2("C5H10O3N2")},
        {"G"c, FormulaScanner.Convert2FormulaObjV2("C2H5O2N")},
        {"H"c, FormulaScanner.Convert2FormulaObjV2("C6H9O2N3")},
        {"I"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"L"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"J"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"K"c, FormulaScanner.Convert2FormulaObjV2("C6H14O2N2")},
        {"M"c, FormulaScanner.Convert2FormulaObjV2("C5H11O2NS")},
        {"F"c, FormulaScanner.Convert2FormulaObjV2("C9H11O2N")},
        {"P"c, FormulaScanner.Convert2FormulaObjV2("C5H9O2N")},
        {"S"c, FormulaScanner.Convert2FormulaObjV2("C3H7O3N")},
        {"T"c, FormulaScanner.Convert2FormulaObjV2("C4H9O3N")},
        {"W"c, FormulaScanner.Convert2FormulaObjV2("C11H12O2N2")},
        {"Y"c, FormulaScanner.Convert2FormulaObjV2("C9H11O3N")},
        {"V"c, FormulaScanner.Convert2FormulaObjV2("C5H11O2N")},
        {"O"c, FormulaScanner.Convert2FormulaObjV2("C12H21N3O3")},
        {"U"c, FormulaScanner.Convert2FormulaObjV2("C3H7NO2Se")}
    }
End Class

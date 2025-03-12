#Region "Microsoft.VisualBasic::976ec492d309e17a893ada495ccabbe3, mzmath\ms2_simulator\SMILESAnnotator.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 52
    '    Code Lines: 39 (75.00%)
    ' Comment Lines: 2 (3.85%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (21.15%)
    '     File Size: 1.92 KB


    ' Class SMILESAnnotator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Annotation
    ' 
    '     Sub: Annotation
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES.Embedding
Imports BioNovoGene.BioDeep.MSFinder

Public Class SMILESAnnotator

    ReadOnly formula As Formula
    ReadOnly da As Tolerance = Tolerance.DeltaMass(0.5)

    Sub New(smiles As ChemicalFormula, formula As Formula)
        Me.formula = formula
    End Sub

    Public Sub Annotation(ByRef spec As ISpectrum, ionMode As IonModes)
        For Each frag As ms2 In spec.GetIons
            frag.Annotation = Annotation(frag.mz, ionMode)
        Next
    End Sub

    Public Function Annotation(mz As Double, ionMode As IonModes) As String
        Dim candiates = FragmentAssigner.getValenceCheckedFragmentFormulaList(formula, ionMode, mz, da.DeltaTolerance)
        Dim topRank As Formula = Nothing

        For Each candiate As Formula In candiates
            If candiate.ExactMass > formula.ExactMass Then
                ' [M+xxx]
                ' precursor ion?
                Dim adducts As Formula = candiate - formula

                Return $"[M+{adducts.CanonicalFormula}]{If(ionMode = IonModes.Positive, "+", "-")}"
            ElseIf candiate = formula Then
                Return $"[M]{If(ionMode = IonModes.Positive, "+", "-")}"
            Else
                Dim loss As Formula = formula - candiate

                Throw New NotImplementedException
            End If
        Next

        If topRank Is Nothing Then
            Return Nothing
        End If

        Return topRank.EmpiricalFormula
    End Function

End Class


Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports stdNum = System.Math

Public Class PrecursorIonSearch : Inherits FormulaSearch

    Public Sub New(opts As SearchOption, Optional progress As Action(Of String) = Nothing)
        MyBase.New(opts, progress)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="charge">abs charge value</param>
    ''' <param name="ionMode">[1, -1]</param>
    ''' <returns></returns>
    Public Iterator Function SearchByPrecursorMz(mz As Double, charge As Double, ionMode As Integer) As IEnumerable(Of PrecursorIonComposition)
        Dim parents As MzCalculator()

        If ionMode = 1 Then
            parents = Provider.Positives.Where(Function(p) stdNum.Abs(p.charge) = stdNum.Abs(charge)).ToArray
        Else
            parents = Provider.Negatives.Where(Function(p) stdNum.Abs(p.charge) = stdNum.Abs(charge)).ToArray
        End If

        For Each type As MzCalculator In parents
            Dim exact_mass As Double = type.CalcMass(mz)

            For Each formula As FormulaComposition In SearchByExactMass(exact_mass, doVerify:=False)
                Yield New PrecursorIonComposition(formula.CountsByElement, formula.EmpiricalFormula) With {
                    .adducts = type.adducts,
                    .charge = charge,
                    .exact_mass = formula.exact_mass,
                    .M = type.M,
                    .ppm = formula.ppm,
                    .precursor_type = type.ToString
                }
            Next
        Next
    End Function
End Class

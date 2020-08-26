Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports stdNum = System.Math

Public Class PrecursorIonSearch : Inherits FormulaSearch

    ReadOnly precursorTypeProgress As Action(Of String)

    Public ReadOnly Property LimitPrecursorTypes As Index(Of String)

    Public Sub New(opts As SearchOption,
                   Optional progress As Action(Of String) = Nothing,
                   Optional precursorTypeProgress As Action(Of String) = Nothing)

        MyBase.New(opts, progress)

        Me.precursorTypeProgress = precursorTypeProgress
    End Sub

    Public Function AddPrecursorTypeRanges(ParamArray precursor_types As String()) As PrecursorIonSearch
        _LimitPrecursorTypes = precursor_types.Indexing
        Return Me
    End Function

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

        If Not (LimitPrecursorTypes Is Nothing OrElse LimitPrecursorTypes.Count = 0) Then
            parents = parents.Where(Function(a) a.name Like LimitPrecursorTypes).ToArray
        End If

        For Each type As MzCalculator In parents
            Dim exact_mass As Double = type.CalcMass(mz)

            Call precursorTypeProgress($"Run search for precursor type: {type.ToString}")

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

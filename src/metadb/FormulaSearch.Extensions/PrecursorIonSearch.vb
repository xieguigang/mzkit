#Region "Microsoft.VisualBasic::a1fdb26135e6a12a797fd350b5a9f725, FormulaSearch.Extensions\PrecursorIonSearch.vb"

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

    ' Class PrecursorIonSearch
    ' 
    '     Properties: LimitPrecursorTypes
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: AddPrecursorTypeRanges, SearchByPrecursorMz
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports stdNum = System.Math

Public Class PrecursorIonSearch : Inherits FormulaSearch

    ReadOnly precursorTypeProgress As Action(Of String)

    Public ReadOnly Property LimitPrecursorTypes As Index(Of String)

    Public Sub New(opts As SearchOption,
                   Optional progress As Action(Of String) = Nothing,
                   Optional precursorTypeProgress As Action(Of String) = Nothing)

        Call MyBase.New(opts, progress)

        If precursorTypeProgress Is Nothing Then
            Me.precursorTypeProgress =
                Sub()
                    ' do nothing 
                End Sub
        Else
            Me.precursorTypeProgress = precursorTypeProgress
        End If
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
    Public Iterator Function SearchByPrecursorMz(mz As Double, charge As Double, ionMode As Integer, Optional cancel As Value(Of Boolean) = Nothing) As IEnumerable(Of PrecursorIonComposition)
        Dim parents As MzCalculator()

        If cancel Is Nothing Then
            cancel = False
        End If

        If ionMode = 1 Then
            parents = Provider.Positives.Where(Function(p) stdNum.Abs(p.charge) = stdNum.Abs(charge)).ToArray
        Else
            parents = Provider.Negatives.Where(Function(p) stdNum.Abs(p.charge) = stdNum.Abs(charge)).ToArray
        End If

        If Not (LimitPrecursorTypes Is Nothing OrElse LimitPrecursorTypes.Count = 0) Then
            parents = parents _
                .Where(Function(a)
                           Return a.name.GetStackValue("[", "]") Like LimitPrecursorTypes
                       End Function) _
                .ToArray
        End If

        For Each type As MzCalculator In parents
            Dim exact_mass As Double = type.CalcMass(mz)

            Call precursorTypeProgress($"Run search for precursor type: {type.ToString}")

            For Each formula As FormulaComposition In SearchByExactMass(exact_mass, doVerify:=False, cancel:=cancel)
                Yield New PrecursorIonComposition(formula.CountsByElement, formula.EmpiricalFormula) With {
                    .adducts = type.adducts,
                    .charge = charge,
                    .M = type.M,
                    .ppm = formula.ppm,
                    .precursor_type = type.ToString
                }
            Next
        Next
    End Function
End Class

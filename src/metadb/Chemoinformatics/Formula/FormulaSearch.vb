#Region "Microsoft.VisualBasic::b67edec5b4f4e25f368301b9a1ed11f1, metadb\Chemoinformatics\Formula\FormulaSearch.vb"

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

    '   Total Lines: 206
    '    Code Lines: 144 (69.90%)
    ' Comment Lines: 24 (11.65%)
    '    - Xml Docs: 16.67%
    ' 
    '   Blank Lines: 38 (18.45%)
    '     File Size: 8.87 KB


    '     Class FormulaSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ConstructAndVerifyCompoundWork, reorderCandidateElements, (+2 Overloads) SearchByExactMass, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Formula

    Public Class FormulaSearch

        ReadOnly opts As SearchOption
        ReadOnly elements As Dictionary(Of String, Element)
        ReadOnly progressReport As Action(Of String)
        ReadOnly candidateElements As ElementSearchCandiate()
        ReadOnly enableHCRatioCheck As Boolean = True

        Sub New(opts As SearchOption, Optional progress As Action(Of String) = Nothing)
            Me.opts = opts
            Me.progressReport = progress
            Me.elements = Element.MemoryLoadElements
            Me.candidateElements = ReorderCandidateElements(enableHCRatioCheck)
        End Sub

        ''' <summary>
        ''' 为了方便计算HC比例以及加速计算，在这里总是将C放在第一位，H放在第二位
        ''' </summary>
        ''' <returns></returns>
        Private Function ReorderCandidateElements(ByRef enableHCRatioCheck As Boolean) As ElementSearchCandiate()
            Dim list As ElementSearchCandiate() = opts.candidateElements.ToArray
            Dim C As Integer = which(list.Select(Function(e) e.Element = "C")).DefaultFirst(-1)
            Dim H As Integer = which(list.Select(Function(e) e.Element = "H")).DefaultFirst(-1)
            Dim reorders As New List(Of ElementSearchCandiate)

            If C > -1 Then
                reorders.Add(list(C))
            End If
            If H > -1 Then
                reorders.Add(list(H))
            End If

            enableHCRatioCheck = (C > -1) And (H > -1)

            For i As Integer = 0 To list.Length - 1
                If i <> C AndAlso i <> H Then
                    reorders.Add(list(i))
                End If
            Next

            Return reorders.ToArray
        End Function

        Public Function SearchByExactMass(exact_mass As Double,
                                          Optional doVerify As Boolean = True,
                                          Optional cancel As Value(Of Boolean) = Nothing,
                                          Optional parallel As Integer = 8) As IEnumerable(Of FormulaComposition)

            Dim atoms As New Stack(Of ElementSearchCandiate)(From c As ElementSearchCandiate
                                                             In candidateElements
                                                             Let isto As Double = elements(c.Element).isotopic
                                                             Order By isto
                                                             Select c)
            If cancel Is Nothing Then
                cancel = False
            End If

            If parallel > 1 Then
                Return ParallelSearchByExactMass(atoms, exact_mass, doVerify, cancel, n_threads:=parallel)
            Else
                Return SingleThreadSearchByExactMass(atoms, exact_mass, doVerify, cancel)
            End If
        End Function

        Private Iterator Function SingleThreadSearchByExactMass(atoms As Stack(Of ElementSearchCandiate),
                                                                exact_mass As Double,
                                                                doVerify As Boolean,
                                                                cancel As Value(Of Boolean)) As IEnumerable(Of FormulaComposition)

            For Each formula As FormulaComposition In SearchByExactMass(exact_mass, FormulaComposition.EmptyComposition, atoms, cancel)
                If doVerify Then
                    formula = VerifyFormula(formula)

                    If formula Is Nothing Then
                        Continue For
                    End If
                End If

                If Not progressReport Is Nothing Then
                    Call progressReport($"find {formula} with tolerance error {formula.ppm} ppm!")
                End If

                formula.massdiff = std.Abs(formula.ExactMass - exact_mass)

                Yield formula
            Next
        End Function

        Private Iterator Function ParallelSearchByExactMass(atoms As Stack(Of ElementSearchCandiate),
                                                            exact_mass As Double,
                                                            doVerify As Boolean,
                                                            cancel As Value(Of Boolean),
                                                            n_threads As Integer) As IEnumerable(Of FormulaComposition)

        End Function

        Private Function VerifyFormula(formula As FormulaComposition) As FormulaComposition
            Dim counts As New ElementNumType(formula)
            Dim checked As Boolean = True

            'If ConstructAndVerifyCompoundWork(counts) Then
            '    ' formula.charge = FormalCharge.CorrectChargeEmpirical(formula.charge, counts)
            '    formula.charge = FormalCharge.EvaluateCharge(formula)

            '    If formula.charge >= chargeMin AndAlso formula.charge <= chargeMax Then
            '        checked = True
            '    End If
            'End If

            If Not checked Then
                Return Nothing
            End If
            If Not SevenGoldenRulesCheck.Check(formula, True, CoverRange.CommonRange, True) Then
                Return Nothing
            End If

            Return formula
        End Function

        Public Shared Function ConstructAndVerifyCompoundWork(elementNum As ElementNumType) As Boolean
            Dim maxH As Integer = 0

            ' Compute maximum number of hydrogens
            If elementNum.Si = 0 AndAlso
                elementNum.C = 0 AndAlso
                elementNum.N = 0 AndAlso
                elementNum.P = 0 AndAlso
                elementNum.Other = 0 AndAlso
                (elementNum.O > 0 OrElse elementNum.S > 0) Then
                ' Only O and S
                maxH = 3
            Else
                ' Formula is: [#C*2 + 3 - (2 if N or P present)] + [#N + 3 - (1 if C or Si present)] + [#other elements * 4 + 3],
                ' where we assume other elements can have a coordination Number of up to 7
                If elementNum.C > 0 Or elementNum.Si > 0 Then
                    maxH += (elementNum.C + elementNum.Si) * 2 + 3
                    ' If udtElementNum.N > 0 Or udtElementNum.P > 0 Then maxh = maxh - 2
                End If

                If elementNum.N > 0 Or elementNum.P > 0 Then
                    maxH += (elementNum.N + elementNum.P) + 3
                    ' If udtElementNum.C > 0 Or udtElementNum.Si > 0 Then maxh = maxh - 1
                End If

                ' Correction for carbon contribution
                'If (udtElementNum.C > 0 Or udtElementNum.Si > 0) And (udtElementNum.N > 0 Or udtElementNum.P > 0) Then udtElementNum.H = udtElementNum.H - 2

                ' Correction for nitrogen contribution
                'If (udtElementNum.N > 0 Or udtElementNum.P > 0) And (udtElementNum.C > 0 Or udtElementNum.Si > 0) Then udtElementNum.H = udtElementNum.H - 1

                ' Combine the above two commented out if's to obtain:
                If (elementNum.N > 0 Or elementNum.P > 0) And (elementNum.C > 0 Or elementNum.Si > 0) Then
                    maxH = maxH - 3
                End If

                If elementNum.Other > 0 Then
                    maxH += elementNum.Other * 4 + 3
                End If
            End If

            ' correct for if H only
            If maxH < 3 Then
                maxH = 3
            End If

            ' correct for halogens
            maxH = maxH -
                elementNum.F -
                elementNum.Cl -
                elementNum.Br -
                elementNum.I

            ' correct for negative udtElementNum.H
            If maxH < 0 Then
                maxH = 0
            End If

            ' Verify H's
            Dim blnHOK = (elementNum.H <= maxH)

            ' Only proceed if hydrogens check out
            Return blnHOK
        End Function

        ''' <summary>
        ''' Search all formula that its exact mass value is identical to target <paramref name="exact_mass"/> input. 
        ''' </summary>
        ''' <param name="exact_mass">target exact mass value for matches</param>
        ''' <param name="parent">parent formula tree</param>
        ''' <param name="candidates">cancidate atom elements and the corresponding search range</param>
        ''' <param name="cancel">cancel token of current search task</param>
        ''' <returns></returns>
        Private Iterator Function SearchByExactMass(exact_mass As Double,
                                            parent As FormulaComposition,
                                            candidates As Stack(Of ElementSearchCandiate),
                                            cancel As Value(Of Boolean)) As IEnumerable(Of FormulaComposition)
            If candidates.Count = 0 Then
                Return
            End If

            Dim current As ElementSearchCandiate = candidates.Pop
            Dim isto As Double = elements(current.Element).isotopic

            For n As Integer = current.MinCount To current.MaxCount
                ' make a copy of the formula element count dictionary data 
                ' and then append new element inside
                Dim formula As FormulaComposition = parent.AppendElement(current.Element, n)
                Dim ppm As Double = PPMmethod.PPM(formula.ExactMass, exact_mass)

                If cancel.Value Then
                    Exit For
                End If

                ' current candidate search result its exact mass is identical to the target exact mass
                If ppm <= opts.ppm Then
                    formula.ppm = ppm
                    ' populate current formula that match exact mass ppm condition
                    Yield formula
                ElseIf formula.ExactMass < exact_mass Then
                    If candidates.Count > 0 Then
                        ' 还可以再增加分子质量
                        ' stack必须要在这里进行重新初始化
                        ' 否则会被其他的循环所修改产生bug
                        For Each subtree As FormulaComposition In SearchByExactMass(exact_mass:=exact_mass,
                                                                                    parent:=formula,
                                                                                    candidates:=New Stack(Of ElementSearchCandiate)(candidates.AsEnumerable.Reverse),
                                                                                    cancel:=cancel)
                            Yield subtree
                        Next
                    End If
                Else
                    Exit For
                End If
            Next
        End Function

        Public Overrides Function ToString() As String
            Return opts.ToString
        End Function
    End Class
End Namespace

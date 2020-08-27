#Region "Microsoft.VisualBasic::99d382e3191ae9582220ec5fe1597363, src\metadb\Chemoinformatics\Formula\FormalCharge.vb"

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

    '     Structure ElementNumType
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Module FormalCharge
    ' 
    '         Function: CorrectChargeEmpirical
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports stdNum = System.Math

Namespace Formula

    Friend Structure ElementNumType

        Public H As Integer
        Public C As Integer
        Public Si As Integer
        Public N As Integer
        Public P As Integer
        Public O As Integer
        Public S As Integer
        Public Cl As Integer
        Public I As Integer
        Public F As Integer
        Public Br As Integer
        Public Other As Integer

        Sub New(formula As Formula)
            H = formula("H")
            C = formula("C")
            Si = formula("Si")
            N = formula("N")
            P = formula("P")
            O = formula("O")
            S = formula("S")
            Cl = formula("Cl")
            I = formula("I")
            F = formula("F")
            Br = formula("Br")

            Dim counts As New Dictionary(Of String, Integer)(formula.CountsByElement)

            Call counts.Remove("H")
            Call counts.Remove("C")
            Call counts.Remove("Si")
            Call counts.Remove("N")
            Call counts.Remove("P")
            Call counts.Remove("O")
            Call counts.Remove("S")
            Call counts.Remove("Cl")
            Call counts.Remove("I")
            Call counts.Remove("F")
            Call counts.Remove("Br")

            Other = counts.Values.Sum
        End Sub
    End Structure

    Module FormalCharge

        ''' <summary>
        ''' Correct charge using rules for an empirical formula
        ''' </summary>
        ''' <param name="totalCharge"></param>
        ''' <param name="elementNum"></param>
        ''' <returns>Corrected charge</returns>
        ''' <remarks></remarks>
        Public Function CorrectChargeEmpirical(totalCharge As Double, elementNum As ElementNumType) As Double
            Dim correctedCharge = totalCharge

            If elementNum.C + elementNum.Si >= 1 Then
                If elementNum.H > 0 And stdNum.Abs(Formula.Elements("H").charge - 1) < Single.Epsilon Then
                    ' Since carbon or silicon are present, assume the hydrogens should be negative
                    ' Subtract udtElementNum.H * 2 since hydrogen is assigned a +1 charge if ElementStats(1).Charge = 1
                    correctedCharge -= elementNum.H * 2
                End If

                ' Correct for udtElementNumber of C and Si
                If elementNum.C + elementNum.Si > 1 Then
                    correctedCharge -= (elementNum.C + elementNum.Si - 1) * 2
                End If
            End If

            If elementNum.N + elementNum.P > 0 And elementNum.C > 0 Then
                ' Assume 2 hydrogens around each Nitrogen or Phosphorus, thus add back +2 for each H
                ' First, decrease udtElementNumber of halogens by udtElementNumber of hydrogens & halogens taken up by the carbons
                ' Determine # of H taken up by all the carbons in a compound without N or P, then add back 1 H for each N and P
                Dim intNumHalogens = elementNum.H + elementNum.F + elementNum.Cl + elementNum.Br + elementNum.I
                intNumHalogens = intNumHalogens - (elementNum.C * 2 + 2) + elementNum.N + elementNum.P

                If intNumHalogens >= 0 Then
                    For intIndex = 1 To elementNum.N + elementNum.P
                        correctedCharge += 2
                        intNumHalogens -= 1

                        If intNumHalogens <= 0 Then
                            Exit For
                        Else
                            correctedCharge += 2
                            intNumHalogens -= 1
                            If intNumHalogens <= 0 Then Exit For
                        End If

                    Next
                End If
            End If

            Return correctedCharge
        End Function
    End Module
End Namespace




Imports System.Runtime.InteropServices
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



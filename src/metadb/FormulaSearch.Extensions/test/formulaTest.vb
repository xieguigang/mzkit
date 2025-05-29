Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module formulaTest

    Sub Main()
        Dim formulas = New String() {"[C21H21O12]+", "[C27H31O16]+", "C27H31O15
C27H31O15+
C38H41O20+
C16H12O6
C16H13O6
[C16H13O6]+
C16H13O6+
C24H25O12
C31H29O14
C43H49O24
C25H25O14+
C31H35O19
C31H29O13
C38H41O17
C28H32O16
C28H33O16
C27H30O15
C37H38O19
C37H39O19
C43H48O24
C49H59O29
C33H40O19
C33H41O19
C43H49O23
C44H51O24
C22H22O11
C22H23O11
C22H23O11+
C34H42O21
C34H43O21
C30H35O17
C43H49O22
C43H49O22+
C21H21O10
C21H21O10+
C57H68O31
C28H33O16+
C43H48O23
C22H22O10
C22H23O10+
C22H23O10
C28H32O15
C28H33O15+
C28H33O15
C34H42O20
C34H43O20
C33H41O20
C34H43O21+
[C34H43O21]+
C30H34O17
C30H35O17+
C43H48O22
[C22H23O11]+
C41H44O25
C25H25O14
C53H57O27
C50H53O26"}

        Dim scores As New List(Of Double)

        For Each str As String In formulas.Select(Function(s) s.LineTokens).IteratesALL
            If str = "" Then
                Continue For
            End If

            Dim score = AnthocyaninValidator.CalculateProbability(str)

            Call Console.WriteLine(str & vbTab & FormulaScanner.EvaluateExactMass(str) & vbTab & score)
            Call scores.Add(score)
        Next

        Call Console.WriteLine(New DoubleRange(scores).MinMax.GetJson)

        Pause()
    End Sub


End Module

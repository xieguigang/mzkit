#Region "Microsoft.VisualBasic::0f91da97c2052ea7de3b039dbf341ce6, src\mzmath\ms2_simulator\test\Module1.vb"

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

' Module Module1
' 
'     Sub: Main, mlrTest
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports Microsoft.VisualBasic.Data.Bootstrapping

Module Module1

    Sub Main()

        Call mlrTest()


        Dim beta As New EnergyModel(Function(x, y) Microsoft.VisualBasic.Math.Distributions.Beta.beta(x, 2, 5), 0, 1)


        Call beta.PercentageGreater(0).__DEBUG_ECHO

        Call beta.PercentageGreater(1).__DEBUG_ECHO

        Call beta.PercentageGreater(0.2).__DEBUG_ECHO

        Call beta.PercentageGreater(0.4).__DEBUG_ECHO

        Call beta.PercentageGreater(0.6).__DEBUG_ECHO

        Call beta.PercentageGreater(0.8).__DEBUG_ECHO

        Pause()
    End Sub

    Sub mlrTest()
        '                  x1    x2     x3   x4    x5
        Dim x As Double(,) = {{1, 3064, 1201, 10, 361},
                         {1, 3000, 1053, 11, 338},
                         {1, 3155, 1133, 19, 393},
                         {1, 3080, 970, 4, 467},
                         {1, 3245, 1258, 36, 294},
                         {1, 3267, 1386, 35, 225},
                         {1, 3080, 966, 13, 417},
                         {1, 2974, 1185, 12, 488},
                         {1, 3038, 1103, 14, 677},
                         {1, 3318, 1310, 29, 427},
                         {1, 3317, 1362, 25, 326},
                         {1, 3182, 1171, 28, 326},
                         {1, 2998, 1102, 9, 349},
                         {1, 3221, 1424, 21, 382},
                         {1, 3019, 1239, 16, 275},
                         {1, 3022, 1285, 9, 303},
                         {1, 3094, 1329, 11, 339},
                         {1, 3009, 1210, 15, 536},
                         {1, 3227, 1331, 21, 414},
                         {1, 3308, 1368, 24, 282},
                         {1, 3212, 1289, 17, 302},
                         {1, 3381, 1444, 25, 253},
                         {1, 3061, 1175, 12, 261},
                         {1, 3478, 1317, 42, 259},
                         {1, 3126, 1248, 11, 315},
                         {1, 3468, 1508, 43, 286},
                         {1, 3252, 1361, 26, 346},
                         {1, 3052, 1186, 14, 443},
                         {1, 3270, 1399, 24, 306},
                         {1, 3198, 1299, 20, 367},
                         {1, 2904, 1164, 6, 311},
                         {1, 3247, 1277, 19, 375}
       }

        Dim y = {1, -1, 1, -2, 2, 3, -2, -2, -3, 1, 2, -1, -2, 2, 0, 0, 0, -2, 1, 2, 1, 3, 0, 3, 1, 3, 1, -2, 2, 2, -2, 2}
        Dim regression = MLRFit.LinearFitting(x, y)
        Dim confidenceBeta0 = MLRFit.ConfidenceInterval(regression.beta(0), -4.97, 5.384)
        Dim confidenceBeta1 = MLRFit.ConfidenceInterval(regression.beta(1), 3.97, 0.00193)
        Dim confidenceBeta2 = MLRFit.ConfidenceInterval(regression.beta(2), 3.06, 0.00144)
        Dim confidenceBeta3 = MLRFit.ConfidenceInterval(regression.beta(3), -0.92, 0.02574)
        Dim confidenceBeta4 = MLRFit.ConfidenceInterval(regression.beta(4), -3.88, 0.00154)

        '       /* Print results */
        println("\n>>Regression equation is:")
        println("y = " & regression.beta(0) & " + " & regression.beta(1) & " x2 + " & regression.beta(2) & " x3 + " & regression.beta(3) & " x4 + " & regression.beta(4) & " x5\n")
        println(">>R^2 = " & (regression.R2()) * 100 & " %\n")
        println(">>SSE = " & regression.SSE)
        println(">>SST = " & regression.SST)
        println("\n>>Confidence intervals:\nbeta1: [" &
                            confidenceBeta0.Min & " , " & confidenceBeta0.Max & "]\nbeta2: [" &
                             confidenceBeta1.Min & " , " & confidenceBeta1.Max & "]\nbeta3: [" &
                             confidenceBeta2.Min & " , " & confidenceBeta2.Max & "]\nbeta4: [" &
                             confidenceBeta3.Min & " , " & confidenceBeta3.Max & "]\nbeta5: [" &
                             confidenceBeta4.Min & " , " & confidenceBeta4.Max & "]\n")
        Pause()
    End Sub
End Module

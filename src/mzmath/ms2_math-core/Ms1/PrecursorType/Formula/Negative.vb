#Region "Microsoft.VisualBasic::325f8fc4495a4c35b9111e8b09d9164c, src\mzmath\ms2_math-core\Ms1\PrecursorType\Formula\Negative.vb"

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

    '     Module NegativeFormula
    ' 
    '         Function: GetFormulas
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Ms1.PrecursorType

    Module NegativeFormula

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFormulas() As Dictionary(Of String, MzCalculator)
            Return New Dictionary(Of String, MzCalculator) From {
                {"M", New MzCalculator("[M]-", charge:=-1, M:=1, adducts:=0, mode:="-")},
                {"M-3H", New MzCalculator("[M-3H]3-", charge:=-3, M:=1, adducts:=-1.007276, mode:="-")},        ' M/3 - 1.007276	    3-	0.33	 -1.007276	 283.436354	 293.113943
                {"M-2H", New MzCalculator("[M-2H]2-", charge:=-2, M:=1, adducts:=-1.007276, mode:="-")},        ' M/2 - 1.007276	    2-	0.50	 -1.007276	 425.658169	 439.167276
                {"M-H2O-H", New MzCalculator("[M-H2O-H]-", charge:=-1, M:=1, adducts:=-19.01839, mode:="-")},   ' M - 19.01839	    1-	1.00	-19.01839	 834.312500	 895.338390
                {"M-H", New MzCalculator("[M-H]-", charge:=-1, M:=1, adducts:=-1.007276, mode:="-")},           ' M - 1.007276	    1-	1.00	 -1.007276	 852.323614	 877.327276
                {"M+Na-2H", New MzCalculator("[M+Na-2H]-", charge:=-1, M:=1, adducts:=20.974666, mode:="-")},   ' M + 20.974666	    1-	1.00	 20.974666	 874.305556	 855.345334
                {"M+Cl", New MzCalculator("[M+Cl]-", charge:=-1, M:=1, adducts:=34.969402, mode:="-")},         ' M + 34.969402	    1-	1.00	 34.969402	 888.300292	 841.350598
                {"M+K-2H", New MzCalculator("[M+K-2H]-", charge:=-1, M:=1, adducts:=36.948606, mode:="-")},     ' M + 36.948606	    1-	1.00	 36.948606	 890.279496	 839.371394
                {"M+FA-H", New MzCalculator("[M+FA-H]-", charge:=-1, M:=1, adducts:=44.998201, mode:="-")},     ' M + 44.998201	    1-	1.00	 44.998201	 898.329091	 831.321799
                {"M+Hac-H", New MzCalculator("[M+Hac-H]-", charge:=-1, M:=1, adducts:=59.013851, mode:="-")},   ' M + 59.013851	    1-	1.00	 59.013851	 912.344741	 817.306149
                {"M+Br", New MzCalculator("[M+Br]-", charge:=-1, M:=1, adducts:=78.918885, mode:="-")},         ' M + 78.918885	    1-	1.00	 78.918885	 932.249775	 797.401115
                {"M+TFA-H", New MzCalculator("[M+TFA-H]-", charge:=-1, M:=1, adducts:=112.985586, mode:="-")},  ' M + 112.985586	    1-	1.00	112.985586	 966.316476	 763.334414
                {"M+F", New MzCalculator("[M+F]-", charge:=-1, M:=1, adducts:=MolWeight.Eval("+F"), mode:="-")},
                {"2M-H", New MzCalculator("[2M-H]-", charge:=-1, M:=2, adducts:=-1.007276, mode:="-")},         ' 2M - 1.007276	    1-	2.00	 -1.007276	1705.654504	1753.647276
                {"2M+FA-H", New MzCalculator("[2M+FA-H]-", charge:=-1, M:=2, adducts:=44.998201, mode:="-")},   ' 2M + 44.998201	    1-	2.00	 44.998201	1751.659981	1707.641799
                {"2M+Hac-H", New MzCalculator("[2M+Hac-H]-", charge:=-1, M:=2, adducts:=59.013851, mode:="-")}, ' 2M + 59.013851	    1-	2.00	 59.013851	1765.675631	1693.626149
                {"3M-H", New MzCalculator("[3M-H]-", charge:=-1, M:=3, adducts:=-1.007276, mode:="-")}          ' 3M - 1.007276	    1-	3.00	  1.007276	2560.999946	2627.952724
            }
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::c751744292f8b0545e7f5298afccd303, src\mzmath\ms2_math-core\Ms1\PrecursorType\Formula\Positive.vb"

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

    '     Module PositiveFormula
    ' 
    '         Function: formulas, GetFormulas
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Ms1.PrecursorType

    Module PositiveFormula

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFormulas() As Dictionary(Of String, MzCalculator)
            Return New Dictionary(Of String, MzCalculator) From {
                {"M", New MzCalculator("[M]+", charge:=1, M:=1, adducts:=0, mode:="+")},
                {"M+3H", New MzCalculator("[M+3H]3+", charge:=3, M:=1, adducts:=1.007276, mode:="+")},                    ' M/3 + 1.007276	    3+	0.33	 1.007276	 285.450906	 291.099391 
                {"M+2H+Na", New MzCalculator("[M+2H+Na]3+", charge:=3, M:=1, adducts:=8.33459, mode:="+")},               ' M/3 + 8.334590	    3+	0.33	 8.334590	 292.778220	 283.772077
                {"M+H+2Na", New MzCalculator("[M+H+2Na]3+", charge:=3, M:=1, adducts:=15.76619, mode:="+")},              ' M/3 + 15.7661904    3+	0.33	15.766190	 300.209820	 276.340476
                {"M+3Na", New MzCalculator("[M+3Na]3+", charge:=3, M:=1, adducts:=22.989218, mode:="+")},                 ' M/3 + 22.989218	    3+	0.33	22.989218	 307.432848	 269.117449
                {"M+2H", New MzCalculator("[M+2H]2+", charge:=2, M:=1, adducts:=1.007276, mode:="+")},                    ' M/2 + 1.007276	    2+	0.50	 1.007276	 427.672721	 437.152724
                {"M+H+NH4", New MzCalculator("[M+H+NH4]2+", charge:=2, M:=1, adducts:=9.52055, mode:="+")},               ' M/2 + 9.520550	    2+	0.50	 9.520550	 436.185995	 428.639450
                {"M+H+Na", New MzCalculator("[M+H+Na]2+", charge:=2, M:=1, adducts:=11.998247, mode:="+")},               ' M/2 + 11.998247	    2+	0.50	11.998247	 438.663692	 426.161753
                {"M+H+K", New MzCalculator("[M+H+K]2+", charge:=2, M:=1, adducts:=19.985217, mode:="+")},                 ' M/2 + 19.985217	    2+	0.50	19.985217	 446.650662	 418.174783
                {"M+ACN+2H", New MzCalculator("[M+ACN+2H]2+", charge:=2, M:=1, adducts:=21.52055, mode:="+")},            ' M/2 + 21.520550	    2+	0.50	21.520550	 448.185995	 416.639450
                {"M+2Na", New MzCalculator("[M+2Na]2+", charge:=2, M:=1, adducts:=22.989218, mode:="+")},                 ' M/2 + 22.989218	    2+	0.50	22.989218	 449.654663	 415.170782
                {"M+2ACN+2H", New MzCalculator("[M+2ACN+2H]2+", charge:=2, M:=1, adducts:=42.033823, mode:="+")},         ' M/2 + 42.033823	    2+	0.50	42.033823	 468.699268	 396.126177
                {"M+3ACN+2H", New MzCalculator("[M+3ACN+2H]2+", charge:=2, M:=1, adducts:=62.547097, mode:="+")},         ' M/2 + 62.547097	    2+	0.50	62.547097	 489.212542	 375.612903
                {"M+H", New MzCalculator("[M+H]+", charge:=1, M:=1, adducts:=1.007276, mode:="+")},                       '  M  + 1.007276	    1+	1.00	 1.007276	 854.338166	 875.312724
                {"M+Li", New MzCalculator("[M+Li]+", charge:=1, M:=1, adducts:=ExactMass.Eval("+Li"), mode:="+")},
                {"M-H2O+NH4", New MzCalculator("[M-H2O+NH4]+", charge:=1, M:=1, adducts:=ExactMass.Eval("-H2O+NH4"), mode:="+")},
                {"M+H-2H2O", New MzCalculator("[M+H-2H2O]+", charge:=1, M:=1, adducts:=ExactMass.Eval("+H-2H2O"), mode:="+")},
                {"M+H-H2O", New MzCalculator("[M+H-H2O]+", charge:=1, M:=1, adducts:=ExactMass.Eval("+H-H2O"), mode:="+")},
                {"M+NH4", New MzCalculator("[M+NH4]+", charge:=1, M:=1, adducts:=18.033823, mode:="+")},                  '  M + 18.033823	    1+	1.00	18.033823	 871.364713	 858.286177
                {"M+Na", New MzCalculator("[M+Na]+", charge:=1, M:=1, adducts:=22.989218, mode:="+")},                    '  M + 22.989218	    1+	1.00	22.989218	 876.320108	 853.330782
                {"M+CH3OH+H", New MzCalculator("[M+CH3OH+H]+", charge:=1, M:=1, adducts:=33.033489, mode:="+")},          '  M + 33.033489	    1+	1.00	33.033489	 886.364379	 843.286511
                {"M+K", New MzCalculator("[M+K]+", charge:=1, M:=1, adducts:=38.963158, mode:="+")},                      '  M + 38.963158	    1+	1.00	38.963158	 892.294048	 837.356842
                {"M+ACN+H", New MzCalculator("[M+ACN+H]+", charge:=1, M:=1, adducts:=42.033823, mode:="+")},              '  M + 42.033823	    1+	1.00	42.033823	 895.364713	 834.286177
                {"M+2Na-H", New MzCalculator("[M+2Na-H]+", charge:=1, M:=1, adducts:=44.97116, mode:="+")},               '  M + 44.971160	    1+	1.00	44.971160	 898.302050	 831.348840
                {"M+IsoProp+H", New MzCalculator("[M+IsoProp+H]+", charge:=1, M:=1, adducts:=61.06534, mode:="+")},       '  M + 61.06534	    1+	1.00	61.065340	 914.396230	 815.254660
                {"M+ACN+Na", New MzCalculator("[M+ACN+Na]+", charge:=1, M:=1, adducts:=64.015765, mode:="+")},            '  M + 64.015765	    1+	1.00	64.015765	 917.346655	 812.304235
                {"M+2K-H", New MzCalculator("[M+2K-H]+", charge:=1, M:=1, adducts:=76.91904, mode:="+")},                 '  M + 76.919040	    1+	1.00	76.919040	 930.249930	 799.400960
                {"M+DMSO+H", New MzCalculator("[M+DMSO+H]+", charge:=1, M:=1, adducts:=79.02122, mode:="+")},             '  M + 79.02122	    1+	1.00	79.021220	 932.352110	 797.298780
                {"M+2ACN+H", New MzCalculator("[M+2ACN+H]+", charge:=1, M:=1, adducts:=83.06037, mode:="+")},             '  M + 83.060370	    1+	1.00	83.060370	 936.391260	 793.259630
                {"M+IsoProp+Na+H", New MzCalculator("[M+IsoProp+Na+H]+", charge:=1, M:=1, adducts:=84.05511, mode:="+")}, '  M + 84.05511	    1+	1.00	84.055110	 937.386000	 792.264890
                {"2M+H", New MzCalculator("[2M+H]+", charge:=1, M:=2, adducts:=1.007276, mode:="+")},                     ' 2M + 1.007276	    1+	2.00	 1.007276	1707.669056	1751.632724
                {"2M+NH4", New MzCalculator("[2M+NH4]+", charge:=1, M:=2, adducts:=18.033823, mode:="+")},                ' 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
                {"2M+Na", New MzCalculator("[2M+Na]+", charge:=1, M:=2, adducts:=22.989218, mode:="+")},                  ' 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
                {"2M+K", New MzCalculator("[2M+K]+", charge:=1, M:=2, adducts:=38.963158, mode:="+")},                    ' 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
                {"2M+ACN+H", New MzCalculator("[2M+ACN+H]+", charge:=1, M:=2, adducts:=42.033823, mode:="+")},            ' 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
                {"2M+ACN+Na", New MzCalculator("[2M+ACN+Na]+", charge:=1, M:=2, adducts:=64.015765, mode:="+")},           ' 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
                {"M+H-C12H20O9", New MzCalculator("[M+H-C12H20O9]+", charge:=1, M:=1, adducts:=ExactMass.Eval("+H-C12H20O9"), mode:="+")}
            }
        End Function

        Private Iterator Function formulas() As IEnumerable(Of MzCalculator)
            Yield "[M]+              ".ParseMzCalculator
            Yield "[M+3H]3+          ".ParseMzCalculator   ' M/3 + 1.007276	    3+	0.33	 1.007276	 285.450906	 291.099391 
            Yield "[M+2H+Na]3+       ".ParseMzCalculator   ' M/3 + 8.334590	    3+	0.33	 8.334590	 292.778220	 283.772077
            Yield "[M+H+2Na]3+       ".ParseMzCalculator   ' M/3 + 15.7661904	3+	0.33	15.766190	 300.209820	 276.340476
            Yield "[M+3Na]3+         ".ParseMzCalculator   ' M/3 + 22.989218	3+	0.33	22.989218	 307.432848	 269.117449

            Yield "[M+2H]2+          ".ParseMzCalculator   ' M/2 + 1.007276	    2+	0.50	 1.007276	 427.672721	 437.152724
            Yield "[M+H+NH4]2+       ".ParseMzCalculator   ' M/2 + 9.520550	    2+	0.50	 9.520550	 436.185995	 428.639450
            Yield "[M+H+Na]2+        ".ParseMzCalculator   ' M/2 + 11.998247	2+	0.50	11.998247	 438.663692	 426.161753
            Yield "[M+H+K]2+         ".ParseMzCalculator   ' M/2 + 19.985217	2+	0.50	19.985217	 446.650662	 418.174783
            Yield "[M+ACN+2H]2+      ".ParseMzCalculator   ' M/2 + 21.520550	2+	0.50	21.520550	 448.185995	 416.639450
            Yield "[M+2Na]2+         ".ParseMzCalculator   ' M/2 + 22.989218	2+	0.50	22.989218	 449.654663	 415.170782
            Yield "[M+2ACN+2H]2+     ".ParseMzCalculator   ' M/2 + 42.033823	2+	0.50	42.033823	 468.699268	 396.126177
            Yield "[M+3ACN+2H]2+     ".ParseMzCalculator   ' M/2 + 62.547097	2+	0.50	62.547097	 489.212542	 375.612903

            Yield "[M+H]+            ".ParseMzCalculator   '  M + 1.007276	    1+	1.00	 1.007276	 854.338166	 875.312724
            Yield "[M+H-2H2O]+       ".ParseMzCalculator
            Yield "[M+H-H2O]+        ".ParseMzCalculator
            Yield "[M+NH4]+          ".ParseMzCalculator   '  M + 18.033823	    1+	1.00	18.033823	 871.364713	 858.286177
            Yield "[M+Na]+           ".ParseMzCalculator   '  M + 22.989218	    1+	1.00	22.989218	 876.320108	 853.330782
            Yield "[M+CH3OH+H]+      ".ParseMzCalculator   '  M + 33.033489	    1+	1.00	33.033489	 886.364379	 843.286511
            Yield "[M+K]+            ".ParseMzCalculator   '  M + 38.963158	    1+	1.00	38.963158	 892.294048	 837.356842
            Yield "[M+ACN+H]+        ".ParseMzCalculator   '  M + 42.033823	    1+	1.00	42.033823	 895.364713	 834.286177
            Yield "[M+2Na-H]+        ".ParseMzCalculator   '  M + 44.971160	    1+	1.00	44.971160	 898.302050	 831.348840
            Yield "[M+IsoProp+H]+    ".ParseMzCalculator   '  M + 61.06534	    1+	1.00	61.065340	 914.396230	 815.254660
            Yield "[M+ACN+Na]+       ".ParseMzCalculator   '  M + 64.015765	    1+	1.00	64.015765	 917.346655	 812.304235
            Yield "[M+2K-H]+         ".ParseMzCalculator   '  M + 76.919040	    1+	1.00	76.919040	 930.249930	 799.400960
            Yield "[M+DMSO+H]+       ".ParseMzCalculator   '  M + 79.02122	    1+	1.00	79.021220	 932.352110	 797.298780
            Yield "[M+2ACN+H]+       ".ParseMzCalculator   '  M + 83.060370	    1+	1.00	83.060370	 936.391260	 793.259630
            Yield "[M+IsoProp+Na+H]+ ".ParseMzCalculator   '  M + 84.05511	    1+	1.00	84.055110	 937.386000	 792.264890

            Yield "[2M+H]+           ".ParseMzCalculator   ' 2M + 1.007276	    1+	2.00	 1.007276	1707.669056	1751.632724
            Yield "[2M+NH4]+         ".ParseMzCalculator   ' 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
            Yield "[2M+Na]+          ".ParseMzCalculator   ' 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
            Yield "[2M+K]+           ".ParseMzCalculator   ' 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
            Yield "[2M+ACN+H]+       ".ParseMzCalculator   ' 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
            Yield "[2M+ACN+Na]+      ".ParseMzCalculator   ' 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
        End Function
    End Module
End Namespace

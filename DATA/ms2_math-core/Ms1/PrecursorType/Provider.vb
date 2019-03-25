#Region "Microsoft.VisualBasic::dab10fbdb5b8177c37b6bbd0d666204e, ms2_math-core\Ms1\PrecursorType\Provider.vb"

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

    '     Module Provider
    ' 
    '         Properties: (+2 Overloads) Negative, (+2 Overloads) Positive
    ' 
    '         Function: Calculator, GetCalculator, ParseIonMode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' 提供一些常用的前体母离子加和物类型的mz计算
    ''' </summary>
    ''' <remarks>
    ''' ###### 2019-03-14 经过测试,无错误
    ''' </remarks>
    Public Module Provider

        '# http://fiehnlab.ucdavis.edu/staff/kind/Metabolomics/MS-Adduct-Calculator
        '#
        '# Example: 
        '# 1) Find Adduct: Taxol, C47H51NO14, M=853.33089 
        '#    Enter 853.33089 In green box read M+22.9, m/z=876.320108 
        '#
        '# 2) Reverse: take 12 Tesla-FT-MS result out Of MS m/z=876.330
        '#    suspect M+Na adduct, read M=853.340782, enter this value into formula finder 
        '#    With 2 ppm mass accuracy (CHNSOP enabled) Get some thousand results, 
        '#    compare isotopic pattern, Get happy.

        '# Table 1. Monoisotopic exact masses Of molecular ion adducts often observed In ESI mass spectra

        '#  	 	 	 	 	                                           Your M here:	Your M+X Or M-X
        '#  	 	 	 	 	                                            853.33089	876.32

        '# Ion name	        Ion mass	    Charge	Mult	Mass	    Result:	    Reverse:

        '# 1. Positive ion mode	 	 	 	 	 	 
        '# M+3H	            M/3 + 1.007276	    3+	0.33	1.007276	285.450906	291.099391
        '# M+2H+Na	        M/3 + 8.334590	    3+	0.33	8.334590	292.778220	283.772077
        '# M+H+2Na	        M/3 + 15.7661904	3+	0.33	15.766190	300.209820	276.340476
        '# M+3Na	        M/3 + 22.989218	    3+	0.33	22.989218	307.432848	269.117449
        '# M+2H	            M/2 + 1.007276	    2+	0.50	1.007276	427.672721	437.152724
        '# M+H+NH4	        M/2 + 9.520550	    2+	0.50	9.520550	436.185995	428.639450
        '# M+H+Na	        M/2 + 11.998247	    2+	0.50	11.998247	438.663692	426.161753
        '# M+H+K	        M/2 + 19.985217	    2+	0.50	19.985217	446.650662	418.174783
        '# M+ACN+2H	        M/2 + 21.520550	    2+	0.50	21.520550	448.185995	416.639450
        '# M+2Na	        M/2 + 22.989218	    2+	0.50	22.989218	449.654663	415.170782
        '# M+2ACN+2H	    M/2 + 42.033823	    2+	0.50	42.033823	468.699268	396.126177
        '# M+3ACN+2H	    M/2 + 62.547097	    2+	0.50	62.547097	489.212542	375.612903
        '# M+H	            M + 1.007276	    1+	1.00	1.007276	854.338166	875.312724
        '# M+NH4	        M + 18.033823	    1+	1.00	18.033823	871.364713	858.286177
        '# M+Na	            M + 22.989218	    1+	1.00	22.989218	876.320108	853.330782
        '# M+CH3OH+H	    M + 33.033489	    1+	1.00	33.033489	886.364379	843.286511
        '# M+K	            M + 38.963158	    1+	1.00	38.963158	892.294048	837.356842
        '# M+ACN+H	        M + 42.033823	    1+	1.00	42.033823	895.364713	834.286177
        '# M+2Na-H	        M + 44.971160	    1+	1.00	44.971160	898.302050	831.348840
        '# M+IsoProp+H	    M + 61.06534	    1+	1.00	61.065340	914.396230	815.254660
        '# M+ACN+Na	        M + 64.015765	    1+	1.00	64.015765	917.346655	812.304235
        '# M+2K-H	        M + 76.919040	    1+	1.00	76.919040	930.249930	799.400960
        '# M+DMSO+H	        M + 79.02122	    1+	1.00	79.021220	932.352110	797.298780
        '# M+2ACN+H	        M + 83.060370	    1+	1.00	83.060370	936.391260	793.259630
        '# M+IsoProp+Na+H	M + 84.05511	    1+	1.00	84.055110	937.386000	792.264890
        '# 2M+H	            2M + 1.007276	    1+	2.00	1.007276	1707.669056	1751.632724
        '# 2M+NH4	        2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
        '# 2M+Na	        2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
        '# 2M+K	            2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
        '# 2M+ACN+H	        2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
        '# 2M+ACN+Na	    2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235

        '# 2. Negative ion mode	 	 	 	 	 
        '# M-3H	            M/3 - 1.007276	    3-	0.33	-1.007276	283.436354	293.113943
        '# M-2H	            M/2 - 1.007276	    2-	0.50	-1.007276	425.658169	439.167276
        '# M-H2O-H	        M- 19.01839	        1-	1.00	-19.01839	834.312500	895.338390
        '# M-H	            M - 1.007276	    1-	1.00	-1.007276	852.323614	877.327276
        '# M+Na-2H	        M + 20.974666	    1-	1.00	20.974666	874.305556	855.345334
        '# M+Cl	            M + 34.969402	    1-	1.00	34.969402	888.300292	841.350598
        '# M+K-2H	        M + 36.948606	    1-	1.00	36.948606	890.279496	839.371394
        '# M+FA-H	        M + 44.998201	    1-	1.00	44.998201	898.329091	831.321799
        '# M+Hac-H	        M + 59.013851	    1-	1.00	59.013851	912.344741	817.306149
        '# M+Br	            M + 78.918885	    1-	1.00	78.918885	932.249775	797.401115
        '# M+TFA-H	        M + 112.985586	    1-	1.00	112.985586	966.316476	763.334414
        '# 2M-H	            2M - 1.007276	    1-	2.00	-1.007276	1705.654504	1753.647276
        '# 2M+FA-H	        2M + 44.998201	    1-	2.00	44.998201	1751.659981	1707.641799
        '# 2M+Hac-H	        2M + 59.013851	    1-	2.00	59.013851	1765.675631	1693.626149
        '# 3M-H	            3M - 1.007276	    1-	3.00	1.007276	2560.999946	2627.952724

        Public ReadOnly Property Positive() As Dictionary(Of String, MzCalculator)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Dictionary(Of String, MzCalculator) From {
                    {"M", New MzCalculator("[M]+", charge:=1, M:=1, adducts:=0)},
                    {"M+3H", New MzCalculator("[M+3H]3+", charge:=3, M:=1, adducts:=1.007276)},                    ' M/3 + 1.007276	    3+	0.33	 1.007276	 285.450906	 291.099391 
                    {"M+2H+Na", New MzCalculator("[M+2H+Na]3+", charge:=3, M:=1, adducts:=8.33459)},               ' M/3 + 8.334590	    3+	0.33	 8.334590	 292.778220	 283.772077
                    {"M+H+2Na", New MzCalculator("[M+H+2Na]3+", charge:=3, M:=1, adducts:=15.76619)},              ' M/3 + 15.7661904	    3+	0.33	15.766190	 300.209820	 276.340476
                    {"M+3Na", New MzCalculator("[M+3Na]3+", charge:=3, M:=1, adducts:=22.989218)},                 ' M/3 + 22.989218	    3+	0.33	22.989218	 307.432848	 269.117449
                    {"M+2H", New MzCalculator("[M+2H]2+", charge:=2, M:=1, adducts:=1.007276)},                    ' M/2 + 1.007276	    2+	0.50	 1.007276	 427.672721	 437.152724
                    {"M+H+NH4", New MzCalculator("[M+H+NH4]2+", charge:=2, M:=1, adducts:=9.52055)},               ' M/2 + 9.520550	    2+	0.50	 9.520550	 436.185995	 428.639450
                    {"M+H+Na", New MzCalculator("[M+H+Na]2+", charge:=2, M:=1, adducts:=11.998247)},               ' M/2 + 11.998247	    2+	0.50	11.998247	 438.663692	 426.161753
                    {"M+H+K", New MzCalculator("[M+H+K]2+", charge:=2, M:=1, adducts:=19.985217)},                 ' M/2 + 19.985217	    2+	0.50	19.985217	 446.650662	 418.174783
                    {"M+ACN+2H", New MzCalculator("[M+ACN+2H]2+", charge:=2, M:=1, adducts:=21.52055)},            ' M/2 + 21.520550	    2+	0.50	21.520550	 448.185995	 416.639450
                    {"M+2Na", New MzCalculator("[M+2Na]2+", charge:=2, M:=1, adducts:=22.989218)},                 ' M/2 + 22.989218	    2+	0.50	22.989218	 449.654663	 415.170782
                    {"M+2ACN+2H", New MzCalculator("[M+2ACN+2H]2+", charge:=2, M:=1, adducts:=42.033823)},         ' M/2 + 42.033823	    2+	0.50	42.033823	 468.699268	 396.126177
                    {"M+3ACN+2H", New MzCalculator("[M+3ACN+2H]2+", charge:=2, M:=1, adducts:=62.547097)},         ' M/2 + 62.547097	    2+	0.50	62.547097	 489.212542	 375.612903
                    {"M+H", New MzCalculator("[M+H]+", charge:=1, M:=1, adducts:=1.007276)},                       '  M + 1.007276	    1+	1.00	 1.007276	 854.338166	 875.312724
                    {"M+NH4", New MzCalculator("[M+NH4]+", charge:=1, M:=1, adducts:=18.033823)},                  '  M + 18.033823	    1+	1.00	18.033823	 871.364713	 858.286177
                    {"M+Na", New MzCalculator("[M+Na]+", charge:=1, M:=1, adducts:=22.989218)},                    '  M + 22.989218	    1+	1.00	22.989218	 876.320108	 853.330782
                    {"M+CH3OH+H", New MzCalculator("[M+CH3OH+H]+", charge:=1, M:=1, adducts:=33.033489)},          '  M + 33.033489	    1+	1.00	33.033489	 886.364379	 843.286511
                    {"M+K", New MzCalculator("[M+K]+", charge:=1, M:=1, adducts:=38.963158)},                      '  M + 38.963158	    1+	1.00	38.963158	 892.294048	 837.356842
                    {"M+ACN+H", New MzCalculator("[M+ACN+H]+", charge:=1, M:=1, adducts:=42.033823)},              '  M + 42.033823	    1+	1.00	42.033823	 895.364713	 834.286177
                    {"M+2Na-H", New MzCalculator("[M+2Na-H]+", charge:=1, M:=1, adducts:=44.97116)},               '  M + 44.971160	    1+	1.00	44.971160	 898.302050	 831.348840
                    {"M+IsoProp+H", New MzCalculator("[M+IsoProp+H]+", charge:=1, M:=1, adducts:=61.06534)},       '  M + 61.06534	    1+	1.00	61.065340	 914.396230	 815.254660
                    {"M+ACN+Na", New MzCalculator("[M+ACN+Na]+", charge:=1, M:=1, adducts:=64.015765)},            '  M + 64.015765	    1+	1.00	64.015765	 917.346655	 812.304235
                    {"M+2K-H", New MzCalculator("[M+2K-H]+", charge:=1, M:=1, adducts:=76.91904)},                 '  M + 76.919040	    1+	1.00	76.919040	 930.249930	 799.400960
                    {"M+DMSO+H", New MzCalculator("[M+DMSO+H]+", charge:=1, M:=1, adducts:=79.02122)},             '  M + 79.02122	    1+	1.00	79.021220	 932.352110	 797.298780
                    {"M+2ACN+H", New MzCalculator("[M+2ACN+H]+", charge:=1, M:=1, adducts:=83.06037)},             '  M + 83.060370	    1+	1.00	83.060370	 936.391260	 793.259630
                    {"M+IsoProp+Na+H", New MzCalculator("[M+IsoProp+Na+H]+", charge:=1, M:=1, adducts:=84.05511)}, '  M + 84.05511	    1+	1.00	84.055110	 937.386000	 792.264890
                    {"2M+H", New MzCalculator("[2M+H]+", charge:=1, M:=2, adducts:=1.007276)},                     ' 2M + 1.007276	    1+	2.00	 1.007276	1707.669056	1751.632724
                    {"2M+NH4", New MzCalculator("[2M+NH4]+", charge:=1, M:=2, adducts:=18.033823)},                ' 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
                    {"2M+Na", New MzCalculator("[2M+Na]+", charge:=1, M:=2, adducts:=22.989218)},                  ' 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
                    {"2M+K", New MzCalculator("[2M+K]+", charge:=1, M:=2, adducts:=38.963158)},                    ' 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
                    {"2M+ACN+H", New MzCalculator("[2M+ACN+H]+", charge:=1, M:=2, adducts:=42.033823)},            ' 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
                    {"2M+ACN+Na", New MzCalculator("[2M+ACN+Na]+", charge:=1, M:=2, adducts:=64.015765)}           ' 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
                }
            End Get
        End Property

        Public ReadOnly Property Negative() As Dictionary(Of String, MzCalculator)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New Dictionary(Of String, MzCalculator) From {
                    {"M", New MzCalculator("[M]-", charge:=-1, M:=1, adducts:=0)},
                    {"M-3H", New MzCalculator("[M-3H]3-", charge:=-3, M:=1, adducts:=-1.007276)},        ' M/3 - 1.007276	    3-	0.33	 -1.007276	 283.436354	 293.113943
                    {"M-2H", New MzCalculator("[M-2H]2-", charge:=-2, M:=1, adducts:=-1.007276)},        ' M/2 - 1.007276	    2-	0.50	 -1.007276	 425.658169	 439.167276
                    {"M-H2O-H", New MzCalculator("[M-H2O-H]-", charge:=-1, M:=1, adducts:=-19.01839)},   ' M - 19.01839	    1-	1.00	-19.01839	 834.312500	 895.338390
                    {"M-H", New MzCalculator("[M-H]-", charge:=-1, M:=1, adducts:=-1.007276)},           ' M - 1.007276	    1-	1.00	 -1.007276	 852.323614	 877.327276
                    {"M+Na-2H", New MzCalculator("[M+Na-2H]-", charge:=-1, M:=1, adducts:=20.974666)},   ' M + 20.974666	    1-	1.00	 20.974666	 874.305556	 855.345334
                    {"M+Cl", New MzCalculator("[M+Cl]-", charge:=-1, M:=1, adducts:=34.969402)},         ' M + 34.969402	    1-	1.00	 34.969402	 888.300292	 841.350598
                    {"M+K-2H", New MzCalculator("[M+K-2H]-", charge:=-1, M:=1, adducts:=36.948606)},     ' M + 36.948606	    1-	1.00	 36.948606	 890.279496	 839.371394
                    {"M+FA-H", New MzCalculator("[M+FA-H]-", charge:=-1, M:=1, adducts:=44.998201)},     ' M + 44.998201	    1-	1.00	 44.998201	 898.329091	 831.321799
                    {"M+Hac-H", New MzCalculator("[M+Hac-H]-", charge:=-1, M:=1, adducts:=59.013851)},   ' M + 59.013851	    1-	1.00	 59.013851	 912.344741	 817.306149
                    {"M+Br", New MzCalculator("[M+Br]-", charge:=-1, M:=1, adducts:=78.918885)},         ' M + 78.918885	    1-	1.00	 78.918885	 932.249775	 797.401115
                    {"M+TFA-H", New MzCalculator("[M+TFA-H]-", charge:=-1, M:=1, adducts:=112.985586)},  ' M + 112.985586	    1-	1.00	112.985586	 966.316476	 763.334414
                    {"2M-H", New MzCalculator("[2M-H]-", charge:=-1, M:=2, adducts:=-1.007276)},         ' 2M - 1.007276	    1-	2.00	 -1.007276	1705.654504	1753.647276
                    {"2M+FA-H", New MzCalculator("[2M+FA-H]-", charge:=-1, M:=2, adducts:=44.998201)},   ' 2M + 44.998201	    1-	2.00	 44.998201	1751.659981	1707.641799
                    {"2M+Hac-H", New MzCalculator("[2M+Hac-H]-", charge:=-1, M:=2, adducts:=59.013851)}, ' 2M + 59.013851	    1-	2.00	 59.013851	1765.675631	1693.626149
                    {"3M-H", New MzCalculator("[3M-H]-", charge:=-1, M:=3, adducts:=-1.007276)}          ' 3M - 1.007276	    1-	3.00	  1.007276	2560.999946	2627.952724
                }
            End Get
        End Property

        ReadOnly pos As Dictionary(Of String, MzCalculator) = Positive
        ReadOnly neg As Dictionary(Of String, MzCalculator) = Negative

        Public ReadOnly Property Positive(mode As String) As MzCalculator
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return pos.GetValueOrNull(mode)
            End Get
        End Property

        Public ReadOnly Property Negative(mode As String) As MzCalculator
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return neg.GetValueOrNull(mode)
            End Get
        End Property

        Public Function ParseIonMode(mode As String) As Integer
            Select Case LCase(mode)
                Case "+", "1", "p", "pos", "positive"
                    Return 1
                Case "-", "-1", "n", "neg", "negative"
                    Return -1
                Case Else
                    Throw New InvalidExpressionException(mode)
            End Select
        End Function

        ''' <summary>
        ''' 采用Friend访问控制是为了避免被不必要的意外修改出现
        ''' </summary>
        ''' <param name="ion_mode"></param>
        ''' <returns></returns>
        Friend Function Calculator(ion_mode As String) As Dictionary(Of String, MzCalculator)
            ' using cache for internal modules
            If ParseIonMode(ion_mode) = 1 Then
                Return pos
            Else
                Return neg
            End If
        End Function

        Public Function GetCalculator(ion_mode As String) As Dictionary(Of String, MzCalculator)
            If ParseIonMode(ion_mode) = 1 Then
                Return Positive
            Else
                Return Negative
            End If
        End Function
    End Module
End Namespace

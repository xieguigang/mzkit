Imports Microsoft.VisualBasic.Language.C
Imports sys = System.Math

''' <summary>
''' 质谱前体离子计算器
''' </summary>
Public Module PrecursorType

    ''' <summary>
    ''' 返回加和物的m/z数据
    ''' </summary>
    ''' <param name="mass#"></param>
    ''' <param name="adduct#"></param>
    ''' <param name="charge%"></param>
    ''' <returns></returns>
    Public Function AdductMass(mass#, adduct#, charge%) As Double
        Return (mass / sys.Abs(charge) + adduct)
    End Function

    ''' <summary>
    ''' 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
    ''' </summary>
    ''' <param name="precursorMZ#"></param>
    ''' <param name="M#"></param>
    ''' <param name="charge%"></param>
    ''' <param name="adduct#"></param>
    ''' <returns></returns>
    Public Function ReverseMass(precursorMZ#, M#, charge%, adduct#) As Double
        Return ((precursorMZ - adduct) * sys.Abs(charge) / M)
    End Function

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

    Public Function Positive() As Dictionary(Of String, Calculator)

        ' AddKey <- Function(type, charge, M, adducts)
        Dim pos As New Dictionary(Of String, Calculator) From {
            {"M+3H", New Calculator("[M+3H]3+", charge:=3, M:=1, adducts:=1.007276)},                    ' M/3 + 1.007276	    3+	0.33	 1.007276	 285.450906	 291.099391 
            {"M+2H+Na", New Calculator("[M+2H+Na]3+", charge:=3, M:=1, adducts:=8.33459)},               ' M/3 + 8.334590	    3+	0.33	 8.334590	 292.778220	 283.772077
            {"M+H+2Na", New Calculator("[M+H+2Na]3+", charge:=3, M:=1, adducts:=15.76619)},              ' M/3 + 15.7661904	    3+	0.33	15.766190	 300.209820	 276.340476
            {"M+3Na", New Calculator("[M+3Na]3+", charge:=3, M:=1, adducts:=22.989218)},                 ' M/3 + 22.989218	    3+	0.33	22.989218	 307.432848	 269.117449
            {"M+2H", New Calculator("[M+2H]2+", charge:=2, M:=1, adducts:=1.007276)},                    ' M/2 + 1.007276	    2+	0.50	 1.007276	 427.672721	 437.152724
            {"M+H+NH4", New Calculator("[M+H+NH4]2+", charge:=2, M:=1, adducts:=9.52055)},               ' M/2 + 9.520550	    2+	0.50	 9.520550	 436.185995	 428.639450
            {"M+H+Na", New Calculator("[M+H+Na]2+", charge:=2, M:=1, adducts:=11.998247)},               ' M/2 + 11.998247	    2+	0.50	11.998247	 438.663692	 426.161753
            {"M+H+K", New Calculator("[M+H+K]2+", charge:=2, M:=1, adducts:=19.985217)},                 ' M/2 + 19.985217	    2+	0.50	19.985217	 446.650662	 418.174783
            {"M+ACN+2H", New Calculator("[M+ACN+2H]2+", charge:=2, M:=1, adducts:=21.52055)},            ' M/2 + 21.520550	    2+	0.50	21.520550	 448.185995	 416.639450
            {"M+2Na", New Calculator("[M+2Na]2+", charge:=2, M:=1, adducts:=22.989218)},                 ' M/2 + 22.989218	    2+	0.50	22.989218	 449.654663	 415.170782
            {"M+2ACN+2H", New Calculator("[M+2ACN+2H]2+", charge:=2, M:=1, adducts:=42.033823)},         ' M/2 + 42.033823	    2+	0.50	42.033823	 468.699268	 396.126177
            {"M+3ACN+2H", New Calculator("[M+3ACN+2H]2+", charge:=2, M:=1, adducts:=62.547097)},         ' M/2 + 62.547097	    2+	0.50	62.547097	 489.212542	 375.612903
            {"M+H", New Calculator("[M+H]+", charge:=1, M:=1, adducts:=1.007276)},                       '  M + 1.007276	    1+	1.00	 1.007276	 854.338166	 875.312724
            {"M+NH4", New Calculator("[M+NH4]+", charge:=1, M:=1, adducts:=18.033823)},                  '  M + 18.033823	    1+	1.00	18.033823	 871.364713	 858.286177
            {"M+Na", New Calculator("[M+Na]+", charge:=1, M:=1, adducts:=22.989218)},                    '  M + 22.989218	    1+	1.00	22.989218	 876.320108	 853.330782
            {"M+CH3OH+H", New Calculator("[M+CH3OH+H]+", charge:=1, M:=1, adducts:=33.033489)},          '  M + 33.033489	    1+	1.00	33.033489	 886.364379	 843.286511
            {"M+K", New Calculator("[M+K]+", charge:=1, M:=1, adducts:=38.963158)},                      '  M + 38.963158	    1+	1.00	38.963158	 892.294048	 837.356842
            {"M+ACN+H", New Calculator("[M+ACN+H]+", charge:=1, M:=1, adducts:=42.033823)},              '  M + 42.033823	    1+	1.00	42.033823	 895.364713	 834.286177
            {"M+2Na-H", New Calculator("[M+2Na-H]+", charge:=1, M:=1, adducts:=44.97116)},               '  M + 44.971160	    1+	1.00	44.971160	 898.302050	 831.348840
            {"M+IsoProp+H", New Calculator("[M+IsoProp+H]+", charge:=1, M:=1, adducts:=61.06534)},       '  M + 61.06534	    1+	1.00	61.065340	 914.396230	 815.254660
            {"M+ACN+Na", New Calculator("[M+ACN+Na]+", charge:=1, M:=1, adducts:=64.015765)},            '  M + 64.015765	    1+	1.00	64.015765	 917.346655	 812.304235
            {"M+2K-H", New Calculator("[M+2K-H]+", charge:=1, M:=1, adducts:=76.91904)},                 '  M + 76.919040	    1+	1.00	76.919040	 930.249930	 799.400960
            {"M+DMSO+H", New Calculator("[M+DMSO+H]+", charge:=1, M:=1, adducts:=79.02122)},             '  M + 79.02122	    1+	1.00	79.021220	 932.352110	 797.298780
            {"M+2ACN+H", New Calculator("[M+2ACN+H]+", charge:=1, M:=1, adducts:=83.06037)},             '  M + 83.060370	    1+	1.00	83.060370	 936.391260	 793.259630
            {"M+IsoProp+Na+H", New Calculator("[M+IsoProp+Na+H]+", charge:=1, M:=1, adducts:=84.05511)}, ' 	M + 84.05511	    1+	1.00	84.055110	 937.386000	 792.264890
            {"2M+H", New Calculator("[2M+H]+", charge:=1, M:=2, adducts:=1.007276)},                     ' 2M + 1.007276	    1+	2.00	 1.007276	1707.669056	1751.632724
            {"2M+NH4", New Calculator("[2M+NH4]+", charge:=1, M:=2, adducts:=18.033823)},                ' 2M + 18.033823	    1+	2.00	18.033823	1724.695603	1734.606177
            {"2M+Na", New Calculator("[2M+Na]+", charge:=1, M:=2, adducts:=22.989218)},                  ' 2M + 22.989218	    1+	2.00	22.989218	1729.650998	1729.650782
            {"2M+K", New Calculator("[2M+K]+", charge:=1, M:=2, adducts:=38.963158)},                    ' 2M + 38.963158	    1+	2.00	38.963158	1745.624938	1713.676842
            {"2M+ACN+H", New Calculator("[2M+ACN+H]+", charge:=1, M:=2, adducts:=42.033823)},            ' 2M + 42.033823	    1+	2.00	42.033823	1748.695603	1710.606177
            {"2M+ACN+Na", New Calculator("[2M+ACN+Na]+", charge:=1, M:=2, adducts:=64.015765)}           ' 2M + 64.015765	    1+	2.00	64.015765	1770.677545	1688.624235
        }

        Return (pos)
    End Function

    Public Function Negative() As Dictionary(Of String, Calculator)
        Dim neg As New Dictionary(Of String, Calculator) From {
            {"M-3H", New Calculator("[M-3H]3-", charge:=-3, M:=1, adducts:=-1.007276)},        ' M/3 - 1.007276	    3-	0.33	 -1.007276	 283.436354	 293.113943
            {"M-2H", New Calculator("[M-2H]2-", charge:=-2, M:=1, adducts:=-1.007276)},        ' M/2 - 1.007276	    2-	0.50	 -1.007276	 425.658169	 439.167276
            {"M-H2O-H", New Calculator("[M-H2O-H]-", charge:=-1, M:=1, adducts:=-19.01839)},   ' M - 19.01839	    1-	1.00	-19.01839	 834.312500	 895.338390
            {"M-H", New Calculator("[M-H]-", charge:=-1, M:=1, adducts:=-1.007276)},           ' M - 1.007276	    1-	1.00	 -1.007276	 852.323614	 877.327276
            {"M+Na-2H", New Calculator("[M+Na-2H]-", charge:=-1, M:=1, adducts:=20.974666)},   ' M + 20.974666	    1-	1.00	 20.974666	 874.305556	 855.345334
            {"M+Cl", New Calculator("[M+Cl]-", charge:=-1, M:=1, adducts:=34.969402)},         ' M + 34.969402	    1-	1.00	 34.969402	 888.300292	 841.350598
            {"M+K-2H", New Calculator("[M+K-2H]-", charge:=-1, M:=1, adducts:=36.948606)},     ' M + 36.948606	    1-	1.00	 36.948606	 890.279496	 839.371394
            {"M+FA-H", New Calculator("[M+FA-H]-", charge:=-1, M:=1, adducts:=44.998201)},     ' M + 44.998201	    1-	1.00	 44.998201	 898.329091	 831.321799
            {"M+Hac-H", New Calculator("[M+Hac-H]-", charge:=-1, M:=1, adducts:=59.013851)},   ' M + 59.013851	    1-	1.00	 59.013851	 912.344741	 817.306149
            {"M+Br", New Calculator("[M+Br]-", charge:=-1, M:=1, adducts:=78.918885)},         ' M + 78.918885	    1-	1.00	 78.918885	 932.249775	 797.401115
            {"M+TFA-H", New Calculator("[M+TFA-H]-", charge:=-1, M:=1, adducts:=112.985586)},  ' M + 112.985586	    1-	1.00	112.985586	 966.316476	 763.334414
            {"2M-H", New Calculator("[2M-H]-", charge:=-1, M:=2, adducts:=-1.007276)},         ' 2M - 1.007276	    1-	2.00	 -1.007276	1705.654504	1753.647276
            {"2M+FA-H", New Calculator("[2M+FA-H]-", charge:=-1, M:=2, adducts:=44.998201)},   ' 2M + 44.998201	    1-	2.00	 44.998201	1751.659981	1707.641799
            {"2M+Hac-H", New Calculator("[2M+Hac-H]-", charge:=-1, M:=2, adducts:=59.013851)}, ' 2M + 59.013851	    1-	2.00	 59.013851	1765.675631	1693.626149
            {"3M-H", New Calculator("[3M-H]-", charge:=-1, M:=3, adducts:=-1.007276)}          ' 3M - 1.007276	    1-	3.00	  1.007276	2560.999946	2627.952724
        }
        Return (neg)
    End Function

    Public Const no_result$ = "Unknown"

    ''' <summary>
    ''' 分子量差值
    ''' </summary>
    ''' <param name="measured#"></param>
    ''' <param name="actualValue#"></param>
    ''' <returns></returns>
    Public Function ppm(measured#, actualValue#) As Double
        ' （测量值-实际分子量）/实际分子量
        ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
        Dim ppmd# = sys.Abs(measured - actualValue) / actualValue
        ppmd = ppmd * 1000000
        Return ppmd
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="chargeMode$">+/-</param>
    ''' <param name="charge%"></param>
    ''' <param name="PrecursorType$"></param>
    ''' <returns></returns>
    Public Function CalcMass(chargeMode$, charge%, PrecursorType$) As Func(Of Double, Double)
        If (PrecursorType = "[M]+" OrElse PrecursorType = "[M]-") Then
            Return (Function(x) x)
        End If

        Dim mode As Dictionary(Of String, Calculator) = calculator(chargeMode)
        Dim found As Calculator = Nothing

        For Each cacl In mode.Values
            If (cacl.Name = PrecursorType) Then
                found = cacl
                Exit For
            End If
        Next

        If found.Name.StringEmpty Then
            Return Nothing
        Else
            Return AddressOf found.CalcMass
        End If
    End Function

    ReadOnly calculator As New Dictionary(Of String, Dictionary(Of String, Calculator)) From {
        {"+", Positive()},
        {"-", Negative()}
    }

    ''' <summary>
    ''' 计算出前体离子的加和模式
    ''' </summary>
    ''' <param name="mass#">分子质量</param>
    ''' <param name="precursorMZ#">前体的m/z</param>
    ''' <param name="charge%">电荷量</param>
    ''' <param name="chargeMode$">极性</param>
    ''' <param name="minError_ppm#">所能够容忍的质量误差</param>
    ''' <param name="debugEcho"></param>
    ''' <returns></returns>
    Public Function FindPrecursorType(mass#, precursorMZ#, charge%,
                                      Optional chargeMode$ = "+",
                                      Optional minError_ppm# = 100,
                                      Optional debugEcho As Boolean = True) As (ppm#, type$)
        If (charge = 0) Then
            println("I can't calculate the ionization mode for no charge(charge = 0)!")
            Return (Double.NaN, no_result)
        End If

        If (mass.IsNaNImaginary OrElse precursorMZ.IsNaNImaginary) Then
            println("  ****** mass='%s' or precursor_M/Z='%s' is an invalid value!", mass, precursorMZ)
            Return (Double.NaN, no_result)
        End If

        Dim ppm = PrecursorType.ppm(precursorMZ, mass / sys.Abs(charge))

        If (ppm <= 500) Then
            ' 本身的分子质量和前体的mz一样，说明为[M]类型
            If (sys.Abs(charge) = 1) Then
                Return (ppm, "[M]" & chargeMode)
            Else
                Return (ppm, sprintf("[M]%s%s", charge, chargeMode))
            End If
        End If

        ' 每一个模式都计算一遍，然后返回最小的ppm差值结果
        Dim min = 999999
        Dim minType$ = Nothing

        ' 得到某一个离子模式下的计算程序
        Dim mode As Dictionary(Of String, Calculator) = calculator(chargeMode)

        If (chargeMode = "-") Then
            ' 对于负离子模式而言，虽然电荷量是负数的，但是使用xcms解析出来的却是一个电荷数的绝对值
            ' 所以需要判断一次，乘以-1 
            If (charge > 0) Then
                charge = -1 * charge
            End If
        End If

        ' 然后遍历这个模式下的所有离子前体计算
        For Each calc In mode.Values
            Dim ptype = calc.Name

            ' 跳过电荷数不匹配的离子模式计算表达式
            If (charge <> calc.charge) Then
                Continue For
            End If

            ' 这里实际上是根据数据库之中的分子质量，通过前体离子的质量计算出mz结果
            ' 然后计算mz计算结果和precursorMZ的ppm信息
            Dim massReverse = calc.CalcMass(precursorMZ)
            Dim deltappm = PrecursorType.ppm(massReverse, actualValue:=mass)

            If (debugEcho) Then
                println("%s - %s = %s(ppm), type=%s", mass, massReverse, deltappm, ptype)
            End If

            ' 根据质量计算出前体质量，然后计算出差值
            If (deltappm < min) Then
                min = deltappm
                minType = ptype
            End If
        Next

        ' 假若这个最小的ppm差值符合误差范围，则认为找到了一个前体模式
        If (debugEcho) Then
            println("  ==> %s", minType)
        End If

        If (min <= minError_ppm) Then
            Return (min, minType)
        Else
            If (debugEcho) Then
                println("But the '%s' ionization mode its ppm error (%s ppm) is ", minType, min)
                println("not satisfy the minError requirement(%s), returns Unknown!", minError_ppm)
            End If

            Return (-1, no_result)
        End If
    End Function
End Module

Public Structure Calculator

    Dim Name$
    Dim charge%
    Dim M%
    Dim adducts#

    Sub New(type$, charge%, M#, adducts#)
        Me.Name = type
        Me.charge = charge
        Me.M = M
        Me.adducts = adducts
    End Sub

    Public Function CalcMass(precursorMZ#) As Double
        Return (ReverseMass(precursorMZ, M, charge, adducts))
    End Function

    Public Function CalcPrecursorMZ(mass#) As Double
        Return (AdductMass(mass, adducts, charge))
    End Function
End Structure
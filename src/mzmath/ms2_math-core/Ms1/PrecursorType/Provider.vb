#Region "Microsoft.VisualBasic::5098426fc64689e1e3e18fcb821a0b76, mzmath\ms2_math-core\Ms1\PrecursorType\Provider.vb"

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

    '   Total Lines: 349
    '    Code Lines: 173 (49.57%)
    ' Comment Lines: 133 (38.11%)
    '    - Xml Docs: 48.87%
    ' 
    '   Blank Lines: 43 (12.32%)
    '     File Size: 16.22 KB


    '     Module Provider
    ' 
    '         Properties: Negative, Negatives, Positive, Positives
    ' 
    '         Function: (+2 Overloads) Calculators, (+2 Overloads) GetCalculator, ParseAdductModel, ParseAdducts, ParseCalculatorInternal
    '                   (+2 Overloads) ParseIonMode, TryAdductPolarityParserInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

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

        Public ReadOnly Property Positives() As MzCalculator()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return PositiveFormula.GetFormulas _
                    .Values _
                    .ToArray
            End Get
        End Property

        Public ReadOnly Property Negatives() As MzCalculator()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return NegativeFormula.GetFormulas _
                    .Values _
                    .ToArray
            End Get
        End Property

        ReadOnly pos As Dictionary(Of String, MzCalculator) = PositiveFormula.GetFormulas
        ReadOnly neg As Dictionary(Of String, MzCalculator) = NegativeFormula.GetFormulas

        ''' <summary>
        ''' 解析出阳离子模式的加合物形式
        ''' </summary>
        ''' <param name="mode">例如[M]+，[M+H]+等</param>
        ''' <returns>
        ''' 
        ''' </returns>
        Public ReadOnly Property Positive(mode As String) As MzCalculator
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Dim mz = pos.GetValueOrNull(mode)

                If mz Is Nothing Then
                    mz = Parser.ParseMzCalculator(mode, "+")
                End If

                Return mz
            End Get
        End Property

        ''' <summary>
        ''' 解析出阴离子模式的加合物形式
        ''' </summary>
        ''' <param name="mode">例如[M]-，[M-H]-等</param>
        ''' <returns></returns>
        Public ReadOnly Property Negative(mode As String) As MzCalculator
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Dim mz = neg.GetValueOrNull(mode)

                If mz Is Nothing Then
                    mz = Parser.ParseMzCalculator(mode, "-")
                End If

                Return mz
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="precursor_type"></param>
        ''' <returns>
        ''' this function will returns nothing if the given string is empty
        ''' </returns>
        Public Function ParseAdductModel(precursor_type As String) As MzCalculator
            Static cache As New Dictionary(Of String, MzCalculator)
            Return cache.ComputeIfAbsent(
                key:=precursor_type,
                lazyValue:=Function(type)
                               Return ParseCalculatorInternal(type, strict:=False)
                           End Function)
        End Function

        Public Iterator Function ParseAdducts(ParamArray adducts As String()) As IEnumerable(Of MzCalculator)
            For Each type As String In adducts
                Yield ParseAdductModel(type)
            Next
        End Function

        ''' <summary>
        ''' get internal m/z calculator
        ''' </summary>
        ''' <param name="precursor_types"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function has an internal cache hash table for the given input adducts string.
        ''' and the new precursor adducts object will be parsed if it not found inside 
        ''' the internal cache.
        ''' </remarks>
        Public Function Calculators(ParamArray precursor_types As String()) As MzCalculator()
            Return Calculators(precursor_types, strict:=False).ToArray
        End Function

        Const empty_adducts_input As String = "one of the given precursor adducts type string for parsed is empty!"

        Private Function ParseCalculatorInternal(type As String, strict As Boolean) As MzCalculator
            type = Strings.Trim(type)

            If type.StringEmpty(, True) Then
                If strict Then
                    Throw New NullReferenceException(empty_adducts_input)
                Else
                    Call type.Warning
                    Return Nothing
                End If
            End If

            ' get adducts
            ' and parse adducts object from the string value type
            ' if the adducts object is not found in the cached data list
            If type.Last = "+"c Then
                Return Positive(type)
            ElseIf type.Last = "-"c Then
                Return Negative(type)
            Else
                If strict Then
                    ' do nothing?
                    Throw New InvalidExpressionException($"unknown charge mode that could be parsed from the given adducts string: '{type}'!")
                Else
                    Call $"unknown charge mode that could be parsed from the given adducts string: '{type}'! Assuming positive mode for parse adducts model.".Warning

                    If Not (type.First = "["c AndAlso type.Last = "]"c) Then
                        type = $"[{type}]+"
                    End If

                    Return Positive(type)
                End If
            End If
        End Function

        ''' <summary>
        ''' get internal m/z calculator
        ''' </summary>
        ''' <param name="precursor_types"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function has an internal cache hash table for the given input adducts string.
        ''' and the new precursor adducts object will be parsed if it not found inside 
        ''' the internal cache.
        ''' </remarks>
        Public Iterator Function Calculators(precursor_types As String(), strict As Boolean) As IEnumerable(Of MzCalculator)
            Dim adduct As MzCalculator

            For Each type As String In precursor_types.SafeQuery
                adduct = ParseCalculatorInternal(type, strict)

                If Not adduct Is Nothing Then
                    Yield adduct
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ParseIonMode(mode$, ByRef ionMode As IonModes,
                                     Optional allowsUnknown As Boolean = False,
                                     Optional allowAdductParser As Boolean = False,
                                     Optional verbose As Boolean = True) As IonModes

            ionMode = ParseIonMode(mode, allowsUnknown, allowAdductParser, verbose:=verbose)
            Return ionMode
        End Function

        ''' <summary>
        ''' 这个函数返回1或者-1,用来分别对应于阳离子和阴离子
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <param name="allowsUnknown">
        ''' zero will be returns if this parameter value is set to TRUE, otherwise this
        ''' parser function will throw an exception
        ''' </param>
        ''' <param name="allowAdductParser">
        ''' allow parse the ion polarity mode value from the adducts model string
        ''' </param>
        ''' <returns>
        ''' function returns 1(positive) or -1(negative)
        ''' </returns>
        Public Function ParseIonMode(mode$,
                                     Optional allowsUnknown As Boolean = False,
                                     Optional allowAdductParser As Boolean = False,
                                     Optional verbose As Boolean = True) As IonModes

            If allowsUnknown AndAlso (mode Is Nothing OrElse mode.StringEmpty(, True)) Then
                Return IonModes.Unknown
            End If

            Select Case LCase(mode)
                Case "+", "1", "p", "pos", "positive"
                    Return IonModes.Positive
                Case "-", "-1", "n", "neg", "negative"
                    Return IonModes.Negative
                Case Else
                    Static unknown As New Dictionary(Of String, IonModes)

                    If Not unknown.ContainsKey(mode) Then
                        Dim pol As IonModes = mode.TryAdductPolarityParserInternal(
                            allowsUnknown,
                            allowAdductParser,
                            verbose
                        )

                        SyncLock unknown
                            unknown(mode) = pol
                        End SyncLock
                    End If

                    Return unknown(mode)
            End Select
        End Function

        <Extension>
        Private Function TryAdductPolarityParserInternal(mode As String,
                                                         allowsUnknown As Boolean,
                                                         allow_adduct_parser As Boolean,
                                                         verbose As Boolean) As IonModes
            Dim msg As String

            If mode.StringEmpty Then
                msg = "the given ion mode string is empty!"
            ElseIf allow_adduct_parser AndAlso mode.IsPattern("\[.+\].*[+-]") Then
                msg = $"parse the ion polarity mode from the precursor adducts: {mode}!"

                If verbose Then
                    Call VBDebugger.EchoLine(msg)
                End If

                If mode.Last = "+"c Then
                    Return IonModes.Positive
                Else
                    Return IonModes.Negative
                End If
            Else
                msg = $"unsure how to parse the given string '{mode}' as ion mode!"
            End If

            If verbose Then
                Call VBDebugger.WriteLine("InvalidExpressionException: " & msg)
            End If

            If allowsUnknown Then
                Return IonModes.Unknown
            Else
                Throw New InvalidExpressionException(msg)
            End If
        End Function

        Public Function GetCalculator(ion_mode As IonModes) As Dictionary(Of String, MzCalculator)
            If ion_mode = IonModes.Positive Then
                Return pos
            Else
                Return neg
            End If
        End Function

        ''' <summary>
        ''' Get the internal default adducts data set via parse the ion polarity value from a given string
        ''' </summary>
        ''' <param name="ion_mode">any character string that could 
        ''' be parse a <see cref="IonModes"/> value via the 
        ''' <see cref="ParseIonMode"/> function.
        ''' </param>
        ''' <returns></returns>
        Public Function GetCalculator(ion_mode As String, Optional verbose As Boolean = False) As Dictionary(Of String, MzCalculator)
            If ParseIonMode(ion_mode, verbose:=verbose) = 1 Then
                Return pos
            Else
                Return neg
            End If
        End Function
    End Module
End Namespace

﻿#Region "Microsoft.VisualBasic::ef2239175080cb8f1c53c894756d086a, mzmath\ms2_math-core\Ms1\PrecursorType\TypeMatch.vb"

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

    '   Total Lines: 37
    '    Code Lines: 20 (54.05%)
    ' Comment Lines: 12 (32.43%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (13.51%)
    '     File Size: 1.13 KB


    '     Structure TypeMatch
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' The precursor type matches result between the ion m/z value and the metabolite exact mass value.
    ''' </summary>
    Public Structure TypeMatch

        ''' <summary>
        ''' The mass error
        ''' </summary>
        Dim errors As Double
        ''' <summary>
        ''' the precursor type adducts matched result in string name
        ''' </summary>
        Dim precursorType As String
        Dim message As String
        ''' <summary>
        ''' the precursor type model object
        ''' </summary>
        Dim adducts As MzCalculator

        Public Overrides Function ToString() As String
            If Not message.StringEmpty Then
                Return message
            Else
                Return precursorType
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(m As TypeMatch) As MzCalculator
            Return m.adducts
        End Operator
    End Structure
End Namespace

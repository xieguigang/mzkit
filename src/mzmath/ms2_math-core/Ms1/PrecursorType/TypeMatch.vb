#Region "Microsoft.VisualBasic::ecf3b1f41d16e6bdee4cf31d88d99a48, mzkit\src\mzmath\ms2_math-core\Ms1\PrecursorType\TypeMatch.vb"

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

'   Total Lines: 13
'    Code Lines: 7
' Comment Lines: 3
'   Blank Lines: 3
'     File Size: 267 B


'     Structure TypeMatch
' 
' 
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

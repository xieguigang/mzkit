#Region "Microsoft.VisualBasic::9f93a8ed12ae3c7febbafe5a60e69ee6, src\mzmath\ms2_math-core\Ms1\PolarityData.vb"

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

    ' Class PolarityData
    ' 
    '     Properties: negative, positive
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Public Class PolarityData(Of T)

    Public Property positive As T
    Public Property negative As T

    Default Public Property Item(ionMode As String) As T
        Get
            If Provider.ParseIonMode(ionMode) = 1 Then
                Return positive
            Else
                Return negative
            End If
        End Get
        Set
            If Provider.ParseIonMode(ionMode) = 1 Then
                positive = Value
            Else
                negative = Value
            End If
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return $"(+) {positive} / (-) {negative}"
    End Function

    Public Shared Widening Operator CType(tuple As (pos As T, neg As T)) As PolarityData(Of T)
        Return New PolarityData(Of T) With {
            .positive = tuple.pos,
            .negative = tuple.neg
        }
    End Operator
End Class

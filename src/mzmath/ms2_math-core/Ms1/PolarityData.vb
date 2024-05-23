#Region "Microsoft.VisualBasic::86fab24cf6a0e479de044f741bee9923, mzmath\ms2_math-core\Ms1\PolarityData.vb"

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

    '   Total Lines: 56
    '    Code Lines: 44 (78.57%)
    ' Comment Lines: 4 (7.14%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.29%)
    '     File Size: 1.50 KB


    ' Class PolarityData
    ' 
    '     Properties: negative, positive
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

''' <summary>
''' tuple data model of the polarity data
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class PolarityData(Of T)

    Public Property positive As T
    Public Property negative As T

    Default Public Property Item(ionMode As String) As T
        Get
            Return Me(Provider.ParseIonMode(ionMode))
        End Get
        Set
            Me(Provider.ParseIonMode(ionMode)) = Value
        End Set
    End Property

    Default Public Property Item(ionMode As IonModes) As T
        Get
            If ionMode = IonModes.Positive Then
                Return positive
            Else
                Return negative
            End If
        End Get
        Set(value As T)
            If ionMode = IonModes.Positive Then
                positive = value
            Else
                negative = value
            End If
        End Set
    End Property

    Sub New()
    End Sub

    Sub New(pos As T, neg As T)
        positive = pos
        negative = neg
    End Sub

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

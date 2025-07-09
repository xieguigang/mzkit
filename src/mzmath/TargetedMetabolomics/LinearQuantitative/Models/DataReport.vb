#Region "Microsoft.VisualBasic::edbe672f4a71e18395aa049c0178aee1, mzmath\TargetedMetabolomics\LinearQuantitative\Models\DataReport.vb"

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

'   Total Lines: 29
'    Code Lines: 21 (72.41%)
' Comment Lines: 3 (10.34%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 5 (17.24%)
'     File Size: 847 B


'     Class DataReport
' 
'         Properties: ID, ISTD, linear, name, R2
'                     samples
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports std = System.Math

Namespace LinearQuantitative

    ''' <summary>
    ''' table model for the linear quantification result
    ''' </summary>
    Public Class DataReport

        Public Property ID As String
        Public Property name As String
        Public Property ISTD As String
        Public Property linear As String
        Public Property R2 As Double
        Public ReadOnly Property R As Double
            Get
                Return std.Sqrt(R2)
            End Get
        End Property
        Public Property invalids As String()
        Public Property [variant] As Double
        Public Property samples As Dictionary(Of String, Double)

        Default Public Property Value(name As String) As Double
            Get
                Return samples.TryGetValue(name)
            End Get
            Set(value As Double)
                samples(name) = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"f({ID}) = {linear}"
        End Function

    End Class
End Namespace

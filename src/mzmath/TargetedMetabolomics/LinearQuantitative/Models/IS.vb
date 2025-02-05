﻿#Region "Microsoft.VisualBasic::f783e30291be14c3bab994ab7a66c112, mzmath\TargetedMetabolomics\LinearQuantitative\Models\IS.vb"

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

    '   Total Lines: 35
    '    Code Lines: 21 (60.00%)
    ' Comment Lines: 7 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (20.00%)
    '     File Size: 1011 B


    '     Class [IS]
    ' 
    '         Properties: CIS, ID, name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CheckIsEmpty, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace LinearQuantitative

    ''' <summary>
    ''' 内标
    ''' </summary>
    Public Class [IS]

        <XmlAttribute> Public Property ID As String
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' 内标的浓度
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property CIS As Double

        Sub New()
        End Sub

        Sub New(id As String, Optional name As String = Nothing)
            Me.ID = id
            Me.name = If(name, id)
            Me.CIS = 1
        End Sub

        Public Function CheckIsEmpty() As Boolean
            Return (ID.StringEmpty(, True) OrElse ID.TextEquals("None")) AndAlso CIS <= 0 AndAlso (name.StringEmpty(, True) OrElse name.TextEquals("None"))
        End Function

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {ID} = {CIS}"
        End Function
    End Class
End Namespace

﻿#Region "Microsoft.VisualBasic::407840dfc0da2d6fea5318d6e1b55ded, metadb\Chemoinformatics\SDF\Struct\Point3D.vb"

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

'   Total Lines: 19
'    Code Lines: 14 (73.68%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 5 (26.32%)
'     File Size: 372 B


'     Class Point3D
' 
'         Properties: X, Y, Z
' 
'         Constructor: (+2 Overloads) Sub New
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace SDF.Models

    ''' <summary>
    ''' [x,y,z]
    ''' </summary>
    Public Class Point3D

        <XmlAttribute> Public Property X As Double
        <XmlAttribute> Public Property Y As Double
        <XmlAttribute> Public Property Z As Double

        Sub New()
        End Sub

        Sub New(x As Double, y As Double, z As Double)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{X}, {Y}, {Z}]"
        End Function

    End Class
End Namespace

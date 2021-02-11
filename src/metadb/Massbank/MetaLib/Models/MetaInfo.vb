#Region "Microsoft.VisualBasic::c5051f471a9130f8a3d36781dcc666b7, Massbank\MetaLib\Models\MetaInfo.vb"

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

    '     Class MetaInfo
    ' 
    '         Properties: exact_mass, formula, ID, name, synonym
    '                     xref
    ' 
    '         Function: Equals, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetaLib.Models

    Public Class MetaInfo : Implements INamedValue
        Implements IEquatable(Of MetaInfo)

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property formula As String
        <XmlAttribute> Public Property exact_mass As Double

        Public Property name As String

        <XmlElement>
        Public Property synonym As String()

        Public Property xref As xref

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Overloads Function Equals(other As MetaInfo) As Boolean Implements IEquatable(Of MetaInfo).Equals
            Static metaEquals As MetaEquals

            If metaEquals Is Nothing Then
                metaEquals = New MetaEquals
            End If

            If other Is Nothing Then
                Return False
            Else
                Return metaEquals.Equals(Me, other)
            End If
        End Function
    End Class
End Namespace

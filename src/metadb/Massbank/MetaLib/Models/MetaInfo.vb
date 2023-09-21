#Region "Microsoft.VisualBasic::9f10aeb6c0f37d04452ce16e426a9092, mzkit\src\metadb\Massbank\MetaLib\Models\MetaInfo.vb"

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

'   Total Lines: 45
'    Code Lines: 32
' Comment Lines: 4
'   Blank Lines: 9
'     File Size: 1.73 KB


'     Class MetaInfo
' 
'         Properties: description, exact_mass, formula, ID, IUPACName
'                     name, synonym, xref
' 
'         Function: Equals, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace MetaLib.Models

    Public Class MetaInfo : Implements INamedValue
        Implements IEquatable(Of MetaInfo)

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <MessagePackMember(1)> <XmlAttribute> Public Property formula As String
        <MessagePackMember(2)> <XmlAttribute> Public Property exact_mass As Double

        <MessagePackMember(3)> Public Property name As String
        <MessagePackMember(4)> Public Property IUPACName As String
        <MessagePackMember(5)> Public Property description As String
        <XmlElement>
        <MessagePackMember(6)> Public Property synonym As String()

        ''' <summary>
        ''' The database cross reference of current metabolite and the molecule structure data.
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(7)> Public Property xref As xref

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToSimpleAnnotation() As MetaboliteAnnotation
            Return New MetaboliteAnnotation With {
                .Id = ID,
                .CommonName = name,
                .ExactMass = exact_mass,
                .Formula = formula
            }
        End Function
    End Class
End Namespace

﻿#Region "Microsoft.VisualBasic::8a956d89e913007d71633573380a391b, metadb\Massbank\Public\NCBI\PubChem\Web\Models\Information.vb"

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

    '   Total Lines: 221
    '    Code Lines: 176 (79.64%)
    ' Comment Lines: 11 (4.98%)
    '    - Xml Docs: 90.91%
    ' 
    '   Blank Lines: 34 (15.38%)
    '     File Size: 8.06 KB


    '     Class Value
    ' 
    '         Properties: [Boolean], DateISO8601, ExternalDataURL, MimeType, Number
    '                     StringWithMarkup, Unit
    ' 
    '     Class StringWithMarkup
    ' 
    '         Properties: [String], Markups
    ' 
    '         Function: ToString
    ' 
    '     Class Markup
    ' 
    '         Properties: Extra, Length, Start, Type, URL
    ' 
    '     Class Information
    ' 
    '         Properties: Description, ExternalDataMimeType, ExternalDataURL, InfoType, InfoValue
    '                     Reference, ReferenceNumber, Table, UnitValue, URL
    '                     Value
    ' 
    '         Function: ToString
    ' 
    '     Class Table
    ' 
    '         Properties: ColumnNames, ExternalTableName, Rows
    ' 
    '         Function: ToDictionary, ToString
    ' 
    '     Class Row
    ' 
    '         Properties: Cells
    ' 
    '     Class Reference
    ' 
    '         Properties: ANID, Description, IsToxnet, Name, ReferenceNumber
    '                     SourceID, SourceName, URL
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports any = Microsoft.VisualBasic.Scripting

Namespace NCBI.PubChem

    ''' <summary>
    ''' A common value model between the json/xml file
    ''' </summary>
    Public Class Value

        <XmlElement("StringWithMarkup")>
        Public Property StringWithMarkup As StringWithMarkup()
        Public Property ExternalDataURL As String
        Public Property MimeType As String
        Public Property Number As Double?
        Public Property Unit As String
        Public Property DateISO8601 As String
        Public Property [Boolean] As Boolean

    End Class

    Public Class StringWithMarkup

        Public Property [String] As String
        <XmlElement("Markup")>
        Public Property Markups As Markup()

        Public Overrides Function ToString() As String
            Return Me.String
        End Function

    End Class

    Public Class Markup
        Public Property Start As Integer
        Public Property Length As Integer
        Public Property URL As String
        Public Property Type As String
        Public Property Extra As String
    End Class

    Public Class Information

        Public Property ReferenceNumber As String
        Public Property Description As String
        Public Property Reference As String
        Public Property Value As Value
        <XmlElement("StringValueList")>
        Public Property Table As Table
        Public Property URL As String
        Public Property ExternalDataURL As String
        Public Property ExternalDataMimeType As String

        Public ReadOnly Property UnitValue As UnitValue
            Get
                If Value.Unit.StringEmpty Then
                    Dim str As String

                    If InfoValue Is Nothing Then
                        str = ""
                    ElseIf InfoValue.GetType.IsArray Then
                        str = DirectCast(InfoValue, Array) _
                            .AsObjectEnumerator _
                            .Select(Function(o) any.ToString(o)) _
                            .JoinBy("; ")
                    Else
                        str = CStr(InfoValue)
                    End If

                    Return New UnitValue With {
                        .value = Me.GetInformationNumber,
                        .unit = Value.Unit,
                        .reference = Reference,
                        .condition = str
                    }
                Else
                    Return New UnitValue With {
                        .value = Me.GetInformationNumber,
                        .unit = Value.Unit,
                        .reference = Reference
                    }
                End If
            End Get
        End Property

        ''' <summary>
        ''' Try get data type of the information its <see cref="Value"/>
        ''' </summary>
        ''' <returns>
        ''' this function will returns nothing if the given <see cref="Value"/> is nothing
        ''' </returns>
        Public ReadOnly Property InfoType As Type
            Get
                If Value Is Nothing Then
                    Return Nothing
                End If

                If Not Value.Number Is Nothing Then
                    Return GetType(Double)
                ElseIf Not Value.StringWithMarkup.IsNullOrEmpty AndAlso Value.StringWithMarkup.Length = 1 Then
                    Return GetType(String)
                ElseIf Not Value.StringWithMarkup.IsNullOrEmpty Then
                    Return GetType(String())
                ElseIf Not Value.DateISO8601.StringEmpty Then
                    Return GetType(Date)
                Else
                    Return GetType(Boolean)
                End If
            End Get
        End Property

        ''' <summary>
        ''' this method will returns nothing if the source <see cref="Value"/> is nothing
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property InfoValue As Object
            Get
                Dim type As Type = InfoType

                If type Is Nothing Then
                    Return Nothing
                End If

                Select Case type
                    Case GetType(Double)
                        Return Val(Value.Number)
                    Case GetType(String)
                        Return Value.StringWithMarkup.First.String
                    Case GetType(String())
                        Return Value.StringWithMarkup _
                            .Select(Function(v) v.String) _
                            .ToArray
                    Case GetType(Date)
                        Return Date.Parse(Value.DateISO8601)
                    Case GetType(Boolean)
                        Return Value.Boolean
                    Case Else
                        Throw New NotImplementedException(type.FullName)
                End Select
            End Get
        End Property

        Public Overrides Function ToString() As String
            If InfoType Is GetType(String()) Then
                Return $"({InfoType.FullName}) {DirectCast(InfoValue, String()).GetJson}"
            Else
                Return $"({InfoType.FullName}) {InfoValue}"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(info As Information) As NamedValue(Of Object)
            Return New NamedValue(Of Object)(info.ReferenceNumber, info.InfoValue, info.Description)
        End Operator
    End Class

    Public Class Table

        Public Property ExternalTableName As String

        <XmlElement("ColumnName")>
        Public Property ColumnNames As String()
        <XmlElement("Row")>
        Public Property Rows As Row()

        Public Shared Function ToDictionary(table As Table) As Dictionary(Of NamedValue(Of String))
            If table Is Nothing OrElse table.ColumnNames.IsNullOrEmpty Then
                Call "Empty table data!".Warning
                ' return a empty dictionary table
                Return New Dictionary(Of NamedValue(Of String))
            ElseIf table.ColumnNames.Length > 2 Then
                Call $"Target table is not a key-value pair! (columns={table.ColumnNames.Length} > 2)".Warning
            End If

            Return table.Rows _
                .Select(Function(r)
                            Return New NamedValue(Of String) With {
                                .Name = r.Cells(0).Value.StringWithMarkup.First.String,
                                .Value = Scripting.ToString(r.Cells(1).InfoValue),
                                .Description = r.Cells(1).Value.Unit
                            }
                        End Function) _
                .ToDictionary
        End Function

        Public Overrides Function ToString() As String
            If ColumnNames.IsNullOrEmpty Then
                Return ExternalTableName
            Else
                Return ColumnNames.GetJson
            End If
        End Function

    End Class

    Public Class Row

        <XmlElement("Cell")>
        Public Property Cells As Information()

    End Class

    Public Class Reference

        Public Property ReferenceNumber As String
        Public Property SourceName As String
        Public Property SourceID As String
        Public Property Name As String
        Public Property URL As String
        Public Property Description As String
        Public Property IsToxnet As String
        Public Property ANID As String

        Public Overrides Function ToString() As String
            Return $"{Name} ({URL})"
        End Function
    End Class
End Namespace

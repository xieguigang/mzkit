#Region "Microsoft.VisualBasic::00a7ffdd6f396a54912fb50e70012ee9, Massbank\Public\NCBI\PubChem\Web\Models\InformationSection.vb"

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

    '     Class InformationSection
    ' 
    '         Properties: Sections
    ' 
    '     Class Section
    ' 
    '         Properties: Description, DisplayControls, HintEmbeddedHTML, HintGroupSubsectionsByReference, HintShowAtMost
    '                     HintSortByLength, Information, TOCHeading
    ' 
    '         Function: ToString
    ' 
    '     Class DisplayControls
    ' 
    '         Properties: CreateTable, ShowAtMost
    ' 
    '     Class CreateTable
    ' 
    '         Properties: ColumnContents, FromInformationIn, NumberOfColumns
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace NCBI.PubChem

    ''' <summary>
    ''' 类似于Folder
    ''' </summary>
    Public MustInherit Class InformationSection

        Dim sectionTable As Dictionary(Of String, Section)

        <XmlElement("Section")>
        Public Property Sections As Section()
            Get
                Return sectionTable.Values.ToArray
            End Get
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Set(value As Section())
                If value.IsNullOrEmpty Then
                    sectionTable = New Dictionary(Of String, Section)
                Else
                    sectionTable = value _
                        .ToDictionary(Function(sec)
                                          Return sec.TOCHeading
                                      End Function)
                End If
            End Set
        End Property

        Default Public ReadOnly Property Section(name As String) As Section
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return sectionTable.TryGetValue(name)
            End Get
        End Property
    End Class

    Public Class Section : Inherits InformationSection

        Public Property TOCHeading As String
        Public Property Description As String
        Public Property HintGroupSubsectionsByReference As Boolean
        Public Property HintEmbeddedHTML As Boolean
        Public Property HintShowAtMost As String
        Public Property HintSortByLength As Boolean
        Public Property DisplayControls As DisplayControls

        <XmlElement("Information")>
        Public Property Information As Information()

        Public Overrides Function ToString() As String
            Return $"[{TOCHeading}]  {Description}"
        End Function
    End Class

    Public Class DisplayControls
        Public Property CreateTable As CreateTable
        Public Property ShowAtMost As String
    End Class

    Public Class CreateTable
        Public Property FromInformationIn As String
        Public Property NumberOfColumns As Integer
        <XmlElement("ColumnContents")>
        Public Property ColumnContents As String()
    End Class
End Namespace

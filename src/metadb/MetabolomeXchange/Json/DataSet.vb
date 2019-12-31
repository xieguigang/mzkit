#Region "Microsoft.VisualBasic::e863a3170bb8489711590f84ff384667, DATA\MetabolomeXchange\Json\DataSet.vb"

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

    '     Class DataSet
    ' 
    '         Properties: [date], accession, description, meta, publications
    '                     submitter, timestamp, title, url
    ' 
    '         Function: ToString
    ' 
    '     Class meta
    ' 
    '         Properties: analysis, metabolites, organism, organism_parts, platform
    ' 
    '         Function: ToString
    ' 
    '     Class publication
    ' 
    '         Properties: doi, pubmed, title
    ' 
    '         Function: ToString
    ' 
    '     Class response
    ' 
    '         Properties: datasets, description, name, url
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Json

    Public Class DataSet

        Public Property description As String()
        Public Property accession As String
        Public Property url As String
        Public Property title As String
        Public Property [date] As String
        Public Property timestamp As Long
        Public Property submitter As String()
        Public Property publications As publication()
        Public Property meta As meta

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Class

    <KnownType(GetType(String()))>
    <KnownType(GetType(String))>
    Public Class meta

        Public Property analysis As String
        Public Property platform As String
        Public Property organism As Object
        Public Property organism_parts As Object
        Public Property metabolites As String()

        Public Overrides Function ToString() As String
            Return metabolites.GetJson
        End Function
    End Class

    Public Class publication

        Public Property title As String
        Public Property doi As String
        Public Property pubmed As String

        Public Overrides Function ToString() As String
            If doi.StringEmpty And pubmed.StringEmpty Then
                Return title
            ElseIf doi.StringEmpty Then
                Return $"{title} ({pubmed})"
            ElseIf pubmed.StringEmpty Then
                Return $"{title} (doi:{doi})"
            Else
                Return $"{title} (doi:{doi}, {pubmed})"
            End If
        End Function
    End Class

    Public Class response

        Public Property name As String
        Public Property url As String
        Public Property description As String
        Public Property datasets As Dictionary(Of String, DataSet)

        Public Overrides Function ToString() As String
            Return url
        End Function
    End Class
End Namespace

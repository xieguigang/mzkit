#Region "Microsoft.VisualBasic::bc05247afd57463bc6bc14952a6b580b, Massbank\Public\NCBI\PubChem\Web\Query\WebResponse.vb"

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

    '     Class JsonQuery
    ' 
    '         Properties: [where], collection, download, limit, order
    '                     start
    ' 
    '     Class [QueryWhere]
    ' 
    '         Properties: ands
    ' 
    '     Class QueryTableExport
    ' 
    '         Properties: aids, annothitcnt, annothits, cid, cidcdate
    '                     cmpdname, cmpdsynonym, complexity, dois, hbondacc
    '                     hbonddonor, heavycnt, inchikey, iupacname, meshheadings
    '                     mf, mw, polararea, rotbonds, xlogp
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace NCBI.PubChem

    ' {"download":"*","collection":"compound","where":{"ands":[{"*":"66-84-2"}]},"order":["relevancescore,desc"],"start":1,"limit":1000000}
    ' {"collection":"compound","download":"*","limit":10,"order":["relevancescore,desc"],"start":1,"where":{"ands":{"*":"650818-62-1"}}}
    Public Class JsonQuery

        Public Property download As String = "*"
        Public Property collection As String = "compound"
        Public Property [where] As QueryWhere
        Public Property order As String() = {"relevancescore,desc"}
        Public Property start As Integer = 1
        Public Property limit As Integer = 10

    End Class

    Public Class [QueryWhere]
        Public Property ands As Dictionary(Of String, String)()
    End Class

    ''' <summary>
    ''' Table export result of <see cref="JsonQuery"/>
    ''' </summary>
    Public Class QueryTableExport
        Public Property cid As String
        Public Property cmpdname As String
        <Collection("cmpdsynonym", "|")>
        Public Property cmpdsynonym As String()
        Public Property mw As Double
        Public Property mf As String
        Public Property polararea As Double
        Public Property complexity As Double
        Public Property xlogp As String
        Public Property heavycnt As Double
        Public Property hbonddonor As Double
        Public Property hbondacc As Double
        Public Property rotbonds As Double
        Public Property inchikey As String
        Public Property iupacname As String
        Public Property meshheadings As String
        Public Property annothits As Double
        Public Property annothitcnt As Double
        <Collection("aids", ",")>
        Public Property aids As String()
        Public Property cidcdate As String
        <Collection("dois", "|")>
        Public Property dois As String()
    End Class

End Namespace

#Region "Microsoft.VisualBasic::9782662c0a05ea9a3bbbf9ff62234f8a, metadb\Massbank\Public\HERB\HERB_herb_info.vb"

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

    '   Total Lines: 25
    '    Code Lines: 22 (88.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (12.00%)
    '     File Size: 942 B


    '     Class HERB_herb_info
    ' 
    '         Properties: [Function], Clinical_manifestations, Herb_, Herb_cn_name, Herb_en_name
    '                     Herb_latin_name, Herb_pinyin_name, Indication, Meridians, Properties
    '                     SymMap_id, TCM_ID_id, TCMID_id, TCMSP_id, Therapeutic_cn_class
    '                     Therapeutic_en_class, Toxicity, UsePart
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace HERB

    Public Class HERB_herb_info

        Public Property Herb_ As String
        Public Property Herb_pinyin_name As String
        Public Property Herb_cn_name As String
        Public Property Herb_en_name As String
        Public Property Herb_latin_name As String
        Public Property Properties As String
        Public Property Meridians As String
        Public Property UsePart As String
        Public Property [Function] As String
        Public Property Indication As String
        Public Property Toxicity As String
        Public Property Clinical_manifestations As String
        Public Property Therapeutic_en_class As String
        Public Property Therapeutic_cn_class As String
        Public Property TCMID_id As String
        Public Property TCM_ID_id As String
        Public Property SymMap_id As String
        Public Property TCMSP_id As String

    End Class
End Namespace

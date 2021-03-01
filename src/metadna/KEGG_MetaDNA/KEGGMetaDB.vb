#Region "Microsoft.VisualBasic::767394fe53d01970836e473a63e548a6, KEGG_MetaDNA\KEGGMetaDB.vb"

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

    ' Class KEGGMetaDB
    ' 
    '     Properties: [class], exact_mass, formula, keggID, libname
    '                 name, xref
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' MetaDNA算法所需要的KEGG注释信息的结构
''' </summary>
Public Class KEGGMetaDB

    Public Property keggID As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double

    ''' <summary>
    ''' 与其他的数据库的编号的外键连接
    ''' </summary>
    ''' <returns></returns>
    Public Property xref As Dictionary(Of String, String)

    ''' <summary>
    ''' 指向来源的数据集的唯一编号
    ''' </summary>
    ''' <returns></returns>
    Public Property libname As String
    Public Property [class] As String

    Public Overrides Function ToString() As String
        Return $"[{libname}] {keggID}={name}"
    End Function

End Class

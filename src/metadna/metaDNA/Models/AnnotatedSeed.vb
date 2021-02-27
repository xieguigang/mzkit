#Region "Microsoft.VisualBasic::68bc59312061b1645f116e9cdfcfda77, metaDNA\Models\AnnotatedSeed.vb"

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

    ' Class AnnotatedSeed
    ' 
    '     Properties: id, kegg_id, parent, products
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' 已经成功进行注释的代谢物信息(作为MetaDNA推断的种子)
''' </summary>
''' <remarks>
''' 
''' </remarks>
Public Class AnnotatedSeed : Implements INamedValue

    Public Property kegg_id As String
    Public Property id As String Implements INamedValue.Key
    Public Property parent As ms1_scan
    Public Property parentTrace As Double

    ''' <summary>
    ''' ``[lib_guid => spectrum]``
    ''' </summary>
    ''' <returns></returns>
    Public Property products As Dictionary(Of String, LibraryMatrix)

    Public Overrides Function ToString() As String
        Return $"{id}: {kegg_id}"
    End Function

End Class

#Region "Microsoft.VisualBasic::cbad1b996e728ae99c4130ee619ba0e8, metadb\Massbank\MetaLib\Models\BiosampleSource.vb"

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

'   Total Lines: 50
'    Code Lines: 27 (54.00%)
' Comment Lines: 11 (22.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 12 (24.00%)
'     File Size: 1.77 KB


'     Class BiosampleSource
' 
'         Properties: biosample, reference, source
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: ToString
' 
'     Class CompoundClass
' 
'         Properties: [class], kingdom, molecular_framework, sub_class, super_class
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Metabolite

    Public Class BiosampleSource

        <Field(0)> Public Property biosample As String
        <Field(1)> Public Property source As String

        ''' <summary>
        ''' the reference source
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property reference As String

        Sub New()
        End Sub

        Sub New(clone As BiosampleSource)
            biosample = clone.biosample
            source = clone.source
            reference = clone.reference
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    ''' <summary>
    ''' the data model of the compound class information
    ''' </summary>
    ''' <remarks>
    ''' this class information model is mainly address on the HMDB
    ''' metabolite ontology class levels.
    ''' </remarks>
    Public Class CompoundClass : Implements ICompoundClass

        <Field(0)> Public Property kingdom As String Implements ICompoundClass.kingdom
        <Field(1)> Public Property super_class As String Implements ICompoundClass.super_class
        <Field(2)> Public Property [class] As String Implements ICompoundClass.class
        <Field(3)> Public Property sub_class As String Implements ICompoundClass.sub_class
        <Field(4)> Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

    End Class

End Namespace

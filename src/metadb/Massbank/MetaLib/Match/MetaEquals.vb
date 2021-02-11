#Region "Microsoft.VisualBasic::fa9ace7dd7eb4137bca0287979a50add, Massbank\MetaLib\Match\MetaEquals.vb"

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

    '     Class MetaEquals
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Agreement, CompareXref, Equals
    ' 
    '         Sub: agreementInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Namespace MetaLib

    Public Class MetaEquals

        Dim threshold As Double

        Sub New(Optional threshold As Double = 0.45)
            Me.threshold = threshold
        End Sub

        Public Overloads Function Equals(meta As MetaInfo, other As MetaInfo) As Boolean
            Return Agreement(meta, other) >= threshold
        End Function

        Public Function CompareXref(xref As xref, otherXref As xref) As Double
            Dim agree As Integer
            Dim total As Integer
            Dim yes = Sub()
                          agree += 1
                          total += 1
                      End Sub
            Dim no = Sub() total += 1

            Call agreementInternal(xref, otherXref, yes, no)

            Return agree / total
        End Function

        Private Sub agreementInternal(xref As xref, otherXref As xref, yes As Action, no As Action)
            Dim compareInteger As New ComparesIdXrefInteger(yes, no)

            ' 下面的这个几个数据库编号可能都是没有的
            Call compareInteger.DoCompares(xref.chebi, otherXref.chebi)
            Call compareInteger.DoCompares(xref.KEGG, otherXref.KEGG)
            Call compareInteger.DoCompares(xref.pubchem, otherXref.pubchem)
            Call compareInteger.DoCompares(xref.HMDB, otherXref.HMDB)

            If xref.CAS.SafeQuery.Any(Function(id) otherXref.CAS.IndexOf(id) > -1) Then
                yes()
            Else
                no()
            End If

            If xref.InChIkey = otherXref.InChIkey Then
                yes()
            Else
                no()
            End If
        End Sub

        Public Function Agreement(meta As MetaInfo, other As MetaInfo) As Double
            Dim agree As Integer
            Dim total As Integer
            Dim yes = Sub()
                          agree += 1
                          total += 1
                      End Sub
            Dim no = Sub() total += 1

            If stdNum.Abs(meta.exact_mass - other.exact_mass) > 0.3 Then
                no()
            End If

            ' 因为name在不同的数据库之间差异有些大,所以在这里只作为可选参考
            ' 不调用no函数了
            If Strings.Trim(meta.name).TextEquals(Strings.Trim(other.name)) AndAlso Not Strings.Trim(other.name).StringEmpty Then
                yes()
            End If

            Call agreementInternal(meta.xref, other.xref, yes, no)

            Return agree / total
        End Function
    End Class
End Namespace

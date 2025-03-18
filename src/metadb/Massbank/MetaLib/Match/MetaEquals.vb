#Region "Microsoft.VisualBasic::510067b865d27cd95550a9a1c8a33f32, metadb\Massbank\MetaLib\Match\MetaEquals.vb"

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

    '   Total Lines: 81
    '    Code Lines: 61 (75.31%)
    ' Comment Lines: 3 (3.70%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 17 (20.99%)
    '     File Size: 2.82 KB


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

Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace MetaLib

    Public Class MetaEquals

        Dim threshold As Double

        Sub New(Optional threshold As Double = 0.45)
            Me.threshold = threshold
        End Sub

        Public Overloads Function Equals(meta As MetaInfo, other As MetaInfo) As Boolean
            Return Agreement(meta, other) >= threshold
        End Function

        Friend Class SimpleCheck

            Public agree, total As Integer

            ''' <summary>
            ''' get the jaccard similarity score
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property score As Double
                Get
                    Return agree / total
                End Get
            End Property

            Public Sub yes()
                agree += 1
                total += 1
            End Sub

            Public Sub no()
                total += 1
            End Sub

            Public Overrides Function ToString() As String
                Return $"{(score * 100).ToString("F2")}%, agree:{agree}/total:{total}"
            End Function
        End Class

        Public Shared Function CompareXref(xref As xref, otherXref As xref) As Double
            Dim check As New SimpleCheck
            Call CompareXref(check, xref, otherXref)
            Return check.score
        End Function

        Private Shared Sub CompareXref(ByRef check As SimpleCheck, xref As xref, otherXref As xref)
            Dim compareInteger As New ComparesIdXrefInteger(check)

            ' 下面的这个几个数据库编号可能都是没有的
            Call compareInteger.DoCompares(xref.chebi, otherXref.chebi)
            Call compareInteger.DoCompares(xref.KEGG, otherXref.KEGG)
            Call compareInteger.DoCompares(xref.pubchem, otherXref.pubchem)
            Call compareInteger.DoCompares(xref.HMDB, otherXref.HMDB)

            If xref.CAS.SafeQuery.Any(Function(id) otherXref.CAS.IndexOf(id) > -1) Then
                check.yes()
            Else
                check.no()
            End If

            If xref.InChIkey = otherXref.InChIkey Then
                check.yes()
            Else
                check.no()
            End If
        End Sub

        Public Function Agreement(meta As MetaInfo, other As MetaInfo) As Double
            Dim check As New SimpleCheck

            If std.Abs(meta.exact_mass - other.exact_mass) > 0.3 Then
                check.no()
            End If

            ' 因为name在不同的数据库之间差异有些大,所以在这里只作为可选参考
            ' 不调用no函数了
            If Strings.Trim(meta.name).TextEquals(Strings.Trim(other.name)) AndAlso Not Strings.Trim(other.name).StringEmpty Then
                check.yes()
            End If

            Call CompareXref(check, meta.xref, other.xref)

            Return check.score
        End Function
    End Class
End Namespace

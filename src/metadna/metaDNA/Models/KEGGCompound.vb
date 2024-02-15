#Region "Microsoft.VisualBasic::56438ec456f24ef0d01f7c1cadcf99e3, mzkit\src\metadna\metaDNA\Models\KEGGCompound.vb"

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

'   Total Lines: 64
'    Code Lines: 47
' Comment Lines: 3
'   Blank Lines: 14
'     File Size: 1.80 KB


' Structure KEGGCompound
' 
'     Properties: CommonName, ExactMass, Formula, kegg_id
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' object model wrapper for the KEGG compound in order to apply of the generic ms search engine
''' </summary>
Public Structure KEGGCompound : Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider

    Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.exactMass
        End Get
    End Property

    Public ReadOnly Property kegg_id As String Implements IReadOnlyId.Identity
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.entry
        End Get
    End Property

    Public ReadOnly Property CommonName As String Implements ICompoundNameProvider.CommonName
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            If KEGG.commonNames Is Nothing Then
                Return kegg_id
            End If

            Return If(KEGG.commonNames.FirstOrDefault, kegg_id)
        End Get
    End Property

    Public ReadOnly Property Formula As String Implements IFormulaProvider.Formula
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.formula
        End Get
    End Property

    Dim KEGG As Compound

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(kegg As Compound)
        Me.KEGG = kegg
    End Sub

    Public Overrides Function ToString() As String
        If KEGG Is Nothing Then
            Return Nothing
        End If

        Return KEGG.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(cpd As KEGGCompound) As Compound
        Return cpd.KEGG
    End Operator

End Structure

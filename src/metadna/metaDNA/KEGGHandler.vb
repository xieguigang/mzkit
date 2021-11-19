#Region "Microsoft.VisualBasic::8bc696897568ce6d806e0c4f877b70ef, src\metadna\metaDNA\KEGGHandler.vb"

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

' Class KEGGHandler
' 
'     Properties: Calculators
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: CreateIndex, GetCompound, QueryByMz
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class KEGGHandler

    ReadOnly engine As MSSearch(Of KEGGCompound)

    Public ReadOnly Property Calculators As Dictionary(Of String, MzCalculator)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return engine.Calculators
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(tree As AVLTree(Of MassIndexKey, KEGGCompound), tolerance As Tolerance, precursorTypes As MzCalculator())
        Me.engine = New MSSearch(Of KEGGCompound)(tree, tolerance, precursorTypes)
    End Sub

    Sub New(engine As MSSearch(Of KEGGCompound))
        Me.engine = engine
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompound(kegg_id As String) As Compound
        Return engine.keggIndex.TryGetValue(kegg_id).KEGG
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' 函数返回符合条件的kegg代谢物编号
    ''' </returns>
    Public Function QueryByMz(mz As Double) As IEnumerable(Of KEGGQuery)
        Return engine _
            .QueryByMz(mz) _
            .Select(Function(q)
                        Return New KEGGQuery With {
                            .mz = q.mz,
                            .kegg_id = q.unique_id,
                            .ppm = q.ppm,
                            .precursorType = q.precursorType,
                            .score = q.score
                        }
                    End Function)
    End Function

    Public Shared Function CreateIndex(compounds As IEnumerable(Of Compound), types As MzCalculator(), tolerance As Tolerance) As KEGGHandler
        Dim wrappers = compounds.Select(Function(c) New KEGGCompound With {.KEGG = c}).ToArray
        Dim engine = MSSearch(Of KEGGCompound).CreateIndex(wrappers, types, tolerance)
        Dim kegg As New KEGGHandler(engine)

        Return kegg
    End Function
End Class

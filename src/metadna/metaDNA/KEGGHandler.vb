#Region "Microsoft.VisualBasic::8f40f475639a01a14f401c5997ac9c12, metaDNA\KEGGHandler.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateIndex, GetCompound, QueryByMz
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports any = Microsoft.VisualBasic.Scripting

Public Class KEGGHandler

    ReadOnly precursorTypes As MzCalculator()
    ReadOnly tolerance As Tolerance
    ReadOnly massIndex As AVLTree(Of MassIndexKey, Compound)
    ReadOnly keggIndex As Dictionary(Of String, Compound)

    Sub New(tree As AVLTree(Of MassIndexKey, Compound), tolerance As Tolerance, precursorTypes As MzCalculator())
        Me.tolerance = tolerance
        Me.massIndex = tree
        Me.precursorTypes = precursorTypes
        Me.keggIndex = tree _
            .GetAllNodes _
            .Select(Function(c) c.Members) _
            .IteratesALL _
            .GroupBy(Function(c) c.entry) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cgroup)
                              Return cgroup.First
                          End Function)
    End Sub

    Public Function GetCompound(kegg_id As String) As Compound
        Return keggIndex.TryGetValue(kegg_id)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' 函数返回符合条件的kegg代谢物编号
    ''' </returns>
    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of KEGGQuery)
        Dim query As New MassIndexKey With {.mz = mz}
        Dim result As Compound() = massIndex.Find(query)?.Members

        For Each cpd As Compound In result.SafeQuery
            Dim minppm = precursorTypes _
                .Select(Function(type)
                            Dim mzhit As Double = type.CalcMZ(cpd.exactMass)

                            Return (type, mzhit, PPMmethod.PPM(mzhit, mz))
                        End Function) _
                .OrderBy(Function(type) type.Item3) _
                .First

            Yield New KEGGQuery With {
                .kegg_id = cpd.entry,
                .precursorType = minppm.type.ToString,
                .mz = minppm.mzhit,
                .ppm = minppm.Item3
            }
        Next
    End Function

    Public Shared Function CreateIndex(compounds As IEnumerable(Of Compound), types As MzCalculator(), tolerance As Tolerance) As KEGGHandler
        Dim tree As New AVLTree(Of MassIndexKey, Compound)(MassIndexKey.ComparesMass(tolerance), AddressOf any.ToString)
        Dim typesCache = types.Select(Function(t) (name:=t.ToString, type:=t)).ToArray

        For Each compound As Compound In compounds.GroupBy(Function(cpd) cpd.entry).Select(Function(cgroup) cgroup.First)
            If compound.exactMass <= 0 Then
                Continue For
            End If

            For Each type As (name$, calc As MzCalculator) In typesCache
                Dim index As New MassIndexKey With {
                    .precursorType = type.name,
                    .mz = type.calc.CalcMZ(compound.exactMass)
                }

                tree.Add(index, compound, valueReplace:=False)
            Next
        Next

        Return New KEGGHandler(tree, tolerance, types)
    End Function
End Class

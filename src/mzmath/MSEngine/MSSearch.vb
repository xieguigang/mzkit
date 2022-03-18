#Region "Microsoft.VisualBasic::ac2fc53da570afe62f72f35b1a7bf275, mzkit\src\mzmath\MSEngine\MSSearch.vb"

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

    '   Total Lines: 138
    '    Code Lines: 91
    ' Comment Lines: 27
    '   Blank Lines: 20
    '     File Size: 5.69 KB


    ' Class MSSearch
    ' 
    '     Properties: Calculators
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateIndex, GetAnnotation, GetCompound, MSetAnnotation, QueryByMz
    '               ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

<Assembly: InternalsVisibleTo("BioNovoGene.BioDeep.MetaDNA")>

Public Class MSSearch(Of Compound As {IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider}) : Implements IMzQuery

    ReadOnly precursorTypes As MzCalculator()
    ReadOnly tolerance As Tolerance
    ReadOnly mzIndex As (Double, Compound())()

    ''' <summary>
    ''' index by unique id
    ''' </summary>
    Friend ReadOnly index As Dictionary(Of String, Compound)

    Public ReadOnly Property Calculators As Dictionary(Of String, MzCalculator)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return precursorTypes.ToDictionary(Function(c) c.ToString)
        End Get
    End Property

    Sub New(tree As IEnumerable(Of Compound), tolerance As Tolerance, precursorTypes As MzCalculator())
        Me.tolerance = tolerance
        Me.precursorTypes = precursorTypes
        Me.index = tree _
            .GroupBy(Function(c) c.Identity) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cgroup)
                              Return cgroup.First
                          End Function)
        Me.mzIndex = Me.index _
            .Values _
            .Select(Function(c)
                        Return precursorTypes.Select(Function(t) (t.CalcMZ(c.ExactMass), c))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(d) d.Item1) _
            .Select(Function(g)
                        Return (g.Key, g.Select(Function(i) i.c).ToArray)
                    End Function) _
            .ToArray
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{index.Count} unique compounds, tree with mzdiff: {tolerance} (precursors: {precursorTypes.JoinBy("; ")})"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompound(id As String) As Compound
        Return index.TryGetValue(id)
    End Function

    Public Function MSetAnnotation(mzlist As IEnumerable(Of Double), Optional topN As Integer = 3) As IEnumerable(Of MzQuery) Implements IMzQuery.MSetAnnotation
        Return mzlist.Select(AddressOf QueryByMz).IteratesALL
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>
    ''' 函数返回符合条件的kegg代谢物编号
    ''' </returns>
    ''' <remarks>
    ''' the query score is zero from this function
    ''' </remarks>
    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery) Implements IMzQuery.QueryByMz
        Dim query As New MassIndexKey With {.mz = mz}
        Dim result As Compound() = mzIndex _
            .Where(Function(d) tolerance(d.Item1, mz)) _
            .Select(Function(d) d.Item2) _
            .IteratesALL _
            .GroupBy(Function(d) d.Identity) _
            .Select(Function(g)
                        Return g.First
                    End Function) _
            .ToArray  ' massIndex.Find(query)?.Members

        For Each cpd As Compound In result.SafeQuery
            Dim minppm = precursorTypes _
                .Select(Function(type)
                            Dim mzhit As Double = type.CalcMZ(cpd.ExactMass)

                            Return (type, mzhit, PPMmethod.PPM(mzhit, mz))
                        End Function) _
                .OrderBy(Function(type) type.Item3) _
                .First

            Yield New MzQuery With {
                .unique_id = cpd.Identity,
                .precursorType = minppm.type.ToString,
                .mz = mz,
                .ppm = minppm.Item3,
                .name = cpd.CommonName,
                .mz_ref = minppm.mzhit
            }
        Next
    End Function

    Public Shared Function CreateIndex(compounds As IEnumerable(Of Compound), types As MzCalculator(), tolerance As Tolerance) As MSSearch(Of Compound)
        'Dim tree As New AVLTree(Of MassIndexKey, Compound)(MassIndexKey.ComparesMass(tolerance), AddressOf any.ToString)
        'Dim typesCache = types.Select(Function(t) (name:=t.ToString, type:=t)).ToArray

        'For Each compound As Compound In compounds.GroupBy(Function(cpd) cpd.Identity).Select(Function(cgroup) cgroup.First)
        '    If compound.ExactMass <= 0 Then
        '        Continue For
        '    End If

        '    For Each type As (name$, calc As MzCalculator) In typesCache
        '        Dim index As New MassIndexKey With {
        '            .precursorType = type.name,
        '            .mz = type.calc.CalcMZ(compound.ExactMass)
        '        }

        '        tree.Add(index, compound, valueReplace:=False)
        '    Next
        'Next

        Return New MSSearch(Of Compound)(compounds, tolerance, types)
    End Function

    Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMzQuery.GetAnnotation
        Dim meta = index.TryGetValue(uniqueId)

        If meta Is Nothing OrElse (meta.CommonName.StringEmpty AndAlso meta.Formula.StringEmpty) Then
            Return Nothing
        Else
            Return (meta.CommonName, meta.Formula)
        End If
    End Function
End Class

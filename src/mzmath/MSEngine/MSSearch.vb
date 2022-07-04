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
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

<Assembly: InternalsVisibleTo("BioNovoGene.BioDeep.MetaDNA")>

Public Class MSSearch(Of Compound As {IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider}) : Implements IMzQuery

    Friend Structure IonIndex

        Dim mz As Double
        Dim precursor As String
        Dim compound As Compound

    End Structure

    ReadOnly precursorTypes As MzCalculator()
    ''' <summary>
    ''' mass tolerance value for match sample mz and threocal mz
    ''' </summary>
    ReadOnly tolerance As Tolerance
    ReadOnly mzIndex As BlockSearchFunction(Of IonIndex)
    ReadOnly score As Func(Of Compound, Double)
    ReadOnly xrefs As Func(Of Compound, Dictionary(Of String, String))

    ''' <summary>
    ''' index by unique id
    ''' </summary>
    Friend ReadOnly index As Dictionary(Of String, Compound)

    Public ReadOnly Property Metadata As IEnumerable(Of Compound)
        Get
            Return index.Values
        End Get
    End Property

    Public ReadOnly Property Calculators As Dictionary(Of String, MzCalculator)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return precursorTypes.ToDictionary(Function(c) c.ToString)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <param name="tolerance">
    ''' the mass tolerance value between two mass value
    ''' </param>
    ''' <param name="precursorTypes"></param>
    ''' <param name="score"></param>
    Sub New(tree As IEnumerable(Of Compound),
            tolerance As Tolerance,
            precursorTypes As MzCalculator(),
            Optional score As Func(Of Compound, Double) = Nothing,
            Optional xrefs As Func(Of Compound, Dictionary(Of String, String)) = Nothing)

        Me.tolerance = tolerance
        Me.precursorTypes = precursorTypes
        Me.xrefs = xrefs
        Me.score = If(score, New Func(Of Compound, Double)(Function() 0.0))
        Me.index = tree _
            .GroupBy(Function(c) c.Identity) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cgroup)
                              Return cgroup.First
                          End Function)

        Dim mzset = Me.index _
            .Values _
            .Select(Function(c)
                        Return precursorTypes _
                            .Select(Function(t)
                                        Return New IonIndex With {
                                            .compound = c,
                                            .mz = t.CalcMZ(c.ExactMass),
                                            .precursor = t.ToString
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .Where(Function(i) i.mz > 0) _
            .ToArray

        ' 20220512
        '
        ' too small tolerance error will cause too much elements to
        ' sort
        ' and then will cause the error of 
        ' Stack overflow.
        ' Repeat 3075 times: 
        ' --------------------------------
        '   at Microsoft.VisualBasic.ComponentModel.Algorithm.QuickSortFunction
        '
        ' pipeline has been test for MS-imaging data analysis
        '
        Me.mzIndex = New BlockSearchFunction(Of IonIndex)(
            data:=mzset,
            eval:=Function(m) m.mz,
            tolerance:=1,
            factor:=3
        )
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
        Dim query As New IonIndex With {.mz = mz}
        Dim result As Compound() = mzIndex _
            .Search(query) _
            .Where(Function(d) tolerance(d.mz, mz)) _
            .Select(Function(d) d.compound) _
            .GroupBy(Function(d) d.Identity) _
            .Select(Function(g)
                        Return g.First
                    End Function) _
            .ToArray

        For Each cpd As Compound In result.SafeQuery
            Dim minppm = precursorTypes _
                .Select(Function(type, i)
                            Dim mzhit As Double = type.CalcMZ(cpd.ExactMass)
                            Dim ppm As Double = PPMmethod.PPM(mzhit, mz)

                            ' 20220426
                            ' precursor type has priority order
                            ' as its annotation score
                            Return (type, mzhit, ppm, priority:=i + 1)
                        End Function) _
                .OrderBy(Function(type) type.Item3) _
                .First

            Yield New MzQuery With {
                .unique_id = cpd.Identity,
                .precursorType = minppm.type.ToString,
                .mz = mz,
                .ppm = minppm.Item3,
                .name = cpd.CommonName,
                .mz_ref = minppm.mzhit,
                .score = (score(cpd) / (.ppm + 1)) / minppm.priority
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

    Private Function GetMetadata(uniqueId As String) As Object Implements IMzQuery.GetMetadata
        Return GetCompound(uniqueId)
    End Function

    Public Function GetDbXref(uniqueId As String) As Dictionary(Of String, String) Implements IMzQuery.GetDbXref
        Dim compound As Compound = GetCompound(uniqueId)

        If xrefs Is Nothing OrElse compound Is Nothing Then
            Return New Dictionary(Of String, String)
        Else
            Return xrefs(compound)
        End If
    End Function
End Class

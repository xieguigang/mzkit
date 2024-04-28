#Region "Microsoft.VisualBasic::b68e0cc332c64cb8ba7a072cddd6359b, E:/mzkit/src/mzmath/MSEngine//MSSearch.vb"

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

    '   Total Lines: 239
    '    Code Lines: 146
    ' Comment Lines: 57
    '   Blank Lines: 36
    '     File Size: 9.46 KB


    ' Class MSSearch
    ' 
    '     Properties: Calculators, Metadata
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateIndex, DoEvalMz, GetAnnotation, GetCompound, GetDbXref
    '               GetMetadata, loadIndex, MSetAnnotation, QueryByMz, ToString
    '     Structure IonIndex
    ' 
    '         Properties: mz
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

<Assembly: InternalsVisibleTo("BioNovoGene.BioDeep.MetaDNA")>

''' <summary>
''' Engine for run m/z query
''' </summary>
''' <typeparam name="Compound"></typeparam>
Public Class MSSearch(Of Compound As {IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider}) : Implements IMzQuery

    Friend Structure IonIndex : Implements IExactMassProvider

        Dim precursor As String
        Dim compound As Compound

        Public ReadOnly Property mz As Double Implements IExactMassProvider.ExactMass

        Sub New(mz As Double)
            Me.mz = mz
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{mz.ToString("F4")}] {compound.ToString}"
        End Function
    End Structure

    ReadOnly precursorTypes As MzCalculator()
    ReadOnly score As Func(Of Compound, Double)
    ReadOnly xrefs As Func(Of Compound, Dictionary(Of String, String))
    ReadOnly mzIndex As MassSearchIndex(Of IonIndex)

    ''' <summary>
    ''' index by unique id
    ''' </summary>
    Friend ReadOnly index As Dictionary(Of String, Compound)

    ''' <summary>
    ''' pull all compound meta data from current m/z search index
    ''' </summary>
    ''' <returns></returns>
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
            Optional xrefs As Func(Of Compound, Dictionary(Of String, String)) = Nothing,
            Optional mass_range As DoubleRange = Nothing)

        Me.precursorTypes = precursorTypes
        Me.xrefs = xrefs
        Me.score = If(score, New Func(Of Compound, Double)(Function() 0.0))
        Me.index = tree _
            .GroupBy(Function(c) c.Identity) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cgroup)
                              Return cgroup.First
                          End Function)

        Me.mzIndex = loadIndex(Me.index,
            precursorTypes:=precursorTypes,
            tolerance:=tolerance,
            mass_range:=mass_range)
    End Sub

    Private Shared Function loadIndex(index As Dictionary(Of String, Compound),
                                      precursorTypes As MzCalculator(),
                                      tolerance As Tolerance,
                                      mass_range As DoubleRange) As MassSearchIndex(Of IonIndex)

        Dim mzset As IonIndex() = index.Values _
            .Select(Function(c) DoEvalMz(c, precursorTypes)) _
            .IteratesALL _
            .Where(Function(i) i.mz > 0) _
            .ToArray

        If mass_range IsNot Nothing AndAlso mass_range.Length > 0 Then
            mzset = mzset _
                .Where(Function(m) mass_range.IsInside(m.mz)) _
                .ToArray
        End If

        Return New MassSearchIndex(Of IonIndex)(mzset, Function(mz) New IonIndex(mz), tolerance)
    End Function

    Private Shared Iterator Function DoEvalMz(c As Compound, precursorTypes As MzCalculator()) As IEnumerable(Of IonIndex)
        For Each t As MzCalculator In precursorTypes
            Yield New IonIndex(mz:=t.CalcMZ(c.ExactMass)) With {
                .compound = c,
                .precursor = t.ToString
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{index.Count} unique compounds, tree with mzdiff: {mzIndex.ToString} (precursors: {precursorTypes.JoinBy("; ")})"
    End Function

    ''' <summary>
    ''' get kegg compound by a given kegg id
    ''' </summary>
    ''' <param name="id">
    ''' kegg compound id in pattern ``C\d+``
    ''' </param>
    ''' <returns>
    ''' this function returns nothing if the target compound id
    ''' is not exists in the current database
    ''' </returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompound(id As String) As Compound
        Return index.TryGetValue(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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
    Public Overridable Iterator Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery) Implements IMzQuery.QueryByMz
        Dim result As Compound() = mzIndex _
            .QueryByMass(mz) _
            .Select(Function(d) d.compound) _
            .GroupBy(Function(d) d.Identity) _
            .Select(Function(g)
                        Return g.First
                    End Function) _
            .ToArray

        For Each cpd As Compound In result
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

        Return New MSSearch(Of Compound)(compounds.Where(Function(c) c.ExactMass > 0), tolerance, types)
    End Function

    Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMzQuery.GetAnnotation
        Dim meta = index.TryGetValue(uniqueId)

        If meta Is Nothing OrElse (meta.CommonName.StringEmpty AndAlso meta.Formula.StringEmpty) Then
            Return Nothing
        Else
            Return (meta.CommonName, meta.Formula)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

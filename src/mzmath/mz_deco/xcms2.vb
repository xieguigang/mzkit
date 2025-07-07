#Region "Microsoft.VisualBasic::73f5668dfc08659063d4dec0f8f45d9c, mzmath\mz_deco\xcms2.vb"

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

    '   Total Lines: 336
    '    Code Lines: 234 (69.64%)
    ' Comment Lines: 59 (17.56%)
    '    - Xml Docs: 91.53%
    ' 
    '   Blank Lines: 43 (12.80%)
    '     File Size: 11.00 KB


    ' Enum Imputation
    ' 
    '     Median, Min, None
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class xcms2
    ' 
    '     Properties: groups, ID, into, mz, mzmax
    '                 mzmin, npeaks, Properties, RI, RImax
    '                 RImin, rt, rtmax, rtmin
    ' 
    '     Constructor: (+6 Overloads) Sub New
    ' 
    '     Function: Impute, MakeUniqueId, Merge, ToString, TotalPeakSum
    ' 
    '     Sub: AddSamples, SetPeaks
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Enum Imputation
    None
    Min
    Median
End Enum

''' <summary>
''' an ion peak ROI data object, the peak table format table file model of xcms version 2
''' </summary>
''' <remarks>
''' a <see cref="IMs1Scan"/> peak object
''' </remarks>
Public Class xcms2 : Inherits DynamicPropertyBase(Of Double)
    Implements INamedValue
    Implements IRetentionIndex
    Implements IRetentionTime
    Implements IMassBin
    Implements IMs1Scan

    ''' <summary>
    ''' the feature unique id
    ''' </summary>
    ''' <returns></returns>
    <DisplayName("xcms id")>
    <Category("MS1")>
    Public Property ID As String Implements INamedValue.Key
    ''' <summary>
    ''' the ion m/z
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property mz As Double Implements IMassBin.mass, IMs1Scan.mz
    ''' <summary>
    ''' the min of ion m/z value
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property mzmin As Double Implements IMassBin.min
    ''' <summary>
    ''' the max of the ion m/z value
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property mzmax As Double Implements IMassBin.max
    ''' <summary>
    ''' the rt value in max peak data point
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property rt As Double Implements IRetentionTime.rt
    <Category("MS1")> Public Property rtmin As Double
    <Category("MS1")> Public Property rtmax As Double

    ''' <summary>
    ''' the retention index value based on the rt transformation
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property RI As Double Implements IRetentionIndex.RI

    <Category("MS1")> Public Property RImin As Double
    <Category("MS1")> Public Property RImax As Double

    Dim int_npeaks As Integer?
    Dim intensity As Double?

    ''' <summary>
    ''' this feature has n sample data(value should be a positive number)
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Category("sample_data")>
    Public ReadOnly Property npeaks As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            If int_npeaks Is Nothing Then
                Return Properties _
                    .Where(Function(s) s.Value > 0) _
                    .Count
            Else
                Return CInt(int_npeaks)
            End If
        End Get
    End Property

    ''' <summary>
    ''' number of the peak groups that merged in this ion ROI
    ''' </summary>
    ''' <returns></returns>
    Public Property groups As Integer

    <Category("sample_data")>
    <DisplayName("sample_data")>
    Public Overrides Property Properties As Dictionary(Of String, Double)
        Get
            Return MyBase.Properties
        End Get
        Set(value As Dictionary(Of String, Double))
            MyBase.Properties = value
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property into As Double Implements IMs1Scan.intensity
        Get
            If intensity Is Nothing Then
                intensity = Properties.Values.Sum
            End If

            Return intensity
        End Get
        Set(value As Double)
            If value > 0 Then
                intensity = value
            End If
        End Set
    End Property

    Sub New()
    End Sub

    Sub New(id As String, mz As Double, rt As Double)
        Call Me.New(mz, rt)
        Me.ID = id
    End Sub

    Sub New(mz As Double, rt As Double)
        Me.mz = mz
        Me.mzmin = mz
        Me.mzmax = mz
        Me.rt = rt
        Me.rtmin = rt
        Me.rtmax = rt
    End Sub

    Sub New(expression As Dictionary(Of String, Double))
        Me.Properties = expression
    End Sub

    Sub New(npeaks As Integer, Optional into As Double? = Nothing)
        intensity = into
        int_npeaks = npeaks
    End Sub

    ''' <summary>
    ''' make peak data information and area value copy
    ''' </summary>
    ''' <param name="clone"></param>
    Sub New(clone As xcms2)
        Me.mz = clone.mz
        Me.mzmin = clone.mzmin
        Me.mzmax = clone.mzmax
        Me.rt = clone.rt
        Me.rtmin = clone.rtmin
        Me.rtmax = clone.rtmax
        Me.ID = clone.ID
        Me.propertyTable = New Dictionary(Of String, Double)(clone.Properties)
        Me.RI = clone.RI
        Me.RImin = clone.RImin
        Me.RImax = clone.RImax
        Me.groups = clone.groups
        Me.intensity = clone.intensity
        Me.int_npeaks = clone.int_npeaks
    End Sub

    ''' <summary>
    ''' just make the <see cref="xcms2.ID"/> unique
    ''' </summary>
    ''' <param name="peaktable"></param>
    ''' <returns></returns>
    Public Shared Iterator Function MakeUniqueId(peaktable As IEnumerable(Of xcms2)) As IEnumerable(Of xcms2)
        Dim guid As New Dictionary(Of String, Counter)
        Dim uid As String

        For Each feature As xcms2 In peaktable
            uid = feature.ID

            If Not guid.ContainsKey(uid) Then
                guid.Add(uid, 0)
            End If

            If guid(uid).Value = 0 Then
                feature.ID = uid
            Else
                feature.ID = uid & "_" & guid(uid).ToString
            End If

            Call guid(uid).Hit()

            Yield feature
        Next
    End Function

    Public Sub SetPeaks(npeaks As Integer)
        Me.int_npeaks = npeaks
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{ID}  {mz.ToString("F4")}@{rt.ToString("F4")}  {npeaks}peaks: {Properties.Keys.GetJson}"
    End Function

    Public Shared Function TotalPeakSum(matrix As IEnumerable(Of xcms2), Optional scale As Double = 10 ^ 8) As IEnumerable(Of xcms2)
        Dim pool As xcms2() = matrix.ToArray
        Dim sampleNames As String() = pool _
            .Select(Function(k) k.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        For Each name As String In TqdmWrapper.Wrap(sampleNames, wrap_console:=App.EnableTqdm)
            Dim v As Double() = pool _
                .Select(Function(k)
                            Dim vi As Double = k(name)

                            If vi.IsNaNImaginary Then
                                Return 0
                            Else
                                Return vi
                            End If
                        End Function) _
                .ToArray
            Dim sum As Double = v.Sum

            ' (v / sum) * scale
            v = SIMD.Divide.f64_op_divide_f64_scalar(v, sum)
            v = SIMD.Multiply.f64_scalar_op_multiply_f64(scale, v)

            For i As Integer = 0 To pool.Length - 1
                pool(i)(name) = v(i)
            Next
        Next

        Return pool
    End Function

    ''' <summary>
    ''' impute missing data with half of the min positive value
    ''' </summary>
    ''' <returns></returns>
    Public Function Impute(Optional method As Imputation = Imputation.Min) As xcms2
        Dim is_zero As Boolean = Properties.Values.All(Function(xi) xi = 0.0)
        Dim fill_missing As Dictionary(Of String, Double)

        If Not is_zero Then
            Dim pos_min As Double

            If method = Imputation.Min Then
                pos_min = (Aggregate xi As Double
                           In Properties.Values
                           Where xi > 0
                           Into Min(xi)) / 2
            Else
                pos_min = Properties.Values _
                    .Where(Function(xi) xi > 0) _
                    .Median
            End If

            fill_missing = Properties _
                .ToDictionary(Function(k) k.Key,
                              Function(k)
                                  Return If(k.Value.IsNaNImaginary OrElse k.Value <= 0, pos_min, k.Value)
                              End Function)
        Else
            ' random fill for all zero
            ' no differece in t-test
            fill_missing = Properties _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return randf.NextDouble(0.5, 1)
                              End Function)
        End If

        Return New xcms2 With {
            .ID = ID,
            .mz = mz,
            .mzmax = mzmax,
            .mzmin = mzmin,
            .rt = rt,
            .rtmax = rtmax,
            .rtmin = rtmin,
            .RI = RI,
            .Properties = fill_missing,
            .groups = groups,
            .RImax = RImax,
            .RImin = RImin
        }
    End Function

    Public Sub AddSamples(samples As Dictionary(Of String, Double))
        For Each sample As KeyValuePair(Of String, Double) In samples
            If propertyTable.ContainsKey(sample.Key) Then
                propertyTable(sample.Key) = propertyTable(sample.Key) + sample.Value
            Else
                propertyTable.Add(sample.Key, sample.Value)
            End If
        Next
    End Sub

    Public Shared Function Merge(group As IEnumerable(Of xcms2), Optional aggregate As Func(Of Double, Double, Double) = Nothing) As xcms2
        Static sum As Func(Of Double, Double, Double) = ParseFlag("sum").GetAggregateFunction2

        Dim topPeaks As xcms2() = group _
            .OrderByDescending(Function(a) a.npeaks) _
            .ToArray
        Dim mergePeak As New xcms2(topPeaks(0))

        aggregate = If(aggregate, sum)

        For Each peak As xcms2 In topPeaks.Skip(1)
            For Each name As String In peak.Properties.Keys
                If mergePeak.HasProperty(name) Then
                    mergePeak(name) = aggregate(mergePeak(name), peak(name))
                Else
                    mergePeak(name) = peak(name)
                End If
            Next

            mergePeak.groups += peak.groups
        Next

        Return mergePeak
    End Function
End Class

#Region "Microsoft.VisualBasic::e217009880b50edc293ed389d31201da, mzmath\mz_deco\xcms2.vb"

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

    '   Total Lines: 218
    '    Code Lines: 145 (66.51%)
    ' Comment Lines: 46 (21.10%)
    '    - Xml Docs: 93.48%
    ' 
    '   Blank Lines: 27 (12.39%)
    '     File Size: 7.03 KB


    ' Class xcms2
    ' 
    '     Properties: ID, intensity, mz, mzmax, mzmin
    '                 npeaks, Properties, RI, rt, rtmax
    '                 rtmin
    ' 
    '     Constructor: (+5 Overloads) Sub New
    '     Function: Impute, MakeUniqueId, ToString, TotalPeakSum
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
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' the peak table format table file model of xcms version 2
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

    Public Property RImin As Double
    Public Property RImax As Double

    Dim int_npeaks As Integer?

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
    ''' A value read only property
    ''' </summary>
    ''' <returns></returns>
    Private Property intensity As Double Implements IMs1Scan.intensity
        Get
            Return Properties.Values.Sum
        End Get
        Set(value As Double)
            ' do nothing
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

    Sub New(npeaks As Integer)
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
    Public Function Impute() As xcms2
        Dim pos_min As Double = (Aggregate xi As Double In Properties.Values Where xi > 0 Into Min(xi)) / 2
        Dim fill_missing = Properties _
            .ToDictionary(Function(k) k.Key,
                          Function(k)
                              Return If(k.Value.IsNaNImaginary OrElse k.Value <= 0, pos_min, k.Value)
                          End Function)

        Return New xcms2 With {
            .ID = ID,
            .mz = mz,
            .mzmax = mzmax,
            .mzmin = mzmin,
            .rt = rt,
            .rtmax = rtmax,
            .rtmin = rtmin,
            .RI = RI,
            .Properties = fill_missing
        }
    End Function

    Public Shared Function Merge(group As IEnumerable(Of xcms2)) As xcms2
        Dim topPeaks = group.OrderByDescending(Function(a) a.npeaks).ToArray
        Dim mergePeak As New xcms2(topPeaks(0))

        For Each peak As xcms2 In topPeaks.Skip(1)
            For Each name As String In peak.Properties.Keys
                If mergePeak.HasProperty(name) Then
                    mergePeak(name) = (mergePeak(name) + peak(name))
                Else
                    mergePeak(name) = peak(name)
                End If
            Next
        Next

        Return mergePeak
    End Function
End Class

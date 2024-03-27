#Region "Microsoft.VisualBasic::71ddb62d865bfaeedeb10435bcd7d467, mzkit\src\mzmath\mz_deco\xcms2.vb"

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

'   Total Lines: 88
'    Code Lines: 73
' Comment Lines: 3
'   Blank Lines: 12
'     File Size: 2.85 KB


' Class xcms2
' 
'     Properties: mz, mzmax, mzmin, npeaks, rt
'                 rtmax, rtmin
' 
'     Function: Load, totalPeakSum
' 
' Class PeakSet
' 
'     Properties: peaks, sampleNames
' 
'     Function: Norm, Subset
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' the peak table format table file model of xcms version 2
''' </summary>
Public Class xcms2 : Inherits DynamicPropertyBase(Of Double)
    Implements INamedValue
    Implements IRetentionIndex
    Implements IRetentionTime

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
    <Category("MS1")> Public Property mz As Double
    ''' <summary>
    ''' the min of ion m/z value
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property mzmin As Double
    ''' <summary>
    ''' the max of the ion m/z value
    ''' </summary>
    ''' <returns></returns>
    <Category("MS1")> Public Property mzmax As Double
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

    ''' <summary>
    ''' this feature has n sample data(value should be a positive number)
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Category("sample_data")>
    Public ReadOnly Property npeaks As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Properties _
                .Where(Function(s) s.Value > 0) _
                .Count
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

    'Public Shared Function Load(file As String) As xcms2()
    '    Return DataSet _
    '        .LoadDataSet(Of xcms2)(file, uidMap:=NameOf(ID)) _
    '        .ToArray
    'End Function

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

    Friend Function totalPeakSum() As xcms2
        Dim totalSum As Double = Properties.Values.Sum

        Return New xcms2 With {
            .ID = ID,
            .mz = mz,
            .mzmax = mzmax,
            .mzmin = mzmin,
            .rt = rt,
            .rtmax = rtmax,
            .rtmin = rtmin,
            .Properties = Properties.Keys _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return Me(name) / totalSum * 10 ^ 8
                              End Function)
        }
    End Function
End Class


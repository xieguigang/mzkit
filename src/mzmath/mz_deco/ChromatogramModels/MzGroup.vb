#Region "Microsoft.VisualBasic::4e2ad78d1d8030447fc601b8adee0ef5, mzmath\mz_deco\ChromatogramModels\MzGroup.vb"

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

    '   Total Lines: 101
    '    Code Lines: 70 (69.31%)
    ' Comment Lines: 19 (18.81%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (11.88%)
    '     File Size: 3.17 KB


    ' Class MzGroup
    ' 
    '     Properties: MaxInto, mz, rt, size, TIC
    '                 XIC
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CreateChromatogram, CreateSignal, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.SignalProcessing

''' <summary>
''' XIC dataset that used for deconv
''' </summary>
''' <remarks>
''' A collection of the <see cref="ChromatogramTick"/> data that 
''' tagged with a numeric m/z value.
''' </remarks>
Public Class MzGroup

    ''' <summary>
    ''' target ion m/z
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property mz As Double
    ''' <summary>
    ''' the chromatogram data of current target ion
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Public Property XIC As ChromatogramTick()

    ''' <summary>
    ''' get point counts of <see cref="XIC"/>.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return XIC.TryCount
        End Get
    End Property

    Public ReadOnly Property rt As DoubleRange
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New DoubleRange(From t As ChromatogramTick In XIC Select t.Time)
        End Get
    End Property

    Public ReadOnly Property TIC As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Aggregate t As ChromatogramTick
                   In XIC
                   Into Sum(t.Intensity)
        End Get
    End Property

    Public ReadOnly Property MaxInto As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            If size = 0 Then
                Return 0
            Else
                Return Aggregate t As ChromatogramTick
                       In XIC
                       Into Max(t.Intensity)
            End If
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(mz As Double, xic As IEnumerable(Of ChromatogramTick))
        _mz = mz
        _XIC = xic.ToArray
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        With rt
            Return mz.ToString("F4") & $"@{size} points [{ .Min.ToString("F2")}s ~ { .Max.ToString("F2")}s]"
        End With
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateSignal(name As String) As GeneralSignal
        Return New GeneralSignal With {
            .reference = name,
            .weight = 1,
            .Strength = XIC.Select(Function(ti) ti.Intensity).ToArray,
            .Measures = XIC.Select(Function(ti) ti.Time).ToArray
        }
    End Function

    Public Function CreateChromatogram() As Chromatogram.Chromatogram
        Return New Chromatogram.Chromatogram() With {
            .scan_time = XIC.Select(Function(a) a.Time).ToArray,
            .BPC = XIC.Select(Function(a) a.Intensity).ToArray,
            .TIC = .BPC
        }
    End Function

End Class

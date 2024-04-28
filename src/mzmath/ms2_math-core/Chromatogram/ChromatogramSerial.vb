#Region "Microsoft.VisualBasic::623d83f57f7679d7653a74fb289ab7fa, E:/mzkit/src/mzmath/ms2_math-core//Chromatogram/ChromatogramSerial.vb"

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

    '   Total Lines: 102
    '    Code Lines: 67
    ' Comment Lines: 19
    '   Blank Lines: 16
    '     File Size: 3.64 KB


    '     Class ChromatogramSerial
    ' 
    '         Properties: Chromatogram, Name, rtmax, rtmin, size
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GetEnumerator, GetIntensity, GetTime, GetTuple, IEnumerable_GetEnumerator
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace Chromatogram

    ''' <summary>
    ''' A label tagged chromatogram data
    ''' </summary>
    Public Class ChromatogramSerial : Implements IEnumerable(Of ChromatogramTick), INamedValue

        ''' <summary>
        ''' the data label
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String Implements INamedValue.Key
        Public Property Chromatogram As ChromatogramTick()

        ''' <summary>
        ''' get the tick count in current <see cref="Chromatogram"/> signal data.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return Chromatogram.TryCount
            End Get
        End Property

        ''' <summary>
        ''' get the min rt of current <see cref="Chromatogram"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property rtmin As Double
            Get
                Return Aggregate t As ChromatogramTick
                       In Chromatogram
                       Into Min(t.Time)
            End Get
        End Property

        ''' <summary>
        ''' get the max rt of current <see cref="Chromatogram"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property rtmax As Double
            Get
                Return Aggregate t As ChromatogramTick
                       In Chromatogram
                       Into Max(t.Time)
            End Get
        End Property

        Default Public ReadOnly Property GetTick(i As Integer) As ChromatogramTick
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Chromatogram(i)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(name As String)
            Me.Name = name
        End Sub

        Sub New(name As String, ticks As IEnumerable(Of ChromatogramTick))
            Me.Name = name
            Me.Chromatogram = ticks.SafeQuery.ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTime() As IEnumerable(Of Double)
            Return From t As ChromatogramTick In Chromatogram Select t.Time
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIntensity() As IEnumerable(Of Double)
            Return From t As ChromatogramTick In Chromatogram Select t.Intensity
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTuple() As NamedCollection(Of ChromatogramTick)
            Return New NamedCollection(Of ChromatogramTick)(Name, Chromatogram)
        End Function

        Public Overrides Function ToString() As String
            Return Name & $" [{rtmin.ToString("F4")} ~ {rtmax.ToString("F4")} sec]"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of ChromatogramTick) Implements IEnumerable(Of ChromatogramTick).GetEnumerator
            For Each tick As ChromatogramTick In Chromatogram
                Yield tick
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace

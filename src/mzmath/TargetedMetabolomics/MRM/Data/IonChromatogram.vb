#Region "Microsoft.VisualBasic::0c03cea7b7a590909d916fc062ddc296, mzmath\TargetedMetabolomics\MRM\Data\IonChromatogram.vb"

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

    '   Total Lines: 66
    '    Code Lines: 40 (60.61%)
    ' Comment Lines: 17 (25.76%)
    '    - Xml Docs: 88.24%
    ' 
    '   Blank Lines: 9 (13.64%)
    '     File Size: 2.29 KB


    '     Structure IonChromatogram
    ' 
    '         Properties: chromatogram, description, hasRTwin, ion, name
    '                     source
    ' 
    '         Function: GetSplineData, GetTimeWindow, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace MRM.Data

    ''' <summary>
    ''' the raw data 
    ''' </summary>
    Public Structure IonChromatogram

        Public Property name As String
        Public Property description As String
        Public Property chromatogram As ChromatogramTick()
        ''' <summary>
        ''' the ion pair data of this ion chromatogram xic data
        ''' 
        ''' this is used to record the original ion pair data
        ''' that this ion chromatogram data is extracted from.
        ''' </summary>
        ''' <returns></returns>
        Public Property ion As IsomerismIonPairs

        ''' <summary>
        ''' the source file of the ion chromatogram xic data
        ''' 
        ''' this is used to record the original raw data file name
        ''' that this ion chromatogram data is extracted from.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String

        Public ReadOnly Property hasRTwin As Boolean
            Get
                Return Not ion.target.rt Is Nothing
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <DebuggerStepThrough>
        Public Function GetTimeWindow(winsize As Double) As DoubleRange
            If hasRTwin Then
                Return New Double() {
                    ion.target.rt - winsize,
                    ion.target.rt + winsize
                }
            Else
                Return Nothing
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetSplineData(degree As Double, res As Integer) As ChromatogramTick()
            Return chromatogram _
                .BSpline(Function(t, i) New ChromatogramTick(t, i), degree, res) _
                .ToArray
        End Function

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Structure
End Namespace

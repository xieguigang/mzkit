#Region "Microsoft.VisualBasic::3046f5638bc7fc2163a77f2dcb97547d, mzmath\TargetedMetabolomics\MRM\Data\IonChromatogram.vb"

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

    '   Total Lines: 42
    '    Code Lines: 32 (76.19%)
    ' Comment Lines: 3 (7.14%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (16.67%)
    '     File Size: 1.30 KB


    '     Structure IonChromatogram
    ' 
    '         Properties: chromatogram, description, hasRTwin, ion, name
    ' 
    '         Function: GetTimeWindow, ToString
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

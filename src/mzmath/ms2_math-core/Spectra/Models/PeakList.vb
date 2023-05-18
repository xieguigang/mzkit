#Region "Microsoft.VisualBasic::cf19bddbd462d696147628928e11de80, mzkit\src\mzmath\ms2_math-core\Spectra\Models\PeakList.vb"

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

    '   Total Lines: 51
    '    Code Lines: 25
    ' Comment Lines: 16
    '   Blank Lines: 10
    '     File Size: 1.34 KB


    '     Class PeakList
    ' 
    '         Properties: into, mz, size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Interface IMsScan
    ' 
    '         Function: GetMs, GetMzIonIntensity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Namespace Spectra

    Public Class PeakList

        ''' <summary>
        ''' the ion fragment mass list
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double()
        ''' <summary>
        ''' the signal intensity strength 
        ''' value of the corresponding ion 
        ''' fragment mass data.
        ''' </summary>
        ''' <returns></returns>
        Public Property into As Double()

        Public Property MRM As MRM()

        ''' <summary>
        ''' the number of the ion fragments 
        ''' in current peak list object 
        ''' data.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return mz.Length
            End Get
        End Property

        <DebuggerStepThrough>
        Public Sub New(masses As Double(), intensities As Double())
            Me.mz = masses
            Me.into = intensities
        End Sub

        Sub New()
        End Sub

    End Class

    Public Structure MRM

        Dim Q1 As Double
        Dim Q2 As Double

        Sub New(q1 As Double, q2 As Double)
            Me.Q1 = q1
            Me.Q2 = q2
        End Sub
    End Structure

    Public Interface IMsScan

        Function GetMs() As IEnumerable(Of ms2)
        Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double

    End Interface
End Namespace

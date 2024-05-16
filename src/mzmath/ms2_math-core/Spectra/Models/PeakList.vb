#Region "Microsoft.VisualBasic::74e81b1e551d770814964182cda86093, mzmath\ms2_math-core\Spectra\Models\PeakList.vb"

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

    '   Total Lines: 94
    '    Code Lines: 45
    ' Comment Lines: 32
    '   Blank Lines: 17
    '     File Size: 2.67 KB


    '     Class PeakList
    ' 
    '         Properties: into, MRM, mz, size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetPeaks
    ' 
    '     Structure MRM
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
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

    ''' <summary>
    ''' Spectrum data in vector model
    ''' </summary>
    ''' <remarks>
    ''' if the <see cref="MRM"/> data is not empty, then it means current spectrum
    ''' peaks data is the data for MRM targetted analysis
    ''' </remarks>
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
        ''' gets the number of the ion fragments 
        ''' in current peak list object data.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' ZERO will be returned if the current peak list
        ''' object has no fragment data or value is NULL
        ''' </remarks>
        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If mz Is Nothing Then
                    Return 0
                End If

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

        Public Iterator Function GetPeaks() As IEnumerable(Of ms2)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms2(_mz(i), _into(i))
            Next
        End Function

    End Class

    ''' <summary>
    ''' the MRM ion pair data model
    ''' </summary>
    Public Structure MRM

        Dim Q1 As Double
        Dim Q3 As Double

        Sub New(q1 As Double, q3 As Double)
            Me.Q1 = q1
            Me.Q3 = q3
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Q1}/{Q3}"
        End Function
    End Structure

    ''' <summary>
    ''' An abstract mass spectrum model, could be used for get spectrum peaks data
    ''' </summary>
    Public Interface IMsScan

        Function GetMs() As IEnumerable(Of ms2)
        Function GetMzIonIntensity(mz As Double, mzdiff As Tolerance) As Double

    End Interface
End Namespace

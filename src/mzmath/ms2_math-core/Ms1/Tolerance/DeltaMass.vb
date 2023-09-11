#Region "Microsoft.VisualBasic::7c8ba45623986f4d4b384a4c7debe53f, mzkit\src\mzmath\ms2_math-core\Ms1\Tolerance\DeltaMass.vb"

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

    '   Total Lines: 45
    '    Code Lines: 33
    ' Comment Lines: 3
    '   Blank Lines: 9
    '     File Size: 1.62 KB


    '     Class DAmethod
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AsScore, Equals, MassError, MassErrorDescription, Scale
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Namespace Ms1

    ''' <summary>
    ''' Mass tolerance in delta mass error
    ''' </summary>
    Public Class DAmethod : Inherits Tolerance

        Public Overrides ReadOnly Property Type As MassToleranceType
            Get
                Return MassToleranceType.Da
            End Get
        End Property

        <DebuggerStepThrough>
        Sub New(Optional da# = 0.3)
            DeltaTolerance = da
        End Sub

        ''' <summary>
        ''' try to convert the mass dalton error as ppm error for 
        ''' compares the da tolerance with the ppm tolerance in 
        ''' some of the situation
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function GetErrorPPM() As Double
            Dim sample_mz As Double() = {50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000}
            Dim sample_ppm As Double() = sample_mz _
                .Select(Function(mzi) PPMmethod.PPM(mzi, mzi + DeltaTolerance)) _
                .ToArray

            Return sample_ppm.Average
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Equals(mz1 As Double, mz2 As Double) As Boolean
            Return stdNum.Abs(mz1 - mz2) <= DeltaTolerance
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function AsScore(mz1 As Double, mz2 As Double) As Double
            Return 1 - (stdNum.Abs(mz1 - mz2) / DeltaTolerance)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function MassError(mz1 As Double, mz2 As Double) As Double
            Return stdNum.Abs(mz1 - mz2)
        End Function

        Public Overrides Function ToString() As String
            Return $"|mz1 - mz2| <= {DeltaTolerance} da"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function MassErrorDescription(mz1 As Double, mz2 As Double) As String
            Return $"{MassError(mz1, mz2)} da"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function Scale(scaler As Double) As Tolerance
            Return New DAmethod(DeltaTolerance * scaler)
        End Function
    End Class
End Namespace

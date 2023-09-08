#Region "Microsoft.VisualBasic::25dfbe4736e54790dda9916c13735daf, mzkit\src\mzmath\ms2_math-core\Spectra\Models\LibraryMatrix.vb"

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

    '   Total Lines: 189
    '    Code Lines: 124
    ' Comment Lines: 43
    '   Blank Lines: 22
    '     File Size: 6.58 KB


    '     Class LibraryMatrix
    ' 
    '         Properties: centroid, entropy, intensity, ms2, mz
    '                     name, parentMz, totalIon
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AlignMatrix, GetMaxInto
    '         Operators: *, /
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting

Namespace Spectra

    ''' <summary>
    ''' The <see cref="ms2"/> library matrix object model
    ''' </summary>
    Public Class LibraryMatrix : Inherits IVector(Of ms2)
        Implements INamedValue
        Implements ISpectrum

        ''' <summary>
        ''' The list of molecular fragment
        ''' </summary>
        ''' <returns></returns>
        Public Property ms2 As ms2()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return buffer
            End Get
            Set(value As ms2())
                Call writeBuffer(value)
            End Set
        End Property

        Public Property name As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' mass spectrometry data in centroid mode? 
        ''' </summary>
        ''' <returns></returns>
        Public Property centroid As Boolean
        Public Property parentMz As Double

        Default Public Overloads ReadOnly Property Item(booleans As IEnumerable(Of Boolean)) As LibraryMatrix
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New LibraryMatrix() With {
                    .ms2 = MyBase _
                        .Item(booleans) _
                        .ToArray,
                    .name = name
                }
            End Get
        End Property

        ''' <summary>
        ''' get the numeric vector of the <see cref="ms2.intensity"/> value
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public ReadOnly Property intensity As Vector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Me!intensity
            End Get
        End Property

        ''' <summary>
        ''' sum of <see cref="intensity"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property totalIon As Double
            Get
                Return intensity.Sum
            End Get
        End Property

        ''' <summary>
        ''' evaluates the spectrum peaks' shannon entropy value based on the <see cref="intensity"/> 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property entropy As Double
            Get
                Dim i As Vector = intensity
                Dim shannon As Double = (i / i.Sum).ShannonEntropy

                Return shannon
            End Get
        End Property

        ''' <summary>
        ''' get all ms2 fragment peaks m/z values
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public ReadOnly Property mz As Double()
            Get
                Return ms2.Select(Function(i) i.mz).ToArray
            End Get
        End Property

        <DebuggerStepThrough>
        Sub New()
            Call MyBase.New({})
        End Sub

        Sub New(name As String, mz As Double(), into As Double(), Optional centroid As Boolean = True)
            Call MyBase.New(mz.Select(Function(mzi, i) New ms2 With {.mz = mzi, .intensity = into(i)}))

            Me.name = name
            Me.centroid = centroid
        End Sub

        <DebuggerStepThrough>
        Sub New(data As IEnumerable(Of ms2))
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function GetIons() As IEnumerable(Of ms2) Implements ISpectrum.GetIons
            Return Array.AsEnumerable
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub SetIons(ions As IEnumerable(Of ms2)) Implements ISpectrum.SetIons
            buffer = ions.ToArray
        End Sub

        ''' <summary>
        ''' get basepeak ion
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMaxInto() As ms2
            Return buffer(WhichIndex.Symbol.Max(buffer.Select(Function(mz) mz.intensity)))
        End Function

        ''' <summary>
        ''' Create ms2 fragment peaks alignment with a given mass tolerance value
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="tolerance">
        ''' the mass tolerance value
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function could be used for generates the peak matrix for the spectrum 
        ''' alignment purpose.
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AlignMatrix(data As ms2(), tolerance As Tolerance) As ms2()
            Return ms2.AlignMatrix(data, tolerance)
        End Function

        Public Overrides Function ToString() As String
            Return $"[{name}, {Length} ions] {ms2.JoinBy("; ")}"
        End Function

        Public Shared Function ParseStream(data As Byte()) As LibraryMatrix
            Return LibraryMatrixExtensions.ParseStream(data)
        End Function

        ''' <summary>
        ''' normalized to [0, 1]
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="x">should be max intensity</param>
        ''' <returns></returns>
        Public Shared Operator /(matrix As LibraryMatrix, x#) As LibraryMatrix
            For Each ms2 As ms2 In matrix.ms2
                If ms2.intensity = 0 Then
                    ms2.intensity = 0
                Else
                    ms2.intensity = ms2.intensity / x
                End If
            Next

            Return matrix
        End Operator

        ''' <summary>
        ''' ``<see cref="ms2.intensity"/> *= x``
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="x#"></param>
        ''' <returns></returns>
        Public Shared Operator *(matrix As LibraryMatrix, x#) As LibraryMatrix
            For Each ms2 As ms2 In matrix.ms2
                ms2.intensity *= x
            Next

            Return matrix
        End Operator

        ''' <summary>
        ''' Convert the ms2 data collection to <see cref="LibraryMatrix"/>
        ''' </summary>
        ''' <param name="ms2"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(ms2 As ms2()) As LibraryMatrix
            Return New LibraryMatrix With {
                .ms2 = ms2
            }
        End Operator

        ''' <summary>
        ''' Convert the ms2 data collection to <see cref="LibraryMatrix"/>
        ''' </summary>
        ''' <param name="ms2"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(ms2 As List(Of ms2)) As LibraryMatrix
            Return New LibraryMatrix With {
                .ms2 = ms2.ToArray
            }
        End Operator

        ''' <summary>
        ''' 将一个整形数列表转换为mz向量，这个转换函数为调试用的
        ''' </summary>
        ''' <param name="mzlist"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(mzlist As Integer()) As LibraryMatrix
            Return New LibraryMatrix With {
                .ms2 = mzlist _
                    .Select(Function(mz)
                                Return New ms2 With {
                                    .mz = mz,
                                    .intensity = 1
                                }
                            End Function) _
                    .ToArray
            }
        End Operator

        ''' <summary>
        ''' Library matrix to ``&lt;m/z, intensity>`` tuples.
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Narrowing Operator CType(matrix As LibraryMatrix) As (mz#, into#)()
            Return matrix.ms2 _
                .Select(Function(r) (r.mz, r.intensity)) _
                .ToArray
        End Operator

    End Class
End Namespace

#Region "Microsoft.VisualBasic::c4fbdcff68bc4eb5baba686fc5dbfacb, G:/mzkit/src/assembly/NMRFidTool//Math/FFT/FFTBase.vb"

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

    '   Total Lines: 138
    '    Code Lines: 85
    ' Comment Lines: 41
    '   Blank Lines: 12
    '     File Size: 5.17 KB


    '     Class FFTBase
    ' 
    '         Function: bitreverseReference, fft
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System

Namespace fidMath.FFT
    ''' <summary>
    ''' Created with IntelliJ IDEA.
    ''' @author: Orlando Selenu
    ''' User: ldpf
    ''' Date: 21/01/2013
    ''' Time: 10:20
    ''' 
    ''' </summary>

    <Obsolete>
    Public Class FFTBase
        ''' <summary>
        ''' The Fast Fourier Transform (generic version, with NO optimizations).
        ''' </summary>
        ''' <param name="inputReal">
        '''            an array of length n, the real part </param>
        ''' <param name="inputImag">
        '''            an array of length n, the imaginary part </param>
        ''' <param name="DIRECT">
        '''            TRUE = direct transform, FALSE = inverse transform </param>
        ''' <returns> a new array of length 2n </returns>
        Public Shared Function fft(inputReal As Double(), inputImag As Double(), DIRECT As Boolean) As Double()
            ' - n is the dimension of the problem
            ' - nu is its logarithm in base e
            Dim n = inputReal.Length

            ' If n is a power of 2, then ld is an integer (_without_ decimals)
            Dim ld = Math.Log(n) / Math.Log(2.0)

            ' Here I check if n is a power of 2. If exist decimals in ld, I quit
            ' from the function returning null.
            If ld - ld <> 0 Then
                Console.WriteLine("The number of elements is not a power of 2.")
                Return Nothing
            End If

            ' Declaration and initialization of the variables
            ' ld should be an integer, actually, so I don't lose any information in
            ' the cast
            Dim nu As Integer = ld
            Dim n2 As Integer = n / 2
            Dim nu1 = nu - 1
            Dim xReal = New Double(n - 1) {}
            Dim xImag = New Double(n - 1) {}
            Dim tReal, tImag, p, arg, c, s As Double

            ' Here I check if I'm going to do the direct transform or the inverse
            ' transform.
            Dim constant As Double
            If DIRECT Then
                constant = -2 * Math.PI
            Else
                constant = 2 * Math.PI
            End If

            ' I don't want to overwrite the input arrays, so here I copy them. This
            ' choice adds \Theta(2n) to the complexity.
            For i = 0 To n - 1
                xReal(i) = inputReal(i)
                xImag(i) = inputImag(i)
            Next

            ' First phase - calculation
            Dim k = 0
            For l = 1 To nu
                While k < n
                    For i = 1 To n2
                        p = bitreverseReference(k >> nu1, nu)
                        ' direct FFT or inverse FFT
                        arg = constant * p / n
                        c = Math.Cos(arg)
                        s = Math.Sin(arg)
                        tReal = xReal(k + n2) * c + xImag(k + n2) * s
                        tImag = xImag(k + n2) * c - xReal(k + n2) * s
                        xReal(k + n2) = xReal(k) - tReal
                        xImag(k + n2) = xImag(k) - tImag
                        xReal(k) += tReal
                        xImag(k) += tImag
                        k += 1
                    Next
                    k += n2
                End While
                k = 0
                nu1 -= 1
                n2 /= 2
            Next

            ' Second phase - recombination
            k = 0
            Dim r As Integer
            While k < n
                r = bitreverseReference(k, nu)
                If r > k Then
                    tReal = xReal(k)
                    tImag = xImag(k)
                    xReal(k) = xReal(r)
                    xImag(k) = xImag(r)
                    xReal(r) = tReal
                    xImag(r) = tImag
                End If
                k += 1
            End While

            ' Here I have to mix xReal and xImag to have an array (yes, it should
            ' be possible to do this stuff in the earlier parts of the code, but
            ' it's here to readibility).
            Dim newArray = New Double(xReal.Length * 2 - 1) {}
            Dim radice = 1 / Math.Sqrt(n)
            For i = 0 To newArray.Length - 1 Step 2
                Dim i2 As Integer = i / 2
                ' I used Stephen Wolfram's Mathematica as a reference so I'm going
                ' to normalize the output while I'm copying the elements.
                newArray(i) = xReal(i2) * radice
                newArray(i + 1) = xImag(i2) * radice
            Next
            Return newArray
        End Function

        ''' <summary>
        ''' The reference bitreverse function.
        ''' </summary>
        Private Shared Function bitreverseReference(j As Integer, nu As Integer) As Integer
            Dim j2 As Integer
            Dim j1 = j
            Dim k = 0
            For i = 1 To nu
                j2 = j1 / 2
                k = 2 * k + j1 - 2 * j2
                j1 = j2
            Next
            Return k
        End Function
    End Class

End Namespace

Imports System

Namespace BrukerDataReader
    Public Class FourierTransform
        Public Function RealFourierTransform(ByRef data As Double()) As Integer
            ' int iSign = 1;
            Dim n = data.Length
            Dim i As Integer
            Const c1 = 0.5
            Dim hir As Double
            n /= 2
            Dim theta = 3.141592653589793 / n

            'if (iSign == 1)
            '{
            Const c2 As Double = -0.5F
            PerformFourierTransform(n, data, 1)
            '}
            'else
            '{
            '    c2 = 0.5f;
            '    theta = -theta;
            '}

            Dim wTemp = Math.Sin(0.5 * theta)
            Dim wpr = -2.0 * wTemp * wTemp
            Dim wpi = Math.Sin(theta)
            Dim wr = 1.0 + wpr
            Dim wi = wpi
            Dim n2p3 = 2 * n + 3

            For i = 2 To n / 2
                Dim i1 As Integer
                Dim i2 As Integer
                Dim i3 As Integer

                i1 = i + i - 1
                i2 = 1 + i1
                i3 = n2p3 - i2

                Dim i4 = 1 + i3
                hir = c1 * (data(i1 - 1) + data(i3 - 1))
                Dim h1i = c1 * (data(i2 - 1) - data(i4 - 1))
                Dim h2r = -c2 * (data(i2 - 1) + data(i4 - 1))
                Dim h2i = c2 * (data(i1 - 1) - data(i3 - 1))
                data(i1 - 1) = hir + wr * h2r - wi * h2i
                data(i2 - 1) = h1i + wr * h2i + wi * h2r
                data(i3 - 1) = hir - wr * h2r + wi * h2i
                data(i4 - 1) = -h1i + wr * h2i + wi * h2r
                wTemp = wr
                wr = wr * wpr - wi * wpi + wr
                wi = wi * wpr + wTemp * wpi + wi
            Next

            'if (iSign == 1)
            '{
            hir = data(0)
            data(0) = data(0) + data(1)
            data(1) = hir - data(1)
            '		for(i=0;i<(n*2);i++) data[i] /= (n);  // GAA 50-30-00
            '}

            Return 0
        End Function

        Private Sub PerformFourierTransform(ByVal nn As Integer, ByRef data As Double(), ByVal iSign As Integer)
            Dim m As Long
            Dim i As Long
            Dim n As Long = nn << 1
            Dim j As Long = 1

            For i = 1 To n - 1 Step 2

                If j > i Then
                    SwapValuesInArray(data, i - 1, j - 1)
                    SwapValuesInArray(data, i, j)
                End If

                m = n >> 1

                While m >= 2 AndAlso j > m
                    j -= m
                    m >>= 1
                End While

                j += m
            Next

            Dim mMax As Long = 2

            While n > mMax
                Dim iStep = 2 * mMax
                Dim theta = 6.28318530717959 / (iSign * mMax)
                Dim wTemp = Math.Sin(0.5 * theta)
                Dim wpr = -2.0 * wTemp * wTemp
                Dim wpi = Math.Sin(theta)
                Dim wr = 1.0
                Dim wi = 0.0

                For m = 1 To mMax - 1 Step 2
                    i = m

                    While i <= n
                        j = i + mMax
                        Dim jm1 = j - 1
                        Dim im1 = i - 1
                        Dim tempR = wr * data(jm1) - wi * data(j)
                        Dim tempI = wr * data(j) + wi * data(jm1)
                        data(jm1) = data(im1) - tempR
                        data(j) = data(i) - tempI
                        data(im1) += tempR
                        data(i) += tempI
                        i += iStep
                    End While

                    wTemp = wr
                    wr = wr * wpr - wi * wpi + wr
                    wi = wi * wpr + wTemp * wpi + wi
                Next

                mMax = iStep
            End While
        End Sub
    End Class
End Namespace

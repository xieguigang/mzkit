Namespace Formula.IsotopicPatterns

    ''' <summary>
    ''' isotopic pattern calculator
    ''' </summary>
    Public Class Calculator

        ''' <summary>
        ''' Returns an iterator of floats
        ''' in the range [&lt;min_val>,&lt;max_val>] including
        ''' &lt;min_val> but excluding &lt;max_val> With
        ''' an interval Of &lt;Step>.
        ''' </summary>
        ''' <param name="min_val"></param>
        ''' <param name="max_val"></param>
        ''' <param name="[step]"></param>
        ''' <returns></returns>
        Public Shared Iterator Function frange(min_val As Double, max_val As Double, [step] As Double) As IEnumerable(Of Double)
            Dim Val = min_val

            Do While Val < max_val
                Yield Val
                Val += [step]
            Loop
        End Function

        ''' <summary>
        ''' Returns an iterator for application
        ''' of the gaussian function with parameters
        ''' &lt;a>, &lt;b> And &lt;fwhm> To the list &lt;xs>.
        ''' </summary>
        ''' <param name="xs"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="fwhm"></param>
        ''' <returns></returns>
        Public Shared Iterator Function gaussian_iter(xs As Double(), a As Double, b As Double, fwhm As Double) As IEnumerable(Of Double)
            Dim c = fwhm / 2.35482

            a *= 1.0 / (c * Math.Sqrt(2 * Math.PI))

            For Each x In xs
                Yield a * Math.E ^ (-(x - b) ^ 2 / (2 * c ^ 2))
            Next
        End Function

        ''' <summary>
        ''' Applies the gaussian with the given
        ''' parameters to the passed in list.
        ''' </summary>
        ''' <param name="xs"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="fwhm"></param>
        ''' <returns></returns>
        Public Shared Function gaussian(xs As Double(), a As Double, b As Double, fwhm As Double) As Double()
            Return gaussian_iter(xs, a, b, fwhm).ToArray
        End Function
    End Class
End Namespace
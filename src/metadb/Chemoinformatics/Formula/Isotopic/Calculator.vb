#Region "Microsoft.VisualBasic::820270bc1428861aa475011d3f09ad5c, metadb\Chemoinformatics\Formula\Isotopic\Calculator.vb"

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

    '   Total Lines: 110
    '    Code Lines: 63
    ' Comment Lines: 32
    '   Blank Lines: 15
    '     File Size: 4.34 KB


    '     Class Calculator
    ' 
    '         Function: frange, gaussian, gaussian_iter, GenerateDistribution
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Formula.IsotopicPatterns

    ''' <summary>
    ''' isotopic pattern calculator
    ''' </summary>
    Public Class Calculator

        Public Shared Function GenerateDistribution(formula As Formula,
                                                    Optional prob_threshold As Double = 0,
                                                    Optional fwhm As Double = 0.1,
                                                    Optional pad_left As Double = 3,
                                                    Optional pad_right As Double = 3,
                                                    Optional interpolate_grid As Double = 0.1) As IsotopeDistribution

            Dim ds As IsotopeCount() = IsotopeDistribution.Distribution(
                formula:=formula,
                prob_threshold:=prob_threshold
            ) _
                .DoCall(AddressOf IsotopeCount.Normalize) _
                .ToArray
            Dim xs As Double() = (From d In ds Select CDbl(d(3))).ToArray
            Dim ys As Double() = (From d In ds Select CDbl(d.prob)).ToArray
            Dim x_min = xs.Min - pad_left
            Dim x_max = xs.Max + pad_right
            Dim plot_xs = Calculator.frange(x_min, x_max, interpolate_grid).ToArray
            Dim plot_ys = plot_xs.Select(Function(_any) 0.0).ToArray

            For Each i As SeqValue(Of Double) In xs.SeqIterator
                Dim peak_x = i.value
                Dim b = peak_x
                Dim a = ys(i)
                Dim gauss_ys = Calculator.gaussian(plot_xs, a, b, fwhm)

                For Each j In gauss_ys.SeqIterator
                    plot_ys(j) += j.value
                Next
            Next

            Dim ymax As Double = plot_ys.Max

            plot_ys = plot_ys _
                .Select(Function(y) y / ymax * 100) _
                .ToArray

            Return New IsotopeDistribution With {
                .data = ds _
                    .OrderBy(Function(a) a.nom_mass.Sum) _
                    .ToArray,
                .mz = plot_xs,
                .intensity = plot_ys,
                .formula = formula.ToString,
                .exactMass = formula.ExactMass
            }
        End Function

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

#Region "Microsoft.VisualBasic::5790b8a1545a4cc8145ec068d6e1f976, metadb\FormulaSearch.Extensions\IsotopicAlignment.vb"

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

    '   Total Lines: 56
    '    Code Lines: 45 (80.36%)
    ' Comment Lines: 5 (8.93%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (10.71%)
    '     File Size: 2.20 KB


    ' Module IsotopicAlignment
    ' 
    '     Function: AlignIsotopic, GetMS
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns

Public Module IsotopicAlignment

    <Extension>
    Public Function AlignIsotopic(isotopic As IsotopeDistribution, MS As LibraryMatrix, cos As CosAlignment) As AlignmentOutput
        Dim isotopic_MS As ms2() = isotopic.data _
            .Select(Function(c)
                        Dim selects As Double() = MS.ms2 _
                            .Where(Function(mzi) cos.Tolerance(mzi.mz, c.abs_mass)) _
                            .Select(Function(i) i.intensity) _
                            .ToArray
                        Dim intensity As Double = If(selects.Length = 0, 0, selects.Max)
                        Dim ms1 As New ms2 With {
                            .mz = c.abs_mass,
                            .intensity = intensity,
                            .Annotation = c.Formula.EmpiricalFormula
                        }

                        Return ms1
                    End Function) _
            .ToArray
        Dim reference As ms2() = isotopic.GetMS.ToArray
        Dim output As AlignmentOutput = cos.CreateAlignment(reference, isotopic_MS)

        output.query = New Meta With {
            .id = isotopic.ToString,
            .mz = isotopic.exactMass
        }
        output.reference = New Meta With {
            .id = MS.name,
            .mz = isotopic.exactMass,
            .intensity = MS.totalIon
        }

        Return output
    End Function

    ''' <summary>
    ''' convert <see cref="IsotopeDistribution.data"/> as <see cref="ms2"/> vector.
    ''' </summary>
    ''' <param name="isotopic"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GetMS(isotopic As IsotopeDistribution) As IEnumerable(Of ms2)
        For Each count As IsotopeCount In isotopic.data
            Yield New ms2 With {
                .mz = count.abs_mass,
                .intensity = count.abundance
            }
        Next
    End Function
End Module

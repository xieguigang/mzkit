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

#Region "Microsoft.VisualBasic::8c5f2439b899a585487ab06d5bafcbc0, mzmath\MSEngine\SequenceToSpec.vb"

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

    '   Total Lines: 460
    '    Code Lines: 341 (74.13%)
    ' Comment Lines: 51 (11.09%)
    '    - Xml Docs: 5.88%
    ' 
    '   Blank Lines: 68 (14.78%)
    '     File Size: 21.09 KB


    ' Class SequenceToSpec
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Convert2SpecObj, Convert2SpecPeaks, GetBasicMsRefProperty, GetSpectrumPeaksByECD, GetSpectrumPeaksByHCD
    '               GetSpectrumPeaksByHotECD, GetTheoreticalSpectrumByHCD
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public NotInheritable Class SequenceToSpec

    Private Sub New()
    End Sub

    Private Shared OH As Double = 17.002739652
    Private Shared H As Double = 1.00782503207
    Private Shared H2O As Double = 18.010564684
    Private Shared Proton As Double = 1.00727646688
    Private Shared Electron As Double = 0.0005485799
    Private Shared NH3 As Double = 17.026549101
    Private Shared NH2 As Double = 16.01872407
    Private Shared H3PO4 As Double = 97.976895575

    ''' <summary>
    ''' default is 20ppm tolerance error for treated the two mass value as equals.
    ''' </summary>
    Private Shared ReadOnly comparer As IEqualityComparer(Of ISpectrumPeak) = Tolerance.PPM(20)

    Public Shared Function Convert2SpecObj(peptide As Peptide, adduct As AdductIon, [cType] As ActivationMethods, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As MoleculeMsReference
        Select Case [cType]
            Case ActivationMethods.CID
                Return GetTheoreticalSpectrumByHCD(peptide, adduct)
            Case ActivationMethods.HCD
                Return GetTheoreticalSpectrumByHCD(peptide, adduct)
            Case Else
                Return GetTheoreticalSpectrumByHCD(peptide, adduct)
        End Select
    End Function

    Public Shared Function Convert2SpecPeaks(peptide As Peptide, adduct As AdductIon, [cType] As ActivationMethods, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As List(Of SpectrumPeak)
        Select Case [cType]
            Case ActivationMethods.CID
                Return GetSpectrumPeaksByHCD(peptide, adduct, minMz, maxMz)
            Case ActivationMethods.HCD
                Return GetSpectrumPeaksByHCD(peptide, adduct, minMz, maxMz)
            Case ActivationMethods.ECD
                Return GetSpectrumPeaksByECD(peptide, adduct, minMz, maxMz)
            Case ActivationMethods.HotECD
                Return GetSpectrumPeaksByHotECD(peptide, adduct, minMz, maxMz)
            Case Else
                Return GetSpectrumPeaksByHCD(peptide, adduct, minMz, maxMz)
        End Select
    End Function

    Public Shared Function GetTheoreticalSpectrumByHCD(peptide As Peptide, adduct As AdductIon, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As MoleculeMsReference

        Dim msref = GetBasicMsRefProperty(peptide, adduct)
        Dim spectrumPeaks = GetSpectrumPeaksByHCD(peptide, adduct, minMz, maxMz)

        msref.Spectrum = spectrumPeaks
        Return msref
    End Function

    Public Shared Function GetSpectrumPeaksByHCD(peptide As Peptide, adduct As AdductIon, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As List(Of SpectrumPeak)

        Dim sequence = peptide.SequenceObj
        Dim precursorMz = adduct.ConvertToMz(peptide.ExactMass)

        Dim spectrum = New List(Of SpectrumPeak)()
        If precursorMz >= minMz AndAlso precursorMz <= maxMz Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = precursorMz,
                .intensity = 1000,
                .SpectrumComment = SpectrumComment.precursor,
                .PeakID = sequence.Count
            })
        End If
        'if (precursorMz * 0.5 >= minMz && precursorMz * 0.5 <= maxMz)
        '    spectrum.Add(new SpectrumPeak() { Mass = (precursorMz + Proton) * 0.5, Intensity = 1000, SpectrumComment = SpectrumComment.precursor, PeakID = sequence.Count });

        Dim bMz = Proton
        Dim yMz = precursorMz

        Dim bSequence = String.Empty
        Dim ySequence = peptide.Sequence

        Dim bModSequence = String.Empty
        Dim yModSequence = peptide.ModifiedSequence

        If yModSequence.Contains("Y[Phospho]") Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = 216.042021256,
                .intensity = 50,
                .SpectrumComment = SpectrumComment.tyrosinep,
                .PeakID = 0
            })
        End If

        For i = 0 To sequence.Count - 1 ' N -> C

            Dim aaResidueMass = sequence(i).ExactMass() - H2O
            bMz += aaResidueMass
            yMz -= aaResidueMass
            If i = sequence.Count - 1 Then bMz += H2O

            bSequence += sequence(i).OneLetter
            ySequence = ySequence.Substring(1)

            bModSequence += sequence(i).ModifiedCode
            yModSequence = yModSequence.Substring(sequence(i).Code().Length)

            If i + 1 = sequence.Count Then Continue For

            If bMz >= minMz AndAlso bMz <= maxMz Then
                spectrum.Add(New SpectrumPeak() With {
                    .mz = bMz,
                    .intensity = 1000,
                    .SpectrumComment = SpectrumComment.b,
                    .PeakID = i + 1
                })
            End If
            If yMz >= minMz AndAlso yMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.y,
.PeakID = sequence.Count - i - 1
})

            'if (bMz * 0.5 >= minMz && bMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = (bMz + Proton) * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.b2, PeakID = i + 1 });
            'if (yMz * 0.5 >= minMz && yMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = (yMz + Proton) * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.y2, PeakID = sequence.Count - i - 1 });

            If bSequence.Contains("D") OrElse bSequence.Contains("E") OrElse bSequence.Contains("S") OrElse bSequence.Contains("T") Then
                If bMz - H2O >= minMz AndAlso bMz - H2O <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = bMz - H2O,
.intensity = 200,
.SpectrumComment = SpectrumComment.b_h2o,
.PeakID = i + 1
})
            End If
            If ySequence.Contains("D") OrElse ySequence.Contains("E") OrElse ySequence.Contains("S") OrElse ySequence.Contains("T") Then
                If yMz - H2O >= minMz AndAlso yMz - H2O <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz - H2O,
.intensity = 200,
.SpectrumComment = SpectrumComment.y_h2o,
.PeakID = sequence.Count - i - 1
})
            End If

            If bSequence.Contains("K") OrElse bSequence.Contains("N") OrElse bSequence.Contains("Q") OrElse bSequence.Contains("R") Then
                If bMz - NH3 >= minMz AndAlso bMz - NH3 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = bMz - NH3,
.intensity = 200,
.SpectrumComment = SpectrumComment.b_nh3,
.PeakID = i + 1
})
            End If
            If ySequence.Contains("K") OrElse ySequence.Contains("N") OrElse ySequence.Contains("Q") OrElse ySequence.Contains("R") Then
                If yMz - NH3 >= minMz AndAlso yMz - NH3 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz - NH3,
.intensity = 200,
.SpectrumComment = SpectrumComment.y_nh3,
.PeakID = sequence.Count - i - 1
})
            End If

            If bModSequence.Contains("S[Phospho]") OrElse bModSequence.Contains("T[Phospho]") Then
                If bMz - H3PO4 >= minMz AndAlso bMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = bMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.b_h3po4,
.PeakID = i + 1
})
            End If
            If yModSequence.Contains("S[Phospho]") OrElse yModSequence.Contains("T[Phospho]") Then
                If yMz - H3PO4 >= minMz AndAlso yMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.y_h3po4,
.PeakID = sequence.Count - i - 1
})
            End If
        Next

        spectrum = spectrum _
            .GroupBy(Function(spec) spec, comparer) _
            .Select(Function(specs)
                        Return New SpectrumPeak(
                            Enumerable.First(specs).mz,
                            specs.Max(Function(n) n.intensity),
                            String.Join(", ", specs.[Select](Function(spec) spec.Annotation)),
                            specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))
                    End Function) _
            .OrderBy(Function(peak) peak.mz) _
            .ToList()

        Return spectrum
    End Function

    Public Shared Function GetSpectrumPeaksByHotECD(peptide As Peptide, adduct As AdductIon, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As List(Of SpectrumPeak)
        Dim sequence = peptide.SequenceObj
        Dim precursorMz = adduct.ConvertToMz(peptide.ExactMass)

        Dim spectrum = New List(Of SpectrumPeak)()
        If precursorMz >= minMz AndAlso precursorMz <= maxMz Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = precursorMz,
                .intensity = 1000,
                .SpectrumComment = SpectrumComment.precursor,
                .PeakID = sequence.Count
            })
        End If
        If precursorMz * 0.5 >= minMz AndAlso precursorMz * 0.5 <= maxMz Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = (precursorMz + Proton) * 0.5,
                .intensity = 1000,
                .SpectrumComment = SpectrumComment.precursor,
                .PeakID = sequence.Count
            })
        End If

        Dim bMz = Proton
        Dim yMz = precursorMz

        Dim bSequence = String.Empty
        Dim ySequence = peptide.Sequence

        Dim bModSequence = String.Empty
        Dim yModSequence = peptide.ModifiedSequence

        Dim cMz = Proton + NH2 + H * 2.0
        Dim zMz = precursorMz - NH2

        spectrum.Add(New SpectrumPeak() With {
            .mz = zMz,
            .intensity = 1000,
            .SpectrumComment = SpectrumComment.z,
            .PeakID = sequence.Count
        })

        Dim cSequence = String.Empty
        Dim zSequence = peptide.Sequence

        Dim cModSequence = String.Empty
        Dim zModSequence = peptide.ModifiedSequence

        If yModSequence.Contains("Y[Phospho]") Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = 216.042021256,
                .intensity = 50,
                .SpectrumComment = SpectrumComment.tyrosinep,
                .PeakID = 0
            })
        End If

        For i = 0 To sequence.Count - 1 ' N -> C

            Dim aaResidueMass = sequence(i).ExactMass() - H2O
            bMz += aaResidueMass
            yMz -= aaResidueMass
            If i = sequence.Count - 1 Then bMz += H2O

            bSequence += sequence(i).OneLetter
            ySequence = ySequence.Substring(1)

            bModSequence += sequence(i).ModifiedCode
            yModSequence = yModSequence.Substring(sequence(i).Code().Length)

            If bMz >= minMz AndAlso bMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = bMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.b,
.PeakID = i + 1
})
            If yMz >= minMz AndAlso yMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.y,
.PeakID = sequence.Count - i - 1
})

            'if (bMz * 0.5 >= minMz && bMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = bMz * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.b2, PeakID = i + 1 });
            'if (yMz * 0.5 >= minMz && yMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = yMz * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.y2, PeakID = sequence.Count - i - 1 });

            'if (bSequence.Contains("D") || bSequence.Contains("E") || bSequence.Contains("S") || bSequence.Contains("T")) {
            '    if (bMz - H2O >= minMz && bMz - H2O <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = bMz - H2O, Intensity = 200, SpectrumComment = SpectrumComment.b_h2o, PeakID = i + 1 });
            '}
            'if (ySequence.Contains("D") || ySequence.Contains("E") || ySequence.Contains("S") || ySequence.Contains("T")) {
            '    if (yMz - H2O >= minMz && yMz - H2O <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = yMz - H2O, Intensity = 200, SpectrumComment = SpectrumComment.y_h2o, PeakID = sequence.Count - i - 1 });
            '}

            'if (bSequence.Contains("K") || bSequence.Contains("N") || bSequence.Contains("Q") || bSequence.Contains("R")) {
            '    if (bMz - NH3 >= minMz && bMz - NH3 <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = bMz - NH3, Intensity = 200, SpectrumComment = SpectrumComment.b_nh3, PeakID = i + 1 });
            '}
            'if (ySequence.Contains("K") || ySequence.Contains("N") || ySequence.Contains("Q") || ySequence.Contains("R")) {
            '    if (yMz - NH3 >= minMz && yMz - NH3 <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = yMz - NH3, Intensity = 200, SpectrumComment = SpectrumComment.y_nh3, PeakID = sequence.Count - i - 1 });
            '}

            cMz += aaResidueMass
            zMz -= aaResidueMass

            cSequence += sequence(i).OneLetter
            zSequence = zSequence.Substring(1)

            cModSequence += sequence(i).ModifiedCode
            zModSequence = zModSequence.Substring(sequence(i).Code().Length)

            If zMz >= minMz AndAlso zMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = zMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.z,
.PeakID = sequence.Count - i - 1
})

            If bModSequence.Contains("S[Phospho]") OrElse bModSequence.Contains("T[Phospho]") Then
                If bMz - H3PO4 >= minMz AndAlso bMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = bMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.b_h3po4,
.PeakID = i + 1
})
            End If
            If yModSequence.Contains("S[Phospho]") OrElse yModSequence.Contains("T[Phospho]") Then
                If yMz - H3PO4 >= minMz AndAlso yMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = yMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.y_h3po4,
.PeakID = sequence.Count - i - 1
})
            End If
        Next
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Max(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return spectrum
    End Function

    Public Shared Function GetSpectrumPeaksByECD(peptide As Peptide, adduct As AdductIon, Optional minMz As Double = 100, Optional maxMz As Double = 1000000) As List(Of SpectrumPeak)

        Dim sequence = peptide.SequenceObj
        Dim precursorMz = adduct.ConvertToMz(peptide.ExactMass)

        Dim spectrum = New List(Of SpectrumPeak)()
        If precursorMz >= minMz AndAlso precursorMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = precursorMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.precursor,
.PeakID = sequence.Count
})
        If precursorMz * 0.5 >= minMz AndAlso precursorMz * 0.5 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = (precursorMz + Proton) * 0.5,
.intensity = 1000,
.SpectrumComment = SpectrumComment.precursor,
.PeakID = sequence.Count
})

        Dim cMz = Proton + NH2 + H * 2.0
        Dim zMz = precursorMz - NH2
        spectrum.Add(New SpectrumPeak() With {
            .mz = zMz,
            .intensity = 1000,
            .SpectrumComment = SpectrumComment.z,
            .PeakID = sequence.Count
        })

        Dim cSequence = String.Empty
        Dim zSequence = peptide.Sequence

        Dim cModSequence = String.Empty
        Dim zModSequence = peptide.ModifiedSequence

        If zModSequence.Contains("Y[Phospho]") Then
            spectrum.Add(New SpectrumPeak() With {
                .mz = 216.042021256,
                .intensity = 50,
                .SpectrumComment = SpectrumComment.tyrosinep,
                .PeakID = 0
            })
        End If

        For i = 0 To sequence.Count - 1 ' N -> C

            Dim aaResidueMass = sequence(i).ExactMass() - H2O
            cMz += aaResidueMass
            zMz -= aaResidueMass

            cSequence += sequence(i).OneLetter
            zSequence = zSequence.Substring(1)

            cModSequence += sequence(i).ModifiedCode
            zModSequence = zModSequence.Substring(sequence(i).Code().Length)

            'if (cMz >= minMz && cMz <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = cMz, Intensity = 1000, SpectrumComment = SpectrumComment.c, PeakID = i + 1 });
            If zMz >= minMz AndAlso zMz <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = zMz,
.intensity = 1000,
.SpectrumComment = SpectrumComment.z,
.PeakID = sequence.Count - i - 1
})

            'if (cMz * 0.5 >= minMz && cMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = cMz * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.c2, PeakID = i + 1 });
            'if (zMz * 0.5 >= minMz && zMz * 0.5 <= maxMz)
            '    spectrum.Add(new SpectrumPeak() { Mass = zMz * 0.5, Intensity = 100, SpectrumComment = SpectrumComment.z2, PeakID = sequence.Count - i - 1 });

            'if (cSequence.Contains("D") || cSequence.Contains("E") || cSequence.Contains("S") || cSequence.Contains("T")) {
            '    if (cMz - H2O >= minMz && cMz - H2O <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = cMz - H2O, Intensity = 200, SpectrumComment = SpectrumComment.b_h2o, PeakID = i + 1 });
            '}
            'if (zSequence.Contains("D") || zSequence.Contains("E") || zSequence.Contains("S") || zSequence.Contains("T")) {
            '    if (zMz - H2O >= minMz && zMz - H2O <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = zMz - H2O, Intensity = 200, SpectrumComment = SpectrumComment.y_h2o, PeakID = sequence.Count - i - 1 });
            '}

            'if (cSequence.Contains("K") || cSequence.Contains("N") || cSequence.Contains("Q") || cSequence.Contains("R")) {
            '    if (cMz - NH3 >= minMz && cMz - NH3 <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = cMz - NH3, Intensity = 200, SpectrumComment = SpectrumComment.b_nh3, PeakID = i + 1 });
            '}
            'if (zSequence.Contains("K") || zSequence.Contains("N") || zSequence.Contains("Q") || zSequence.Contains("R")) {
            '    if (zMz - NH3 >= minMz && zMz - NH3 <= maxMz)
            '        spectrum.Add(new SpectrumPeak() { Mass = zMz - NH3, Intensity = 200, SpectrumComment = SpectrumComment.y_nh3, PeakID = sequence.Count - i - 1 });
            '}

            If cModSequence.Contains("S[Phospho]") OrElse cModSequence.Contains("T[Phospho]") Then
                If cMz - H3PO4 >= minMz AndAlso cMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = cMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.b_h3po4,
.PeakID = i + 1
})
            End If
            If zModSequence.Contains("S[Phospho]") OrElse zModSequence.Contains("T[Phospho]") Then
                If zMz - H3PO4 >= minMz AndAlso zMz - H3PO4 <= maxMz Then spectrum.Add(New SpectrumPeak() With {
.mz = zMz - H3PO4,
.intensity = 400,
.SpectrumComment = SpectrumComment.y_h3po4,
.PeakID = sequence.Count - i - 1
})
            End If
        Next
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Max(Function(n) n.intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.mz).ToList()
        Return spectrum
    End Function

    Public Shared Function GetBasicMsRefProperty(peptide As Peptide, adduct As AdductIon) As MoleculeMsReference
        Dim precursorMz = adduct.ConvertToMz(peptide.ExactMass)
        Dim msref = New MoleculeMsReference() With {
            .PrecursorMz = precursorMz,
            .IonMode = adduct.IonMode,
            .Name = peptide.ModifiedSequence,
            .Formula = peptide.Formula,
            .Ontology = "Peptide",
            .DatabaseID = peptide.DatabaseOriginID,
            .DatabaseUniqueIdentifier = peptide.DatabaseOrigin
        }
        Return msref
    End Function
End Class

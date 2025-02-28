#Region "Microsoft.VisualBasic::3d24f67236da635971df83c5ad8b143d, metadb\Chemoinformatics\Formula\MS\MemorySheet.vb"

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

    '   Total Lines: 66
    '    Code Lines: 56 (84.85%)
    ' Comment Lines: 4 (6.06%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (9.09%)
    '     File Size: 3.65 KB


    '     Class MemorySheet
    ' 
    '         Function: (+2 Overloads) Formula, GetDefault, GetDefaultNeutralLoss
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula.MS

    Public NotInheritable Class MemorySheet

        ''' <summary>
        ''' https://www.acdlabs.com/blog/common-adduct-and-fragment-ions-in-mass-spectrometry/
        ''' </summary>
        ''' <returns></returns>
        Public Shared Iterator Function GetDefault(Optional nmax As Integer = 3) As IEnumerable(Of ProductIon)
            Yield New ProductIon("[COH]+", "Aldehyde", "RCHO → [COH]⁺ (可能伴随CO丢失)")
            Yield New ProductIon("[NO]+", "Nitro", "")
            Yield New ProductIon("[CH2OH]+", "Alcohol", "Aliphatic")
            Yield New ProductIon("[C3H3]+", "Aromatic", "")
            Yield New ProductIon("[CH2CNH]+", "Nitrile", "")
            Yield New ProductIon("[C2H4O]+", "Aldehyde", "McLafferty重排产物（如酮类断裂）")
            Yield New ProductIon("[OCOH]+", "Carboxylic acid or ester", "")
            Yield New ProductIon("[C4H3]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C3H3O]+", "Ketone", "Cyclic, saturated")
            Yield New ProductIon("[COOCH3]+", "Methyl ester", "")
            Yield New ProductIon("[C5H5]+", "Aromatic", "")
            Yield New ProductIon("[C6H5]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C6H6]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C6H7]+", "Aromatic", "Substituted")
            Yield New ProductIon("[C7H7]+", "Aromatic", "[C₇H₇]⁺ (Tropylium ion，苯环重排产物)")
            Yield New ProductIon("[C7H8]+", "Aromatic", "McLafferty rearrangement")
            Yield New ProductIon("[C6H5O]+", "Ether", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Aldehyde", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Carboxylic acid or ester", "Aromatic")
            Yield New ProductIon("[C7H5O]+", "Ketone", "Aromatic")

            For n As Integer = 1 To nmax
                Yield New ProductIon(Formula(n, 2 * n + 1), "Alkane", "14n+1")
                ' 羧酸/酯系列：CnH2n-1O2 (如n=2: C2H3O2)
                Yield New ProductIon(Formula(n, 2 * n - 1, 2), "Carboxylic acid/ester", $"C{n}H{2 * n - 1}O2 (e.g., COOH loss) [14n+31]")
                Yield New ProductIon(Formula(n, 2 * n - 1), "Alkane", "14n-1")
                Yield New ProductIon(Formula(n, 2 * n), "Alkane", "16n")

                ' 含氮化合物
                Yield New ProductIon(Formula(n, 2 * n + 1, 0, 1), "Amine derivative", $"C{n}H{2 * n + 1}N (胺类碎片)")
            Next
        End Function

        Private Shared Function Formula(C As Integer, H As Integer, O As Integer, N As Integer,
                                        Optional S As Integer = 0,
                                        Optional Cl As Integer = 0,
                                        Optional P As Integer = 0) As Dictionary(Of String, Integer)

            Return New Dictionary(Of String, Integer) From {
                {"C", C}, {"H", H}, {"O", O}, {"N", N}, {"S", S}, {"Cl", Cl}, {"P", P}
            }
        End Function

        Private Shared Function Formula(C As Integer, H As Integer) As Dictionary(Of String, Integer)
            Return New Dictionary(Of String, Integer) From {
                {NameOf(C), C},
                {NameOf(H), H}
            }
        End Function

        Private Shared Function Formula(C As Integer, H As Integer, O As Integer) As Dictionary(Of String, Integer)
            Return New Dictionary(Of String, Integer) From {
                {NameOf(C), C},
                {NameOf(H), H},
                {NameOf(O), O}
            }
        End Function

        Public Shared Iterator Function GetDefaultNeutralLoss() As IEnumerable(Of NeutralLoss)
            Yield New NeutralLoss("[M-OH]+", "Carboxylic acid or ester", "")
            Yield New NeutralLoss("[M-H2O]+", "Alcohol/Aldehyde/Carboxylic acid or ester", "Straight chain/Aromatic")
            Yield New NeutralLoss("[M-NH3]+", "Ammonia Loss", "NH₃ loss (17 Da) from amines, amides")
            Yield New NeutralLoss("[M-CO]+", "Alcohol", "Phenol")
            Yield New NeutralLoss("[M-CO2]+", "Decarboxylation", "CO₂ loss (44 Da) from carboxylic acids")
            Yield New NeutralLoss("[M-C2H4]+", "Cycloalkane/Aldehyde", "")
            Yield New NeutralLoss("[M-COH]+", "Alcohol", "Phenol")
            Yield New NeutralLoss("[M-CH2CH3]+", "Cycloalkane", "")
            Yield New NeutralLoss("[M-CH2CHO]+", "Aldehyde", "Straight chain")
            Yield New NeutralLoss("[M-CH2CHOH]+", "Aldehyde", "Straight chain")
            Yield New NeutralLoss("[M-CO2H]+", "Carboxylic acid or ester", "")
            Yield New NeutralLoss("[M-HCl]+", "Chloride Loss", "HCl loss (36 Da) in chlorinated compounds")

            ' 烷基/芳基丢失
            Yield New NeutralLoss("[M-CH3]+", "Methyl Loss", "CH₃ loss (15 Da) from esters/ethers")
            Yield New NeutralLoss("[M-C2H5]+", "Ethyl Loss", "C₂H₅ loss (29 Da) from ethyl derivatives")
            Yield New NeutralLoss("[M-C6H5]+", "Phenyl Loss", "C₆H₅ loss (77 Da) from aromatic systems")

            ' 含杂原子基团
            Yield New NeutralLoss("[M-HSO3]+", "Sulfate Loss", "HSO₃ loss (80 Da) from sulfates")
            Yield New NeutralLoss("[M-PO3H]+", "Phosphate Loss", "PO₃H loss (98 Da) from nucleotides")
            Yield New NeutralLoss("[M-CH3CO]+", "Acetyl Loss", "CH₃CO loss (43 Da) from acetylated groups")
        End Function
    End Class
End Namespace

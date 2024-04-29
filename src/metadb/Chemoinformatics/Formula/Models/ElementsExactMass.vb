#Region "Microsoft.VisualBasic::f6bce9ccd9a7451d51c14b337f5d5995, E:/mzkit/src/metadb/Chemoinformatics//Formula/Models/ElementsExactMass.vb"

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

    '   Total Lines: 68
    '    Code Lines: 57
    ' Comment Lines: 4
    '   Blank Lines: 7
    '     File Size: 3.48 KB


    '     Module ElementsExactMass
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Formula

    ''' <summary>
    ''' A in-memory database that place the most common used element mass data
    ''' [Mass Diff Dictionary]
    ''' </summary>
    Public Module ElementsExactMass

        Public Const C13_C12 As Double = 1.003354838
        Public Const H2_H1 As Double = 1.006276746
        Public Const N15_N14 As Double = 0.997034893
        Public Const O17_O16 As Double = 1.00421708
        Public Const Si29_Si28 As Double = 0.999568168
        Public Const S33_S32 As Double = 0.99938776
        Public Const CHNO_AverageStepSize As Double = 1.004667751

        Public Const C13_12_Plus_H2_H1 As Double = 2.009631584
        Public Const C13_12_Plus_N15_N14 As Double = 2.000389731
        Public Const C13_12_Plus_O17_O16 As Double = 2.007571918
        Public Const C13_12_Plus_S33_S32 As Double = 2.002742598
        Public Const C13_12_Plus_Si29_Si28 As Double = 2.002923006
        Public Const H2_H1_Plus_N15_N14 As Double = 2.003311639
        Public Const H2_H1_Plus_O17_O16 As Double = 2.010493826
        Public Const H2_H1_Plus_S33_S32 As Double = 2.005664506
        Public Const H2_H1_Plus_Si29_Si28 As Double = 2.005844914
        Public Const N15_N14_Plus_O17_O16 As Double = 2.001251973
        Public Const N15_N14_Plus_S33_S32 As Double = 1.996422653
        Public Const N15_N14_Plus_Si29_Si28 As Double = 1.996603061
        Public Const O17_O16_Plus_S33_S32 As Double = 2.00360484
        Public Const O17_O16_Plus_Si29_Si28 As Double = 2.003785248
        Public Const S33_S32_Plus_Si29_Si28 As Double = 1.998955928
        Public Const C13_C12_Plus_C13_C12 As Double = 2.006709676
        Public Const H2_H1_Plus_H2_H1 As Double = 2.012553492
        Public Const N15_N14_Plus_N15_N14 As Double = 1.994069786
        Public Const O17_O16_Plus_O17_O16 As Double = 2.00843416
        Public Const S33_S32_Plus_S33_S32 As Double = 1.99877552
        Public Const Si29_Si28_Plus_Si29_Si28 As Double = 1.999136336

        Public Const S34_S32 As Double = 1.9957959
        Public Const Si30_Si28 As Double = 1.996843638
        Public Const O18_O16 As Double = 2.00424638
        Public Const Cl37_Cl35 As Double = 1.99704991
        Public Const Br81_Br79 As Double = 1.9979535

        Public Const CarbonMass As Double = 12.0
        Public Const HydrogenMass As Double = 1.00782503207
        Public Const ProtonMass As Double = 1.00727646688
        Public Const NitrogenMass As Double = 14.0030740048
        Public Const OxygenMass As Double = 15.99491461956
        Public Const SulfurMass As Double = 31.972071
        Public Const PhosphorusMass As Double = 30.97376163
        Public Const FluorideMass As Double = 18.99840322
        Public Const SiliconMass As Double = 27.9769265325
        Public Const ChlorideMass As Double = 34.96885268
        Public Const BromineMass As Double = 78.9183371
        Public Const IodineMass As Double = 126.904473
        Public Const SeleniumMass As Double = 79.9165196

        Public Const Carbon13Mass As Double = 13.00335484
        Public Const Hydrogen2Mass As Double = 2.014101778
        Public Const Nitrogen15Mass As Double = 15.0001089
        Public Const Oxygen18Mass As Double = 17.999161
        Public Const Sulfur34Mass As Double = 33.9678669
        Public Const Chloride37Mass As Double = 36.96590259
        Public Const Bromine81Mass As Double = 80.9162906

    End Module
End Namespace

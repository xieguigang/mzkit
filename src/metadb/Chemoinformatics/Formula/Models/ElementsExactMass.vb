Namespace Formula

    ''' <summary>
    ''' A in-memory database that place the most common used element mass data
    ''' </summary>
    Public Module ElementsExactMass

        Public ReadOnly C13_C12 As Double = 1.003354838
        Public ReadOnly H2_H1 As Double = 1.006276746
        Public ReadOnly N15_N14 As Double = 0.997034893
        Public ReadOnly O17_O16 As Double = 1.00421708
        Public ReadOnly Si29_Si28 As Double = 0.999568168
        Public ReadOnly S33_S32 As Double = 0.99938776
        Public ReadOnly CHNO_AverageStepSize As Double = 1.004667751

        Public ReadOnly C13_12_Plus_H2_H1 As Double = 2.009631584
        Public ReadOnly C13_12_Plus_N15_N14 As Double = 2.000389731
        Public ReadOnly C13_12_Plus_O17_O16 As Double = 2.007571918
        Public ReadOnly C13_12_Plus_S33_S32 As Double = 2.002742598
        Public ReadOnly C13_12_Plus_Si29_Si28 As Double = 2.002923006
        Public ReadOnly H2_H1_Plus_N15_N14 As Double = 2.003311639
        Public ReadOnly H2_H1_Plus_O17_O16 As Double = 2.010493826
        Public ReadOnly H2_H1_Plus_S33_S32 As Double = 2.005664506
        Public ReadOnly H2_H1_Plus_Si29_Si28 As Double = 2.005844914
        Public ReadOnly N15_N14_Plus_O17_O16 As Double = 2.001251973
        Public ReadOnly N15_N14_Plus_S33_S32 As Double = 1.996422653
        Public ReadOnly N15_N14_Plus_Si29_Si28 As Double = 1.996603061
        Public ReadOnly O17_O16_Plus_S33_S32 As Double = 2.00360484
        Public ReadOnly O17_O16_Plus_Si29_Si28 As Double = 2.003785248
        Public ReadOnly S33_S32_Plus_Si29_Si28 As Double = 1.998955928
        Public ReadOnly C13_C12_Plus_C13_C12 As Double = 2.006709676
        Public ReadOnly H2_H1_Plus_H2_H1 As Double = 2.012553492
        Public ReadOnly N15_N14_Plus_N15_N14 As Double = 1.994069786
        Public ReadOnly O17_O16_Plus_O17_O16 As Double = 2.00843416
        Public ReadOnly S33_S32_Plus_S33_S32 As Double = 1.99877552
        Public ReadOnly Si29_Si28_Plus_Si29_Si28 As Double = 1.999136336

        Public ReadOnly S34_S32 As Double = 1.9957959
        Public ReadOnly Si30_Si28 As Double = 1.996843638
        Public ReadOnly O18_O16 As Double = 2.00424638
        Public ReadOnly Cl37_Cl35 As Double = 1.99704991
        Public ReadOnly Br81_Br79 As Double = 1.9979535

        Public ReadOnly CarbonMass As Double = 12.0
        Public ReadOnly HydrogenMass As Double = 1.00782503207
        Public ReadOnly ProtonMass As Double = 1.00727646688
        Public ReadOnly NitrogenMass As Double = 14.0030740048
        Public ReadOnly OxygenMass As Double = 15.99491461956
        Public ReadOnly SulfurMass As Double = 31.972071
        Public ReadOnly PhosphorusMass As Double = 30.97376163
        Public ReadOnly FluorideMass As Double = 18.99840322
        Public ReadOnly SiliconMass As Double = 27.9769265325
        Public ReadOnly ChlorideMass As Double = 34.96885268
        Public ReadOnly BromineMass As Double = 78.9183371
        Public ReadOnly IodineMass As Double = 126.904473
        Public ReadOnly SeleniumMass As Double = 79.9165196

        Public ReadOnly Carbon13Mass As Double = 13.00335484
        Public ReadOnly Hydrogen2Mass As Double = 2.014101778
        Public ReadOnly Nitrogen15Mass As Double = 15.0001089
        Public ReadOnly Oxygen18Mass As Double = 17.999161
        Public ReadOnly Sulfur34Mass As Double = 33.9678669
        Public ReadOnly Chloride37Mass As Double = 36.96590259
        Public ReadOnly Bromine81Mass As Double = 80.9162906
    End Module
End Namespace
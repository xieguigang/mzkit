#Region "Microsoft.VisualBasic::35077982ac95664925d9902b874f1953, E:/mzkit/src/mzmath/ms2_math-core//Ms1/Annotation/ExactMassProvider.vb"

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

    '   Total Lines: 223
    '    Code Lines: 11
    ' Comment Lines: 203
    '   Blank Lines: 9
    '     File Size: 11.94 KB


    '     Interface IExactMassProvider
    ' 
    '         Properties: ExactMass
    ' 
    '     Interface ICompoundNameProvider
    ' 
    '         Properties: CommonName
    ' 
    '     Interface IFormulaProvider
    ' 
    '         Properties: Formula
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Ms1.Annotations

    ''' <summary>
    ''' An interface for provides the exact mass value
    ''' </summary>
    ''' <remarks>
    ''' The mass recorded by a mass spectrometer can refer to different physical 
    ''' quantities depending on the characteristics of the instrument and the 
    ''' manner in which the mass spectrum is displayed.
    ''' 
    ''' ## Units
    ''' 
    ''' The dalton(symbol: Da) Is the standard unit that Is used for indicating 
    ''' mass on an atomic Or molecular scale (atomic mass).[1] The unified atomic 
    ''' mass unit (symbol: u) Is equivalent to the dalton. One dalton Is 
    ''' approximately the mass of one a single proton Or neutron.[2] The unified 
    ''' atomic mass unit has a value of 1.660538921(73)×10−27 kg.[3] The amu 
    ''' without the "unified" prefix Is an obsolete unit based on oxygen, which 
    ''' was replaced in 1961.
    ''' 
    ''' ## Molecular mass
    '''
    ''' Theoretical isotope distribution For the molecular ion Of caffeine
    ''' The molecular mass (abbreviated Mr) Of a substance, formerly also called 
    ''' molecular weight And abbreviated As MW, Is the mass Of one molecule Of 
    ''' that substance, relative To the unified atomic mass unit u (equal To 1/12 
    ''' the mass Of one atom Of 12C). Due To this relativity, the molecular mass 
    ''' Of a substance Is commonly referred To As the relative molecular mass, 
    ''' And abbreviated To Mr.
    ''' 
    ''' ## Average mass
    ''' 
    ''' The average mass Of a molecule Is obtained by summing the average atomic 
    ''' masses Of the constituent elements. For example, the average mass Of 
    ''' natural water With formula H2O Is 
    ''' 
    ''' 1.00794 + 1.00794 + 15.9994 = 18.01528 Da.
    ''' 
    ''' ## Mass number
    ''' 
    ''' The mass number, also called the nucleon number, Is the number Of protons 
    ''' And neutrons In an atomic nucleus. The mass number Is unique For Each 
    ''' isotope Of an element And Is written either after the element name Or As 
    ''' a superscript To the left Of an element's symbol. For example, carbon-12 
    ''' (12C) has 6 protons and 6 neutrons.
    ''' 
    ''' ## Nominal mass
    ''' 
    ''' The nominal mass For an element Is the mass number Of its most abundant 
    ''' naturally occurring stable isotope, And For an ion Or molecule, the nominal 
    ''' mass Is the sum Of the nominal masses Of the constituent atoms.[4][5] 
    ''' Isotope abundances are tabulated by IUPAC:[6] for example carbon has two 
    ''' stable isotopes 12C at 98.9% natural abundance And 13C at 1.1% natural 
    ''' abundance, thus the nominal mass of carbon Is 12. The nominal mass Is Not 
    ''' always the lowest mass number, for example iron has isotopes 54Fe, 56Fe, 
    ''' 57Fe, And 58Fe with abundances 6%, 92%, 2%, And 0.3%, respectively, And a 
    ''' nominal mass of 56 Da. For a molecule, the nominal mass Is obtained by 
    ''' summing the nominal masses of the constituent elements, for example water 
    ''' has two hydrogen atoms with nominal mass 1 Da And one oxygen atom with nominal 
    ''' mass 16 Da, therefore the nominal mass of H2O Is 18 Da.
    ''' 
    ''' In mass spectrometry, the difference between the nominal mass And the 
    ''' monoisotopic mass Is the mass defect.[7] This differs from the definition 
    ''' of mass defect used in physics which Is the difference between the mass of
    ''' a composite particle And the sum of the masses of its constituent parts.[8]
    ''' 
    ''' ## Accurate mass
    ''' 
    ''' The accurate mass (more appropriately, the measured accurate mass[9]) Is an 
    ''' experimentally determined mass that allows the elemental composition To be 
    ''' determined.[10] For molecules With mass below 200 Da, 5 ppm accuracy Is often 
    ''' sufficient To uniquely determine the elemental composition.[11]
    ''' 
    ''' ## Exact mass
    ''' 
    ''' The exact mass Of an isotopic species (more appropriately, the calculated 
    ''' exact mass[9]) Is obtained by summing the masses Of the individual isotopes 
    ''' Of the molecule. For example, the exact mass Of water containing two hydrogen-1 
    ''' (1H) And one oxygen-16 (16O) Is 1.0078 + 1.0078 + 15.9949 = 18.0105 Da. 
    ''' The exact mass Of heavy water, containing two hydrogen-2 (deuterium Or 2H) 
    ''' And one oxygen-16 (16O) Is 2.0141 + 2.0141 + 15.9949 = 20.0229 Da.
    '''
    ''' When an exact mass value Is given without specifying an isotopic species, it 
    ''' normally refers to the most abundant isotopic species.
    ''' 
    ''' ## Monoisotopic mass
    ''' Main article: monoisotopic mass
    ''' The monoisotopic mass Is the sum Of the masses Of the atoms In a molecule Using 
    ''' the unbound, ground-state, rest mass Of the principal (most abundant) isotope 
    ''' For Each element.[12][5] The monoisotopic mass Of a molecule Or ion Is the exact
    ''' mass obtained Using the principal isotopes. Monoisotopic mass Is typically 
    ''' expressed In daltons.
    '''
    ''' For typical organic compounds, where the monoisotopic mass Is most commonly used, 
    ''' this also results in the lightest isotope being selected. For some heavier atoms 
    ''' such as iron And argon the principal isotope Is Not the lightest isotope. The 
    ''' mass spectrum peak corresponding To the monoisotopic mass Is often Not observed 
    ''' For large molecules, but can be determined from the isotopic distribution.
    ''' 
    ''' ## Most abundant mass
    '''
    ''' Theoretical isotope distribution For the molecular ion Of glucagon (C153H224N42O50S)
    ''' 
    ''' This refers To the mass Of the molecule With the most highly represented isotope 
    ''' distribution, based On the natural abundance Of the isotopes.
    ''' 
    ''' ## Isotopomer and isotopologue
    ''' 
    ''' Isotopomers (isotopic isomers) are isomers having the same number of each isotopic 
    ''' atom, but differing in the positions of the isotopic atoms.[15] For example, 
    ''' CH3CHDCH3 And CH3CH2CH2D are a pair of structural isotopomers.
    '''
    ''' Isotopomers should Not be confused With isotopologues, which are chemical species 
    ''' that differ In the isotopic composition Of their molecules Or ions. For example, 
    ''' three isotopologues Of the water molecule With different isotopic composition Of 
    ''' hydrogen are: HOH, HOD And DOD, where D stands for deuterium (2H).
    ''' 
    ''' ## Kendrick mass
    ''' 
    ''' The Kendrick mass Is a mass obtained by multiplying the measured mass by a numeric
    ''' factor. The Kendrick mass Is used To aid In the identification Of molecules Of similar 
    ''' chemical Structure from peaks In mass spectra.[16][17] The method Of stating mass was 
    ''' suggested In 1963 by the chemist Edward Kendrick.
    '''
    ''' According to the procedure outlined by Kendrick, the mass of CH2 Is defined as 14.000 Da, 
    ''' instead of 14.01565 Da.[18][19]
    '''
    ''' The Kendrick mass For a family Of compounds F Is given by[20]
    '''
    ''' Kendrick mass = (observed mass)×nominal mass()exact mass
    '''
    ''' {\displaystyle {\mbox{Kendrick mass}}~(F)=({\mbox{observed mass}})\times {\frac {{\mbox{nominal mass}}~(F)}{{\mbox{exact mass}}~(F)}}.}
    ''' 
    ''' For hydrocarbon analysis, F = CH2.
    ''' 
    ''' ## Mass defect (mass spectrometry)
    ''' 
    ''' The mass defect used In nuclear physics Is different from its use In mass spectrometry. 
    ''' In nuclear physics, the mass defect Is the difference In the mass Of a composite particle
    ''' And the sum Of the masses Of its component parts. In mass spectrometry the mass defect 
    ''' Is defined As the difference between the exact mass And the nearest Integer mass.[21][22]
    '''
    ''' The Kendrick mass defect Is the exact Kendrick mass subtracted from the nearest Integer 
    ''' Kendrick mass.[23]
    '''
    ''' Mass defect filtering can be used To selectively detect compounds With a mass spectrometer
    ''' based On their chemical composition.[7]
    ''' 
    ''' ## Packing fraction (mass spectrometry)
    '''
    ''' Francis William Aston won the 1922 Nobel Prize In Chemistry For his discovery, by means 
    ''' Of his mass spectrograph, Of isotopes, In a large number Of non-radioactive elements, 
    ''' And For his enunciation Of the whole number rule.[24][25]
    ''' 
    ''' The term packing fraction was defined by Aston As the difference Of the measured mass M And 
    ''' the nearest Integer mass I (based On the oxygen-16 mass scale) divided by the quantity 
    ''' comprising the mass number multiplied by ten thousand:[26]
    '''
    ''' {\displaystyle f={\frac {M-I}{10^{4}\ I}}}.
    '''
    ''' Aston's early model of nuclear structure (prior to the discovery of the neutron) postulated 
    ''' that the electromagnetic fields of closely packed protons and electrons in the nucleus would 
    ''' interfere and a fraction of the mass would be destroyed.[27] A low packing fraction is 
    ''' indicative of a stable nucleus.[28]
    ''' 
    ''' ## Nitrogen rule
    ''' 
    ''' The nitrogen rule states that organic compounds containing exclusively hydrogen, carbon, 
    ''' nitrogen, oxygen, silicon, phosphorus, sulfur, And the halogens either have an odd nominal 
    ''' mass that indicates an odd number Of nitrogen atoms are present Or an even nominal mass that 
    ''' indicates an even number Of nitrogen atoms are present In the molecular ion.[29][30]
    ''' 
    ''' ## Prout's hypothesis and the whole number rule
    ''' 
    ''' The whole number rule states that the masses Of the isotopes are Integer multiples Of the mass 
    ''' Of the hydrogen atom.[31] The rule Is a modified version Of Prout's hypothesis proposed in 1815, 
    ''' to the effect that atomic weights are multiples of the weight of the hydrogen atom.[32]
    ''' </remarks>
    Public Interface IExactMassProvider

        ''' <summary>
        ''' The exact mass of an isotopic species (more appropriately, the calculated exact mass)
        ''' is obtained by summing the masses of the individual isotopes of the molecule. For 
        ''' example, the exact mass of water containing two hydrogen-1 (1H) and one oxygen-16 (16O)
        ''' is 1.0078 + 1.0078 + 15.9949 = 18.0105 Da. The exact mass of heavy water, containing 
        ''' two hydrogen-2 (deuterium or 2H) and one oxygen-16 (16O) is 
        ''' 2.0141 + 2.0141 + 15.9949 = 20.0229 Da.
        '''
        ''' When an exact mass value Is given without specifying an isotopic species, it normally
        ''' refers to the most abundant isotopic species.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ExactMass As Double

    End Interface

    ''' <summary>
    ''' An interface for provides the metabolite common names
    ''' </summary>
    Public Interface ICompoundNameProvider

        ReadOnly Property CommonName As String

    End Interface

    ''' <summary>
    ''' An interface for provides the chemical formula value
    ''' </summary>
    Public Interface IFormulaProvider

        ''' <summary>
        ''' A chemical formula is a notation used by scientists to show 
        ''' the number and type of atoms present in a molecule, using 
        ''' the atomic symbols and numerical subscripts. A chemical formula 
        ''' is a simple representation, in writing, of a three dimensional 
        ''' molecule that exists. A chemical formula describes a substance,
        ''' down to the exact atoms which make it up.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Formula As String

    End Interface
End Namespace

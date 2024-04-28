#Region "Microsoft.VisualBasic::b73e452bd7d7e597fa2ca2f69732f8ac, E:/mzkit/src/assembly/assembly//mzPack/ActivationMethods.vb"

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

    '   Total Lines: 252
    '    Code Lines: 47
    ' Comment Lines: 161
    '   Blank Lines: 44
    '     File Size: 6.44 KB


    '     Enum ActivationMethods
    ' 
    '         IRMPD, SID
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace mzData

    ''' <summary>
    ''' Activation Types enum
    ''' </summary>
    ''' <remarks>
    ''' This enum mirrors ThermoFisher.CommonCore.Data.FilterEnums.ActivationType
    ''' 
    ''' (这个枚举值为Byte类型，请勿修改，否则mzPack文件格式读写会出现问题)
    ''' </remarks>
    <CLSCompliant(True)>
    Public Enum ActivationMethods As Byte

        ''' <summary>
        ''' Unknown activation type
        ''' </summary>
        Unknown = -1

        ''' <summary>
        ''' Collision-Induced Dissociation
        ''' 
        ''' In collision-induced dissociation (CID), activation of the selected ions 
        ''' occurs by collision(s) with neutral gas molecules in a collision cell. 
        ''' This experiment can be done at high (keV) collision energies, using tandem 
        ''' sector and time-of-flight instruments, or at low (eV range) energies, in 
        ''' tandem quadrupole and ion trapping instruments.
        ''' </summary>
        CID = 0

        ''' <summary>
        ''' Multi Photon Dissociation
        ''' </summary>
        MPD = 1

        ''' <summary>
        ''' Electron Capture Dissociation
        ''' 
        ''' unique fragmentation mechanisms of multiply-charged species can be studied 
        ''' by electron-capture dissociation (ECD). The ECD technique has been recognized 
        ''' as an efficient means to study non-covalent interactions and to gain 
        ''' sequence information in proteomics applications.
        ''' </summary>
        ECD = 2

        ''' <summary>
        ''' Pulsed Q Dissociation
        ''' </summary>
        PQD = 3

        ''' <summary>
        ''' Electron Transfer Dissociation
        ''' 
        ''' Electron transfer dissociation (ETD) involves transfer of an electron from a 
        ''' radical anion to the analyte cation and also produces c and z type ions, and 
        ''' has been implemented with wide variety of mass analyzers, most commonly ion 
        ''' traps.
        ''' </summary>
        ETD = 4

        ''' <summary>
        ''' High-energy Collision-induce Dissociation (psi-ms: beam-type collision-induced dissociation)
        ''' </summary>
        HCD = 5

        ''' <summary>
        ''' Any activation type
        ''' </summary>
        AnyType = 6

        ''' <summary>
        ''' Supplemental Activation
        ''' </summary>
        SA = 7

        ''' <summary>
        ''' Proton Transfer Reaction
        ''' </summary>
        PTR = 8

        ' ReSharper disable once IdentifierTypo
        ''' <summary>
        ''' Negative Electron Transfer Dissociation
        ''' </summary>
        NETD = 9

        ' ReSharper disable once IdentifierTypo
        ''' <summary>
        ''' Negative Proton Transfer Reaction
        ''' </summary>
        NPTR = 10

        ''' <summary>
        ''' Ultraviolet Photo Dissociation
        ''' </summary>
        UVPD = 11

        ''' <summary>
        ''' Mode A
        ''' </summary>
        ModeA = 12

        ''' <summary>
        ''' Mode B
        ''' </summary>
        ModeB = 13

        ''' <summary>
        ''' Mode C
        ''' </summary>
        ModeC = 14

        ''' <summary>
        ''' Mode D
        ''' </summary>
        ModeD = 15

        ''' <summary>
        ''' Mode E
        ''' </summary>
        ModeE = 16

        ''' <summary>
        ''' Mode F
        ''' </summary>
        ModeF = 17

        ''' <summary>
        ''' Mode G
        ''' </summary>
        ModeG = 18

        ''' <summary>
        ''' Mode H
        ''' </summary>
        ModeH = 19

        ''' <summary>
        ''' Mode I
        ''' </summary>
        ModeI = 20

        ''' <summary>
        ''' Mode J
        ''' </summary>
        ModeJ = 21

        ''' <summary>
        ''' Mode K
        ''' </summary>
        ModeK = 22

        ''' <summary>
        ''' Mode L
        ''' </summary>
        ModeL = 23

        ''' <summary>
        ''' Mode M
        ''' </summary>
        ModeM = 24

        ''' <summary>
        ''' Mode N
        ''' </summary>
        ModeN = 25

        ''' <summary>
        ''' Mode O
        ''' </summary>
        ModeO = 26

        ''' <summary>
        ''' Mode P
        ''' </summary>
        ModeP = 27

        ''' <summary>
        ''' Mode Q
        ''' </summary>
        ModeQ = 28

        ''' <summary>
        ''' Mode R
        ''' </summary>
        ModeR = 29

        ''' <summary>
        ''' Mode S
        ''' </summary>
        ModeS = 30

        ''' <summary>
        ''' Mode T
        ''' </summary>
        ModeT = 31

        ''' <summary>
        ''' Mode U
        ''' </summary>
        ModeU = 32

        ''' <summary>
        ''' Mode V
        ''' </summary>
        ModeV = 33

        ''' <summary>
        ''' Mode W
        ''' </summary>
        ModeW = 34

        ''' <summary>
        ''' Mode X
        ''' </summary>
        ModeX = 35

        ''' <summary>
        ''' Mode Y
        ''' </summary>
        ModeY = 36

        ''' <summary>
        ''' Mode Z
        ''' </summary>
        ModeZ = 37

        ''' <summary>
        ''' Last Activation
        ''' </summary>
        LastActivation = 38

        ' ! 在这之前的枚举值不可以修改，否则无法正确映射
        ' Raw文件之中的枚举值

        ''' <summary>
        ''' Collisional activation upon impact of precursor ions on solid surfaces, 
        ''' surface-induced dissociation (SID), is gaining importance as an alternative 
        ''' to gas targets and has been implemented in several different types of mass 
        ''' spectrometers.
        ''' </summary>
        SID

        ''' <summary>
        ''' Trapping instruments, such as quadrupole ion traps and Fourier transform ion 
        ''' cyclotron resonance instruments, are particularly useful for the photoactivation 
        ''' of ions, specifically for fragmentation of precursor ions by infrared 
        ''' multiphoton dissociation (IRMPD). IRMPD is a non-selective activation method 
        ''' and usually yields rich fragmentation spectra. 
        ''' </summary>
        IRMPD
    End Enum
End Namespace

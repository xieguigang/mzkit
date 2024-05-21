#Region "Microsoft.VisualBasic::b73e452bd7d7e597fa2ca2f69732f8ac, assembly\assembly\mzPack\ActivationMethods.vb"

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

''' <summary>
''' Activation Types enum
''' </summary>
''' <remarks>
''' This enum mirrors ThermoFisher.CommonCore.Data.FilterEnums.ActivationType/CollisionType
''' 
''' 在质谱数据中，Collision Type（碰撞类型）和Activation Method（活化方法）通常是指代同一概念的不同术语。
''' 在质谱分析中，为了将大分子（如蛋白质、多肽、聚合物等）打碎成更小的片段以进行分析，常使用一种称为碰撞
''' 诱导解离（Collision-Induced Dissociation, CID）的技术。这个过程中，使用高能碰撞来激活分子，
''' 从而产生碎片。
''' 
''' 活化方法（Activation Method）是一个更广泛的术语，它包括了碰撞诱导解离（CID）以及其他可能的活化技术，
''' 例如电子捕获解离（Electron Capture Dissociation, ECD）、红外多光子解离（Infrared Multiphoton 
''' Dissociation, IRMPD）和黑体红外辐射解离（Blackbody Infrared Radiative Dissociation, BIRD）等。
''' 
''' 碰撞类型（Collision Type）则更具体地指明了在碰撞诱导解离过程中所使用的特定类型，例如使用氩、氖等气体作为碰撞气体来产生碰撞。
''' 
''' 因此，虽然这两个术语在语境上有所不同，但它们都是描述在质谱分析中用来使分子离子发生裂解的方法。
''' 在具体的实验和文献中，它们的含义可能会根据上下文有所变化，但基本上可以认为是相关的概念。
'''
''' 除了碰撞诱导解离（CID），还有其他几种常见的活化方法，用于在质谱分析中产生离子碎片，以便进行结构解析和表征。
''' 这些方法包括：
''' 
''' + 电子捕获解离（Electron Capture Dissociation, ECD）：这种方法在低温下使用电子来断裂分子中的键，特别适用于大分子，如蛋白质和肽。ECD通常用于保持分子中的非共价相互作用，以便更好地理解分子结构。
''' + 红外多光子解离（Infrared Multiphoton Dissociation, IRMPD）：IRMPD使用红外激光的光子能量来激发分子振动，从而导致分子内部的键断裂。这种方法对于具有强红外吸收的分子特别有效。
''' + 黑体红外辐射解离（Blackbody Infrared Radiative Dissociation, BIRD）：BIRD是一种利用黑体辐射源的红外光子能量来解离分子中的键的方法。这种方法通常用于研究大分子和生物大分子。
''' + 快速加热解离（Pulsed Q-TOF）:这种方法通过在离子飞行管中施加快速电压脉冲，使离子加速并获得足够的动能，从而在碰撞过程中发生解离。
''' + 紫外光解离（Ultraviolet Photodissociation, UVPD）：UVPD使用紫外光来激发离子，导致特定的键断裂。这种方法对于具有特定紫外线吸收特性的分子非常有效。
''' + 活化离子飞行时间质谱（Activating Ion Time-of-Flight, ACTOF）：这是一种将离子在飞行管中加速并使其在特定的距离内发生碰撞，从而产生碎片的方法。
''' + 溶剂辅助解离（Solvent Assisted Dissociation, SAD）：SAD是一种在离子源中引入溶剂，以促进离子在形成过程中的解离。
''' 
''' 这些活化方法的选择取决于分析物的类型、所需的解析水平和质谱仪的配置。不同的活化方法可以提供不同的碎片模式和
''' 结构信息，因此在实际应用中，研究人员会根据具体的研究目的选择最合适的活化方法。
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

    EIEIO
    HotECD
    EID
    OAD
End Enum

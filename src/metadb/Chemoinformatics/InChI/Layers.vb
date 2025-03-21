﻿#Region "Microsoft.VisualBasic::15755ea1ca662f1e028790c1bd1b63d1, metadb\Chemoinformatics\InChI\Layers.vb"

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

    '   Total Lines: 100
    '    Code Lines: 40 (40.00%)
    ' Comment Lines: 45 (45.00%)
    '    - Xml Docs: 84.44%
    ' 
    '   Blank Lines: 15 (15.00%)
    '     File Size: 3.39 KB


    '     Class Layer
    ' 
    '         Function: ParseChargeLayer, ParseFixedHLayer, ParseIsotopicLayer, ParseMainLayer, ParseReconnectedLayer
    '                   ParseStereochemicalLayer
    ' 
    '     Enum StereoType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text

Namespace IUPAC.InChI

    ''' <summary>
    ''' ### Layers of the identifier
    ''' 
    ''' ``{InChI version}`` 
    '''
    ''' #### 1. Main Layer (M): 
    ''' + ``/{formula}`` 
    ''' + ``/c{connections}``
    ''' + ``/h{H_atoms}`` 
    '''
    ''' #### 2. Charge Layer 
    ''' + ``/q{charge}`` 
    ''' + ``/p{protons}``
    '''
    ''' #### 3. Stereo Layer 
    ''' + ``/b{stereo:dbond}`` 
    ''' + ``/t{stereo:sp3}`` 
    ''' + ``/m{stereo:sp3:inverted}`` 
    ''' + ``/s{stereo:type (1=abs, 2=rel, 3=rac)}`` 
    '''
    ''' #### 4. Isotopic Layer (MI): 
    ''' + ``/i{isotopicatoms}*`` 
    ''' + ``/h{isotopic:exchangeable_H}`` 
    ''' + ``/b{isotopic:stereo:dbond}`` 
    ''' + ``/t{isotopic:stereo:sp3}`` 
    ''' + ``/m{isotopic:stereo:sp3:inverted}`` 
    ''' + ``/s{isotopic:stereo:type (1=abs, 2=rel, 3=rac)}`` 
    '''
    ''' #### 5. Fixed H Layer (F): 
    ''' + ``/f{fixed_Hformula}*`` 
    ''' + ``/h{fixed_H:H_fixed}`` 
    ''' + ``/q{fixed_H:charge}`` 
    ''' + ``/b{fixed_H:stereo:dbond}`` 
    ''' + ``/t{fixed_H:stereo:sp3}`` 
    ''' + ``/m{fixed_H:stereo:sp3:inverted}`` 
    ''' + ``/s{fixed_H:stereo:type (1=abs, 2=rel, 3=rac)}`` 
    '''
    ''' ### (6.) Fixed/Isotopic Combination (FI) 
    ''' + ``/i{fixed_H:isotopic:atoms}*`` 
    ''' + ``/b{fixed_H:isotopic:stereo:dbond}`` 
    ''' + ``/t{fixed_H:isotopic:stereo:sp3}`` 
    ''' + ``/m{fixed_H:isotopic:stereo:sp3:inverted}`` 
    ''' + ``/s{fixed_H:isotopic:stereo:type (1=abs, 2=rel, 3=rac)}`` 
    ''' + ``/o{transposition}``
    ''' </summary>
    Public MustInherit Class Layer

        Friend Shared Function ParseMainLayer(tokens As InChIStringReader) As MainLayer
            Dim main As New MainLayer With {
                .Formula = tokens.GetByPrefix(ASCII.NUL),
                .Struct = MainLayer.ParseBounds(tokens.GetByPrefix("c"c)),
                .Hydrogen = MainLayer.ParseHAtoms(tokens.GetByPrefix("h"c)).ToArray
            }

            Return main
        End Function

        Friend Shared Function ParseChargeLayer(tokens As InChIStringReader) As ChargeLayer
            Dim charge As New ChargeLayer With {
                .Proton = tokens.GetByPrefix("p"c).ParseInteger,
                .Charge = tokens.GetByPrefix("q"c)
            }

            Return charge
        End Function

        Friend Shared Function ParseStereochemicalLayer(tokens As InChIStringReader) As StereochemicalLayer
            Dim stereochemical As New StereochemicalLayer With {
                .DoubleBounds = tokens.GetByPrefix("b"c),
                .Tetrahedral = tokens.GetByPrefix({"t"c, "m"c}),
                .Type = tokens.GetByPrefix("s"c)
            }

            Return stereochemical
        End Function

        Friend Shared Function ParseIsotopicLayer(tokens As InChIStringReader) As IsotopicLayer

        End Function

        Friend Shared Function ParseFixedHLayer(tokens As InChIStringReader) As FixedHLayer

        End Function

        Friend Shared Function ParseReconnectedLayer(tokens As InChIStringReader) As ReconnectedLayer

        End Function
    End Class

    Public Enum StereoType
        Abs = 1
        Rel = 2
        Rac = 3
    End Enum
End Namespace

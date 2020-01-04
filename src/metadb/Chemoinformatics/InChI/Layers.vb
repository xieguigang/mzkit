#Region "Microsoft.VisualBasic::2a79a82e0473f78c9958fabb0ed52864, src\metadb\Massbank\InChI\Layers.vb"

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

    '     Class Layer
    ' 
    '         Function: GetByPrefix, ParseChargeLayer, ParseFixedHLayer, ParseIsotopicLayer, ParseMainLayer
    '                   ParseReconnectedLayer, ParseStereochemicalLayer
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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace IUPAC.InChILayers

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

        Shared ReadOnly prefixes As Index(Of Char) = "chpqbtmsihfr"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function GetByPrefix(tokens As String()) As Func(Of [Variant](Of Char, Char()), String)
            Return Function(c As [Variant](Of Char, Char()))
                       Dim str As String

                       If c Like GetType(Char) Then
                           If c = ASCII.NUL Then
                               Return tokens.First(Function(t) Not t.First Like prefixes)
                           Else
                               str = tokens.FirstOrDefault(Function(t) c = t.First)
                           End If
                       Else
                           With CType(c, Char())
                               str = tokens.FirstOrDefault(Function(t) .Any(Function(cc) cc = t.First))
                           End With
                       End If

                       If str.StringEmpty Then
                           Return ""
                       Else
                           Return str.Substring(1)
                       End If
                   End Function
        End Function

        Friend Shared Function ParseMainLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As MainLayer
            Dim main As New MainLayer With {
                .Formula = tokens(ASCII.NUL),
                .Bounds = MainLayer.ParseBounds(tokens("c"c)).ToArray,
                .Hydrogen = tokens("h"c)
            }

            Return main
        End Function

        Friend Shared Function ParseChargeLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As ChargeLayer
            Dim charge As New ChargeLayer With {
                .Proton = tokens("p"c).ParseInteger,
                .Charge = tokens("q"c)
            }

            Return charge
        End Function

        Friend Shared Function ParseStereochemicalLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As StereochemicalLayer
            Dim stereochemical As New StereochemicalLayer With {
                .DoubleBounds = tokens("b"c),
                .Tetrahedral = tokens({"t"c, "m"c}),
                .Type = tokens("s"c)
            }

            Return stereochemical
        End Function

        Friend Shared Function ParseIsotopicLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As IsotopicLayer

        End Function

        Friend Shared Function ParseFixedHLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As FixedHLayer

        End Function

        Friend Shared Function ParseReconnectedLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As ReconnectedLayer

        End Function
    End Class

    Public Enum StereoType
        Abs = 1
        Rel = 2
        Rac = 3
    End Enum
End Namespace

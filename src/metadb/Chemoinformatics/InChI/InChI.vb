#Region "Microsoft.VisualBasic::d89872b4ec68e7a0a2f2d549122c97fd, metadb\Chemoinformatics\InChI\InChI.vb"

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

'   Total Lines: 64
'    Code Lines: 42 (65.62%)
' Comment Lines: 12 (18.75%)
'    - Xml Docs: 83.33%
' 
'   Blank Lines: 10 (15.62%)
'     File Size: 2.52 KB
    '   Total Lines: 63
    '    Code Lines: 41 (65.08%)
    ' Comment Lines: 12 (19.05%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 10 (15.87%)
    '     File Size: 2.47 KB


'     Class InChI
' 
'         Properties: Charge, FixedH, Isotopic, IsStandard, Key
'                     Main, Reconnected, Stereochemical, Version
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: Parse, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace IUPAC.InChI

    ''' <summary>
    ''' The InChI identifier parser
    ''' 
    ''' 国际化合物标识（英语：InChI，英语：International Chemical Identifier）是由国际纯粹与
    ''' 应用化学联合会和美国国家标准技术研究所（National Institute of Standards and Technology，NIST）
    ''' 联合制定的，用以唯一标识化合物IUPAC名称的字符串。
    ''' </summary>
    ''' <remarks>
    ''' Parsing an InChI (International Chemical Identifier) string can be a complex task 
    ''' due to the detailed and structured format of the InChI. However, the InChI format 
    ''' is designed to be readable by both humans and machines, and it can be broken down 
    ''' into several layers that represent different aspects of the chemical substance.
    ''' 
    ''' Here's a basic guide on how to manually parse an InChI string:
    ''' 
    ''' 1. **InChI Version**: The InChI string starts with the version number, followed by a slash. For example, `InChI=1S/`.
    ''' 2. **Formula Layer**: This is the chemical formula of the compound. It starts after the version and
    '''    is enclosed in parentheses. For example, `InChI=1S/C6H12O6/` indicates that the compound is composed of 6 carbon, 
    '''    12 hydrogen, and 6 oxygen atoms.
    ''' 3. **Main Layer**: This layer describes the connectivity of atoms in the molecule and is followed by a slash. It can 
    '''    include:
    '''    - Atom connectivity (`c`)
    '''    - Hydrogen atoms (`h`)
    '''    - Charge (`q`)
    '''    - Stereochemistry (`b`, `t`, `m`)
    ''' 4. **Stereochemical Layer**: This layer provides information about the stereochemistry of the compound if applicable. 
    '''    It is also followed by a slash.
    ''' 5. **Isotopic Layer**: If the compound contains isotopes, this layer will provide that information, followed by a slash.
    ''' 6. **Fixed-H Layer**: This layer indicates the positions of non-exchangeable hydrogen atoms and is followed by a slash.
    ''' 7. **Reconnected Layer**: In cases where the molecule can be disconnected into multiple subunits, this layer will describe 
    '''    the reconnected form of the molecule.
    ''' 8. **Charge Layer**: This layer indicates the net electric charge of the compound.
    ''' 9. **Auxiliary Information**: Sometimes additional information is provided in parentheses at the end of the InChI string.
    ''' 
    ''' Here's an example of an InChI string broken down into its components:
    ''' 
    ''' ```
    ''' InChI=1S/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-1/h1-11H/t2-6+/m1/s1
    ''' ```
    ''' 
    ''' - `InChI=1S/` indicates the InChI version.
    ''' - `C6H12O6/` is the formula layer.
    ''' - `c7-1-2-3(8)4(9)5(10)6(11)` is the main layer with atom connectivity.
    ''' - `h1-11H` indicates the presence of hydrogen atoms.
    ''' - `t2-6+` indicates the stereochemistry.
    ''' - `/m1/s1` provides additional stereochemical information.
    ''' </remarks>
    <ContentType("chemical/x-inchi")> Public Class InChI

        Public Property Main As MainLayer
        Public Property Charge As ChargeLayer
        Public Property Stereochemical As StereochemicalLayer
        Public Property Isotopic As IsotopicLayer
        Public Property FixedH As FixedHLayer
        Public Property Reconnected As ReconnectedLayer

        Public Property Version As Integer
        Public Property IsStandard As Boolean

        Public ReadOnly Property Key As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return ToString()
            End Get
        End Property

        Public ReadOnly Property InChIKey As InChIKey
            Get
                Return New InChIKey(Me)
            End Get
        End Property

        Sub New(inchi As String)
            Dim tokens$() = inchi.GetTagValue("=").Value.Split("/"c)
            Dim version = tokens(Scan0)
            Dim populator As New InChIStringReader(tokens.Skip(1).ToArray)

            Me.Version = Val(version)
            Me.IsStandard = Not version.IsPattern("\d+")

            Main = Layer.ParseMainLayer(populator)
            Charge = Layer.ParseChargeLayer(populator)
            Stereochemical = Layer.ParseStereochemicalLayer(populator)
            Isotopic = Layer.ParseIsotopicLayer(populator)
            FixedH = Layer.ParseFixedHLayer(populator)
            Reconnected = Layer.ParseReconnectedLayer(populator)
        End Sub

        ''' <summary>
        ''' convert the inchi structure to SDF structure data
        ''' </summary>
        ''' <returns></returns>
        Public Function GetStruct() As [Structure]
            Dim atoms As New Dictionary(Of UInteger, Atom)


        End Function

        ''' <summary>
        ''' Generate A InChI string base on the layer information
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return New InChIKey(Me).ToString()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(inchi As String) As InChI
            Return New InChI(inchi)
        End Function
    End Class
End Namespace

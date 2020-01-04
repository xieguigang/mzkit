#Region "Microsoft.VisualBasic::138aaaa8d90958c29fbab8a07a5a2155, src\metadb\Massbank\InChI\InChI.vb"

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
Imports BioNovoGene.BioDeep.Chemistry.IUPAC.InChILayers
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace IUPAC

    ''' <summary>
    ''' The InChI identifier parser
    ''' 
    ''' 国际化合物标识（英语：InChI，英语：International Chemical Identifier）是由国际纯粹与
    ''' 应用化学联合会和美国国家标准技术研究所（National Institute of Standards and Technology，NIST）
    ''' 联合制定的，用以唯一标识化合物IUPAC名称的字符串。
    ''' </summary>
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

        Sub New(inchi As String)
            Dim tokens$() = inchi.GetTagValue("=").Value.Split("/"c)
            Dim version = tokens(Scan0)
            Dim populator = Layer.GetByPrefix(tokens.Skip(1).ToArray)

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports SMRUCC.MassSpectrum.DATA.IUPAC.InChILayers

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

        Public Property Version As String
        Public Property IsStandard As Boolean

        Sub New(inchi As String)
            Dim tokens$() = inchi.GetTagValue("=").Value.Split("/"c)
            Dim version = tokens(Scan0)
            Dim populator = Layer.GetByPrefix(tokens.Skip(1).ToArray)

            If version.IsPattern("\d+") Then
                Me.Version = version
                Me.IsStandard = False
            Else
                Me.Version = Val(version)
                Me.IsStandard = True
            End If

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
        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(inchi As String) As InChI
            Return New InChI(inchi)
        End Function
    End Class
End Namespace


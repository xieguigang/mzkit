Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace IUPAC.InChILayers

    Public MustInherit Class Layer

        Shared ReadOnly prefixes As Index(Of Char) = "chpqbtmsihfr"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function GetByPrefix(tokens As String()) As Func(Of [Variant](Of Char, Char()), String)
            Return Function(c As [Variant](Of Char, Char()))
                       If c Like GetType(Char) Then
                           If c = ASCII.NUL Then
                               Return tokens.First(Function(t) Not t.First Like prefixes)
                           Else
                               Return tokens.FirstOrDefault(Function(t) c = t.First)
                           End If
                       Else
                           With CType(c, Char())
                               Return tokens.FirstOrDefault(Function(t) .Any(Function(cc) cc = t.First))
                           End With
                       End If
                   End Function
        End Function

        Friend Shared Function ParseMainLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As MainLayer
            Dim main As New MainLayer With {
                .Formula = tokens(ASCII.NUL),
                .Bounds = tokens("c"c),
                .Hydrogen = tokens("h"c)
            }

            Return main
        End Function

        Friend Shared Function ParseChargeLayer(tokens As Func(Of [Variant](Of Char, Char()), String)) As ChargeLayer
            Dim charge As New ChargeLayer With {
                .Proton = tokens("p"c),
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

    ''' <summary>
    ''' 主层（main layer）：以``1``表示
    ''' </summary>
    Public Class MainLayer : Inherits Layer

        ''' <summary>
        ''' Chemical formula (no prefix). This is the only sublayer that must occur in every InChI.
        ''' </summary>
        ''' <returns></returns>
        Public Property Formula As String
        ''' <summary>
        ''' Atom connections (prefix: "c"). The atoms in the chemical formula (except for hydrogens) 
        ''' are numbered in sequence; this sublayer describes which atoms are connected by bonds to 
        ''' which other ones.
        ''' </summary>
        ''' <returns></returns>
        Public Property Bounds As String
        ''' <summary>
        ''' Hydrogen atoms (prefix: "h"). Describes how many hydrogen atoms are connected to each of 
        ''' the other atoms.
        ''' </summary>
        ''' <returns></returns>
        Public Property Hydrogen As String

    End Class

    ''' <summary>
    ''' 电荷层（charge layer）：以``q``表示
    ''' </summary>
    Public Class ChargeLayer : Inherits Layer

        ''' <summary>
        ''' proton sublayer (prefix: "p" for "protons")
        ''' </summary>
        ''' <returns></returns>
        Public Property Proton As String
        ''' <summary>
        ''' charge sublayer (prefix: "q")
        ''' </summary>
        ''' <returns></returns>
        Public Property Charge As String
    End Class

    ''' <summary>
    ''' 立体化学层（Stereochemical layer）：以``t``，``m``，``s``表示
    ''' </summary>
    Public Class StereochemicalLayer : Inherits Layer
        ''' <summary>
        ''' double bonds and cumulenes (prefix: "b")
        ''' </summary>
        ''' <returns></returns>
        Public Property DoubleBounds As String
        ''' <summary>
        ''' tetrahedral stereochemistry of atoms and allenes (prefixes: "t", "m")
        ''' </summary>
        ''' <returns></returns>
        Public Property Tetrahedral As String
        ''' <summary>
        ''' type of stereochemistry information (prefix: "s")
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
    End Class

    ''' <summary>
    ''' 异构体层（Isotopic layer）：以``i``表示
    ''' </summary>
    ''' <remarks>
    ''' Isotopic layer (prefixes: "i", "h", as well as "b", "t", "m", "s" for isotopic 
    ''' stereochemistry)
    ''' </remarks>
    Public Class IsotopicLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 固定氢原子（Fixed-H layer）：以``f``表示
    ''' </summary>
    ''' <remarks>
    ''' Fixed-H layer (prefix: "f"); contains some or all of the above types of layers 
    ''' except atom connections; may end with "o" sublayer; never included in standard 
    ''' InChI
    ''' </remarks>
    Public Class FixedHLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 再连接层（Reconnected Layer）：以``r``表示
    ''' </summary>
    ''' <remarks>
    ''' Reconnected layer (prefix: "r"); contains the whole InChI of a structure with 
    ''' reconnected metal atoms; never included in standard InChI
    ''' </remarks>
    Public Class ReconnectedLayer : Inherits Layer

    End Class
End Namespace
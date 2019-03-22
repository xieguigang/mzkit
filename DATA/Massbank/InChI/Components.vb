Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser
Imports SMRUCC.MassSpectrum.DATA.File

Namespace IUPAC.InChILayers

    ''' <summary>
    ''' 主层（main layer）：以``1``表示
    ''' </summary>
    Public Class MainLayer : Inherits Layer

        ''' <summary>
        ''' Chemical formula (no prefix). This is the only sublayer that must occur in every InChI.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' For a compounds composed of a single component, this is the conventional Hill-sorted 
        ''' elemental formula. For compounds containing multiple components, the Hill-sorted 
        ''' formulas of the individual components are sorted according to the guidelines in Figure 
        ''' 1 and separated by dots.
        ''' </remarks>
        Public Property Formula As String
        ''' <summary>
        ''' Atom connections (prefix: "c"). The atoms in the chemical formula (except for hydrogens) 
        ''' are numbered in sequence; this sublayer describes which atoms are connected by bonds to 
        ''' which other ones.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' ##### Connections (except ``H``)
        ''' 
        ''' This lists the bonds between the atoms in the structure, partitioned into as many as three 
        ''' sublayers. The first represents all bonds other than those to non-bridging H-atoms, the 
        ''' second represents bonds of all immobile H-atoms, and the third provides locations of any 
        ''' mobile H-atoms. The last sublayer represents H-atoms that can be found at more than one 
        ''' location in a compound due to well-known varieties of isomerization. It identifies the groups 
        ''' of atoms that share one or more mobile hydrogen atoms In addition to hydrogen atoms, mobile 
        ''' H groups may contain mobile negative charges. These charges are included in the charge layer.
        ''' </remarks>
        Public Property Bounds As Bound()
        ''' <summary>
        ''' Hydrogen atoms (prefix: "h"). Describes how many hydrogen atoms are connected to each of 
        ''' the other atoms.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' H (static H and mobile H groups)
        ''' </remarks>
        Public Property Hydrogen As String

        Public Shared Iterator Function ParseBounds(token As String) As IEnumerable(Of Bound)
            Dim chars As New CharPtr(token)
            Dim i As Integer
            Dim j As Integer
            Dim buffer As New List(Of Char)
            Dim c As Value(Of Char) = ASCII.NUL
            Dim popIndex = Function() Integer.Parse(buffer.PopAll.JoinBy(""))
            Dim indexStack As New Stack(Of Integer)
            Dim previous As Char = ASCII.NUL

            Do While True
                If Char.IsNumber(c = ++chars) Then
                    buffer += c
                Else
                    i = popIndex()
                    Exit Do
                End If
            Loop

            Do While Not chars.EndRead
                If Char.IsNumber(c = ++chars) Then
                    buffer += c
                ElseIf c.Equals("-"c) Then
                    j = popIndex()

                    Yield New Bound With {
                        .i = i,
                        .j = j
                    }
                    i = j
                ElseIf c.Equals("("c) Then
                    indexStack.Push(i)
                    i = popIndex()
                ElseIf c.Equals(")"c) Then
                    If buffer > 0 Then
                        j = popIndex()

                        Yield New Bound With {
                            .i = i,
                            .j = j
                        }
                    End If
                Else
                    Throw New NotImplementedException(c)
                End If

                previous = c
            Loop

            If buffer > 0 Then
                j = popIndex()

                Yield New Bound With {
                    .i = i,
                    .j = j
                }
            End If
        End Function
    End Class

    ''' <summary>
    ''' 电荷层（charge layer）：以``q``表示
    ''' </summary>
    Public Class ChargeLayer : Inherits Layer

        ''' <summary>
        ''' proton sublayer (prefix: "p" for "protons")
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The number of protons removed from or added to the substance so that a given component may be 
        ''' represented without regard to its degree of protonation.
        ''' </remarks>
        Public Property Proton As Integer
        ''' <summary>
        ''' charge sublayer (prefix: "q")
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The net charges of the components are represented in this layer as independent tags. By design, 
        ''' the InChI does not distinguish between structures that differ only in the formal positions of 
        ''' their electrons.
        ''' </remarks>
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
        ''' <remarks>
        ''' ##### Double Bond sp2 (Z/E) Stereo
        ''' 
        ''' Expression of this stereo configuration is easily done in 2-dimensional drawings. When double bonds 
        ''' are rigid, stereoisomerism is readily represented without ambiguity. However, in alternating bond 
        ''' systems, some non-rigid bonds may be formally drawn as double. Bonds in these systems, when discovered 
        ''' by InChI algorithms, are not assigned stereo labels. Also, to avoid needless stereodescriptors in 
        ''' aromatic and other small rings, no sp2 stereoisomerism information is generated in rings containing 
        ''' 7 or fewer members.
        ''' </remarks>
        Public Property DoubleBounds As String
        ''' <summary>
        ''' tetrahedral stereochemistry of atoms and allenes (prefixes: "t", "m")
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' ##### Tetrahedral Stereo
        ''' 
        ''' Tetrahedral (typically, sp3) stereochemistry is readily represented using conventional wedge/hatch (out/in) 
        ''' bonds commonly employed in 2D drawings. Relative tetrahedral stereochemistry is represented first, 
        ''' optionally followed by a tag to indicate absolute stereochemistry. When a stereocenter configuration is not 
        ''' known to the structure author, an „unknown‟ descriptor may be specified, which will then appear in the 
        ''' stereo layer. If a possible stereocenter is found, but no stereo information is provided, it will be 
        ''' represented in a stereolayer by a not-given (``undefined``) flag.
        ''' </remarks>
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
    ''' 
    ''' This is a layer in which different isotopically labeled atoms are identified. Exchangeable isotopic hydrogen atoms 
    ''' (deuterium and tritium) are listed separately. The layer also holds any changes in stereochemistry caused by the 
    ''' presence of isotopes.
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
    ''' 
    ''' When potentially mobile H atoms are detected and the user specifies that they should be immobile (tautomerism not 
    ''' allowed), this layer binds these H atoms to the atoms specified in the input structure. When this, in effect, 
    ''' causes a change in earlier layers, appropriate changes are added to this layer (earlier layers 1-4 are not 
    ''' affected).
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
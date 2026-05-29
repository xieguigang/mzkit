#Region "Microsoft.VisualBasic::bd13fd64aebb9da1bc918f2d57a9903c, metadb\Chemoinformatics\Lipid\Chain.vb"

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

    '   Total Lines: 73
    '    Code Lines: 57 (78.08%)
    ' Comment Lines: 3 (4.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (17.81%)
    '     File Size: 2.64 KB


    '     Class Chain
    ' 
    '         Properties: carbons, doubleBonds, groups, hasStructureInfo, position
    '                     tag
    ' 
    '         Function: ParseName, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Lipidomics

    ''' <summary>
    ''' the lipid carbon chain
    ''' </summary>
    Public Class Chain

        Public Property tag As String
        Public Property carbons As Integer
        Public Property doubleBonds As Integer
        Public Property position As BondPosition()
        Public Property groups As Group()

        Public ReadOnly Property hasStructureInfo As Boolean
            Get
                Return Not (position.IsNullOrEmpty AndAlso groups.IsNullOrEmpty AndAlso tag.StringEmpty)
            End Get
        End Property

        Public Overrides Function ToString() As String
            If hasStructureInfo Then
                Dim name As String = $"{tag}{carbons}:{doubleBonds}"

                If Not position.IsNullOrEmpty Then
                    name = name & $"({position.JoinBy(",")})"
                End If

                If Not groups.IsNullOrEmpty Then
                    name = name & "-" & groups.JoinBy("-")
                End If

                Return name
            Else
                Return $"{carbons}:{doubleBonds}"
            End If
        End Function

        Friend Shared Function ParseName(components As String) As Chain
            Dim overview As String() = components.Match(".+[:]\d+").Split(":"c)
            Dim carbons As Integer
            Dim tag As String = Nothing
            Dim is_empty As Boolean = overview.Length = 1 AndAlso overview(Scan0).StringEmpty

            If Not is_empty Then
                components = components.Replace(overview.JoinBy(":"), "")
            End If

            If overview(Scan0).IsInteger Then
                carbons = Integer.Parse(overview(Scan0))
            ElseIf Not is_empty Then
                tag = overview(Scan0).StringReplace("\d+", "")
                carbons = Integer.Parse(overview(Scan0).Match("\d+"))
            End If

            Dim DBes As Integer = If(is_empty, 0, Integer.Parse(overview(1)))
            Dim bonds As BondPosition() = BondPosition.ParseStructure(components).ToArray

            Return New Chain With {
                .carbons = carbons,
                .tag = tag,
                .doubleBonds = DBes,
                .groups = (From b As BondPosition
                           In bonds
                           Where TypeOf b Is Group
                           Select DirectCast(b, Group)).ToArray,
                .position = bonds _
                    .Where(Function(b) Not TypeOf b Is Group) _
                    .ToArray
            }
        End Function
    End Class

End Namespace

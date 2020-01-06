#Region "Microsoft.VisualBasic::d5f6f7d55a1234b24b3080d548b92e27, src\metadb\Chemoinformatics\InChI\InChIKey.vb"

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

    '     Class InChIKey
    ' 
    '         Properties: InChIVersion, IsStandard, MolecularSkeleton, NumberOfProtons, StereochemistryIsotopes
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text

Namespace IUPAC.InChILayers

    ''' <summary>
    ''' ``AAAAAAAAAAAAAA-BBBBBBBBFV-P``
    ''' </summary>
    Public Class InChIKey

        Public ReadOnly Property MolecularSkeleton As String
            Get

            End Get
        End Property

        Public ReadOnly Property StereochemistryIsotopes As String
            Get

            End Get
        End Property

        Public ReadOnly Property NumberOfProtons As String
            Get
                Return Chr(ASCII.N + inchi.Charge.Proton)
            End Get
        End Property

        Public ReadOnly Property InChIVersion As String
            Get
                Return Chr(ASCII.A + inchi.Version)
            End Get
        End Property

        Public ReadOnly Property IsStandard As String
            Get
                If inchi.IsStandard Then
                    Return "S"
                Else
                    Return "N"
                End If
            End Get
        End Property

        ReadOnly inchi As InChI

        Sub New(inchi As InChI)
            Me.inchi = inchi
        End Sub

        Public Overrides Function ToString() As String
            Return $"{MolecularSkeleton}-{StereochemistryIsotopes}{IsStandard}{InChIVersion}-{NumberOfProtons}"
        End Function
    End Class
End Namespace

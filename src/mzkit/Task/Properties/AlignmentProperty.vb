#Region "Microsoft.VisualBasic::949aecc8a3bb74e6d3aef08843e9af97, mzkit\src\mzkit\Task\Properties\AlignmentProperty.vb"

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

    '   Total Lines: 31
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 937.00 B


    ' Class AlignmentProperty
    ' 
    '     Properties: forward, jaccard, query, reference, reverse
    '                 shares
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Public Class AlignmentProperty

    <Category("Ion Information")>
    Public Property query As String
    <Category("Ion Information")>
    Public Property reference As String

    <Category("Alignment")>
    Public Property forward As Double
    <Category("Alignment")>
    Public Property reverse As Double
    <Category("Alignment")>
    Public Property jaccard As Double
    <Category("Alignment")>
    Public Property shares As Integer

    Sub New(alignment As AlignmentOutput)
        query = alignment.query.id
        reference = alignment.reference.id
        forward = alignment.forward
        reverse = alignment.reverse

        Dim all = alignment.alignments.Length

        shares = alignment.alignments.Where(Function(a) a.da <> "NaN").Count
        jaccard = shares / all
    End Sub
End Class

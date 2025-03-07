#Region "Microsoft.VisualBasic::cd8d5a40e88ce262d1fcfffaf3c00c4f, mzmath\MSEngine\Proteomics\Modification.vb"

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

    '   Total Lines: 54
    '    Code Lines: 26 (48.15%)
    ' Comment Lines: 4 (7.41%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (44.44%)
    '     File Size: 1.65 KB


    ' Class Modification
    ' 
    '     Properties: Composition, CreateDate, Description, IsSelected, IsVariable
    '                 LastModifiedDate, ModificationSites, Position, ReporterCorrectionM1, ReporterCorrectionM2
    '                 ReporterCorrectionP1, ReporterCorrectionP2, ReporterCorrectionType, TerminusType, Title
    '                 Type, User
    ' 
    ' Class ModificationSite
    ' 
    '     Properties: DiagnosticIons, DiagnosticNLs, Site
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

'proteinNterm modification is allowed only once.
'proteinCterm modification is allowed only once.
'anyCterm modification is allowed only once.
'anyNterm modification is allowed only once.

Public Class Modification

    Public Property Title As String

    Public Property Description As String

    Public Property CreateDate As String

    Public Property LastModifiedDate As String

    Public Property User As String

    Public Property ReporterCorrectionM2 As Integer

    Public Property ReporterCorrectionM1 As Integer

    Public Property ReporterCorrectionP1 As Integer

    Public Property ReporterCorrectionP2 As Integer

    Public Property ReporterCorrectionType As Boolean

    Public Property Composition As Formula ' only derivative moiety 

    Public Property ModificationSites As List(Of ModificationSite) = New List(Of ModificationSite)()

    Public Property Position As String ' anyCterm, anyNterm, anywhere, notCterm, proteinCterm, proteinNterm

    Public Property Type As String ' Standard, Label, IsobaricLabel, Glycan, AaSubstitution, CleavedCrosslink, NeuCodeLabel

    Public Property TerminusType As String

    Public Property IsSelected As Boolean

    Public Property IsVariable As Boolean
End Class


Public Class ModificationSite

    Public Property Site As String

    Public Property DiagnosticIons As List(Of ProductIon) = New List(Of ProductIon)()

    Public Property DiagnosticNLs As List(Of NeutralLoss) = New List(Of NeutralLoss)()
End Class


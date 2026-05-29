#Region "Microsoft.VisualBasic::cbf4b462b2b89023716e69c2deb1cb12, metadb\Chemoinformatics\Formula\MS\NeutralLoss.vb"

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

    '   Total Lines: 76
    '    Code Lines: 45 (59.21%)
    ' Comment Lines: 9 (11.84%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 22 (28.95%)
    '     File Size: 2.54 KB


    '     Class NeutralLoss
    ' 
    '         Properties: CandidateInChIKeys, CandidateOntologies, Comment, Formula, Frequency
    '                     Iontype, MassError, MassLoss, Name, PrecursorIntensity
    '                     PrecursorMz, ProductIntensity, ProductMz, ShortName, Smiles
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports AdductParser = BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType.Parser
Imports std = System.Math

Namespace Formula.MS

    ''' <summary>
    ''' This class is the storage of each neutral loss information used in MS-FINDER program.
    ''' </summary>
    Public Class NeutralLoss

        Public Property MassLoss As Double

        Public Property PrecursorMz As Double

        Public Property ProductMz As Double

        Public Property PrecursorIntensity As Double

        Public Property ProductIntensity As Double

        Public Property MassError As Double

        Public Property Formula As New Formula()

        Public Property Iontype As IonModes

        Public Property Comment As String

        Public Property Smiles As String

        Public Property Frequency As Double

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As New List(Of String)()

        Public Property CandidateOntologies As New List(Of String)()

        Sub New()
        End Sub

        Sub New(loss As String, name As String, comment As String, Optional absMass As Boolean = False)
            Dim adduct As MzCalculator = Provider.ParseAdductModel(loss)
            Dim parts = AdductParser.Formula(Strings.Trim(loss), raw:=True) _
                .TryCast(Of IEnumerable(Of (sign%, expression As String))) _
                .ToArray
            Dim lossFormula As New Formula

            For Each part In parts
                ' 20250228
                ' due to the reason of get loss formula at here
                ' so we should reverse the sign relationship
                ' example as [M-H2O]+ its loss formula should be 
                ' H2O, so if the sign is -1, then we should add
                ' H2O this loss part
                If part.sign < 0 Then
                    lossFormula = lossFormula + part.expression
                Else
                    lossFormula = lossFormula - part.expression
                End If
            Next

            _Name = name
            _ShortName = loss.GetStackValue("[", "]")
            _Comment = comment
            _Frequency = 1
            _Iontype = IonModes.Unknown
            _MassLoss = If(absMass, std.Abs(adduct.adducts), adduct.adducts)
            _Formula = lossFormula
        End Sub

    End Class
End Namespace

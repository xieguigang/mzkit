#Region "Microsoft.VisualBasic::a31cd4df22fae89b4b3c3058fd771172, metadb\Chemoinformatics\Formula\MS\ProductIon.vb"

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

    '   Total Lines: 62
    '    Code Lines: 39 (62.90%)
    ' Comment Lines: 3 (4.84%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 20 (32.26%)
    '     File Size: 1.86 KB


    '     Class ProductIon
    ' 
    '         Properties: CandidateInChIKeys, CandidateOntologies, Comment, Formula, Frequency
    '                     Intensity, IonMode, IsotopeDiff, Mass, MassDiff
    '                     Name, ShortName, Smiles, Type
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Formula.MS

    ''' <summary>
    ''' This class is the storage of product ion assignment used in MS-FINDER program.
    ''' </summary>
    Public Class ProductIon

        Public Property Mass As Double

        Public Property Intensity As Double

        Public Property Formula As Formula

        Public Property IonMode As IonModes
        Public Property Type As AnnotationType = AnnotationType.Product

        Public Property Smiles As String

        Public Property MassDiff As Double

        Public Property IsotopeDiff As Double

        Public Property Comment As String

        Public Property Name As String

        Public Property ShortName As String

        Public Property CandidateInChIKeys As New List(Of String)()

        Public Property Frequency As Double

        Public Property CandidateOntologies As New List(Of String)()

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(formula As String, name As String, comment As String)
            Call Me.New(FormulaScanner.ScanFormula(formula).CountsByElement, name, comment)
        End Sub

        Sub New(formula As IDictionary(Of String, Integer), name As String, comment As String)
            _Formula = New Formula(formula)
            _Mass = _Formula.ExactMass
            _Intensity = 1
            _IonMode = IonModes.Unknown
            _Comment = comment
            _Name = name
            _ShortName = name
            _Frequency = 1
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Name} ({Formula.EmpiricalFormula}); m/z: {Mass}"
        End Function
    End Class

End Namespace

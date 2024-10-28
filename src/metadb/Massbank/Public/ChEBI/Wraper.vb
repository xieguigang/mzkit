#Region "Microsoft.VisualBasic::e1b8ba608c3dce78699e0bc6a86680d5, metadb\Massbank\Public\ChEBI\Wraper.vb"

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

    '   Total Lines: 49
    '    Code Lines: 33 (67.35%)
    ' Comment Lines: 7 (14.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (18.37%)
    '     File Size: 1.62 KB


    '     Class Wraper
    ' 
    '         Properties: chebi, CommonName, ExactMass, Formula, Id
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace ChEBI

    ''' <summary>
    ''' mzkit data reader wrapper of the <see cref="ChEBIEntity"/> xml request result data.
    ''' </summary>
    Public Class Wraper : Implements GenericCompound

        Public ReadOnly Property Id As String Implements IReadOnlyId.Identity
            Get
                Return chebi.chebiId
            End Get
        End Property

        Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return FormulaScanner.EvaluateExactMass(Formula)
            End Get
        End Property

        Public ReadOnly Property CommonName As String Implements ICompoundNameProvider.CommonName
            Get
                Return chebi.chebiAsciiName
            End Get
        End Property

        Public ReadOnly Property Formula As String Implements IFormulaProvider.Formula
            Get
                Return chebi.Formulae.data
            End Get
        End Property

        ''' <summary>
        ''' the source data object
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property chebi As ChEBIEntity

        Public Overrides Function ToString() As String
            Return $"({Id}) {CommonName}"
        End Function

    End Class
End Namespace

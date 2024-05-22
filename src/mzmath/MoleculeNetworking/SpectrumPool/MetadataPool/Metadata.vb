#Region "Microsoft.VisualBasic::49ce5daf9e284075aea9cd9c3edb094a, mzmath\MoleculeNetworking\SpectrumPool\MetadataPool\Metadata.vb"

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

    '   Total Lines: 75
    '    Code Lines: 50 (66.67%)
    ' Comment Lines: 15 (20.00%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 10 (13.33%)
    '     File Size: 2.78 KB


    '     Class Metadata
    ' 
    '         Properties: adducts, biodeep_id, block, ExactMass, formula
    '                     guid, instrument, intensity, mz, name
    '                     organism, project, rt, sample_source, source_file
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO

Namespace PoolData

    ''' <summary>
    ''' metadata of the spectrum object
    ''' </summary>
    Public Class Metadata : Implements INamedValue
        Implements IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider

        Public Property guid As String Implements INamedValue.Key
        Public Property mz As Double
        Public Property rt As Double
        Public Property intensity As Double
        Public Property source_file As String

        ''' <summary>
        ''' the spectrum data is store in mzpack <see cref="ScanMs2"/> format
        ''' </summary>
        ''' <returns></returns>
        Public Property block As BufferRegion

        ''' <summary>
        ''' blood, urine, etc
        ''' </summary>
        ''' <returns></returns>
        Public Property sample_source As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property organism As String
        Public Property instrument As String = "Thermo Scientific Q Exactive"
        Public Property name As String Implements ICompoundNameProvider.CommonName
        Public Property biodeep_id As String Implements IReadOnlyId.Identity
        Public Property formula As String Implements IFormulaProvider.Formula
        Public Property adducts As String
        Public Property project As String

        Private ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
            Get
                Return FormulaScanner.EvaluateExactMass(formula)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(copy As Metadata)
            Me.adducts = copy.adducts
            Me.sample_source = copy.sample_source
            Me.source_file = copy.source_file
            Me.rt = copy.rt
            Me.biodeep_id = copy.biodeep_id
            Me.block = New BufferRegion(copy.block)
            Me.formula = copy.formula
            Me.guid = copy.guid
            Me.instrument = copy.instrument
            Me.intensity = copy.intensity
            Me.mz = copy.mz
            Me.name = copy.name
            Me.organism = copy.organism
            Me.project = copy.project
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{guid}] {name} {mz.ToString("F4")}@{(rt / 60).ToString("F2")}min"
        End Function

    End Class
End Namespace

#Region "Microsoft.VisualBasic::6cf7b1d5eceb7e9ba26b7e87289b3c6f, metadna\metaDNA\Models\KEGGCompound.vb"

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

    '   Total Lines: 78
    '    Code Lines: 59
    ' Comment Lines: 3
    '   Blank Lines: 16
    '     File Size: 2.28 KB


    ' Structure KEGGCompound
    ' 
    '     Properties: CommonName, ExactMass, Formula, kegg_id
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' object model wrapper for the KEGG <see cref="Compound"/> in order to apply of the generic ms search engine
''' </summary>
Public Structure KEGGCompound
    Implements GenericCompound
    Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider

    Public ReadOnly Property ExactMass As Double Implements IExactMassProvider.ExactMass
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.exactMass
        End Get
    End Property

    Public ReadOnly Property kegg_id As String Implements IReadOnlyId.Identity
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.entry
        End Get
    End Property

    Public ReadOnly Property CommonName As String Implements ICompoundNameProvider.CommonName
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            If KEGG.commonNames Is Nothing Then
                Return Formula
            End If

            Return If(KEGG.commonNames.FirstOrDefault, kegg_id)
        End Get
    End Property

    Public ReadOnly Property Formula As String Implements IFormulaProvider.Formula
        Get
            If KEGG Is Nothing Then
                Return Nothing
            End If

            Return KEGG.formula
        End Get
    End Property

    Dim KEGG As Compound

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(kegg As Compound)
        Me.KEGG = kegg
    End Sub

    Public Overrides Function ToString() As String
        If KEGG Is Nothing Then
            Return Nothing
        End If

        Return KEGG.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(cpd As KEGGCompound) As Compound
        Return cpd.KEGG
    End Operator

End Structure

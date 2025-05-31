﻿#Region "Microsoft.VisualBasic::89364ae72b0d344ca017340adcdc1445, metadb\MoNA\SpectraSection.vb"

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

    '   Total Lines: 158
    '    Code Lines: 106 (67.09%)
    ' Comment Lines: 34 (21.52%)
    '    - Xml Docs: 97.06%
    ' 
    '   Blank Lines: 18 (11.39%)
    '     File Size: 5.25 KB


    ' Class SpectraSection
    ' 
    '     Properties: Comment, GetSpectrumPeaks, libtype, MetaDB, MetaReader
    '                 ms_level, SpectraInfo
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: GetCrossReference, GetMetabolite
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' Union of the spectrum data and the metabolite annotation metadata
''' </summary>
''' <remarks>
''' this data object used for union the metabolite annotation <see cref="MetaData"/> and 
''' the spectrum data(<see cref="SpectraInfo"/>).
''' 
''' also is a sub class of the <see cref="MetaInfo"/> annotation information object
''' </remarks>
Public Class SpectraSection : Inherits MetaInfo

    ''' <summary>
    ''' The reference spectrum data
    ''' </summary>
    ''' <returns></returns>
    Public Property SpectraInfo As SpectraInfo

    ''' <summary>
    ''' should contains the necessary header data for <see cref="FillData"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Comment As NameValueCollection

    Dim meta As MetaData = Nothing

    ''' <summary>
    ''' MoNA里面都主要是将注释的信息放在<see cref="Comment"/>字段里面的。
    ''' 物质的注释信息主要是放在这个结构体之中，这个属性是对<see cref="Comment"/>
    ''' 属性的解析结果
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MetaDB As MetaData
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            If meta Is Nothing Then
                meta = Comment.FillData
            End If

            Return meta
        End Get
    End Property

    Public ReadOnly Property MetaReader As UnionReader
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New UnionReader(MetaDB, Me)
        End Get
    End Property

    ''' <summary>
    ''' get the current reference spectrum object
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' the <see cref="ID"/> will be tagged as the <see cref="PeakMs2.lib_guid"/>.
    ''' </remarks>
    Public ReadOnly Property GetSpectrumPeaks As PeakMs2
        Get
            Return SpectraInfo.ToPeaksMs2(id:=ID)
        End Get
    End Property

    Public ReadOnly Property libtype As IonModes
        Get
            If SpectraInfo.precursor_type.StringEmpty Then
                Return Provider.ParseIonMode(SpectraInfo.ion_mode, allowsUnknown:=True)
            ElseIf SpectraInfo.precursor_type.Last = "+"c Then
                Return IonModes.Positive
            Else
                Return IonModes.Negative
            End If
        End Get
    End Property

    Public ReadOnly Property ms_level As Integer
        Get
            Dim meta = MetaDB
            Dim mslevel As String = meta.ms_level

            If mslevel.StringEmpty Then
                Return 2
            Else
                Return CInt(Val(mslevel.Match("\d+")))
            End If
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(metadata As MetaData)
        Me.meta = metadata
        Me.ID = metadata.accession
        Me.name = metadata.name
        Me.IUPACName = metadata.name
        Me.formula = metadata.GetFormula
        Me.exact_mass = FormulaScanner.EvaluateExactMass(formula)

        If exact_mass <= 0 Then
            exact_mass = metadata.exact_mass
        End If

        Me.xref = GetCrossReference()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCrossReference() As xref
        Return New xref With {
            .CAS = meta.cas_number,
            .chebi = meta.chebi,
            .ChEMBL = meta.chembl,
            .ChemIDplus = meta.ChemIDplus,
            .DrugBank = meta.drugbank,
            .HMDB = meta.hmdb,
            .InChI = meta.InChI,
            .InChIkey = meta.InChIKey,
            .KEGG = meta.kegg,
            .KNApSAcK = meta.knapsack,
            .lipidmaps = meta.lipidmaps,
            .MeSH = meta.Mesh,
            .MetaCyc = "",
            .metlin = "",
            .pubchem = meta.pubchem_cid,
            .SMILES = meta.SMILES.ElementAtOrDefault(0, ""),
            .Wikipedia = meta.wikipedia,
            .extras = New Dictionary(Of String, String()) From {
                {"MoNA", {Me.ID}}
            }
        }
    End Function

    ''' <summary>
    ''' get metabolite information based on the metadata
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMetabolite() As MetaLib.Models.MetaLib
        Dim mass As Double = FormulaScanner.ScanFormula(formula)

        Return New MetaLib.Models.MetaLib With {
            .ID = Me.ID,
            .name = Me.name,
            .IUPACName = If(Me.IUPACName, .name),
            .formula = Me.formula,
            .exact_mass = If(mass > 0, mass, Me.exact_mass),
            .xref = GetCrossReference(),
            .description = meta.comment.JoinBy(vbCrLf),
            .synonym = {meta.name}
        }
    End Function
End Class

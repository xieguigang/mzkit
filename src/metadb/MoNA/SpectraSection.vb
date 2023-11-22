#Region "Microsoft.VisualBasic::41ef46ae57cf6439acea6b55ae740cc7, mzkit\src\metadb\MoNA\SpectraSection.vb"

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

'   Total Lines: 83
'    Code Lines: 60
' Comment Lines: 10
'   Blank Lines: 13
'     File Size: 2.64 KB


' Class SpectraSection
' 
'     Properties: Comment, MetaDB, MetaReader, SpectraInfo
' 
'     Constructor: (+2 Overloads) Sub New
' 
' Class SpectraInfo
' 
'     Properties: collision_energy, column, flow_gradient, flow_rate, fragmentation_mode
'                 instrument, instrument_type, ion_mode, ionization, MassPeaks
'                 MsLevel, mz, precursor_type, resolution, retention_time
'                 solvent_a, solvent_b
' 
'     Function: ToPeaksMs2
' 
' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' Union of the spectrum data and the metabolite annotation metadata
''' </summary>
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

    Public ReadOnly Property GetSpectrumPeaks As PeakMs2
        Get
            Return SpectraInfo.ToPeaksMs2(id:=ID)
        End Get
    End Property

    Public ReadOnly Property libtype As Integer
        Get
            If SpectraInfo.precursor_type.StringEmpty Then
                Return 1
            ElseIf SpectraInfo.precursor_type.Last = "+"c Then
                Return 1
            Else
                Return -1
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
        Me.formula = If(metadata.molecular_formula, metadata.derivative_formula)
        Me.exact_mass = metadata.exact_mass

        If Not formula.StringEmpty AndAlso formula.IndexOf(","c) > 0 Then
            formula = formula.Split(","c).First
        End If
    End Sub

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
            .xref = New xref With {
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
                .Wikipedia = meta.wikipedia
            },
            .description = meta.comment.JoinBy(vbCrLf),
            .synonym = {meta.name}
        }
    End Function
End Class

''' <summary>
''' The reference spectra data which is parsed from the MoNA database
''' </summary>
Public Class SpectraInfo

    Public Property MsLevel As String
    Public Property mz As Double
    Public Property precursor_type As String
    Public Property instrument_type As String
    Public Property instrument As String
    Public Property collision_energy As String
    Public Property ion_mode As String
    Public Property ionization As String
    Public Property fragmentation_mode As String
    Public Property resolution As String
    Public Property column As String
    Public Property flow_gradient As String
    Public Property flow_rate As String
    Public Property retention_time As String
    Public Property solvent_a As String
    Public Property solvent_b As String

    Public Property MassPeaks As ms2()

    Public Function ToPeaksMs2(Optional id As String = Nothing) As PeakMs2
        Dim precursor_type As String = Me.precursor_type

        If precursor_type.StringEmpty Then
            Dim ionMode As Integer = Provider.ParseIonMode(ion_mode, allowsUnknown:=True)

            If ionMode <> 0 Then
                precursor_type = $"[M]{If(ionMode > 0, "+", "-")}"
            End If
        End If
        If (Not precursor_type.StringEmpty) AndAlso (precursor_type.Last <> "+" AndAlso precursor_type.Last <> "-") Then
            Dim ionMode As Integer = Provider.ParseIonMode(ion_mode, allowsUnknown:=True)

            If ionMode <> 0 Then
                precursor_type = precursor_type.Trim("["c, "]"c)
                precursor_type = $"[{precursor_type}]{If(ionMode > 0, "+", "-")}"
            End If
        End If

        Return New PeakMs2 With {
            .activation = ionization,
            .collisionEnergy = Val(collision_energy.Match("\d+(\.\d+)?")),
            .intensity = MassPeaks.Sum(Function(a) a.intensity),
            .lib_guid = If(id, $"M{mz.ToString("F0")}T{retention_time}, m/z={mz} {precursor_type}"),
            .mz = mz,
            .mzInto = MassPeaks,
            .precursor_type = precursor_type,
            .rt = Val(retention_time),
            .meta = New Dictionary(Of String, String) From {
                {NameOf(instrument), instrument},
                {NameOf(fragmentation_mode), fragmentation_mode}
            }
        }
    End Function
End Class

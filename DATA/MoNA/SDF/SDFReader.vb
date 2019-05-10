#Region "Microsoft.VisualBasic::0e1f911df52e6eb3a2a3260432390c8e, Massbank\Public\MoNA\MoNAJson.vb"

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

' Class MoNAJson
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP
Imports SMRUCC.MassSpectrum.DATA.File
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports SMRUCC.MassSpectrum.Math.Ms1.PrecursorType
Imports SMRUCC.MassSpectrum.Math.Spectra

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
End Class

''' <summary>
''' Reader for file ``MoNA-export-LC-MS-MS_Spectra.sdf``
''' </summary>
Public Module SDFReader

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="skipSpectraInfo">
    ''' 是否只解析注释信息部分的数据
    ''' </param>
    ''' <returns></returns>
    Public Iterator Function ParseFile(path As String, Optional skipSpectraInfo As Boolean = False) As IEnumerable(Of SpectraSection)
        For Each mol As SDF In SDF.IterateParser(path, parseStruct:=False)
            Dim M As Func(Of String, String) = mol.readMeta
            Dim commentMeta = mol.MetaData!COMMENT.ToTable
            Dim ms2 As ms2() = Nothing
            Dim info As SpectraInfo = Nothing
            Dim commonName$ = Strings.Trim(M("NAME")).Trim(ASCII.Quot)
            Dim exact_mass# = M("EXACT MASS")

            If Not skipSpectraInfo Then
                info = M.readSpectraInfo
                ms2 = mol.MetaData("MASS SPECTRAL PEAKS") _
                    .Select(Function(line) line.Split) _
                    .Select(Function(line)
                                Return New ms2 With {
                                    .mz = line(0),
                                    .intensity = line(1),
                                    .quantity = line(1)
                                }
                            End Function) _
                    .ToArray

                ' precursor_type可能在其他的位置, 或者读取的字符串主键不正确
                If info.precursor_type.StringEmpty Then
                    info.precursor_type = PrecursorType _
                        .FindPrecursorType(exact_mass, info.mz, 1, info.ion_mode) _
                        .precursorType
                End If
            End If

            Yield New SpectraSection With {
                .name = commonName,
                .ID = M("ID"),
                .Comment = commentMeta,
                .formula = M("FORMULA"),
                .exact_mass = exact_mass,
                .MassPeaks = ms2,
                .xref = commentMeta.readXref(M),
                .SpectraInfo = info
            }
        Next
    End Function

    <Extension>
    Private Function readSpectraInfo(M As Func(Of String, String)) As SpectraInfo
        Dim info As New SpectraInfo With {
            .MsLevel = M("SPECTRUM TYPE"),
            .mz = M("PRECURSOR M/Z"),
            .precursor_type = M("PRECURSOR TYPE"),
            .instrument_type = M("INSTRUMENT TYPE"),
            .instrument = M("INSTRUMENT"),
            .collision_energy = M("COLLISION ENERGY"),
            .ion_mode = M("ION MODE"),
            .ionization = M("ionization"),
            .fragmentation_mode = M("fragmentation mode"),
            .resolution = M("resolution"),
            .column = M("column"),
            .flow_gradient = M("flow gradient"),
            .flow_rate = M("flow rate"),
            .retention_time = M("retention time"),
            .solvent_a = M("solvent a"),
            .solvent_b = M("solvent a")
        }

        Return info
    End Function

    ''' <summary>
    ''' 读取数据库编号引用信息
    ''' </summary>
    ''' <param name="commentMeta"></param>
    ''' <param name="M"></param>
    ''' <returns></returns>
    <Extension>
    Private Function readXref(commentMeta As NameValueCollection, M As Func(Of String, String)) As xref
        Dim xref As New xref

        xref.CAS = commentMeta.GetValues("cas").AsList + commentMeta.GetValues("cas number")
        xref.CAS = xref.CAS _
            .Select(Function(id) id.StringSplit("\s+")) _
            .IteratesALL _
            .Where(Function(id) xref.IsCASNumber(id)) _
            .ToArray
        xref.chebi = commentMeta("chebi")
        xref.HMDB = commentMeta("hmdb")
        xref.InChI = commentMeta("InChI")
        xref.InChIkey = commentMeta("InChIKey") Or M("INCHIKEY").AsDefault
        xref.KEGG = commentMeta("KEGG")
        xref.pubchem = commentMeta("pubchem cid")
        xref.SMILES = commentMeta("SMILES")
        xref.Wikipedia = commentMeta("wikipedia")

        Return xref
    End Function

    ''' <summary>
    ''' Create a lambda function for read meta data by key
    ''' </summary>
    ''' <param name="mol">Molecule annotation data in sdf format.</param>
    ''' <returns></returns>
    <Extension>
    Private Function readMeta(mol As SDF) As Func(Of String, String)
        Dim meta As Dictionary(Of String, String()) = mol.MetaData _
            !COMMENT _
            .ToTable _
            .ToDictionary(allStrings:=True)

        For Each [property] As KeyValuePair(Of String, String()) In mol.MetaData
            If meta.ContainsKey([property].Key) Then
                meta([property].Key) = meta([property].Key) _
                    .Join([property].Value) _
                    .ToArray
            Else
                meta([property].Key) = [property].Value
            End If
        Next

        Return Function(key As String)
                   Return meta.TryGetValue(key, [default]:={}).FirstOrDefault
               End Function
    End Function
End Module
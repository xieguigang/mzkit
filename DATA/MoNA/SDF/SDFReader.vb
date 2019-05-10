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
Imports SMRUCC.MassSpectrum.Math.Spectra

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
            Dim info As Dictionary(Of String, String) = Nothing
            Dim commonName$ = Strings.Trim(M("NAME")).Trim(ASCII.Quot)

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
            End If

            Yield New SpectraSection With {
                .name = commonName,
                .ID = M("ID"),
                .Comment = commentMeta,
                .formula = M("FORMULA"),
                .exact_mass = M("EXACT MASS"),
                .MassPeaks = ms2,
                .xref = commentMeta.readXref(M),
                .SpectraInfo = info
            }
        Next
    End Function

    <Extension>
    Private Function readSpectraInfo(M As Func(Of String, String)) As Dictionary(Of String, String)
        Dim info As New Dictionary(Of String, String)

        info!MsLevel = M("SPECTRUM TYPE")
        info!mz = M("PRECURSOR M/Z")
        info!instrument_type = M("INSTRUMENT TYPE")
        info!instrument = M("INSTRUMENT")
        info!collision_energy = M("COLLISION ENERGY")
        info!ion_mode = M("ION MODE")
        info!ionization = M("ionization")
        info!fragmentation_mode = M("fragmentation mode")
        info!resolution = M("resolution")
        info!column = M("column")
        info!flow_gradient = M("flow gradient")
        info!flow_rate = M("flow rate")
        info!retention_time = M("retention time")
        info!solvent_a = M("solvent a")
        info!solvent_b = M("solvent a")

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
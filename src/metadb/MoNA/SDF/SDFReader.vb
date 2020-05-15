#Region "Microsoft.VisualBasic::07c4e436c9533ec48197e2a5a52e0527, src\metadb\MoNA\SDF\SDFReader.vb"

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

    ' Module SDFReader
    ' 
    '     Function: createMoNAData, FixMzType, ParseFile, readMeta, readSpectraInfo
    '               readXref, TrimGNPSName
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' Reader for file ``MoNA-export-LC-MS-MS_Spectra.sdf``
''' </summary>
Public Module SDFReader

    Private Function TrimGNPSName(commonName As String) As String
        Dim prefix = r.Match(commonName, "^[A-Z]+\d+[-]\d+[!_]", RegexOptions.Multiline)

        If Not prefix.Success Then
            Return commonName
        Else
            commonName = commonName.Replace(prefix.Value, "")

            If prefix.Value.Last = "!" Then
                Return commonName
            Else
                ' 还会包含分子式
                prefix = r.Match(commonName, "^[CHOPNS0-9]+[_]", RegexOptions.Multiline)

                If prefix.Success Then
                    commonName = commonName.Replace(prefix.Value, "")
                End If

                Return commonName
            End If
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="skipSpectraInfo">
    ''' 是否只解析注释信息部分的数据
    ''' </param>
    ''' <param name="recalculateMz">
    ''' 假若重新计算m/z的话,会要求目标具有正确的exact_mass结果值, 在打开了这个选项之后,
    ''' 程序会将所有的非[M]+/[M]-/[M+H]+/[M-H]-的mz重新更新计算为[M+H]+或者[M-H]-的
    ''' m/z结果值
    ''' </param>
    ''' <returns></returns>
    Public Function ParseFile(path As String,
                              Optional skipSpectraInfo As Boolean = False,
                              Optional recalculateMz As Boolean = False,
                              Optional parallel As Boolean = False) As IEnumerable(Of SpectraSection)

        If parallel Then
            Return SDF.IterateParser(path, parseStruct:=False) _
                .AsParallel _
                .Select(Function(mol)
                            Return mol.createMoNAData(skipSpectraInfo, recalculateMz)
                        End Function) _
                .Where(Function(a)
                           Return Not a Is Nothing
                       End Function) _
                .OrderBy(Function(a) a.ID)
        Else
            Return Iterator Function() As IEnumerable(Of SpectraSection)
                       Dim MoNAData As New Value(Of SpectraSection)

                       For Each mol As SDF In SDF.IterateParser(path, parseStruct:=False)
                           If Not (MoNAData = mol.createMoNAData(skipSpectraInfo, recalculateMz)) Is Nothing Then
                               Yield MoNAData.Value
                           End If
                       Next
                   End Function()
        End If
    End Function

    <Extension>
    Private Function createMoNAData(mol As SDF, skipSpectraInfo As Boolean, recalculateMz As Boolean) As SpectraSection
        Dim M As Func(Of String, String) = mol.readMeta
        Dim commentMeta = mol.MetaData!COMMENT.ToTable
        Dim info As SpectraInfo = Nothing
        Dim commonName$ = Strings.Trim(M("NAME")).Trim(ASCII.Quot)
        Dim exact_mass# = M("EXACT MASS")
        Dim xrefID$ = M("ID").Trim
        Dim xref As xref = commentMeta.readXref(M)

        ' fix naming bugs in GNPS library
        If InStr(xrefID, "CCMSLIB") = 1 Then
            commonName = TrimGNPSName(commonName)

            If commonName.StringEmpty AndAlso xref.IsEmpty(xref) Then
                Return Nothing
            End If
        End If

        If Not skipSpectraInfo Then
            info = M.readSpectraInfo.FixMzType(exact_mass, recalculateMz)
            info.MassPeaks = mol _
                .MetaData("MASS SPECTRAL PEAKS") _
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

        Return New SpectraSection With {
            .name = commonName,
            .ID = xrefID,
            .Comment = commentMeta,
            .formula = M("FORMULA"),
            .exact_mass = exact_mass,
            .xref = xref,
            .SpectraInfo = info
        }
    End Function

    ReadOnly standards As Index(Of String) = {"M", "M+H", "M-H", "[M]", "[M]+", "[M]-", "[M+H]", "[M-H]", "[M+H]+", "[M-H]-"}

    ''' <summary>
    ''' MoNA库之中一些比较重要的字段比较混乱,会需要使用这个函数来重新构建出所需要的数据
    ''' </summary>
    ''' <param name="info"></param>
    ''' <param name="recalculateMz"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function FixMzType(info As SpectraInfo, exact_mass#, recalculateMz As Boolean) As SpectraInfo
        Dim precursor_type As String = info.precursor_type
        Dim ion_mode$ = ParseIonMode(info.ion_mode, allowsUnknown:=True)

        If ion_mode = "0" Then
            ion_mode = ParseIonMode(Strings.Trim(precursor_type).Last, allowsUnknown:=True)

            ' 默认为阳离子
            If ion_mode = "0" Then
                ion_mode = "+"
                Call $"[{info.ion_mode}] is invalid, use positive mode as default".__DEBUG_ECHO
            End If
        End If

        ' 2019-05-14
        ' Parser.ParseMzCalculator函数之中的缓存需要+/-作为主键
        If ion_mode = "1" Then
            ion_mode = "+"
        Else
            ion_mode = "-"
        End If

        If recalculateMz Then
            If precursor_type Like standards Then
                ' 重新格式化一次
                With Parser.ParseMzCalculator(precursor_type, ion_mode, skipEvalAdducts:=False)
                    info.precursor_type = .ToString
                    info.mz = .CalcMZ(exact_mass)
                    info.ion_mode = ion_mode
                End With
            Else
                Call $"Recalculate m/z precursor_type for [{info.precursor_type}]".__DEBUG_ECHO

                ' 对于其他的类型,则重新计算为[M+H]+或者[M-H]-类型的数据
                If ion_mode = "1" Then
                    info.precursor_type = "[M+H]+"
                    info.mz = Provider.Positive("M+H").CalcMZ(exact_mass)
                    info.ion_mode = "+"
                Else
                    info.precursor_type = "[M-H]-"
                    info.mz = Provider.Negative("M-H").CalcMZ(exact_mass)
                    info.ion_mode = "-"
                End If
            End If
        Else
            ' precursor_type可能在其他的位置, 或者读取的字符串主键不正确
            If info.precursor_type.StringEmpty Then
                info.precursor_type = PrecursorType _
                    .FindPrecursorType(exact_mass, info.mz, 1, info.ion_mode) _
                    .precursorType

                If info.precursor_type = "Unknown" Then
                    ' [M+H]+/[M-H]- default
                    If ParseIonMode(info.ion_mode, allowsUnknown:=True) >= 0 Then
                        info.precursor_type = "[M+H]+"
                        info.mz = Provider.Positive("M+H").CalcMZ(exact_mass)
                        info.ion_mode = "+"
                    Else
                        info.precursor_type = "[M-H]-"
                        info.mz = Provider.Negative("M-H").CalcMZ(exact_mass)
                        info.ion_mode = "-"
                    End If
                End If
            End If
        End If

        Return info
    End Function

    <Extension>
    Private Function readSpectraInfo(M As Func(Of String, String)) As SpectraInfo
        ' 有些数据是错误标记上了的,有些precursor_type和precursor_mz被标反了
        Dim precursor_type$ = M("PRECURSOR TYPE")
        Dim mz$ = M("PRECURSOR M/Z")

        If mz.StringEmpty Then
            mz = 0
        ElseIf Not Double.TryParse(mz, Nothing) Then
            Dim tmp = precursor_type
            precursor_type = mz
            mz = tmp
        End If

        Dim info As New SpectraInfo With {
            .MsLevel = M("SPECTRUM TYPE"),
            .mz = Double.Parse(mz),
            .precursor_type = precursor_type,
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

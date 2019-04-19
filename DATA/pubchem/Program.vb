#Region "Microsoft.VisualBasic::7f5349c4d573ac8fc3bc3e444577335d, pubchem\Program.vb"

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

' Module Program
' 
'     Function: Decompress, Descriptor, ImageFlyCLI, Main, PubchemUnifyMetaLib
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.DATA.File
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/unify.metalib")>
    <Usage("/unify.metalib /in <CID-Synonym-filtered.txt> [/dbtype <chebi/hmdb/kegg/cas> /out <out.Xml>]")>
    <Description("Create a unify xref database file.")>
    Public Function PubchemUnifyMetaLib(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim dbtype$ = args <= "/dbtype"
        Dim out$
        Dim subsetTest As Func(Of MetaLib, Boolean) = Function() True
        Dim reportTick As Integer = 10000

        If dbtype.StringEmpty Then
            ' all
            out = args("/out") Or $"{[in].TrimSuffix}.metlib.Xml"
        Else
            out = args("/out") Or $"{[in].TrimSuffix}.metlib_{dbtype}.Xml"
            reportTick = 100

            Select Case dbtype.ToLower
                Case "chebi" : subsetTest = Function(m) Not m.xref.chebi.StringEmpty(True)
                Case "hmdb" : subsetTest = Function(m) Not m.xref.HMDB.StringEmpty(True)
                Case "kegg" : subsetTest = Function(m) Not m.xref.KEGG.StringEmpty(True)
                Case "cas"
                    reportTick = 10000
                    subsetTest = Function(m) Not m.xref.CAS.IsNullOrEmpty
                Case Else
                    Throw New NotSupportedException(dbtype)
            End Select
        End If

        Using dataset As New DataSetWriter(Of MetaLib)(out)
            Dim i As VBInteger = 0

            For Each meta As MetaLib In CIDSynonym.LoadMetaInfo([in]).Where(subsetTest)
                Call dataset.Write(meta)

                If ++i Mod reportTick = 0 Then
                    Call Console.Write(i)
                    Call Console.Write(vbTab)
                    Call dataset.Flush()
                End If
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 通过物质注释信息查询物质的Descriptor计算数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/descriptor.query")>
    <Usage("/descriptor.query /meta <metaLib.Xml> /descriptor <descriptor.db> [/out <matrix.csv>]")>
    Public Function CompoundDescriptorQuery(args As CommandLine) As Integer
        Dim in$ = args <= "/meta"
        Dim db$ = args <= "/descriptor"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.descriptor.csv"
        ' 必须要保证这里面有正确的pubchem CID
        Dim metaLib As MetaLib() = LoadUltraLargeXMLDataSet(Of MetaLib)(path:=[in]) _
            .Where(Function(m) m.xref.pubchem > 0) _
            .ToArray

        ' 在这里不适用using,因为using在结束的时候会自动写数据
        Dim repo As New DescriptorDatabase(out.Open(FileMode.OpenOrCreate, doClear:=False))
        Dim matrix As DataSet() = metaLib _
            .Select(Function(m)
                        Return New DataSet With {
                            .ID = m.ID,
                            .Properties = repo.GetDescriptor(m.xref.pubchem)
                        }
                    End Function) _
            .ToArray

        Return matrix.SaveTo(out).CLICode
    End Function

    <ExportAPI("/descriptor")>
    <Usage("/descriptor /compounds <*.gz directory> [/out <description.db>]")>
    <Description("Build a chemical descriptor database repository.")>
    Public Function Descriptor(args As CommandLine) As Integer
        Dim compoundRepo$ = args <= "/compounds"
        Dim out$ = args("/out") Or $"{compoundRepo.TrimDIR}.description.db"
        Dim BlockOrderFiles = (ls - l - r - "*.gz" <= compoundRepo) _
            .OrderBy(Function(path)
                         Return Val(path.BaseName.Replace("Compound_", ""))
                     End Function) _
            .ToArray
        Dim descript As ChemicalDescriptor
        ' Dim verify As ChemicalDescriptor

        Using repo As New DescriptorDatabase(out.Open(FileMode.OpenOrCreate, doClear:=False))
            For Each file As String In BlockOrderFiles
                file = Decompress(New FileInfo(file))

                For Each mol As SDF In SDF.IterateParser(file, parseStruct:=False)
                    descript = mol.ChemicalProperties
                    repo.Write(mol.CID, descript)
                    ' repo.Flush()
                    ' verify = repo.GetDescriptor(mol.CID)

                    ' If Not descript.SequenceEqual(verify) Then
                    '    Throw New Exception
                    ' End If
                Next

                Call file.DeleteFile
                Call repo.Flush()
            Next
        End Using

        Return 0
    End Function

    <Extension>
    Private Function Decompress(fileToDecompress As FileInfo) As String
        Using originalFileStream As FileStream = fileToDecompress.OpenRead()
            Dim currentFileName As String = fileToDecompress.FullName
            Dim newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length)

            Using decompressedFileStream As FileStream = System.IO.File.Create(newFileName)
                Using decompressionStream As New GZipStream(originalFileStream, CompressionMode.Decompress)
                    decompressionStream.CopyTo(decompressedFileStream)
                    Console.WriteLine($"Decompressed: {fileToDecompress.Name}")
                End Using
            End Using

            Return newFileName
        End Using
    End Function

    <ExportAPI("/image.fly")>
    <Usage("/image.fly /cid <cid> [/out <save.png>]")>
    Public Function ImageFlyCLI(args As CommandLine) As Integer
        Dim cid$ = args("/cid")
        Dim out$ = args("/out") Or $"./{cid}.png"

        Return ImageFly.GetImage(cid) _
            .SaveAs(out) _
            .CLICode
    End Function
End Module


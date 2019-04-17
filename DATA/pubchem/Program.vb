Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
    <Usage("/unify.metalib /in <CID-Synonym-filtered.txt> [/out <out.Xml>]")>
    Public Function PubchemUnifyMetaLib(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.metlib.Xml"

        Using dataset As New DataSetWriter(Of MetaLib)(out)
            Dim i As VBInteger = 0

            For Each meta As MetaLib In CIDSynonym.LoadMetaInfo([in])
                Call dataset.Write(meta)

                If ++i Mod 10000 = 0 Then
                    Call Console.Write(i)
                    Call Console.Write(vbTab)
                    Call dataset.Flush()
                End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/descriptor")>
    <Usage("/descriptor /compounds <*.gz directory> /meta <CID-Synonym> [/out <description.csv>]")>
    Public Function Descriptor(args As CommandLine) As Integer
        Dim compoundRepo$ = args <= "/compounds"
        Dim meta$ = args <= "/meta"
        Dim out$ = args("/out") Or $"{compoundRepo.TrimDIR}.description.csv"
        Dim metaIterator = CIDSynonym.LoadMetaInfo(meta).GetEnumerator
        Dim BlockOrderFiles = (ls - l - r - "*.gz" <= compoundRepo) _
            .OrderBy(Function(path)
                         Return Val(path.BaseName.Replace("Compound_", ""))
                     End Function) _
            .ToArray



        For Each file As String In BlockOrderFiles
            file = Decompress(New FileInfo(file))

            For Each mol As SDF In SDF.IterateParser(file)
                Dim metaInfo As MetaLib = metaIterator.Next
                Dim descript = mol.ChemicalProperties


            Next

            Call file.DeleteFile
        Next

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

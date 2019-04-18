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
    <Usage("/unify.metalib /in <CID-Synonym-filtered.txt> [/dbtype <chebi/hmdb/kegg/cas> /out <out.Xml>]")>
    Public Function PubchemUnifyMetaLib(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim dbtype$ = args <= "/dbtype"
        Dim out$
        Dim subsetTest As Func(Of MetaLib, Boolean) = Function() True

        If dbtype.StringEmpty Then
            ' all
            out = args("/out") Or $"{[in].TrimSuffix}.metlib.Xml"
        Else
            out = args("/out") Or $"{[in].TrimSuffix}.metlib_{dbtype}.Xml"

            Select Case dbtype.ToLower
                Case "chebi" : subsetTest = Function(m) Not m.xref.chebi.StringEmpty(True)
                Case "hmdb" : subsetTest = Function(m) Not m.xref.HMDB.StringEmpty(True)
                Case "kegg" : subsetTest = Function(m) Not m.xref.KEGG.StringEmpty(True)
                Case "cas" : subsetTest = Function(m) Not m.xref.CAS.IsNullOrEmpty
                Case Else
                    Throw New NotSupportedException(dbtype)
            End Select
        End If

        Using dataset As New DataSetWriter(Of MetaLib)(out)
            Dim i As VBInteger = 0

            For Each meta As MetaLib In CIDSynonym.LoadMetaInfo([in]).Where(subsetTest)
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
    <Usage("/descriptor /compounds <*.gz directory> [/out <description.db>]")>
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

                For Each mol As SDF In SDF.IterateParser(file)
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

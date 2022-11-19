Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml

Public Module mzStreamWriter

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    ''' <param name="meta_size">
    ''' too small buffer size will make the data corrept
    ''' if the tree size is greater than this buffer size 
    ''' parameter value.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function WriteStream(mzpack As mzPack,
                                file As Stream,
                                Optional meta_size As Long = 8 * 1024 * 1024) As Boolean

        Dim metadata As New Dictionary(Of String, String)

        Using pack As New StreamPack(file)
            Dim summary As New Dictionary(Of String, Double)

            Call pack.Clear(meta_size)
            Call mzpack.WriteStream(pack, index:=summary)
            Call pack.WriteText(mzpack.readme(summary), "readme.txt")
            Call metadata.Add("thumbnail", mzpack.Thumbnail IsNot Nothing)
            Call metadata.Add("ms1", mzpack.MS.TryCount)
            Call metadata.Add("create_time", Now.ToString)
            Call metadata.Add("github", "https://github.com/xieguigang/mzkit")
            Call metadata.Add("application", GetType(mzPack).Assembly.ToString)
            Call metadata.Add("platform", If(App.IsMicrosoftPlatform, "Microsoft Windows", "UNIX/LINUX"))
            Call metadata.Add("source", mzpack.source)

            For Each data As KeyValuePair(Of String, String) In mzpack.metadata.SafeQuery
                metadata(data.Key) = data.Value
            Next

            Call pack.WriteText(metadata.GetJson, ".etc/metadata.json")
        End Using

        Return True
    End Function

    <Extension>
    Private Function readme(mzpack As mzPack, summary As Dictionary(Of String, Double)) As String
        Dim sb As New StringBuilder
        Dim app = mzpack.Application

        Call sb.AppendLine($"mzPack data v2.0")
        Call sb.AppendLine($"for MZKit application {app.ToString}({app.Description}) data analysis.")
        Call sb.AppendLine()
        Call sb.AppendLine($"has {mzpack.MS.Length} ms scans")
        Call sb.AppendLine("summary:")
        Call sb.AppendLine()

        For Each line In summary
            Call sb.AppendLine($"{line.Key}: {line.Value}")
        Next

        Return sb.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Friend Function getScan1DirName(scan_id As String) As String
        Return scan_id.Replace("\", "/").Replace("/", "_")
    End Function

    Public Const SampleMetaName As String = "sample"

    <Extension>
    Private Sub WriteStream(mzpack As mzPack, pack As StreamPack, ByRef index As Dictionary(Of String, Double))
        Dim rtmin As Double = 99999
        Dim rtmax As Double = -9999
        Dim mzmin As Double = 99999
        Dim mzmax As Double = -9999
        Dim samples As New List(Of String)

        For Each ms1 As ScanMS1 In mzpack.MS
            Dim sampleTag As String

            If ms1.hasMetaKeys(SampleMetaName) Then
                sampleTag = ms1.meta(SampleMetaName)
                samples.Add(sampleTag)
            Else
                sampleTag = ""
            End If

            Dim dir As String = $"/MS/{sampleTag}/{ms1.scan_id.getScan1DirName}/"
            Dim dirMetadata As New Dictionary(Of String, Object)
            Dim ms1Bin As String = $"{dir}/Scan1.mz"
            Dim ms1Metadata As New Dictionary(Of String, Object)

            Call dirMetadata.Add("scan_id", ms1.scan_id)
            Call dirMetadata.Add("products", ms1.products.TryCount)
            Call dirMetadata.Add("id", ms1.products.SafeQuery.Select(Function(i) i.scan_id).ToArray)

            Using scan1 As New BinaryDataWriter(pack.OpenBlock(ms1Bin)) With {
                .ByteOrder = ByteOrder.LittleEndian
            }
                Call ms1.WriteScan1(scan1)
            End Using

            For Each tag In ms1.meta.SafeQuery
                Call ms1Metadata.Add(tag.Key, tag.Value)
            Next

            For Each product As ScanMS2 In ms1.products.SafeQuery
                Using scan2 As New BinaryDataWriter(pack.OpenBlock($"{dir}/{product.scan_id.MD5}.mz")) With {
                    .ByteOrder = ByteOrder.LittleEndian
                }
                    Call product.WriteBuffer(scan2)
                End Using

                If product.rt > rtmax Then
                    rtmax = product.rt
                End If
                If product.rt < rtmin Then
                    rtmin = product.rt
                End If
                If product.parentMz > mzmax Then
                    mzmax = product.parentMz
                End If
                If product.parentMz < mzmin Then
                    mzmin = product.parentMz
                End If
            Next

            Call pack.SetAttribute(ms1Bin, ms1Metadata)
            Call pack.SetAttribute(dir, dirMetadata)
        Next

        If Not mzpack.Scanners.IsNullOrEmpty Then
            For Each name As String In mzpack.Scanners.Keys
                Dim scanner As ChromatogramOverlap = mzpack.Scanners(name)

                Using buffer As Stream = pack.OpenBlock($"/Scanners/{name}.cdf")
                    Call scanner.SavePackData(buffer)
                End Using
            Next
        End If

        If Not mzpack.Chromatogram Is Nothing Then
            ' 20220718  put chromatogram.cdf file into the /MS/ 
            ' data directory will cause the ms1 data reader error!
            ' so move this file to root block
            Using buffer As New BinaryDataWriter(pack.OpenBlock("/chromatogram.cdf")) With {
                .ByteOrder = ByteOrder.LittleEndian
            }
                Call buffer.Write(mzpack.Chromatogram.GetBytes)
            End Using
        End If

        Call index.Add(NameOf(mzmin), mzmin)
        Call index.Add(NameOf(mzmax), mzmax)
        Call index.Add(NameOf(rtmin), rtmin)
        Call index.Add(NameOf(rtmax), rtmax)
        Call index.Add("totalIons", mzpack.totalIons)
        Call index.Add("maxIntensity", mzpack.maxIntensity)

        Call pack.WriteText(mzpack.Application.ToString, ".etc/app.cls")
        Call pack.WriteText(index.GetJson, ".etc/ms_scans.json")
        Call pack.WriteText(samples.Distinct.GetJson, ".etc/sample_tags.json")

        If Not mzpack.Thumbnail Is Nothing Then
            Using snapshot As Stream = pack.OpenBlock("/thumbnail.png")
                Call mzpack.Thumbnail.Save(snapshot, ImageFormats.Png.GetFormat)
            End Using
        End If
    End Sub
End Module

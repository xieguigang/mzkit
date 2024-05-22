#Region "Microsoft.VisualBasic::453343237aae027eda799d4ad8400906, assembly\mzPack\v2.0\mzStreamWriter.vb"

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

    '   Total Lines: 248
    '    Code Lines: 177 (71.37%)
    ' Comment Lines: 27 (10.89%)
    '    - Xml Docs: 48.15%
    ' 
    '   Blank Lines: 44 (17.74%)
    '     File Size: 9.76 KB


    ' Module mzStreamWriter
    ' 
    '     Function: getScan1DirName, readme, WriteStream
    ' 
    '     Sub: writeAnnotations, WriteApplicationClass, (+2 Overloads) WriteStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module mzStreamWriter

    Public Const metadata_json As String = ".etc/metadata.json"
    Public Const annotations_xml As String = ".etc/annotations.xml"

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

            Call writeAnnotations(mzpack, pack)
            Call pack.WriteText(metadata.GetJson, metadata_json)
        End Using

        Return True
    End Function

    Private Sub writeAnnotations(mzpack As mzPack, pack As StreamPack)
        If mzpack.Annotations.IsNullOrEmpty Then
            Return
        End If

        Dim annos = mzpack.Annotations.Select(Function(v) New NamedValue(v.Key, v.Value)).ToArray
        Dim xml As String = annos.GetXml

        Call pack.WriteText(xml, annotations_xml)
    End Sub

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

    ''' <summary>
    ''' the attribute tag name for indicate the sample source in ms1 scan
    ''' </summary>
    Public Const SampleMetaName As String = "sample"

    <Extension>
    Public Sub WriteStream(mzpack As IEnumerable(Of ScanMS1),
                           pack As StreamPack,
                           ByRef index As Dictionary(Of String, Double),
                           Optional ByRef samples As List(Of String) = Nothing)

        Dim rtmin As Double = 99999
        Dim rtmax As Double = -9999
        Dim mzmin As Double = 99999
        Dim mzmax As Double = -9999

        If samples Is Nothing Then
            samples = New List(Of String)
        End If

        For Each ms1 As ScanMS1 In mzpack
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
                Using blockStream As Stream = pack.OpenBlock($"{dir}/{product.scan_id.MD5}.mz")
                    If TypeOf blockStream Is SubStream Then
                        ' 20230210
                        '
                        ' has duplicated scan id will cause the
                        ' open block function returns a stream
                        ' in readonly!

                        Throw New InvalidDataException($"A duplicated scan id({product.scan_id}) was found!")
                    Else
                        Dim scan2 As New BinaryDataWriter(blockStream) With {
                            .ByteOrder = ByteOrder.LittleEndian
                        }

                        Call product.WriteBuffer(scan2)
                        Call scan2.Flush()

                        ' do not close the writer at here
                        ' or the exception of can not access to
                        ' a closed stream will be happended
                        '
                        ' Call scan2.Close()
                    End If
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

        Call index.Add(NameOf(mzmin), mzmin)
        Call index.Add(NameOf(mzmax), mzmax)
        Call index.Add(NameOf(rtmin), rtmin)
        Call index.Add(NameOf(rtmax), rtmax)
    End Sub

    <Extension>
    Private Sub WriteStream(mzpack As mzPack, pack As StreamPack, ByRef index As Dictionary(Of String, Double))
        Dim samples As New List(Of String)

        Call mzpack.MS.WriteStream(pack, index, samples)

        If Not mzpack.Scanners.IsNullOrEmpty Then
            For Each name As String In mzpack.Scanners.Keys
                Dim scanner As ChromatogramOverlapList = mzpack.Scanners(name)

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

        Call index.Add("totalIons", mzpack.totalIons)
        Call index.Add("maxIntensity", mzpack.maxIntensity)

        Call pack.WriteText(mzpack.Application.ToString, applicationClassFile)
        Call pack.WriteText(index.GetJson, ".etc/ms_scans.json")
        Call pack.WriteText(samples.Distinct.GetJson, ".etc/sample_tags.json")

        If Not mzpack.Thumbnail Is Nothing Then
            Using snapshot As Stream = pack.OpenBlock("/thumbnail.png")
#Disable Warning
                Call mzpack.Thumbnail.Save(snapshot, ImageFormats.Png.GetFormat)
#Enable Warning
            End Using
        End If
    End Sub

    Private Const applicationClassFile As String = ".etc/app.cls"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub WriteApplicationClass(app As FileApplicationClass, pack As StreamPack)
        Call pack.WriteText(app.ToString, applicationClassFile)
    End Sub
End Module

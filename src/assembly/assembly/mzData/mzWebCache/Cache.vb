Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.Net.Http

Namespace mzData.mzWebCache

    Public Module Cache

        ''' <summary>
        ''' load scan data from ``mzml`` file
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzErr$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(raw As IEnumerable(Of spectrum), Optional mzErr$ = "da:0.1") As IEnumerable(Of ScanMS1)
            Return New mzMLScans(mzErr).Load(raw)
        End Function

        ''' <summary>
        ''' load scan data from ``mzxml`` file
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzErr$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(raw As IEnumerable(Of mzXML.scan), Optional mzErr$ = "da:0.1") As IEnumerable(Of ScanMS1)
            Return New mzXMLScans(mzErr).Load(raw)
        End Function

        <Extension>
        Public Sub Write(scans As IEnumerable(Of ScanMS1), file As Stream)
            Using writer As New StreamWriter(file)
                For Each scan As ScanMS1 In scans
                    Call writer.WriteLine(scan.scan_id)
                    Call writer.WriteLine({scan.rt, scan.BPC, scan.TIC}.JoinBy(","))
                    Call writer.WriteLine(scan.mz.vectorBase64)
                    Call writer.WriteLine(scan.into.vectorBase64)

                    For Each product As ScanMS2 In scan.products
                        Call writer.WriteLine(product.scan_id)
                        Call writer.WriteLine({product.parentMz, product.rt, product.intensity, product.polarity}.JoinBy(","))
                        Call writer.WriteLine(product.mz.vectorBase64)
                        Call writer.WriteLine(product.into.vectorBase64)
                    Next

                    Call writer.WriteLine("-----")
                Next

                Call writer.Flush()
            End Using
        End Sub

        <Extension>
        Private Function vectorBase64(vec As Double()) As String
            Dim convertToNetworkByteOrder As Boolean = BitConverter.IsLittleEndian
            Dim data As Byte()

            Using buffer As New MemoryStream
                For Each x As Double In vec
                    data = BitConverter.GetBytes(x)

                    If convertToNetworkByteOrder Then
                        ' 需要颠倒为network byteorder
                        Call Array.Reverse(data)
                    End If

                    buffer.Write(data, Scan0, data.Length)
                Next

                Return buffer.ToArray.ToBase64String
            End Using
        End Function
    End Module
End Namespace

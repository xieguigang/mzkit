Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace MarkupData.mzML

    Public Module Extensions

        Public Const Xmlns$ = "http://psi.hupo.org/ms/mzml"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetAllMs1(spectrums As IEnumerable(Of spectrum)) As IEnumerable(Of spectrum)
            Return spectrums.Where(Function(s) s.ms_level = "1")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateMS1(filePath As String) As IEnumerable(Of (scan_time#, mz#, intensity#))
            Return filePath _
                .LoadXmlDataSet(Of spectrum)(, xmlns:=mzML.Xmlns) _
                .GetAllMs1 _
                .Select(Function(ms1)
                            Dim time# = ms1.scan_time
                            Dim mz = ms1.ByteArray("m/z array").Base64Decode
                            Dim intensity = ms1.ByteArray("intensity array").Base64Decode

                            Return CInt(ms1.defaultArrayLength) _
                                .Sequence _
                                .Select(Function(index)
                                            Return (time, mz(index), intensity(index))
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        End Function

        ''' <summary>
        ''' Working for MRM method
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LoadChromatogramList(path As String) As IEnumerable(Of chromatogram)
            Return path.LoadXmlDataSet(Of chromatogram)(, xmlns:=mzML.Xmlns)
        End Function

        ''' <summary>
        ''' MRM ion selector based on the precursor ion m/z and the product ion m/z value.
        ''' </summary>
        ''' <param name="chromatograms"></param>
        ''' <param name="ionPairs"></param>
        ''' <returns>Nothing for ion not found</returns>
        <Extension>
        Public Function MRMSelector(chromatograms As IEnumerable(Of chromatogram), ionPairs As IEnumerable(Of IonPair)) As IEnumerable(Of (ion As IonPair, chromatogram As chromatogram))
            With chromatograms.ToArray
                Return ionPairs _
                    .Select(Function(ion)
                                Dim chromatogram =
                                    .Where(Function(c)
                                               Return Not c.id = "TIC" AndAlso ion.Assert(c)
                                           End Function) _
                                    .FirstOrDefault
                                Return (ion, chromatogram)
                            End Function)
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MRMTargetMz(selector As IMRMSelector) As Double
            Return selector _
                .isolationWindow _
                .cvParams _
                .KeyItem("isolation window target m/z") _
                .value
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="type$">
        ''' + ``m/z`` array
        ''' + intensity array
        ''' + time array
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ByteArray(data As BinaryData, type$) As binaryDataArray
            Return data _
                .binaryDataArrayList _
                .list _
                .Where(Function(a)
                           Return Not a.cvParams.KeyItem(type) Is Nothing
                       End Function) _
                .FirstOrDefault
        End Function

        ''' <summary>
        ''' Get this chromatogram signal ticks.(返回来的时间的单位都统一为秒)
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Ticks(chromatogram As chromatogram) As ChromatogramTick()
            Dim time = chromatogram.ByteArray("time array")
            Dim into = chromatogram.ByteArray("intensity array")
            Dim timeUnit = time.cvParams.KeyItem("time array").unitName
            Dim intoUnit = into.cvParams.KeyItem("intensity array").unitName
            Dim time_array = time.Base64Decode.AsVector
            Dim intensity_array = into.Base64Decode

            If timeUnit.TextEquals("minute") Then
                time_array = time_array * 60
            End If

            Dim data = time_array _
                .Select(Function(t, i)
                            Return New ChromatogramTick(t, intensity_array(i))
                        End Function) _
                .ToArray

            Return data
        End Function
    End Module
End Namespace
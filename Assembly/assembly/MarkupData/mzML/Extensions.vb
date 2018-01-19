Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.Math

Namespace MarkupData.mzML

    Public Module Extensions

        Public Const Xmlns$ = "http://psi.hupo.org/ms/mzml"

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
        ''' 
        ''' </summary>
        ''' <param name="chromatograms"></param>
        ''' <param name="ionPairs"></param>
        ''' <returns>Nothing for ion not found</returns>
        <Extension>
        Public Function MRMSelector(chromatograms As IEnumerable(Of chromatogram), ionPairs As IEnumerable(Of IonPair)) As IEnumerable(Of (ion As IonPair, chromatogram As chromatogram))
            With chromatograms.ToArray
                Return ionPairs.Select(Function(ion) (ion, .Where(Function(c) Not c.id = "TIC" AndAlso ion.Assert(c)).FirstOrDefault))
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MRMTargetMz(selector As IMRMSelector) As Double
            Return selector.isolationWindow.cvParams.KeyItem("isolation window target m/z").value
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ByteArray(chromatogram As chromatogram, type$) As binaryDataArray
            Return chromatogram.binaryDataArrayList.list.Where(Function(a) Not a.cvParams.KeyItem(type) Is Nothing).FirstOrDefault
        End Function

        ''' <summary>
        ''' 返回来的时间的单位都统一为秒
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PeakArea(chromatogram As chromatogram) As ChromatogramTick()
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
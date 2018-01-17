Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports sys = System.Math

Namespace mzML

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

        <Extension>
        Public Function PeakArea(chromatogram As chromatogram) As Double
            Dim time = chromatogram.ByteArray("time array")
            Dim into = chromatogram.ByteArray("intensity array")
            Dim timeUnit = time.cvParams.KeyItem("time array").unitName
            Dim intoUnit = into.cvParams.KeyItem("intensity array").unitName
            Dim time_array = time.binary
            Dim intensity_array = into.binary


        End Function
    End Module

    Public Class IonPair : Implements INamedValue

        Public Property name As String Implements IKeyedEntity(Of String).Key
        Public Property precursor As Double
        Public Property product As Double

        Public Overrides Function ToString() As String
            If name.StringEmpty Then
                Return $"{precursor}/{product}"
            Else
                Return $"Dim {name} As [{precursor}, {product}]"
            End If
        End Function

        Public Function Assert(chromatogram As chromatogram) As Boolean
            Dim pre = chromatogram.precursor.MRMTargetMz
            Dim pro = chromatogram.product.MRMTargetMz

            ' less than 0.3da or 20ppm??
            If sys.Abs(Val(pre) - precursor) <= 0.3 AndAlso sys.Abs(Val(pro) - product) <= 0.3 Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
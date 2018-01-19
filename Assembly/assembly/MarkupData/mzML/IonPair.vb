Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports sys = System.Math

Namespace mzML

    Public Class IonPair : Implements INamedValue

        ''' <summary>
        ''' The database accession ID
        ''' </summary>
        ''' <returns></returns>
        <Column("ID")>
        Public Property AccID As String
        ''' <summary>
        ''' The display title name
        ''' </summary>
        ''' <returns></returns>
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
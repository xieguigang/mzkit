Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct.LinkedList

Public Module LoadR

    <Extension>
    Public Function LoadPeakMs2(obj As RObject) As PeakMs2
        Dim peakData As New PeakMs2
        Dim schema As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of PeakMs2)(
            flag:=PropertyAccess.Writeable,
            nonIndex:=True
        )

        If obj.attributes Is Nothing Then
            ' do nothing
            ' obj = obj
        Else
            obj = obj.attributes
        End If

        Do While obj.value.nodeType = ListNodeType.LinkedList
            Dim name As String = obj.symbolName

            If schema.ContainsKey(name) Then
                Dim val As RList = obj.value.CAR.value
                Dim data As Array = val.data
                Dim target As PropertyInfo = schema(name)
                Dim dataVal As Object

                If target.PropertyType Is GetType(String) Then
                    dataVal = DirectCast(data(0), RObject).characters
                ElseIf DataFramework.IsPrimitive(target.PropertyType) Then
                    dataVal = CTypeDynamic(data(0), target.PropertyType)
                ElseIf target.PropertyType Is GetType(ms2()) Then
                    dataVal = obj.value.CAR.LoadProductMatrix
                Else
                    dataVal = Nothing
                End If

                Call target.SetValue(peakData, dataVal)
            End If

            obj = obj.value.CDR
        Loop

        Return peakData
    End Function

    <Extension>
    Public Function LoadProductMatrix(data As RObject) As ms2()
        Dim attrs As RObject = data.attributes
        Dim mz As Double() = attrs.LinkValue("mz").DoCall(AddressOf RStreamReader.ReadNumbers)
        Dim into As Double() = attrs.LinkValue("intensity").DoCall(AddressOf RStreamReader.ReadNumbers)
        Dim annotation = attrs.LinkValue("annotation").DoCall(AddressOf RStreamReader.ReadStrings)

        Return mz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = into(i),
                            .Annotation = annotation.ElementAtOrDefault(i)
                        }
                    End Function) _
            .ToArray
    End Function

End Module

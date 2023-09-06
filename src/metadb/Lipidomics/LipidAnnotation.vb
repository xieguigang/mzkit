Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports System.Collections.Generic

Namespace CompMs.Common.Lipidomics
    Public Class LipoqualityAnnotation
        Public Property Mz As Single
        Public Property Rt As Single
        Public Property AveragedIntensity As Single
        Public Property Name As String = String.Empty
        Public Property LipidClass As String = String.Empty
        Public Property LipidSuperClass As String = String.Empty
        Public Property TotalChain As String = String.Empty
        Public Property Sn1AcylChain As String = String.Empty
        Public Property Sn2AcylChain As String = String.Empty
        Public Property Sn3AcylChain As String = String.Empty
        Public Property Sn4AcylChain As String = String.Empty
        Public Property Adduct As AdductIon
        Public Property IonMode As IonMode
        Public Property Smiles As String = String.Empty
        Public Property Inchikey As String = String.Empty
        Public Property StandardDeviation As Single
        Public Property SpotID As Integer
        Public Property Formula As String = String.Empty
        Public Property Intensities As List(Of Double) = New List(Of Double)()
    End Class
End Namespace

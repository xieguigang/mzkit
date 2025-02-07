Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Ms1.Annotations

    Public Class MetID : Implements IReadOnlyId, IMS1Annotation, ICompoundNameProvider

        Public Property id As String Implements IReadOnlyId.Identity, IKeyedEntity(Of String).Key
        Public Property precursor_type As String Implements IMS1Annotation.precursor_type
        Public Property intensity As Double Implements IMs1Scan.intensity
        Public Property mz As Double Implements IMs1.mz
        Public Property rt As Double Implements IRetentionTime.rt
        Public Property name As String Implements ICompoundNameProvider.CommonName

    End Class
End Namespace
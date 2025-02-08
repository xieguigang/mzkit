Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Ms1.Annotations

    ''' <summary>
    ''' the ion annotation model
    ''' </summary>
    Public Class MetID : Implements IReadOnlyId, IMS1Annotation, ICompoundNameProvider

        ''' <summary>
        ''' the unique id of the target query result metabolite
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property unique_id As String Implements IReadOnlyId.Identity, IKeyedEntity(Of String).Key
        ''' <summary>
        ''' the ion adducts of this ion precursor
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property precursor_type As String Implements IMS1Annotation.precursor_type
        <XmlAttribute> Public Property intensity As Double Implements IMs1Scan.intensity

        ''' <summary>
        ''' the source ``m/z`` value of current annotated ion feature.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mz As Double Implements IMs1.mz
        <XmlAttribute> Public Property rt As Double Implements IRetentionTime.rt
        ''' <summary>
        ''' the metabolite name of current ion that annotated.
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property name As String Implements ICompoundNameProvider.CommonName

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
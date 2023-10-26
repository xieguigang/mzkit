
Imports System.Xml.Serialization

Namespace XMass

    ''' <summary>
    ''' AnalysisParameter.xml
    ''' </summary>
    ''' 
    Public Class AnnotationParameter

        <XmlAttribute>
        Public Property cid As String

        Public Property BuildingBlockName As String
        Public Property FontFaceName As String
        Public Property FontPointSize As Double
        Public Property FontOrientation As Double
        Public Property SearchTolerance As Double
        Public Property SearchToleranceUnit As String
        Public Property StringType As String
        Public Property PreviewFlag As Boolean
        Public Property ShowMassDifferenceFlag As Boolean

    End Class
End Namespace
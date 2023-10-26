
Imports System.Xml.Serialization

Namespace XMass

    Public Class pklist
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property creator As String
        <XmlAttribute> Public Property shots As Integer
        <XmlAttribute> Public Property [date] As String
        <XmlAttribute> Public Property spectrumid As String

        <XmlElement>
        Public Property pk As pk()

    End Class

    ''' <summary>
    ''' A peak
    ''' </summary>
    Public Class pk

        Public Property absi As Double
        Public Property area As Double
        Public Property chisq As Double
        Public Property goodn As Double
        Public Property goodn2 As Double
        Public Property ind As Double
        Public Property lind As Double
        Public Property lmass As Double
        Public Property mass As Double
        Public Property massemg As Double
        Public Property massgaussian As Double
        Public Property meth As Double
        Public Property reso As Double
        Public Property rind As Double
        Public Property rmass As Double
        Public Property s2n As Double
        Public Property sigmaemg As Double
        Public Property tauemg As Double
        Public Property type As Double

    End Class
End Namespace
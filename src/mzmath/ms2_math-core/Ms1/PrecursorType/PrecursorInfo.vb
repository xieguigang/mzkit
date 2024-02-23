Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' a ion m/z data model, includes the adducts data and <see cref="IonModes"/> polarity data.
    ''' </summary>
    Public Class PrecursorInfo

        ''' <summary>
        ''' the precursor adducts information
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property precursor_type As String
        Public Property charge As Double
        Public Property M As Double
        ''' <summary>
        ''' the exact mass value of the <see cref="precursor_type"/> adducts information.
        ''' </summary>
        ''' <returns></returns>
        Public Property adduct As Double

        ''' <summary>
        ''' mz or exact mass
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="m/z")>
        Public Property mz As String
        ''' <summary>
        ''' the ion polarity data, related to the <see cref="charge"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property ionMode As IonModes

        Sub New()
        End Sub

        Sub New(mz As MzCalculator)
            precursor_type = mz.ToString
            charge = mz.charge
            M = mz.M
            adduct = mz.adducts
            ionMode = ParseIonMode(mz.mode)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{precursor_type} m/z={mz}"
        End Function
    End Class
End Namespace
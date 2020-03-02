Imports System.Data.Linq.Mapping
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace ASCII.MSL

    Public Class MSL

        <Column(Name:="NAME")> Public Property Name As String
        <Column(Name:="CONTRIB")> Public Property Contributor As String
        <Column(Name:="FORM")> Public Property Formula As String
        <Column(Name:="CASNO")> Public Property CASNO As String
        <Column(Name:="NIST")> Public Property Nist As String
        <Column(Name:="RI")> Public Property RI As String
        <Column(Name:="COMMENT")> Public Property Comment As String
        <Column(Name:="SOURCE")> Public Property Source As String

        Public Property Peaks As ms2()

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace
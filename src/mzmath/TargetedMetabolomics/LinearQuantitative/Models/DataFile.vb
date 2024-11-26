Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace LinearQuantitative

    Public Class DataFile

        <XmlAttribute> Public Property filename As String
        <XmlElement> Public Property compounds As IonPeakTableRow()

        Public ReadOnly Property FileType As SampleFiles
            Get
                Return MeasureFileType()
            End Get
        End Property

        Sub New(filename As String, compounds As IEnumerable(Of IonPeakTableRow))
            _filename = filename
            _compounds = compounds.ToArray
        End Sub

        Sub New()
        End Sub

        Public Function MeasureFileType() As SampleFiles
            If filename.IsPattern(".*kb[-\s]*\d*") Then
                Return SampleFiles.KB
            ElseIf filename.IsPattern(".*QC[-\s]*\d*") Then
                Return SampleFiles.QC
            Else
                Return SampleFiles.Sample
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{FileType}] {compounds.Count} compounds@{filename}: {compounds.Keys.JoinBy("; ")}"
        End Function

    End Class
End Namespace
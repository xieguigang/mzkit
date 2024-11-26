Imports System.Xml.Serialization

Namespace LinearQuantitative

    Public Class DataFile

        <XmlAttribute> Public Property filename As String

        ''' <summary>
        ''' the multiple ion peak area data
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("Compound")> Public Property ionPeaks As IonPeakTableRow()

        Public ReadOnly Property FileType As SampleFiles
            Get
                Return MeasureFileType()
            End Get
        End Property

        Sub New(filename As String, compounds As IEnumerable(Of IonPeakTableRow))
            _filename = filename
            _ionPeaks = compounds.ToArray
        End Sub

        Sub New()
        End Sub

        Public Function MeasureFileType() As SampleFiles
            If filename.IsPattern(".*kb[-\s]*\d*") Then
                Return SampleFiles.KB
            ElseIf filename.IsPattern(".*QC[-\s]*\d*") Then
                Return SampleFiles.QC
            ElseIf filename.IsPattern(".*((Cal)|(L))[-\s]*\d+") Then
                Return SampleFiles.Standard
            Else
                Return SampleFiles.Sample
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{FileType}] {ionPeaks.Count} compounds@{filename}"
        End Function

    End Class
End Namespace
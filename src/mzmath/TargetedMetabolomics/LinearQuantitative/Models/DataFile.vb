Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace LinearQuantitative

    ''' <summary>
    ''' model for a single rawdata file
    ''' </summary>
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPeakData() As Dictionary(Of String, Double)
            Return ionPeaks.ToDictionary(Function(a) a.ID, Function(a) a.TPA)
        End Function

        Public Function CreateQuantifyData(linears As IEnumerable(Of StandardCurve)) As Dictionary(Of String, Double)
            Dim ions = ionPeaks.ToDictionary(Function(a) a.ID)
            Dim contents As New Dictionary(Of String, Double)

            For Each line As StandardCurve In linears
                If ions.ContainsKey(line.name) Then
                    If line.IS Is Nothing Then
                        ' use peak area
                        Call contents.Add(line.name, line.linear.GetY(ions(line.name).TPA))
                    ElseIf ions(line.name).TPA_IS <= 0 Then
                        Call contents.Add(line.name, Double.NaN)
                    Else
                        ' use the A/IS ratio
                        Call contents.Add(line.name, line.linear.GetY(ions(line.name).TPA / ions(line.name).TPA_IS))
                    End If
                Else
                    Call contents.Add(line.name, 0)
                End If
            Next

            Return contents
        End Function

        Public Overrides Function ToString() As String
            Return $"[{FileType}] {ionPeaks.Count} compounds@{filename}"
        End Function

    End Class
End Namespace
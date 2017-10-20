Imports System.Xml.Serialization

Namespace HMDB

    Public Structure [property]

        Public Property kind As String
        Public Property value As String
        Public Property source As String

        Public Overrides Function ToString() As String
            Return $"{value} [{kind}]"
        End Function
    End Structure

    Public Structure Properties
        <XmlElement("property")>
        Public Property PropertyList As [property]()

        Public Overrides Function ToString() As String
            Return $"{PropertyList.Length} properties..."
        End Function
    End Structure

    Public Structure concentration

        Public Property biofluid As String
        Public Property concentration_value As String
        Public Property concentration_units As String
        Public Property subject_age As String
        Public Property subject_sex As String
        Public Property subject_condition As String
        Public Property references As reference()

        Public ReadOnly Property AgeType As PeopleAgeTypes
            Get
                If subject_age.StringEmpty Then
                    Return PeopleAgeTypes.Unknown
                Else
                    With subject_age.Split.First
                        If .TextEquals(NameOf(PeopleAgeTypes.Adult)) Then
                            Return PeopleAgeTypes.Adult
                        ElseIf .TextEquals(NameOf(PeopleAgeTypes.Children)) Then
                            Return PeopleAgeTypes.Children
                        ElseIf .TextEquals(NameOf(PeopleAgeTypes.Newborn)) Then
                            Return PeopleAgeTypes.Newborn
                        Else
                            Return PeopleAgeTypes.Unknown
                        End If
                    End With
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim value$ = concentration_value

            If Not concentration_units.StringEmpty Then
                value &= $" ({concentration_units})"
            End If
            If subject_sex.StringEmpty OrElse subject_sex.TextEquals("Both") Then
                Return value
            Else
                Return $"[{subject_sex}] {value}"
            End If
        End Function
    End Structure

    Public Enum PeopleAgeTypes As Byte
        Unknown
        Newborn
        Children
        Adult
    End Enum
End Namespace
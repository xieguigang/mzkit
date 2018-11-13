#Region "Microsoft.VisualBasic::69ce46539791f88daecd7d0ba9ada650, Massbank\Public\TMIC\HMDB\Assembly\property.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Structure [property]
    ' 
    '         Properties: kind, source, value
    ' 
    '         Function: ToString
    ' 
    '     Structure Properties
    ' 
    '         Properties: PropertyList
    ' 
    '         Function: ToString
    ' 
    '     Structure concentration
    ' 
    '         Properties: AgeType, biofluid, concentration_units, concentration_value, references
    '                     subject_age, subject_condition, subject_sex
    ' 
    '         Function: ToString
    ' 
    '     Enum PeopleAgeTypes
    ' 
    '         Adult, Children, Newborn, Unknown
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace TMIC.HMDB

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

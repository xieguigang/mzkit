#Region "Microsoft.VisualBasic::e87ae6a07d4ffa777244f8c18fa6110b, E:/mzkit/src/metadb/Massbank//Public/TMIC/HMDB/Assembly/property.vb"

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


    ' Code Statistics:

    '   Total Lines: 78
    '    Code Lines: 65
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 2.86 KB


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
    '         Properties: AgeType, biospecimen, concentration_units, concentration_value, references
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
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace TMIC.HMDB

    Public Structure [property]

        <MessagePackMember(0)> Public Property kind As String
        <MessagePackMember(1)> Public Property value As String
        <MessagePackMember(2)> Public Property source As String

        Public Overrides Function ToString() As String
            Return $"{value} [{kind}]"
        End Function
    End Structure

    Public Structure Properties

        <XmlElement("property")>
        <MessagePackMember(0)>
        Public Property PropertyList As [property]()

        Public Overrides Function ToString() As String
            Return $"{PropertyList.Length} properties..."
        End Function
    End Structure

    Public Structure concentration

        <MessagePackMember(0)> Public Property biospecimen As String
        <MessagePackMember(1)> Public Property concentration_value As String
        <MessagePackMember(2)> Public Property concentration_units As String
        <MessagePackMember(3)> Public Property subject_age As String
        <MessagePackMember(4)> Public Property subject_sex As String
        <MessagePackMember(5)> Public Property subject_condition As String
        <MessagePackMember(6)> Public Property references As reference()

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

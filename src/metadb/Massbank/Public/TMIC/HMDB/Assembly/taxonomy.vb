#Region "Microsoft.VisualBasic::0984ed057d9ed57ed72b473c72bbcc94, metadb\Massbank\Public\TMIC\HMDB\Assembly\taxonomy.vb"

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

    '   Total Lines: 129
    '    Code Lines: 83
    ' Comment Lines: 13
    '   Blank Lines: 33
    '     File Size: 3.85 KB


    '     Class taxonomy
    ' 
    '         Properties: alternative_parents, description, direct_parent, external_descriptors, substituents
    ' 
    '         Function: ToString
    ' 
    '     Structure external_descriptors
    ' 
    '         Properties: external_descriptor
    ' 
    '     Structure alternative_parents
    ' 
    '         Properties: alternative_parent
    ' 
    '     Structure substituents
    ' 
    '         Properties: substituent
    ' 
    '     Class ontology
    ' 
    '         Properties: root
    ' 
    '     Class ontology_term
    ' 
    '         Properties: definition, descendants, level, parent_id, synonyms
    '                     term, type
    ' 
    '     Structure descendants
    ' 
    '         Properties: descendant
    ' 
    '     Structure origins
    ' 
    '         Properties: origin
    ' 
    '         Function: ToString
    ' 
    '     Structure biofunctions
    ' 
    '         Properties: biofunction
    ' 
    '         Function: ToString
    ' 
    '     Structure applications
    ' 
    '         Properties: application
    ' 
    '         Function: ToString
    ' 
    '     Structure cellular_locations
    ' 
    '         Properties: cellular
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TMIC.HMDB

    ''' <summary>
    ''' Metabolite compound chemical structure classification
    ''' </summary>
    ''' <remarks>
    ''' all these data contains in this model is the string array:
    ''' 
    ''' + <see cref="alternative_parents"/>
    ''' + <see cref="substituents"/>
    ''' + <see cref="external_descriptors"/>
    ''' </remarks>
    Public Class taxonomy : Inherits CompoundClass
        Implements ICompoundClass

        <MessagePackMember(5)> Public Property description As String
        <MessagePackMember(6)> Public Property direct_parent As String
        <MessagePackMember(7)> Public Property alternative_parents As alternative_parents
        <MessagePackMember(8)> Public Property substituents As substituents
        <MessagePackMember(9)> Public Property external_descriptors As external_descriptors

        Public Overrides Function ToString() As String
            Return description
        End Function

    End Class

    Public Structure external_descriptors

        <XmlElement>
        <MessagePackMember(0)>
        Public Property external_descriptor As String()

    End Structure

    Public Structure alternative_parents

        <XmlElement>
        <MessagePackMember(0)>
        Public Property alternative_parent As String()

    End Structure

    Public Structure substituents

        <XmlElement>
        <MessagePackMember(0)>
        Public Property substituent As String()

    End Structure

    ''' <summary>
    ''' a collection of the <see cref="ontology_term"/>
    ''' </summary>
    Public Class ontology

        <XmlElement>
        <MessagePackMember(0)>
        Public Property root As ontology_term()

    End Class

    Public Class ontology_term

        <MessagePackMember(0)> Public Property term As String
        <MessagePackMember(1)> Public Property definition As String
        <MessagePackMember(2)> Public Property parent_id As String
        <MessagePackMember(3)> Public Property level As Integer
        <MessagePackMember(4)> Public Property type As String
        <MessagePackMember(5)> Public Property descendants As descendants
        <MessagePackMember(6)> Public Property synonyms As synonyms

    End Class

    Public Structure descendants
        <XmlElement>
        <MessagePackMember(0)>
        Public Property descendant As ontology_term()
    End Structure

    Public Structure origins

        <XmlElement(NameOf(origin))>
        <MessagePackMember(0)>
        Public Property origin As String()

        Public Overrides Function ToString() As String
            Return origin.GetJson
        End Function
    End Structure

    Public Structure biofunctions

        <XmlElement(NameOf(biofunction))>
        <MessagePackMember(0)>
        Public Property biofunction As String()

        Public Overrides Function ToString() As String
            Return biofunction.GetJson
        End Function
    End Structure

    Public Structure applications

        <XmlElement(NameOf(application))>
        <MessagePackMember(0)>
        Public Property application As String()

        Public Overrides Function ToString() As String
            Return application.GetJson
        End Function
    End Structure

    Public Structure cellular_locations

        <XmlElement(NameOf(cellular))>
        <MessagePackMember(0)>
        Public Property cellular As String()

        Public Overrides Function ToString() As String
            Return cellular.GetJson
        End Function
    End Structure
End Namespace

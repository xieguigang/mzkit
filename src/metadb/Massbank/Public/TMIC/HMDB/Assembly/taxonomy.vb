#Region "Microsoft.VisualBasic::77c40c99104a074720a1bf24ac0a56aa, mzkit\src\metadb\Massbank\Public\TMIC\HMDB\Assembly\taxonomy.vb"

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

    '   Total Lines: 98
    '    Code Lines: 70
    ' Comment Lines: 3
    '   Blank Lines: 25
    '     File Size: 3.01 KB


    '     Class taxonomy
    ' 
    '         Properties: [class], alternative_parents, description, direct_parent, external_descriptors
    '                     kingdom, molecular_framework, sub_class, substituents, super_class
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
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TMIC.HMDB

    ''' <summary>
    ''' Metabolite compound chemical structure classification
    ''' </summary>
    Public Class taxonomy : Inherits CompoundClass
        Implements ICompoundClass

        Public Property description As String
        Public Property direct_parent As String
        Public Property alternative_parents As alternative_parents
        Public Property substituents As substituents
        Public Property external_descriptors As external_descriptors
    End Class

    Public Structure external_descriptors
        <XmlElement> Public Property external_descriptor As String()
    End Structure

    Public Structure alternative_parents
        <XmlElement> Public Property alternative_parent As String()
    End Structure

    Public Structure substituents
        <XmlElement> Public Property substituent As String()
    End Structure

    Public Class ontology

        <XmlElement>
        Public Property root As ontology_term()

    End Class

    Public Class ontology_term

        Public Property term As String
        Public Property definition As String
        Public Property parent_id As String
        Public Property level As Integer
        Public Property type As String
        Public Property descendants As descendants
        Public Property synonyms As synonyms

    End Class

    Public Structure descendants
        <XmlElement>
        Public Property descendant As ontology_term()
    End Structure

    Public Structure origins

        <XmlElement(NameOf(origin))>
        Public Property origin As String()

        Public Overrides Function ToString() As String
            Return origin.GetJson
        End Function
    End Structure

    Public Structure biofunctions

        <XmlElement(NameOf(biofunction))>
        Public Property biofunction As String()

        Public Overrides Function ToString() As String
            Return biofunction.GetJson
        End Function
    End Structure

    Public Structure applications

        <XmlElement(NameOf(application))>
        Public Property application As String()

        Public Overrides Function ToString() As String
            Return application.GetJson
        End Function
    End Structure

    Public Structure cellular_locations

        <XmlElement(NameOf(cellular))> Public Property cellular As String()

        Public Overrides Function ToString() As String
            Return cellular.GetJson
        End Function
    End Structure
End Namespace

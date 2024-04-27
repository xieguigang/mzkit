#Region "Microsoft.VisualBasic::2f4e4702d58a74c4eb6386c9cecf8555, G:/mzkit/src/metadb/Massbank//MetaLib/Models/MetaLib.vb"

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

    '   Total Lines: 113
    '    Code Lines: 62
    ' Comment Lines: 28
    '   Blank Lines: 23
    '     File Size: 4.52 KB


    '     Class BiosampleSource
    ' 
    '         Properties: biosample, reference, source
    ' 
    '         Function: ToString
    ' 
    '     Class CompoundClass
    ' 
    '         Properties: [class], kingdom, molecular_framework, sub_class, super_class
    ' 
    '     Class MetaLib
    ' 
    '         Properties: [class], chemical, keywords, kingdom, molecular_framework
    '                     organism, pathways, samples, sub_class, super_class
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Equals, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MetaLib.Models

    Public Class BiosampleSource

        <MessagePackMember(0)> Public Property biosample As String
        <MessagePackMember(1)> Public Property source As String

        ''' <summary>
        ''' the reference source
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(2)> Public Property reference As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    ''' <summary>
    ''' the data model of the compound class information
    ''' </summary>
    ''' <remarks>
    ''' this class information model is mainly address on the HMDB
    ''' metabolite ontology class levels.
    ''' </remarks>
    Public Class CompoundClass : Implements ICompoundClass

        <MessagePackMember(0)> Public Property kingdom As String Implements ICompoundClass.kingdom
        <MessagePackMember(1)> Public Property super_class As String Implements ICompoundClass.super_class
        <MessagePackMember(2)> Public Property [class] As String Implements ICompoundClass.class
        <MessagePackMember(3)> Public Property sub_class As String Implements ICompoundClass.sub_class
        <MessagePackMember(4)> Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

    End Class

    ''' <summary>
    ''' 对``chebi/kegg/pubchem/HMDB/metlin``的物质注释信息整合库，这个数据库只要为了生成编号，名称之类的注释信息而构建的
    ''' </summary>
    Public Class MetaLib : Inherits MetaInfo
        Implements IEquatable(Of MetaLib)
        Implements ICompoundClass

        <MessagePackMember(8)> Public Property chemical As ChemicalDescriptor

#Region "化合物分类"

        <MessagePackMember(9)> Public Property kingdom As String Implements ICompoundClass.kingdom
        <MessagePackMember(10)> Public Property super_class As String Implements ICompoundClass.super_class
        <MessagePackMember(11)> Public Property [class] As String Implements ICompoundClass.class
        <MessagePackMember(12)> Public Property sub_class As String Implements ICompoundClass.sub_class
        <MessagePackMember(13)> Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

#End Region

        <MessagePackMember(14)> Public Property organism As String()

        ''' <summary>
        ''' 包含有这个物质的KEGG pathway的编号的集合，只有当<see cref="xref.KEGG"/>
        ''' 存在值的时候才会存在这个属性
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(15)> Public Property pathways As String()
        <MessagePackMember(16)> Public Property samples As BiosampleSource()

        <MessagePackMember(17)> Public Property keywords As String()

        Sub New()
        End Sub

        ''' <summary>
        ''' creator a metabolite annotation data with compound class assigned
        ''' </summary>
        ''' <param name="class">the compound class data</param>
        Sub New([class] As ICompoundClass)
            If Not [class] Is Nothing Then
                Me.kingdom = [class].kingdom
                Me.super_class = [class].super_class
                Me.[class] = [class].class
                Me.sub_class = [class].sub_class
                Me.molecular_framework = [class].molecular_framework
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return name
        End Function

        ''' <summary>
        ''' 相同的物质可能在数据库之间有好几个编号?
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        Public Overloads Function Equals(other As MetaLib) As Boolean Implements IEquatable(Of MetaLib).Equals
            Static metaEquals As MetaEquals

            If metaEquals Is Nothing Then
                metaEquals = New MetaEquals
            End If

            If other Is Nothing Then
                Return False
            Else
                Return metaEquals.Equals(Me, other)
            End If
        End Function
    End Class
End Namespace

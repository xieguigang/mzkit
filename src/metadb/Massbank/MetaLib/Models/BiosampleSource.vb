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

End Namespace
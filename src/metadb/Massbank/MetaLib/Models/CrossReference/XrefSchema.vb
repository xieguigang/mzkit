Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace MetaLib.CrossReference

    Module XrefSchema

        ReadOnly strings As Dictionary(Of String, PropertyInfo)
        ReadOnly arrays As Dictionary(Of String, PropertyInfo)

        Sub New()
            Dim schema As Type = GetType(xref)

        End Sub

        ''' <summary>
        ''' Merge two cross reference set and then create a new cross reference set.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function Join(a As xref, b As xref) As xref

        End Function

        ''' <summary>
        ''' Convert a cross reference set as a database id collection
        ''' </summary>
        ''' <param name="xref"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function PullCollection(xref As xref) As IEnumerable(Of NamedValue(Of String))

        End Function

    End Module
End Namespace
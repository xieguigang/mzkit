Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models

Namespace LipidMaps

    ''' <summary>
    ''' A helper module for get lipidmaps <see cref="CompoundClass"/> information via a given lipidmaps id
    ''' </summary>
    ''' <remarks>
    ''' the lipidmaps metabolite data in this module is indexed via the lipidmaps id: <see cref="MetaData.LM_ID"/>.
    ''' </remarks>
    Public Class LipidClassReader : Inherits ClassReader

        ''' <summary>
        ''' the lipidmaps database was indexed via the lipidmaps id at here
        ''' </summary>
        ''' <remarks>
        ''' the key is the lipidmaps id <see cref="MetaData.LM_ID"/>
        ''' </remarks>
        ReadOnly index As Dictionary(Of String, MetaData)

        ''' <summary>
        ''' get number of the lipids inside the database
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property lipids As Integer
            Get
                Return index.TryCount
            End Get
        End Property

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        ''' <summary>
        ''' get lipidmaps class information via a given lipidmaps id 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>this function may returns nothing if the given <paramref name="id"/>
        ''' is not exists inside the database index.</returns>
        Public Overrides Function GetClass(id As String) As CompoundClass
            If index.ContainsKey(id) Then
                Dim lipid As MetaData = index(id)
                Dim [class] As New CompoundClass With {
                    .kingdom = "Lipids",
                    .super_class = lipid.CATEGORY,
                    .[class] = lipid.MAIN_CLASS,
                    .sub_class = lipid.SUB_CLASS,
                    .molecular_framework = lipid.CLASS_LEVEL4
                }

                Return [class]
            Else
                Return Nothing
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function EnumerateId() As IEnumerable(Of String)
            Return index.Keys
        End Function
    End Class

End Namespace
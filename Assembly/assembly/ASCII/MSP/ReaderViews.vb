Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.MSP

    Module ReaderViews

        <Extension>
        Public Function Read_compound_source(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.compound_class), NameOf(.compound_source)})
            End With
        End Function

        <Extension>
        Public Function Read_collision_energy(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple(
                    {
                        NameOf(.collision_energy),
                        NameOf(.collision_energy_voltage),
                        NameOf(.source_voltage),
                        NameOf(.ionization_energy)
                    })
            End With
        End Function

        <Extension>
        Public Function Read_instrument_type(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.ion_source), NameOf(.instrument_type)})
            End With
        End Function

        ''' <summary>
        ''' 这个函数会自动对保留时间进行单位的转换，返回结果的单位为秒
        ''' </summary>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Read_retention_time(meta As MetaData) As Double
            With meta
                Dim s$ = Trim(.ReadStringMultiple({NameOf(.retention_time)}))

                If s.StringEmpty Then
                    Return 0
                ElseIf InStr(s, "min", CompareMethod.Text) Then
                    Return Val(s) * 60
                Else
                    Return Val(s)
                End If
            End With
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Read_precursor_type(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple(
                    {
                        NameOf(.precursor_type),
                        NameOf(.adduct),
                        NameOf(.ion_type)
                    })
            End With
        End Function

        <Extension>
        Public Function Read_source_file(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.raw_data_file), NameOf(.source_file)})
            End With
        End Function

        <Extension>
        Public Function Read_exact_mass(meta As MetaData) As Double
            With meta
                Dim mw = .ReadDoubleMultiple({NameOf(.exact_mass), NameOf(.exactmass)})
                If mw = 0R Then
                    Return .ReadDoubleMultiple({NameOf(.total_exact_mass)})
                Else
                    Return mw
                End If
            End With
        End Function

        <Extension>
        Public Function Read_CAS(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.cas), NameOf(.cas_number)})
            End With
        End Function

#Region "Generic Reader"

        <Extension>
        Private Function ReadStringMultiple(meta As MetaData, names$()) As String
            Dim value As Object = meta.ReadMultiple(names)
            Return Scripting.ToString(value)
        End Function

        <Extension>
        Public Function ReadDoubleMultiple(meta As MetaData, names$()) As Double
            Dim s$ = meta.ReadStringMultiple(names)
            Return Val(s)
        End Function

        <Extension>
        Private Function ReadMultiple(meta As MetaData, names$()) As Object
            Dim value As Object = Nothing

            For Each name$ In names
                Dim field As BindProperty(Of ColumnAttribute) = fields(name)

                value = field.GetValue(meta)
                If Not value Is Nothing Then
                    Return value
                End If
            Next

            Return Nothing
        End Function

        <Extension>
        Private Function ReadStringsMultiple(meta As MetaData, names$()) As String()
            Dim value = meta.ReadMultiple(names)
            If value Is Nothing Then
                Return value
            Else
                Return DirectCast(value, String())
            End If
        End Function

        <Extension>
        Private Function ReadDoublesMultiple(meta As MetaData, names$()) As Double()
            Dim value = meta.ReadMultiple(names)
            If value Is Nothing Then
                Return value
            Else
                Return DirectCast(value, Double())
            End If
        End Function

#End Region
    End Module
End Namespace
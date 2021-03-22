#Region "Microsoft.VisualBasic::895c86593e9e8c9979092c329c8b0ab1, src\metadb\MoNA\ReaderViews.vb"

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

    ' Module ReaderViews
    ' 
    '     Function: Read_CAS, Read_collision_energy, Read_compound_source, Read_exact_mass, Read_instrument_type
    '               Read_precursor_type, Read_pubchemID, Read_retention_time, Read_source_file, ReadDoubleMultiple
    '               ReadDoublesMultiple, ReadMultiple, ReadStringMultiple, ReadStringsMultiple
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Scripting.Runtime

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

    <Extension>
    Public Function Read_pubchemID(meta As MetaData) As String
        With meta
            Return .ReadStringMultiple({NameOf(.pubchem), NameOf(.pubchem_cid)})
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
    Public Function Read_CAS(meta As MetaData) As String()
        With meta
            Return .ReadMultiple({NameOf(.cas), NameOf(.cas_number)}) _
                .As(Of String)() _
                .ToArray
        End With
    End Function

#Region "Generic Reader"

    <Extension>
    Private Function ReadStringMultiple(meta As MetaData, names$()) As String
        Return meta.ReadMultiple(names).As(Of String).FirstOrDefault
    End Function

    <Extension>
    Public Function ReadDoubleMultiple(meta As MetaData, names$()) As Double
        Dim s$ = meta.ReadStringMultiple(names)
        Return Val(s)
    End Function

    <Extension>
    Private Iterator Function ReadMultiple(meta As MetaData, names$()) As IEnumerable(Of Object)
        Dim value As Object = Nothing

        For Each name$ In names
            Dim field As BindProperty(Of ColumnAttribute) = fields(name)

            value = field.GetValue(meta)

            If value Is Nothing Then
                Continue For
            End If

            Select Case value.GetType
                Case GetType(String)
                    If Not DirectCast(value, String).StringEmpty Then
                        Yield value
                    End If
                Case GetType(Double), GetType(Integer), GetType(Long), GetType(Short), GetType(Byte)
                    If Not CDbl(value) = 0R Then
                        Yield value
                    End If
                Case Else
                    ' object 肯定有值，则直接返回
                    Yield value
            End Select
        Next
    End Function

    <Extension>
    Private Function ReadStringsMultiple(meta As MetaData, names$()) As String()
        Dim value = meta.ReadMultiple(names).As(Of String)

        If value Is Nothing Then
            Return {}
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

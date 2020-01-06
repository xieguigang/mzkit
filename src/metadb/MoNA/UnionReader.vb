#Region "Microsoft.VisualBasic::e660f6c2570369d69523c690966f27fc, src\metadb\MoNA\UnionReader.vb"

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

' Class UnionReader
' 
'     Properties: CAS, chebi, collision_energy, compound_source, exact_mass
'                 formula, hmdb, instrument_type, ionMode, precursor_type
'                 pubchem, retention_time, sourcefile
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: numericIdInternal, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports Microsoft.VisualBasic.Language

Public Class UnionReader

    ReadOnly meta As MetaData
    ReadOnly mold As [Variant](Of MspData, SpectraSection)

#Region "Reader Properties"

    Public ReadOnly Property collision_energy As String
        Get
            Return meta.Read_collision_energy
        End Get
    End Property

    Public ReadOnly Property hmdb As String
        Get
            Return meta.hmdb
        End Get
    End Property

    ''' <summary>
    ''' 返回纯数字类型的chebi编号，如果物质没有chebi编号，则返回-1
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property chebi As Long
        Get
            Return numericIdInternal(meta.chebi)
        End Get
    End Property

    Public ReadOnly Property CAS As String()
        Get
            Return meta.Read_CAS
        End Get
    End Property

    Public ReadOnly Property precursor_type As String
        Get
            Return meta.Read_precursor_type
        End Get
    End Property

    ''' <summary>
    ''' 单位已经统一为秒
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property retention_time As Double
        Get
            Return meta.Read_retention_time
        End Get
    End Property

    Public ReadOnly Property instrument_type As String
        Get
            Return meta.Read_instrument_type
        End Get
    End Property

    Public ReadOnly Property exact_mass As Double
        Get
            Dim mass# = meta.Read_exact_mass

            If mass = 0R Then
                If mold Like GetType(MspData) Then
                    Return mold.TryCast(Of MspData).MW
                Else
                    Return mold.TryCast(Of SpectraSection).exact_mass
                End If
            Else
                Return mass
            End If
        End Get
    End Property

    Public ReadOnly Property formula As String
        Get
            Dim formulaVal$

            If mold Like GetType(MspData) Then
                formulaVal = mold.TryCast(Of MspData).Formula
            Else
                formulaVal = mold.TryCast(Of SpectraSection).formula
            End If

            If formulaVal.StringEmpty Then
                Return meta.molecular_formula
            Else
                Return formulaVal
            End If
        End Get
    End Property

    Public ReadOnly Property pubchem As Long
        Get
            Return numericIdInternal(meta.Read_pubchemID)
        End Get
    End Property

    Public ReadOnly Property ionMode As String
        Get
            Dim ionVal$

            If mold Like GetType(MspData) Then
                ionVal = mold.TryCast(Of MspData).Ion_mode
            Else
                ionVal = mold.TryCast(Of SpectraSection).MetaDB.ionization_mode
            End If

            If ionVal.StringEmpty Then
                Return meta.ionization_mode _
                    ?.Split(","c) _
                    ?.FirstOrDefault
            Else
                Return ionVal
            End If
        End Get
    End Property

    Public ReadOnly Property sourcefile As String
        Get
            Return meta.Read_source_file
        End Get
    End Property

    Public ReadOnly Property compound_source As String
        Get
            Return meta.Read_compound_source
        End Get
    End Property
#End Region

    Sub New(meta As MetaData, Optional msp As MspData = Nothing)
        Me.meta = meta
        Me.mold = msp
    End Sub

    Sub New(meta As MetaData, spectra As SpectraSection)
        Me.meta = meta
        Me.mold = spectra
    End Sub

    Private Function numericIdInternal(idStr As String, <CallerMemberName> Optional name$ = Nothing) As Long
        Static delimiter As Char() = {":"c, " "c, Text.ASCII.TAB, "="c}

        idStr = Strings.Trim(idStr)

        If idStr.StringEmpty Then
            Return -1
        ElseIf PrimitiveParser.IsNumeric(idStr) Then
            Return CLng(Val(idStr))
        Else
            Dim tokens() = idStr _
                .Split(delimiter) _
                .Where(Function(s) s.Length > 0) _
                .ToArray
            Dim first = tokens.ElementAtOrDefault(0)
            Dim last = tokens.ElementAtOrDefault(1)

            If PrimitiveParser.IsNumeric(first) Then
                Return CLng(Val(first))
            ElseIf PrimitiveParser.IsNumeric(last) Then
                Return CLng(Val(last))
            Else
                Call $"Invalid format for {name} = ""{idStr}""".Warning
                Return -1
            End If
        End If
    End Function

    Public Overrides Function ToString() As String
        Return meta.name
    End Function
End Class

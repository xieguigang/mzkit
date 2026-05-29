#Region "Microsoft.VisualBasic::b18ed2192119d0bc20d85e8f62c2bdcd, metadb\Chemoinformatics\InChI\Models\InchiOptions.vb"

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

    '   Total Lines: 136
    '    Code Lines: 94 (69.12%)
    ' Comment Lines: 23 (16.91%)
    '    - Xml Docs: 34.78%
    ' 
    '   Blank Lines: 19 (13.97%)
    '     File Size: 5.60 KB


    '     Class InchiOptions
    ' 
    '         Properties: Flags, Timeout, TimeoutMilliSeconds
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    '         Class InchiOptionsBuilder
    ' 
    '             Function: build, withFlag, withTimeout, withTimeoutMilliSeconds
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text


' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public Class InchiOptions

        Friend Shared ReadOnly DEFAULT_OPTIONS As InchiOptions = New InchiOptionsBuilder().build()

        Private Shared ReadOnly IS_WINDOWS As Boolean = False ' = System.getProperty("os.name", "").ToLower(Locale.ROOT).StartsWith("windows");

        Private ReadOnly flagsField As IList(Of InchiFlag)
        Private ReadOnly timeoutMilliSecs As Long

        Private Sub New(builder As InchiOptionsBuilder)
            flagsField = New List(Of InchiFlag)(builder.flags)
            timeoutMilliSecs = builder.timeoutMilliSecs
        End Sub

        Public Class InchiOptionsBuilder

            Friend ReadOnly flags As New List(Of InchiFlag)
            Friend timeoutMilliSecs As Long = 0

            Public Overridable Function withFlag(ParamArray flags As InchiFlag()) As InchiOptionsBuilder
                For Each flag In flags
                    Me.flags.add(flag)
                Next
                Return Me
            End Function

            ''' <summary>
            ''' Timeout in seconds (0 = infinite timeout) </summary>
            ''' <param name="timeoutSecs">
            ''' @return </param>
            Public Overridable Function withTimeout(timeoutSecs As Integer) As InchiOptionsBuilder
                If timeoutSecs < 0 Then
                    Throw New ArgumentException("Timeout should be a time in seconds or 0 for infinite: " & timeoutSecs.ToString())
                End If
                timeoutMilliSecs = CLng(timeoutSecs) * 1000
                Return Me
            End Function

            ''' <summary>
            ''' Timeout in milliseconds (0 = infinite timeout) </summary>
            ''' <param name="timeoutMilliSecs">
            ''' @return </param>
            Public Overridable Function withTimeoutMilliSeconds(timeoutMilliSecs As Long) As InchiOptionsBuilder
                If timeoutMilliSecs < 0 Then
                    Throw New ArgumentException("Timeout should be a time in milliseconds or 0 for infinite: " & timeoutMilliSecs.ToString())
                End If
                Me.timeoutMilliSecs = timeoutMilliSecs
                Return Me
            End Function

            Public Overridable Function build() As InchiOptions
                Dim stereoOptionFlags = 0
                Dim chiralFlagFlags = 0
                For Each flag As InchiFlag In flags
                    Select Case flag.innerEnumValue
                        Case InchiFlag.InnerEnum.SNon, InchiFlag.InnerEnum.SRac, InchiFlag.InnerEnum.SRel, InchiFlag.InnerEnum.SUCF, InchiFlag.InnerEnum.SAbs
                            stereoOptionFlags += 1
                        Case InchiFlag.InnerEnum.ChiralFlagOFF, InchiFlag.InnerEnum.ChiralFlagON
                            chiralFlagFlags += 1
                        Case Else
                    End Select
                Next
                If stereoOptionFlags > 1 Then
                    Throw New ArgumentException("Ambiguous flags: SAbs, SNon, SRel, SRac and SUCF are mutually exclusive")
                End If
                If chiralFlagFlags > 1 Then
                    Throw New ArgumentException("Ambiguous flags: ChiralFlagOFF and ChiralFlagON are mutually exclusive")
                End If
                Return New InchiOptions(Me)
            End Function
        End Class

        Public Overridable ReadOnly Property Flags As IList(Of InchiFlag)
            Get
                Return flagsField
            End Get
        End Property

        Public Overridable ReadOnly Property Timeout As Integer
            Get
                Return timeoutMilliSecs / 1000
            End Get
        End Property

        Public Overridable ReadOnly Property TimeoutMilliSeconds As Long
            Get
                Return timeoutMilliSecs
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim sb As StringBuilder = New StringBuilder()
            For Each inchiFlag In flagsField
                If inchiFlag Is InchiFlag.SAbs Then
                    Continue For
                End If
                If sb.Length > 0 Then
                    sb.Append(" "c)
                End If
                sb.Append(If(IS_WINDOWS, "/", "-"))
                sb.Append(inchiFlag.ToString())
            Next
            If timeoutMilliSecs <> 0 Then
                If sb.Length > 0 Then
                    sb.Append(" "c)
                End If
                sb.Append(If(IS_WINDOWS, "/", "-"))
                sb.Append("WM")
                sb.Append(timeoutMilliSecs.ToString())
            End If
            Return sb.ToString()
        End Function
    End Class

End Namespace

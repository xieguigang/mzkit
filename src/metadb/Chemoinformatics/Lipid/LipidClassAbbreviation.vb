#Region "Microsoft.VisualBasic::7c12675e39691f64cc33b0ced25b67d6, metadb\Chemoinformatics\Lipid\LipidClassAbbreviation.vb"

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

    '   Total Lines: 85
    '    Code Lines: 57 (67.06%)
    ' Comment Lines: 7 (8.24%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 21 (24.71%)
    '     File Size: 2.81 KB


    '     Enum LipidClassAbbreviation
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum LipidMAPSAlias
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module AliasNameMapper
    ' 
    '         Function: LipidSearchToLipidMaps
    ' 
    '     Module AbbreviationNameMapper
    ' 
    '         Function: CheckFullName, GetAbbreviationName, ToFullName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Lipidomics

    Public Enum LipidClassAbbreviation

        <Description("Sphingosine")> SPH

    End Enum

    Public Enum LipidMAPSAlias

        <Description("CAR")> AcCa

    End Enum

    Public Module AliasNameMapper

        ''' <summary>
        ''' this function will make a value copy of the given <paramref name="lipidsearch"/> and
        ''' then try to alias mapping of the lipid class name.
        ''' </summary>
        ''' <param name="lipidsearch"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function LipidSearchToLipidMaps(lipidsearch As LipidName) As LipidName
            Static aliasMap As Dictionary(Of String, String) = Enums(Of LipidMAPSAlias)() _
                .ToDictionary(Function(f) f.ToString.ToLower,
                              Function(f)
                                  Return f.Description
                              End Function)

            lipidsearch = New LipidName(lipidsearch)

            If aliasMap.ContainsKey(lipidsearch.className.ToLower) Then
                lipidsearch.className = aliasMap(lipidsearch.className.ToLower)
            End If

            Return lipidsearch
        End Function

    End Module

    Public Module AbbreviationNameMapper

        ReadOnly mapping As Dictionary(Of String, String) = Enums(Of LipidClassAbbreviation) _
            .ToDictionary(Function(f) f.ToString.ToLower,
                            Function(f)
                                Return f.Description
                            End Function)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckFullName(abbr As String) As Boolean
            Return mapping.ContainsKey(abbr.ToLower)
        End Function

        Public Function ToFullName(abbr As String) As String
            Dim lcase As String = abbr.ToLower

            If mapping.ContainsKey(lcase) Then
                Return mapping(lcase)
            Else
                Return abbr
            End If
        End Function

        Public Function GetAbbreviationName(full As String) As String
            Static mapping As Dictionary(Of String, String) = Enums(Of LipidClassAbbreviation) _
                .ToDictionary(Function(f) f.Description.ToLower,
                              Function(f)
                                  Return f.ToString
                              End Function)
            Dim lcase As String = full.ToLower

            If mapping.ContainsKey(lcase) Then
                Return mapping(lcase)
            Else
                Return full
            End If
        End Function

    End Module
End Namespace

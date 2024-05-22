#Region "Microsoft.VisualBasic::9d4204608cef2c7f29dd93e826a8aba7, assembly\ThermoRawFileReader\DataObjects\TuneMethodSettingType.vb"

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

    '   Total Lines: 30
    '    Code Lines: 11 (36.67%)
    ' Comment Lines: 15 (50.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (13.33%)
    '     File Size: 858 B


    '     Structure TuneMethodSettingType
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DataObjects

    ''' <summary>
    ''' Type for Tune Method Settings
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure TuneMethodSettingType
        ''' <summary>
        ''' Tune category
        ''' </summary>
        Public Category As String

        ''' <summary>
        ''' Tune name
        ''' </summary>
        Public Name As String

        ''' <summary>
        ''' Tune value
        ''' </summary>
        Public Value As String

        ''' <summary>
        ''' Display the category, name, and value of this setting
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format("{0,-20}  {1,-40} = {2}", If(Category, "Undefined") & ":", If(Name, String.Empty), If(Value, String.Empty))
        End Function
    End Structure
End Namespace

#Region "Microsoft.VisualBasic::c6d7e092ab8b115483a26d5002b59398, Massbank\Concentration.vb"

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

    ' Module Concentration
    ' 
    '     Function: ConvertTo
    ' 
    ' Enum Units
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Public Module Concentration

    ReadOnly CFactors#() = {
        1,
        10000.0,
        1000.0,
        6250.0,
        7812.5,
        0.001728,
        0.046656,
        0.0909218,
        0.07570823568,
        0.56633693184,
        62500.0,
        0.03584,
        31.25,
        1,
        1000.0,
        1000.0,
        1000.0,
        1,
        50000.0,
        50.0,
        0.05,
        1,
        1000.0,
        1
    }

    <Convertor(GetType(Units))>
    <Extension>
    Public Function ConvertTo(value As UnitValue(Of Units), unit As Units) As UnitValue(Of Units)
        Dim standardVal#
        Dim uniVal#

        ' Calculate standard value for input
        Select Case value.Unit
            Case Units.NA : standardVal = value                                                ' 0
            Case Units.percent : standardVal = value * CFactors(Units.percent)                 ' 1
            Case Units.permil : standardVal = value * CFactors(Units.permil)                   ' 2
            Case Units.floz_gallon_UK : standardVal = value * CFactors(Units.floz_gallon_UK)   ' 3
            Case Units.floz_gallon_US : standardVal = value * CFactors(Units.floz_gallon_US)   ' 4
            Case Units.cuin_cuft : standardVal = value / CFactors(Units.cuin_cuft)             ' 5
            Case Units.cuin_cuyard : standardVal = value / CFactors(Units.cuin_cuyard)         ' 6
            Case Units.drops_gallon_UK : standardVal = value / CFactors(Units.drops_gallon_UK) ' 7
            Case Units.drops_gallon_US : standardVal = value / CFactors(Units.drops_gallon_US) ' 8
            Case Units.drops_cuft : standardVal = value / CFactors(Units.drops_cuft)           ' 9
            Case Units.oz_pound : standardVal = value * CFactors(Units.oz_pound)               ' 10
            Case Units.oz_ton_UK : standardVal = value / CFactors(Units.oz_ton_UK)             ' 11
            Case Units.oz_ton_US : standardVal = value * CFactors(Units.oz_ton_US)             ' 12
            Case Units.ppm : standardVal = value * CFactors(Units.ppm)                         ' 13
            Case Units.parts_billion : standardVal = value / CFactors(Units.parts_billion)     ' 14
            Case Units.mL_litre : standardVal = value * CFactors(Units.mL_litre)               ' 15
            Case Units.mL_megalitre : standardVal = value / CFactors(Units.mL_megalitre)       ' 16
            Case Units.mL_cumetre : standardVal = value * CFactors(Units.mL_cumetre)           ' 17
            Case Units.drops_mL : standardVal = value * CFactors(Units.drops_mL)               ' 18
            Case Units.drops_litre : standardVal = value * CFactors(Units.drops_litre)         ' 19
            Case Units.drops_cumetre : standardVal = value * CFactors(Units.drops_cumetre)     ' 20
            Case Units.milligrams_kg : standardVal = value * CFactors(Units.milligrams_kg)     ' 21
            Case Units.grams_kg : standardVal = value * CFactors(Units.grams_kg)               ' 22
            Case Units.grams_tonne : standardVal = value * CFactors(Units.grams_tonne)         ' 23
            Case Else
                Throw New NotSupportedException("Invalid unit type value!")
        End Select

        ' Convert to target unit from this standard value.
        Select Case unit
            Case Units.percent : uniVal = standardVal / CFactors(Units.percent)                 ' 1
            Case Units.permil : uniVal = standardVal / CFactors(Units.permil)                   ' 2
            Case Units.floz_gallon_UK : uniVal = standardVal / CFactors(Units.floz_gallon_UK)   ' 3
            Case Units.floz_gallon_US : uniVal = standardVal / CFactors(Units.floz_gallon_US)   ' 4
            Case Units.cuin_cuft : uniVal = standardVal * CFactors(Units.cuin_cuft)             ' 5
            Case Units.cuin_cuyard : uniVal = standardVal * CFactors(Units.cuin_cuyard)         ' 6
            Case Units.drops_gallon_UK : uniVal = standardVal * CFactors(Units.drops_gallon_UK) ' 7
            Case Units.drops_gallon_US : uniVal = standardVal * CFactors(Units.drops_gallon_US) ' 8
            Case Units.drops_cuft : uniVal = standardVal * CFactors(Units.drops_cuft)           ' 9
            Case Units.oz_pound : uniVal = standardVal / CFactors(Units.oz_pound)               ' 10
            Case Units.oz_ton_UK : uniVal = standardVal * CFactors(Units.oz_ton_UK)             ' 11
            Case Units.oz_ton_US : uniVal = standardVal / CFactors(Units.oz_ton_US)             ' 12
            Case Units.ppm : uniVal = standardVal / CFactors(Units.ppm)                         ' 13
            Case Units.parts_billion : uniVal = standardVal * CFactors(Units.parts_billion)     ' 14
            Case Units.mL_litre : uniVal = standardVal / CFactors(Units.mL_litre)               ' 15
            Case Units.mL_megalitre : uniVal = standardVal * CFactors(Units.mL_megalitre)       ' 16
            Case Units.mL_cumetre : uniVal = standardVal / CFactors(Units.mL_cumetre)           ' 17
            Case Units.drops_mL : uniVal = standardVal / CFactors(Units.drops_mL)               ' 18
            Case Units.drops_litre : uniVal = standardVal / CFactors(Units.drops_litre)         ' 19
            Case Units.drops_cumetre : uniVal = standardVal / CFactors(Units.drops_cumetre)     ' 20
            Case Units.milligrams_kg : uniVal = standardVal / CFactors(Units.milligrams_kg)     ' 21
            Case Units.grams_kg : uniVal = standardVal / CFactors(Units.grams_kg)               ' 22
            Case Units.grams_tonne : uniVal = standardVal / CFactors(Units.grams_tonne)         ' 23
            Case Else
                Throw New NotSupportedException("Invalid unit type value!")
        End Select

        Return New UnitValue(Of Units)(uniVal, unit)
    End Function
End Module

<Convertor(GetType(Concentration))>
Public Enum Units As Integer

    NA = 0

    ''' <summary>
    ''' 百分之一``[%]``
    ''' </summary>
    <Description("%")> percent = 1
    ''' <summary>
    ''' 千分之一``[‰]``
    ''' </summary>
    <Description("‰")> permil
    ''' <summary>
    ''' ``液量盎司/加仑``(UK)
    ''' </summary>
    <Description("fl.oz/gallon(UK)")> floz_gallon_UK
    ''' <summary>
    ''' ``液量盎司/加仑``(US)
    ''' </summary>
    <Description("fl.oz/gallon(US)")> floz_gallon_US
    ''' <summary>
    ''' ``立方英寸/立方英尺``
    ''' </summary>
    <Description("cu.in/cu.ft")> cuin_cuft
    ''' <summary>
    ''' ``立方英寸/立方码``
    ''' </summary>
    <Description("cu.in/cu.yard")> cuin_cuyard
    ''' <summary>
    ''' ``滴/加仑``(UK)
    ''' </summary>
    <Description("drops/gallon(UK)")> drops_gallon_UK
    ''' <summary>
    ''' ``滴/加仑``(US)
    ''' </summary>
    <Description("drops/gallon(US)")> drops_gallon_US
    ''' <summary>
    ''' ``滴/立方英尺``
    ''' </summary>
    <Description("drops/cu.ft")> drops_cuft
    ''' <summary>
    ''' ``盎司/镑``
    ''' </summary>
    <Description("oz/pound")> oz_pound
    ''' <summary>
    ''' ``盎司/吨``(UK)
    ''' </summary>
    <Description("oz/ton(UK)")> oz_ton_UK
    ''' <summary>
    ''' ``盎司/吨``(US)
    ''' </summary>
    <Description("oz/ton(US)")> oz_ton_US
    ''' <summary>
    ''' Parts per million
    ''' </summary>
    <Description("ppm")> ppm
    ''' <summary>
    ''' 十亿分之一
    ''' </summary>
    <Description("parts/billion")> parts_billion
    ''' <summary>
    ''' ``毫升/升``
    ''' </summary>
    <Description("mL/litre")> mL_litre
    ''' <summary>
    ''' ``毫升/兆升``
    ''' </summary>
    <Description("mL/megalitre")> mL_megalitre
    ''' <summary>
    ''' ``毫升/立方米``
    ''' </summary>
    <Description("mL/cu.metre")> mL_cumetre
    ''' <summary>
    ''' ``滴/毫升``
    ''' </summary>
    <Description("drops/mL")> drops_mL
    ''' <summary>
    ''' ``滴/升``
    ''' </summary>
    <Description("drops/litre")> drops_litre
    ''' <summary>
    ''' ``滴/立方米``
    ''' </summary>
    <Description("drops/cu.metre")> drops_cumetre
    ''' <summary>
    ''' ``毫克/公斤`` 
    ''' </summary>
    <Description("milligrams/kg")> milligrams_kg
    ''' <summary>
    ''' ``克/公斤``
    ''' </summary>
    <Description("grams/kg")> grams_kg
    ''' <summary>
    ''' ``克/吨`` 
    ''' </summary>
    <Description("grams/tonne")> grams_tonne

End Enum

#Region "Microsoft.VisualBasic::d26ee91b14322e7259ad91fbfb4934e3, metadb\Massbank\Concentration\Concentration.vb"

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

    '   Total Lines: 100
    '    Code Lines: 92
    ' Comment Lines: 2
    '   Blank Lines: 6
    '     File Size: 6.01 KB


    ' Module Concentration
    ' 
    '     Function: ConvertTo
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

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

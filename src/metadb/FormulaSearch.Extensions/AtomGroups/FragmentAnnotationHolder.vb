#Region "Microsoft.VisualBasic::3800e26f172d0ca652799d13276d4456, mzkit\src\metadb\FormulaSearch.Extensions\AtomGroups\FragmentAnnotationHolder.vb"

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

    '   Total Lines: 111
    '    Code Lines: 83
    ' Comment Lines: 7
    '   Blank Lines: 21
    '     File Size: 4.41 KB


    '     Class FragmentAnnotationHolder
    ' 
    '         Properties: base, exactMass, name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: getReferMathName, ToString
    '         Operators: -, *, +, (+2 Overloads) Like
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Namespace AtomGroups

    ''' <summary>
    ''' A cache for the <see cref="exactMass"/> value
    ''' </summary>
    Public Class FragmentAnnotationHolder

        Public ReadOnly Property name As String

        ''' <summary>
        ''' a cache value of the exact mass value which is evaluated from <see cref="base"/> object
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property exactMass As Double
        Public ReadOnly Property base As IExactMassProvider

        Sub New(anno As IExactMassProvider, Optional name As String = Nothing)
            exactMass = anno.ExactMass
            base = anno

            If TypeOf anno Is MassGroup Then
                Me.name = If(name.StringEmpty, DirectCast(anno, MassGroup).name, name)
            ElseIf TypeOf anno Is Formula Then
                Me.name = If(name.StringEmpty, DirectCast(anno, Formula).EmpiricalFormula, name)
            Else
                Throw New NotImplementedException(anno.GetType.FullName)
            End If
        End Sub

        Private Sub New(name As String, exactMass As Double, base As IExactMassProvider)
            Me.name = name
            Me.exactMass = exactMass
            Me.base = base
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return name
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator Like(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As Boolean
            If a.base.GetType Is b.base.GetType Then
                Return True
            Else
                Return False
            End If
        End Operator

        Public Shared Operator *(formula As FragmentAnnotationHolder, n As Integer) As FragmentAnnotationHolder
            Dim name As String = $"({formula.name}){n}"
            Dim mass As Double = formula.exactMass * n

            Return New FragmentAnnotationHolder(name, mass, formula.base)
        End Operator

        Private Shared Function getReferMathName(name As String) As String
            If name.Contains("+"c) OrElse name.Contains("-"c) Then
                If name.First = "("c AndAlso name.Last = ")"c Then
                    Return name
                Else
                    Return $"({name})"
                End If
            Else
                Return name
            End If
        End Function

        Public Shared Operator +(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As FragmentAnnotationHolder
            Dim newName As String = $"{getReferMathName(a.name)}+{getReferMathName(b.name)}"

            If TypeOf a.base Is Formula AndAlso a Like b Then
                Dim newBase As Formula = DirectCast(a.base, Formula) + DirectCast(b.base, Formula)
                Dim newMass As Double = newBase.ExactMass

                Return New FragmentAnnotationHolder(newName, newMass, newBase)
            Else
                Dim newGroup As New MassGroup With {
                    .name = newName,
                    .exactMass = a.exactMass + b.exactMass
                }

                Return New FragmentAnnotationHolder(newGroup)
            End If
        End Operator

        Public Shared Operator -(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As FragmentAnnotationHolder
            Dim newName As String = $"{getReferMathName(a.name)}-{getReferMathName(b.name)}"

            If TypeOf a.base Is Formula AndAlso a Like b Then
                Dim newBase As Formula = DirectCast(a.base, Formula) - DirectCast(b.base, Formula)
                Dim newMass As Double = newBase.ExactMass

                Return New FragmentAnnotationHolder(newName, newMass, newBase)
            Else
                Dim newGroup As New MassGroup With {
                    .name = newName,
                    .exactMass = a.exactMass - b.exactMass
                }

                Return New FragmentAnnotationHolder(newGroup)
            End If
        End Operator

    End Class
End Namespace

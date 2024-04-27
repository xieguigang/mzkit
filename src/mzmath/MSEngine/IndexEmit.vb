#Region "Microsoft.VisualBasic::b91ccfda25e1e80ac4475e8c95e6b16a, G:/mzkit/src/mzmath/MSEngine//IndexEmit.vb"

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

    '   Total Lines: 76
    '    Code Lines: 62
    ' Comment Lines: 1
    '   Blank Lines: 13
    '     File Size: 2.62 KB


    ' Class IndexEmit
    ' 
    '     Properties: [delegate]
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateActivator, delegate_mass_argument, delegate_no_argument, ParseMassArgument, ParseNoArgument
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.Emit.Delegates

Public Class IndexEmit

    ReadOnly no_arg As ConstructorInfo
    ReadOnly mass_arg As ConstructorInfo
    ReadOnly type As Type

    Public ReadOnly Property [delegate] As Type

    Sub New(schema As Type)
        no_arg = ParseNoArgument(schema)
        mass_arg = ParseMassArgument(schema)
        type = schema
        [delegate] = GetType(Func(Of,  )).MakeGenericType(GetType(Double), GetType(Object))
    End Sub

    Private Shared Function ParseNoArgument(schema As Type) As ConstructorInfo
        Return schema _
            .GetConstructors _
            .Where(Function(c) c.GetParameters.Length = 0) _
            .FirstOrDefault
    End Function

    Private Shared Function ParseMassArgument(schema As Type) As ConstructorInfo
        Return schema _
            .GetConstructors _
            .Where(Function(c)
                       Dim args = c.GetParameters

                       If args.Length <> 1 Then
                           Return False
                       End If

                       Return args(Scan0).ParameterType Is GetType(Double)
                   End Function) _
            .FirstOrDefault
    End Function

    Private Function delegate_no_argument() As Object
        Dim writeMap As InterfaceMapping = type.GetInterfaceMap(GetType(IExactMassProvider))
        Dim target = writeMap.TargetMethods(0)
        ' get_xxx
        Dim set_prop As String = target.Name.Substring(4)
        Dim set_func = DelegateFactory.PropertySet(type, set_prop)
        Dim del As Func(Of Double, Object) =
            Function(m As Double)
                Dim obj As IExactMassProvider = Activator.CreateInstance(type)
                Call set_func(obj, m)
                Return obj
            End Function

        Return del
    End Function

    Private Function delegate_mass_argument() As Object
        Dim del As Func(Of Double, Object) =
            Function(mass As Double)
                Return Activator.CreateInstance(type, mass)
            End Function

        Return del
    End Function

    Public Function CreateActivator() As Object
        If no_arg IsNot Nothing Then
            Return delegate_no_argument()
        ElseIf mass_arg IsNot Nothing Then
            Return delegate_mass_argument()
        Else
            Throw New InvalidProgramException("No suitable data interface could be found!")
        End If
    End Function
End Class

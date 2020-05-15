#Region "Microsoft.VisualBasic::ffb7af420686ccadb72ddbf9532c7278, Rscript\Library\mzkit.interopArguments\Math.vb"

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

    ' Module Math
    ' 
    '     Function: getTolerance
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Module Math

    Public Function getTolerance(val As Object, env As Environment) As [Variant](Of Tolerance, Message)
        If val Is Nothing Then
            Return Tolerance.DefaultTolerance.DefaultValue
        ElseIf val.GetType.IsInheritsFrom(GetType(Tolerance)) Then
            Return val
        ElseIf val.GetType Is GetType(String) Then
            Return Tolerance.ParseScript(val)
        ElseIf val.GetType Is GetType(String()) Then
            Return Tolerance.ParseScript(DirectCast(val, String())(Scan0))
        ElseIf val.GetType Is GetType(vector) Then
            Return Tolerance.ParseScript(DirectCast(val, vector).data(Scan0))
        Else
            Return debug.stop(New NotImplementedException(val.GetType.FullName), env)
        End If
    End Function
End Module

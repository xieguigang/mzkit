#Region "Microsoft.VisualBasic::4824744322295efbade38f2415cc592f, mzkit\src\mzkit\ServiceHub\ServiceProtocols\Payloads\LayerLoader.vb"

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

    '   Total Lines: 32
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.23 KB


    ' Class LayerLoader
    ' 
    '     Properties: densityCut, method, mz, mzErr
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetTolerance
    '     Class Schema
    ' 
    '         Function: GetObjectSchema
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class LayerLoader

    Public Property mz As Double()
    Public Property mzErr As Double
    Public Property method As String
    Public Property densityCut As Double

    Public Function GetTolerance() As Tolerance
        Return Tolerance.ParseScript($"{method}:{mzErr}")
    End Function

    Shared Sub New()
        Call MsgPackSerializer.DefaultContext.RegisterSerializer(New Schema)
    End Sub

    Private Class Schema : Inherits SchemaProvider(Of LayerLoader)

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(LayerLoader), New Dictionary(Of String, NilImplication) From {
                {NameOf(mz), NilImplication.MemberDefault},
                {NameOf(mzErr), NilImplication.MemberDefault},
                {NameOf(method), NilImplication.MemberDefault},
                {NameOf(densityCut), NilImplication.MemberDefault}
            })
        End Function
    End Class

End Class

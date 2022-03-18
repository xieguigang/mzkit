#Region "Microsoft.VisualBasic::9c3ccd65d02c2c0829dc96d0d2c70ee7, mzkit\src\mzkit\mzkit\forms\Inputs\InputNetworkLayout.vb"

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

    '   Total Lines: 36
    '    Code Lines: 26
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.14 KB


    ' Class InputNetworkLayout
    ' 
    '     Sub: Button1_Click, Button2_Click, InputNetworkLayout_Load
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce

Public Class InputNetworkLayout

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim args As ForceDirectedArgs = Globals.Settings.network.layout

        If args Is Nothing Then
            args = ForceDirectedArgs.DefaultNew
        End If

        args.Damping = damping.Value
        args.Stiffness = stiffness.Value
        args.Repulsion = repulsion.Value

        Globals.Settings.Save()

        DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub InputNetworkLayout_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim args As ForceDirectedArgs = Globals.Settings.network.layout

        If args Is Nothing Then
            args = ForceDirectedArgs.DefaultNew
        End If

        damping.Value = args.Damping
        stiffness.Value = args.Stiffness
        repulsion.Value = args.Repulsion
    End Sub
End Class

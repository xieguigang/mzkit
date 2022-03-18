#Region "Microsoft.VisualBasic::55d1058044bb4d53d5405f82ac07fab6, mzkit\src\mzkit\mzkit\pages\Settings\MolecularNetworking.vb"

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

    '   Total Lines: 68
    '    Code Lines: 57
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 3.38 KB


    ' Class MolecularNetworking
    ' 
    '     Sub: LoadSettings, SaveSettings, ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My

Public Class MolecularNetworking : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.network Is Nothing Then
            Globals.Settings.network = New NetworkArguments With {
                .layout = New ForceDirectedArgs With {.Damping = 0.4, .Iterations = 100, .Repulsion = 10000, .Stiffness = 41.76},
                .linkWidth = New ElementRange With {.min = 1, .max = 5},
                .nodeRadius = New ElementRange With {.min = 8, .max = 40}
            }
        End If
        If Globals.Settings.network.nodeRadius Is Nothing Then
            Globals.Settings.network.nodeRadius = New ElementRange With {.min = 8, .max = 40}
        End If
        If Globals.Settings.network.linkWidth Is Nothing Then
            Globals.Settings.network.linkWidth = New ElementRange With {.min = 1, .max = 5}
        End If
        If Globals.Settings.network.layout Is Nothing Then
            Globals.Settings.network.layout = New ForceDirectedArgs With {.Damping = 0.4, .Iterations = 100, .Repulsion = 10000, .Stiffness = 41.76}
        End If

        NumericUpDown1.Value = Globals.Settings.network.layout.Iterations
        stiffness.Value = Globals.Settings.network.layout.Stiffness
        damping.Value = Globals.Settings.network.layout.Damping
        repulsion.Value = Globals.Settings.network.layout.Repulsion

        TrackBar1.Value = Globals.Settings.network.nodeRadius.min
        TrackBar2.Value = Globals.Settings.network.nodeRadius.max

        TrackBar4.Value = Globals.Settings.network.linkWidth.min
        TrackBar3.Value = Globals.Settings.network.linkWidth.max

        NumericUpDown3.Value = Globals.Settings.network.treeNodeIdentical
        NumericUpDown2.Value = Globals.Settings.network.treeNodeSimilar
        NumericUpDown4.Value = Globals.Settings.network.defaultFilter

        MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue = Globals.Settings.network.defaultFilter
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.network.layout = New ForceDirectedArgs With {
            .Iterations = NumericUpDown1.Value,
            .Repulsion = repulsion.Value,
            .Damping = damping.Value,
            .Stiffness = stiffness.Value
        }
        Globals.Settings.network.nodeRadius = New ElementRange With {
            .min = TrackBar1.Value, .max = TrackBar2.Value
        }
        Globals.Settings.network.linkWidth = New ElementRange With {
            .min = TrackBar4.Value, .max = TrackBar3.Value
        }

        Globals.Settings.network.treeNodeIdentical = NumericUpDown3.Value
        Globals.Settings.network.treeNodeSimilar = NumericUpDown2.Value
        Globals.Settings.network.defaultFilter = NumericUpDown4.Value

        MyApplication.host.ribbonItems.SpinnerSimilarity.DecimalValue = Globals.Settings.network.defaultFilter
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class

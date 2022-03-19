#Region "Microsoft.VisualBasic::ad30782f4af83e14814f6e476e7d2a2e, mzkit\src\mzkit\mzkit\pages\Settings\buttons\DropDownButton.vb"

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

    '   Total Lines: 405
    '    Code Lines: 309
    ' Comment Lines: 32
    '   Blank Lines: 64
    '     File Size: 15.89 KB


    '     Class DropDownButton
    ' 
    ' 
    '         Enum Renderers
    ' 
    '             [Default], Native
    ' 
    ' 
    ' 
    '         Delegate Sub
    ' 
    '             Properties: DropDownMenu, DropDownSelectedItem, Renderer
    ' 
    '             Constructor: (+1 Overloads) Sub New
    ' 
    '             Function: IsInputKey, IsWinXP
    ' 
    '             Sub: DrawPreVistaStyle, DrawVisualStyle, DropDownButton_TextChanged, DropDownMenu_ItemAdded, DropDownMenu_ItemClicked
    '                  DropDownMenu_ItemRemoved, OnClick, OnDropDownClicked, OnDropDownItemClicked, OnGotFocus
    '                  OnKeyDown, OnLostFocus, OnMouseDown, OnMouseLeave, OnMouseMove
    '                  OnMouseUp, OnPaint, ShowContextMenuStrip
    ' 
    '     Class DropDownItemEventArgs
    ' 
    '         Properties: Item, ItemIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class DropDownMenu
    ' 
    '         Properties: DropDownButton
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles


Namespace UnFound.Controls
    Public Class DropDownButton
        Inherits Control

        Public Enum Renderers
            [Default]
            Native
        End Enum

        Public Delegate Sub DropDownItemHandler(sender As Object, e As DropDownItemEventArgs)
        ''' <summary>
        ''' Occurs when the button is clicked.
        ''' </summary>
        Public Event ClickButton As EventHandler
        ''' <summary>
        ''' Occurs when the drop down button is clicked, opening the drop down menu.
        ''' </summary>
        Public Event DropDownClicked As EventHandler
        ''' <summary>
        ''' Occurs when a menu item in the drop down menu is clicked.
        ''' </summary>
        Public Event DropDownItemClicked As DropDownItemHandler

        Public Sub New()
            SetStyle(ControlStyles.ResizeRedraw, True)
            DoubleBuffered = True
            Size = New Size(142, 23)
            AddHandler TextChanged, AddressOf DropDownButton_TextChanged
        End Sub

#Region "Events"

        Private Sub DropDownMenu_ItemAdded(sender As Object, e As ToolStripItemEventArgs)
            If TryCast(sender, DropDownMenu).DropDownButton Is Me Then Invalidate()
        End Sub

        Private Sub DropDownMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
            If TryCast(sender, DropDownMenu).DropDownButton Is Me Then
                p_DropDownSelectedItem = p_DropDownMenu.Items.IndexOf(e.ClickedItem)
                TryCast(sender, DropDownMenu).Close(ToolStripDropDownCloseReason.ItemClicked)
                Invalidate()
                OnDropDownItemClicked(New DropDownItemEventArgs(e.ClickedItem, p_DropDownSelectedItem))
            End If
        End Sub

        Private Sub DropDownMenu_ItemRemoved(sender As Object, e As ToolStripItemEventArgs)
            If TryCast(sender, DropDownMenu).DropDownButton Is Me Then
                Invalidate()
            End If
        End Sub

        Private Sub DropDownButton_TextChanged(sender As Object, e As EventArgs)
            Invalidate()
        End Sub

#End Region

#Region "Fields"

        Private p_DropDownMenu As DropDownMenu
        Private p_DropDownSelectedItem As Integer
        Private p_Renderer As Renderers = Renderers.Default
        Private dropDownRect As Rectangle
        Private pushedState As Integer = 0
        Private buttonState As PushButtonState = PushButtonState.Normal
        Private dropDownState As ComboBoxState = ComboBoxState.Normal

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the DropDownMenu to display.
        ''' </summary>
        <DefaultValue(DirectCast(Nothing, String))>
        Public Property DropDownMenu As DropDownMenu
            Get
                Return p_DropDownMenu
            End Get
            Set(value As DropDownMenu)

                If p_DropDownMenu IsNot Nothing Then
                    p_DropDownMenu.DropDownButton = Nothing
                    p_DropDownMenu.Renderer = Nothing
                    RemoveHandler p_DropDownMenu.ItemAdded, AddressOf DropDownMenu_ItemAdded
                    RemoveHandler p_DropDownMenu.ItemClicked, AddressOf DropDownMenu_ItemClicked
                    RemoveHandler p_DropDownMenu.ItemRemoved, AddressOf DropDownMenu_ItemRemoved
                End If

                p_DropDownMenu = value

                If p_DropDownMenu IsNot Nothing Then
                    p_DropDownMenu.DropDownButton = Me
                    p_DropDownMenu.Renderer = If(p_Renderer.Equals(Renderers.Default), Nothing, New NativeToolStripRenderer(New ToolbarTheme()))
                    AddHandler p_DropDownMenu.ItemAdded, AddressOf DropDownMenu_ItemAdded
                    AddHandler p_DropDownMenu.ItemClicked, AddressOf DropDownMenu_ItemClicked
                    AddHandler p_DropDownMenu.ItemRemoved, AddressOf DropDownMenu_ItemRemoved
                End If

                p_DropDownSelectedItem = 0
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the DropDownMenu selected item. (Last clicked item)
        ''' </summary>
        <DefaultValue(0)>
        Public Property DropDownSelectedItem As Integer
            Get
                Return p_DropDownSelectedItem
            End Get
            Set(value As Integer)
                p_DropDownSelectedItem = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the DropDownMenu renderer.
        ''' </summary>
        <DefaultValue(Renderers.Default)>
        Public Property Renderer As Renderers
            Get
                Return p_Renderer
            End Get
            Set(value As Renderers)

                If p_DropDownMenu IsNot Nothing Then
                    p_DropDownMenu.Renderer = If(value = Renderers.Default, Nothing, New NativeToolStripRenderer(New ToolbarTheme()))
                End If

                p_Renderer = value
            End Set
        End Property

#End Region

        Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
            If keyData.Equals(Keys.Down) Then
                Return True
            Else
                Return MyBase.IsInputKey(keyData)
            End If
        End Function

        Protected Overrides Sub OnClick(e As EventArgs)
            ' The actual drop down button has it's own event.
            If dropDownState = ComboBoxState.Pressed Then Return
            RaiseEvent ClickButton(Me, EventArgs.Empty)

            If p_DropDownMenu Is Nothing OrElse p_DropDownMenu.Items.Count = 0 OrElse Not Equals(Text, "") Then
                Return
            End If

            If ShowFocusCues Then Focus()
            p_DropDownMenu.DropDownButton = Me
            p_DropDownMenu.Items(p_DropDownSelectedItem).PerformClick()
        End Sub

        Protected Overridable Sub OnDropDownClicked(e As EventArgs)
            RaiseEvent DropDownClicked(Me, e)
        End Sub

        Protected Overridable Sub OnDropDownItemClicked(e As DropDownItemEventArgs)
            RaiseEvent DropDownItemClicked(Me, e)
        End Sub

        Protected Overrides Sub OnGotFocus(e As EventArgs)
            Invalidate()
            MyBase.OnGotFocus(e)
        End Sub

        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            If e.KeyCode = Keys.Down Then
                ShowContextMenuStrip(True)
            ElseIf e.KeyCode = Keys.Enter Then

                If Equals(Text, "") Then
                    OnClick(EventArgs.Empty)
                Else
                    ShowContextMenuStrip(True)
                End If
            End If

            MyBase.OnKeyDown(e)
        End Sub

        Protected Overrides Sub OnLostFocus(e As EventArgs)
            Invalidate()
            MyBase.OnLostFocus(e)
        End Sub

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            If e.Button = MouseButtons.Left Then
                If dropDownRect.Contains(e.Location) Then
                    buttonState = PushButtonState.Normal
                    dropDownState = ComboBoxState.Pressed
                    pushedState = 1
                ElseIf DisplayRectangle.Contains(e.Location) Then
                    buttonState = PushButtonState.Pressed
                    dropDownState = ComboBoxState.Normal
                    pushedState = 2
                End If

                Invalidate()
            End If

            MyBase.OnMouseDown(e)
        End Sub

        Protected Overrides Sub OnMouseLeave(e As EventArgs)
            buttonState = PushButtonState.Normal
            dropDownState = ComboBoxState.Normal
            Invalidate()
            MyBase.OnMouseLeave(e)
        End Sub

        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
            If dropDownRect.Contains(e.Location) AndAlso pushedState <> 2 Then
                If Not (buttonState = PushButtonState.Normal AndAlso dropDownState = ComboBoxState.Hot) Then
                    buttonState = PushButtonState.Normal
                    dropDownState = ComboBoxState.Hot
                    Invalidate()
                End If
            ElseIf DisplayRectangle.Contains(e.Location) AndAlso pushedState <> 1 Then

                If Not (buttonState = PushButtonState.Hot AndAlso dropDownState = ComboBoxState.Normal) Then
                    buttonState = PushButtonState.Hot
                    dropDownState = ComboBoxState.Normal
                    Invalidate()
                End If
            End If

            MyBase.OnMouseMove(e)
        End Sub

        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            If dropDownRect.Contains(e.Location) Then
                If pushedState = 1 Then ShowContextMenuStrip()
                buttonState = PushButtonState.Normal
                dropDownState = ComboBoxState.Hot
            ElseIf DisplayRectangle.Contains(e.Location) Then
                buttonState = PushButtonState.Hot
                dropDownState = ComboBoxState.Normal
            Else
                buttonState = PushButtonState.Normal
                dropDownState = ComboBoxState.Normal
            End If

            pushedState = 0
            Invalidate()
            MyBase.OnMouseUp(e)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            Dim text = String.Empty
            Dim rect = DisplayRectangle
            Dim rectText = DisplayRectangle
            Dim stringFormat As StringFormat = New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
            dropDownRect = New Rectangle(DisplayRectangle.Width - 20, DisplayRectangle.Y + 1, 20, DisplayRectangle.Height - 2)
            rectText.Width -= 20

            If Not Equals(Me.Text, String.Empty) Then
                text = Me.Text
                buttonState = PushButtonState.Normal
            Else

                If Not p_DropDownMenu Is Nothing AndAlso p_DropDownMenu.Items.Count > 0 Then
                    text = p_DropDownMenu.Items(p_DropDownSelectedItem).Text
                Else
                    text = Me.Text
                End If
            End If

            If Application.RenderWithVisualStyles Then
                DrawVisualStyle(e.Graphics, rect, dropDownRect)
            Else
                Dim btnState = Windows.Forms.ButtonState.Normal
                Dim ddbState = Windows.Forms.ButtonState.Normal

                Select Case buttonState
                    Case PushButtonState.Hot, PushButtonState.Normal
                        btnState = Windows.Forms.ButtonState.Normal
                    Case PushButtonState.Pressed
                        btnState = Windows.Forms.ButtonState.Pushed
                End Select

                Select Case dropDownState
                    Case ComboBoxState.Hot, ComboBoxState.Normal
                        ddbState = Windows.Forms.ButtonState.Normal
                    Case ComboBoxState.Pressed
                        ddbState = Windows.Forms.ButtonState.Pushed
                End Select

                DrawPreVistaStyle(e.Graphics, rect, btnState, dropDownRect, ddbState)
            End If

            e.Graphics.DrawString(text, Font, New SolidBrush(ForeColor), rectText, stringFormat)
            MyBase.OnPaint(e)
        End Sub

        Private Sub DrawVisualStyle(g As Graphics, rect As Rectangle, dropDownRect As Rectangle, Optional focused As Boolean = False)
            Dim focus = ShowFocusCues AndAlso Me.Focused

            If IsWinXP() Then
                Dim sf As StringFormat = New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                Dim rectNew = dropDownRect
                rectNew.Y -= 1
                rectNew.Height += 2
                Dim ddState As PushButtonState = dropDownState
                ButtonRenderer.DrawButton(g, rect, False, buttonState)
                ButtonRenderer.DrawButton(g, rectNew, focus, ddState Or If(focus AndAlso ddState = PushButtonState.Normal, PushButtonState.Default, 0))
                g.DrawString("v", Font, New SolidBrush(Color.Black), rectNew, sf)
            Else

                If Equals(Text, "") Then
                    ButtonRenderer.DrawButton(g, rect, focus, buttonState Or If(focus AndAlso buttonState = PushButtonState.Normal, PushButtonState.Default, 0))
                    ComboBoxRenderer.DrawDropDownButton(g, dropDownRect, dropDownState)
                Else
                    ButtonRenderer.DrawButton(g, rect, False, buttonState)
                    ComboBoxRenderer.DrawDropDownButton(g, dropDownRect, dropDownState Or If(focus, ComboBoxState.Hot, 0))

                    If focus Then
                        Dim focusBounds = dropDownRect
                        focusBounds.Inflate(-2, -2)
                        ControlPaint.DrawBorder(g, focusBounds, Color.Black, ButtonBorderStyle.Dotted)
                    End If
                End If
            End If
        End Sub

        Private Sub DrawPreVistaStyle(g As Graphics, rect As Rectangle, state As ButtonState, dropDownRect As Rectangle, dropDownState As ButtonState)
            Dim newDropDownRect = dropDownRect
            newDropDownRect.Y -= 2
            newDropDownRect.Height += 3
            ControlPaint.DrawButton(g, rect, state)
            ControlPaint.DrawComboButton(g, newDropDownRect, dropDownState)

            If ShowFocusCues AndAlso Focused Then
                Dim focusRect = newDropDownRect
                focusRect.Inflate(-3, -3)
                newDropDownRect.Y += 1
                newDropDownRect.Height -= 2
                focusRect.X -= 1
                focusRect.Width += 1
                ControlPaint.DrawBorder(g, focusRect, Color.Black, ButtonBorderStyle.Dotted)
                ControlPaint.DrawBorder(g, newDropDownRect, Color.Black, ButtonBorderStyle.Solid)
            End If
        End Sub

        ''' <summary>
        ''' Shows the DropDownButton at the button.
        ''' </summary>
        ''' <param name="selectFirstItem">Selects the first item so arrow keys can be used.</param>
        Public Sub ShowContextMenuStrip(Optional selectFirstItem As Boolean = False)
            If p_DropDownMenu Is Nothing Then Return
            Dim width = p_DropDownMenu.Width
            p_DropDownMenu.DropDownButton = Me
            p_DropDownMenu.Show(Me, DisplayRectangle.Width - width, DisplayRectangle.Y + Height - 1)
            If selectFirstItem Then p_DropDownMenu.Items(0).Select()
        End Sub

        Private Function IsWinXP() As Boolean
            Return Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso Environment.OSVersion.Version.Major = 5 AndAlso Environment.OSVersion.Version.Minor = 1
        End Function
    End Class

    Public Class DropDownItemEventArgs
        Inherits EventArgs
        ''' <summary>
        ''' Gets clicked ToolStripItem,
        ''' </summary>
        Public Property Item As ToolStripItem
        ''' <summary>
        ''' Gets the index of clicked item.
        ''' </summary>
        Public Property ItemIndex As Integer

        Public Sub New(item As ToolStripItem, index As Integer)
            Me.Item = item
            ItemIndex = index
        End Sub
    End Class

    Public Class DropDownMenu
        Inherits ContextMenuStrip
        ''' <summary>
        ''' Gets or sets which DropDownButton is currently interacting with the menu.
        ''' </summary>
        Public Property DropDownButton As DropDownButton
    End Class
End Namespace

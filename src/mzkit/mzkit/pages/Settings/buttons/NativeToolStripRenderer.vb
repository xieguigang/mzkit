#Region "Microsoft.VisualBasic::c0a03144fc37845c070faa25f0c6e199, mzkit\src\mzkit\mzkit\pages\Settings\buttons\NativeToolStripRenderer.vb"

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

    '   Total Lines: 451
    '    Code Lines: 324
    ' Comment Lines: 64
    '   Blank Lines: 63
    '     File Size: 20.61 KB


    '     Enum ToolbarTheme
    ' 
    '         BrowserTabBar, CommunicationsToolbar, HelpBar, MediaToolbar, Toolbar
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class NativeToolStripRenderer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Class NativeMethods
    ' 
    '             Function: GetThemeMargins
    '             Structure MARGINS
    ' 
    ' 
    ' 
    ' 
    ' 
    '         Enum MenuParts
    ' 
    ' 
    ' 
    ' 
    '         Enum MenuBarStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MenuBarItemStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MenuPopupItemStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MenuPopupCheckStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MenuPopupCheckBackgroundStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MenuPopupSubMenuStates
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '         Enum MarginTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: IsSupported, MenuClass, RebarClass, SubclassPrefix, Theme
    '                 ToolbarClass
    ' 
    '     Function: EnsureRenderer, GetBackgroundRectangle, GetItemState, GetItemTextColor, GetThemeMargins
    '               Subclass
    ' 
    '     Sub: Initialize, InitializePanel, OnRenderArrow, OnRenderImageMargin, OnRenderItemCheck
    '          OnRenderItemText, OnRenderMenuItemBackground, OnRenderOverflowButtonBackground, OnRenderSeparator, OnRenderSplitButtonBackground
    '          OnRenderToolStripBackground, OnRenderToolStripBorder, OnRenderToolStripPanelBackground
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

' Thanks for fixes:
'* 
'*  * Marco Minerva, jachymko - http://www.codeplex.com/windowsformsaero
'*  * Ben Ryves - http://www.benryves.com/
'*
'* ** Note for anyone considering using this: **
'*
'* A better alternative to using this class is to use the MainMenu and ContextMenu
'* controls instead of MenuStrip and ContextMenuStrip, as they provide true native
'* rendering. If you require icons, try this:
'*
'* http://wyday.com/blog/2009/making-the-menus-in-your-net-app-look-professional/
'* 
'* Note from AlexxEG:
'* - I don't remeber where I got this from, but it was originally called
'*   "ToolStripAeroRenderer".
'*   
'*   Also, namespace was originally "Szotar.WindowsForms".


Namespace UnFound
    Friend Enum ToolbarTheme
        Toolbar
        MediaToolbar
        CommunicationsToolbar
        BrowserTabBar
        HelpBar
    End Enum

    ''' <summary>Renders a toolstrip using the UxTheme API via VisualStyleRenderer and a specific style.</summary>
    ''' <remarks>Perhaps surprisingly, this does not need to be disposable.</remarks>
    Friend Class NativeToolStripRenderer
        Inherits ToolStripSystemRenderer

        Private renderer As VisualStyleRenderer

        Public Sub New(theme As ToolbarTheme)
            Me.Theme = theme
        End Sub

        ''' <summary>
        ''' It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins
        ''' misses out a parameter in its own P/Invoke.
        ''' </summary>
        NotInheritable Friend Class NativeMethods
            <StructLayout(LayoutKind.Sequential)>
            Public Structure MARGINS
                Public cxLeftWidth As Integer
                Public cxRightWidth As Integer
                Public cyTopHeight As Integer
                Public cyBottomHeight As Integer
            End Structure

            <DllImport("uxtheme.dll")>
            Public Shared Function GetThemeMargins(hTheme As IntPtr, hdc As IntPtr, iPartId As Integer, iStateId As Integer, iPropId As Integer, rect As IntPtr, <Out> ByRef pMargins As MARGINS) As Integer
            End Function
        End Class

        ' See http://msdn2.microsoft.com/en-us/library/bb773210.aspx - "Parts and States"
        ' Only menu-related parts/states are needed here, VisualStyleRenderer handles most of the rest.
        Friend Enum MenuParts As Integer
            ItemTMSchema = 1
            DropDownTMSchema = 2
            BarItemTMSchema = 3
            BarDropDownTMSchema = 4
            ChevronTMSchema = 5
            SeparatorTMSchema = 6
            BarBackground = 7
            BarItem = 8
            PopupBackground = 9
            PopupBorders = 10
            PopupCheck = 11
            PopupCheckBackground = 12
            PopupGutter = 13
            PopupItem = 14
            PopupSeparator = 15
            PopupSubmenu = 16
            SystemClose = 17
            SystemMaximize = 18
            SystemMinimize = 19
            SystemRestore = 20
        End Enum

        Friend Enum MenuBarStates As Integer
            Active = 1
            Inactive = 2
        End Enum

        Friend Enum MenuBarItemStates As Integer
            Normal = 1
            Hover = 2
            Pushed = 3
            Disabled = 4
            DisabledHover = 5
            DisabledPushed = 6
        End Enum

        Friend Enum MenuPopupItemStates As Integer
            Normal = 1
            Hover = 2
            Disabled = 3
            DisabledHover = 4
        End Enum

        Friend Enum MenuPopupCheckStates As Integer
            CheckmarkNormal = 1
            CheckmarkDisabled = 2
            BulletNormal = 3
            BulletDisabled = 4
        End Enum

        Friend Enum MenuPopupCheckBackgroundStates As Integer
            Disabled = 1
            Normal = 2
            Bitmap = 3
        End Enum

        Friend Enum MenuPopupSubMenuStates As Integer
            Normal = 1
            Disabled = 2
        End Enum

        Friend Enum MarginTypes As Integer
            Sizing = 3601
            Content = 3602
            Caption = 3603
        End Enum

        Private Shared ReadOnly RebarBackground As Integer = 6

        Private Function GetThemeMargins(dc As IDeviceContext, marginType As MarginTypes) As Padding
            Dim margins As NativeMethods.MARGINS

            Try
                Dim hDC As IntPtr = dc.GetHdc()
                If 0 = NativeMethods.GetThemeMargins(renderer.Handle, hDC, renderer.Part, renderer.State, marginType, IntPtr.Zero, margins) Then Return New Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight)
                Return New Padding(0)
            Finally
                dc.ReleaseHdc()
            End Try
        End Function

        Private Shared Function GetItemState(item As ToolStripItem) As Integer
            Dim hot = item.Selected

            If item.IsOnDropDown Then
                If item.Enabled Then Return If(hot, MenuPopupItemStates.Hover, MenuPopupItemStates.Normal)
                Return If(hot, MenuPopupItemStates.DisabledHover, MenuPopupItemStates.Disabled)
            Else
                If item.Pressed Then Return If(item.Enabled, MenuBarItemStates.Pushed, MenuBarItemStates.DisabledPushed)
                If item.Enabled Then Return If(hot, MenuBarItemStates.Hover, MenuBarItemStates.Normal)
                Return If(hot, MenuBarItemStates.DisabledHover, MenuBarItemStates.Disabled)
            End If
        End Function

        Public Property Theme As ToolbarTheme

        Private ReadOnly Property RebarClass As String
            Get
                Return SubclassPrefix & "Rebar"
            End Get
        End Property

        Private ReadOnly Property ToolbarClass As String
            Get
                Return SubclassPrefix & "ToolBar"
            End Get
        End Property

        Private ReadOnly Property MenuClass As String
            Get
                Return SubclassPrefix & "Menu"
            End Get
        End Property

        Private ReadOnly Property SubclassPrefix As String
            Get

                Select Case Theme
                    Case ToolbarTheme.MediaToolbar
                        Return "Media::"
                    Case ToolbarTheme.CommunicationsToolbar
                        Return "Communications::"
                    Case ToolbarTheme.BrowserTabBar
                        Return "BrowserTabBar::"
                    Case ToolbarTheme.HelpBar
                        Return "Help::"
                    Case Else
                        Return String.Empty
                End Select
            End Get
        End Property

        Private Function Subclass(element As VisualStyleElement) As VisualStyleElement
            Return VisualStyleElement.CreateElement(SubclassPrefix & element.ClassName, element.Part, element.State)
        End Function

        Private Function EnsureRenderer() As Boolean
            If Not IsSupported Then Return False
            If renderer Is Nothing Then renderer = New VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal)
            Return True
        End Function

        ' Gives parented ToolStrips a transparent background.
        Protected Overrides Sub Initialize(toolStrip As ToolStrip)
            If TypeOf toolStrip.Parent Is ToolStripPanel Then toolStrip.BackColor = Color.Transparent
            MyBase.Initialize(toolStrip)
        End Sub

        ' Using just ToolStripManager.Renderer without setting the Renderer individually per ToolStrip means
        ' that the ToolStrip is not passed to the Initialize method. ToolStripPanels, however, are. So we can 
        ' simply initialize it here too, and this should guarantee that the ToolStrip is initialized at least 
        ' once. Hopefully it isn't any more complicated than this.
        Protected Overrides Sub InitializePanel(toolStripPanel As ToolStripPanel)
            For Each control As Control In toolStripPanel.Controls
                If TypeOf control Is ToolStrip Then Initialize(CType(control, ToolStrip))
            Next

            MyBase.InitializePanel(toolStripPanel)
        End Sub

        Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)
            If EnsureRenderer() Then
                renderer.SetParameters(MenuClass, MenuParts.PopupBorders, 0)

                If e.ToolStrip.IsDropDown Then
                    Dim oldClip = e.Graphics.Clip

                    ' Tool strip borders are rendered *after* the content, for some reason.
                    ' So we have to exclude the inside of the popup otherwise we'll draw over it.
                    Dim insideRect = e.ToolStrip.ClientRectangle
                    insideRect.Inflate(-1, -1)
                    e.Graphics.ExcludeClip(insideRect)
                    renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds)

                    ' Restore the old clip in case the Graphics is used again (does that ever happen?)
                    e.Graphics.Clip = oldClip
                End If
            Else
                MyBase.OnRenderToolStripBorder(e)
            End If
        End Sub

        Private Function GetBackgroundRectangle(item As ToolStripItem) As Rectangle
            If Not item.IsOnDropDown Then Return New Rectangle(New Point(), item.Bounds.Size)

            ' For a drop-down menu item, the background rectangles of the items should be touching vertically.
            ' This ensures that's the case.
            Dim rect = item.Bounds

            ' The background rectangle should be inset two pixels horizontally (on both sides), but we have 
            ' to take into account the border.
            rect.X = item.ContentRectangle.X + 1
            rect.Width = item.ContentRectangle.Width - 1

            ' Make sure we're using all of the vertical space, so that the edges touch.
            rect.Y = 0
            Return rect
        End Function

        Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)
            If EnsureRenderer() Then
                Dim partID As Integer = If(e.Item.IsOnDropDown, MenuParts.PopupItem, MenuParts.BarItem)
                renderer.SetParameters(MenuClass, partID, GetItemState(e.Item))
                Dim bgRect = GetBackgroundRectangle(e.Item)
                renderer.DrawBackground(e.Graphics, bgRect, bgRect)
            Else
                MyBase.OnRenderMenuItemBackground(e)
            End If
        End Sub

        Protected Overrides Sub OnRenderToolStripPanelBackground(e As ToolStripPanelRenderEventArgs)
            If EnsureRenderer() Then
                ' Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to
                ' Rebar.Band.Normal)
                If VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RebarBackground, 0)) Then
                    renderer.SetParameters(RebarClass, RebarBackground, 0)
                Else
                    renderer.SetParameters(RebarClass, 0, 0)
                End If

                If renderer.IsBackgroundPartiallyTransparent() Then renderer.DrawParentBackground(e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel)
                renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle)
                e.Handled = True
            Else
                MyBase.OnRenderToolStripPanelBackground(e)
            End If
        End Sub

        ' Render the background of an actual menu bar, dropdown menu or toolbar.
        Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)
            If EnsureRenderer() Then
                If e.ToolStrip.IsDropDown Then
                    renderer.SetParameters(MenuClass, MenuParts.PopupBackground, 0)
                Else
                    ' It's a MenuStrip or a ToolStrip. If it's contained inside a larger panel, it should have a
                    ' transparent background, showing the panel's background.

                    If TypeOf e.ToolStrip.Parent Is ToolStripPanel Then
                        ' The background should be transparent, because the ToolStripPanel's background will be visible.
                        ' (Of course, we assume the ToolStripPanel is drawn using the same theme, but it's not my fault
                        ' if someone does that.)
                        Return
                    Else
                        ' A lone toolbar/menubar should act like it's inside a toolbox, I guess.
                        ' Maybe I should use the MenuClass in the case of a MenuStrip, although that would break
                        ' the other themes...
                        If VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RebarBackground, 0)) Then
                            renderer.SetParameters(RebarClass, RebarBackground, 0)
                        Else
                            renderer.SetParameters(RebarClass, 0, 0)
                        End If
                    End If
                End If

                If renderer.IsBackgroundPartiallyTransparent() Then renderer.DrawParentBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip)
                renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds)
            Else
                MyBase.OnRenderToolStripBackground(e)
            End If
        End Sub

        ' The only purpose of this override is to change the arrow colour.
        ' It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer.
        Protected Overrides Sub OnRenderSplitButtonBackground(e As ToolStripItemRenderEventArgs)
            If EnsureRenderer() Then
                Dim sb = CType(e.Item, ToolStripSplitButton)
                MyBase.OnRenderSplitButtonBackground(e)

                ' It doesn't matter what colour of arrow we tell it to draw. OnRenderArrow will compute it from the item anyway.
                OnRenderArrow(New ToolStripArrowRenderEventArgs(e.Graphics, sb, sb.DropDownButtonBounds, Color.Red, ArrowDirection.Down))
            Else
                MyBase.OnRenderSplitButtonBackground(e)
            End If
        End Sub

        Private Function GetItemTextColor(item As ToolStripItem) As Color
            Dim partId As Integer = If(item.IsOnDropDown, MenuParts.PopupItem, MenuParts.BarItem)
            renderer.SetParameters(MenuClass, partId, GetItemState(item))
            Return renderer.GetColor(ColorProperty.TextColor)
        End Function

        Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
            If EnsureRenderer() Then e.TextColor = GetItemTextColor(e.Item)
            MyBase.OnRenderItemText(e)
        End Sub

        Protected Overrides Sub OnRenderImageMargin(e As ToolStripRenderEventArgs)
            If EnsureRenderer() Then
                If e.ToolStrip.IsDropDown Then
                    renderer.SetParameters(MenuClass, MenuParts.PopupGutter, 0)
                    ' The AffectedBounds is usually too small, way too small to look right. Instead of using that,
                    ' use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge
                    ' based on whether or not it's RTL. (It doesn't need to be narrowed to an edge in LTR mode, but let's
                    ' do that anyway.)
                    ' Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text.
                    Dim margins = GetThemeMargins(e.Graphics, MarginTypes.Sizing)
                    Dim extraWidth = e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width - margins.Left - margins.Right - 1 - e.AffectedBounds.Width
                    Dim rect = e.AffectedBounds
                    rect.Y += 2
                    rect.Height -= 4
                    Dim sepWidth = renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width

                    If e.ToolStrip.RightToLeft = RightToLeft.Yes Then
                        rect = New Rectangle(rect.X - extraWidth, rect.Y, sepWidth, rect.Height)
                        rect.X += sepWidth
                    Else
                        rect = New Rectangle(rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height)
                    End If

                    renderer.DrawBackground(e.Graphics, rect)
                End If
            Else
                MyBase.OnRenderImageMargin(e)
            End If
        End Sub

        Protected Overrides Sub OnRenderSeparator(e As ToolStripSeparatorRenderEventArgs)
            If e.ToolStrip.IsDropDown AndAlso EnsureRenderer() Then
                renderer.SetParameters(MenuClass, MenuParts.PopupSeparator, 0)
                Dim rect As Rectangle = New Rectangle(e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height)
                renderer.DrawBackground(e.Graphics, rect, rect)
            Else
                MyBase.OnRenderSeparator(e)
            End If
        End Sub

        Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)
            If EnsureRenderer() Then
                Dim bgRect = GetBackgroundRectangle(e.Item)
                bgRect.Width = bgRect.Height

                ' Now, mirror its position if the menu item is RTL.
                If e.Item.RightToLeft = RightToLeft.Yes Then bgRect = New Rectangle(e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width, bgRect.Y, bgRect.Width, bgRect.Height)
                renderer.SetParameters(MenuClass, MenuParts.PopupCheckBackground, If(e.Item.Enabled, MenuPopupCheckBackgroundStates.Normal, MenuPopupCheckBackgroundStates.Disabled))
                renderer.DrawBackground(e.Graphics, bgRect)
                Dim checkRect = e.ImageRectangle
                checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2
                checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2

                ' I don't think ToolStrip even supports radio box items, so no need to render them.
                renderer.SetParameters(MenuClass, MenuParts.PopupCheck, If(e.Item.Enabled, MenuPopupCheckStates.CheckmarkNormal, MenuPopupCheckStates.CheckmarkDisabled))
                renderer.DrawBackground(e.Graphics, checkRect)
            Else
                MyBase.OnRenderItemCheck(e)
            End If
        End Sub

        Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
            ' The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions),
            ' but it will get the colour wrong in many cases. The text colour is probably the best colour to use.
            If EnsureRenderer() Then e.ArrowColor = GetItemTextColor(e.Item)
            MyBase.OnRenderArrow(e)
        End Sub

        Protected Overrides Sub OnRenderOverflowButtonBackground(e As ToolStripItemRenderEventArgs)
            If EnsureRenderer() Then
                ' BrowserTabBar::Rebar draws the chevron using the default background. Odd.
                Dim rebarClass = Me.RebarClass
                If Theme = ToolbarTheme.BrowserTabBar Then rebarClass = "Rebar"
                Dim state = VisualStyleElement.Rebar.Chevron.Normal.State

                If e.Item.Pressed Then
                    state = VisualStyleElement.Rebar.Chevron.Pressed.State
                ElseIf e.Item.Selected Then
                    state = VisualStyleElement.Rebar.Chevron.Hot.State
                End If

                renderer.SetParameters(rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state)
                renderer.DrawBackground(e.Graphics, New Rectangle(Point.Empty, e.Item.Size))
            Else
                MyBase.OnRenderOverflowButtonBackground(e)
            End If
        End Sub

        Public ReadOnly Property IsSupported As Boolean
            Get
                If Not VisualStyleRenderer.IsSupported Then Return False

                ' Needs a more robust check. It seems mono supports very different style sets.
                Return VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement("Menu", MenuParts.BarBackground, MenuBarStates.Active))
            End Get
        End Property
    End Class
End Namespace

﻿'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.
Public Class CmdSwap
    Inherits Command

    Public Property OnKick As String = "%u was swapped by %a."
    Public Property OnKickReason As String = "%u was swapped by %a for %r"
    Public Property OnSyntaxError As String = "Syntax: !swap <user> <reason>"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"
    'Public Property OnNoPermission As String = "You can't swap %p (no permission)."

    Sub New()
        Me.CommandAlias = "swap"
        Me.Permission = "swap"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length < 2 Then 'Kein Suchstring
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim affectedUser As User = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        If affectedUser Is Nothing Then 'Kein User gefunden
            Me.Say(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}))
            Return False
        End If

        If params.Length > 2 Then
            Dim reason As String = String.Join(" ", params, 2, params.Length - 2)
            Me.Say(Me.ParseTemplate(Me.OnKickReason, {affectedUser.UserName, player.UserName, reason}, {"u", "a", "r"}))
        Else
            Me.Say(Me.ParseTemplate(Me.OnKick, {affectedUser.UserName, player.UserName}, {"u", "a"}))
        End If

        Me.adminIface.RCClient.SendRaw("swap " & affectedUser.SlotId)
        Return True
    End Function
End Class
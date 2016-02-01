'This file is part of SWBF2 SADS-Administation Helper.
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
Public Class CmdTest
    Inherits Command

    Public Property OnTestRegistered As String = "You are User %u, Slot %s, User-ID: %i, Group: %g"
    Public Property OnTestPlayer As String = "You are Player %u, Slot %s. You are not registered."
    Public Property OnSyntaxError As String = "Syntax: #test [<user>]"
    Public Property OnInvalidPermissions As String = "You cannot test other players!"

    Sub New()
        Me.CommandAlias = "test"
        Me.Permission = "test"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length > 1 And Not Me.adminIface.PHandler.HasPermission(player, Me) Then
            Me.Pm(Me.OnInvalidPermissions, player)
            Return False
        End If

        If params.Length > 2 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim affectedUser As User
        If params.Length < 2 Then
            affectedUser = player
        Else
            affectedUser = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        End If
        Me.adminIface.SQL.GetUserDetails(affectedUser)
        If affectedUser.IsRegistered Then
            Me.Pm(Me.ParseTemplate(Me.OnTestRegistered,
           {affectedUser.UserName, affectedUser.SlotId.ToString(), affectedUser.UserId.ToString(), affectedUser.GroupName},
           {"u", "s", "i", "g"}), player)
        Else
            Me.Pm(Me.ParseTemplate(Me.OnTestPlayer,
           {affectedUser.UserName, affectedUser.SlotId.ToString()},
           {"u", "s"}), player)
        End If

        Return True
    End Function
End Class
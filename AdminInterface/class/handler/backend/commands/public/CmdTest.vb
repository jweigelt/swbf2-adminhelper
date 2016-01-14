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

    Sub New()
        Me.CommandAlias = "test"
        Me.Permission = "test"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.adminIface.SQL.GetUserDetails(player)
        If player.IsRegistered Then
            Me.Say(Me.ParseTemplate(Me.OnTestRegistered,
           {player.UserName, player.SlotId.ToString(), player.UserId.ToString(), player.GroupName},
           {"u", "s", "i", "g"}))
        Else
            Me.Say(Me.ParseTemplate(Me.OnTestPlayer,
           {player.UserName, player.SlotId.ToString()},
           {"u", "s"}))
        End If

        Return True
    End Function
End Class
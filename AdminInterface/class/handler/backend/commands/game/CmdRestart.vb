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
Public Class CmdRestart
    Inherits Command

    Public Property BroadcastMessage As String = "Map-reload forced by %u."
    Public Property DoBroadCast As Boolean = True

    Sub New()
        Me.CommandAlias = "restart"
        Me.Permission = "restart"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        If Me.DoBroadCast Then
            Me.Say(Me.ParseTemplate(BroadcastMessage, {player.UserName}, {"u"}))
        End If
        Me.adminIface.RCClient.SendRaw("restart")
        Return True
    End Function
End Class
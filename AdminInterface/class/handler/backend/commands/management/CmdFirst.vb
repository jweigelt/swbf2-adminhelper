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
Public Class CmdFirst
    Inherits Command

    Public Property OnSet As String = "Setting %u's usergroup to Admin. Have fun!"
    Public Property OnAlreadySet As String = "Command already executed. Truncate users-table to proceed."
    Public Property UserGroup As Int32 = 1

    Sub New()
        Me.CommandAlias = "adminpls"
        Me.Permission = "first"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        If Me.adminIface.SQL.FirstUser Then
            Me.adminIface.SQL.RegisterUser(player)
            Me.adminIface.SQL.GetUserDetails(player)
            Me.adminIface.SQL.PutGroup(player.UserId, Me.UserGroup)
            Me.Say(Me.ParseTemplate(OnSet, {player.UserName}, {"u"}))
            Return True
        Else
            Me.Say(OnAlreadySet)
            Return False
        End If
    End Function
End Class
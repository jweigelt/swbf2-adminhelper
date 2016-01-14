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
Public Class CmdPutGroup
    Inherits Command

    Public Property OnPut As String = "%u was put into user group %g."
    Public Property OnSyntaxError As String = "Syntax: !putgroup <user> <group>"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"
    Public Property OnNoGroupMatch As String = "No group matching %e could be found!"
    Public Property NoGroupAlias As String = "none"

    Sub New()
        Me.CommandAlias = "putgroup"
        Me.Permission = "putgroup"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length < 3 Then
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim groupId As Int32 = -1
        If NoGroupAlias <> params(2) Then
            groupId = Me.adminIface.SQL.GetGroupId(params(2))
            If groupId = -1 Then
                Me.Say(Me.ParseTemplate(Me.OnNoGroupMatch, {params(2)}, {"e"}))
                Return False
            End If
        End If

        Dim affectedUser As User = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        If affectedUser Is Nothing Then
            Me.Say(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}))
            Return False
        End If

        If Not affectedUser.IsRegistered Then
            Me.adminIface.SQL.RegisterUser(player)
        End If

        Me.adminIface.SQL.PutGroup(affectedUser.UserId, groupId)
        Me.Say(Me.ParseTemplate(Me.OnPut, {affectedUser.UserName, Replace(params(2), "/", String.Empty)}, {"u", "g"}))
        Return True
    End Function
End Class
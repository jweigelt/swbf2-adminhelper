Public Class CmdBan
    Inherits Command

    Public Property OnBan As String = "%u was banned by %a."
    Public Property OnBanReason As String = "%u was banned by %a for %r"
    Public Property OnSyntaxError As String = "Syntax: !ban <user> <reason>"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"
    'Public Property OnNoPermission As String = "You can't ban %p (no permission)."

    Sub New()
        Me.CommandAlias = "ban"
        Me.Permission = "ban"
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
            Me.Say(Me.ParseTemplate(Me.OnBanReason, {affectedUser.UserName, player.UserName, reason}, {"u", "a", "r"}))
        Else
            Me.Say(Me.ParseTemplate(Me.OnBan, {affectedUser.UserName, player.UserName}, {"u", "a"}))
        End If

        Me.SubmitBan(affectedUser, player)
        Me.adminIface.RCClient.SendRaw("kick " & affectedUser.SlotId)

        Return True
    End Function

    Public Overridable Sub SubmitBan(ByVal affectedUser As User, ByVal player As User)
        Me.adminIface.MySQL.InsertBan(affectedUser, player, False)
    End Sub

End Class
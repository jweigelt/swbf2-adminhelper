Public Class CmdTempBan
    Inherits Command

    Public Property OnBan As String = "%u was banned(%t) by %a."
    Public Property OnBanReason As String = "%u was banned(%t) by %a for %r"
    Public Property OnSyntaxError As String = "Syntax: !tempban <user> <duration> <reason>"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"
    'Public Property OnNoPermission As String = "You can't ban %p (no permission)."

    Sub New()
        Me.CommandAlias = "tempban"
        Me.Permission = "tempban"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length < 3 Then 'Kein Suchstring
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim duration As Int32 = 0
        If Not Int32.TryParse(params(2), duration) Then
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim affectedUser As User = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        If affectedUser Is Nothing Then 'Kein User gefunden
            Me.Say(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}))
            Return False
        End If

        If params.Length > 3 Then
            Dim reason As String = String.Join(" ", params, 3, params.Length - 3)
            Me.Say(Me.ParseTemplate(Me.OnBanReason, {affectedUser.UserName, player.UserName, reason, duration.ToString()}, {"u", "a", "r", "t"}))
        Else
            Me.Say(Me.ParseTemplate(Me.OnBan, {affectedUser.UserName, player.UserName, duration.ToString()}, {"u", "a", "t"}))
        End If

        Me.SubmitBan(affectedUser, player, duration)
        Me.adminIface.RCClient.SendRaw("kick " & affectedUser.SlotId)
        Return True
    End Function

    Public Overridable Sub SubmitBan(ByVal affectedUser As User, ByVal player As User, ByVal duration As Int32)
        Me.adminIface.MySQL.InsertBan(affectedUser, player, False, duration)
    End Sub
End Class
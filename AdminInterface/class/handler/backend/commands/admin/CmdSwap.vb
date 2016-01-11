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
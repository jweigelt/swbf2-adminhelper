Public Class CmdUnban
    Inherits Command

    Public Property OnBan As String = "%u was unbanned by %a."
    Public Property OnSyntaxError As String = "Syntax: #unban <user>"
    Public Property OnNoPlayerMatch As String = "No player matching %e was banned."

    Sub New()
        Me.CommandAlias = "unban"
        Me.Permission = "ban"
        Me.IsPublic = False
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length <> 2 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim affectedUsername As String = Me.adminIface.SQL.UnbanPlayerByNameMatch(params(1))

        If Not affectedUsername Is Nothing Then
            Me.Pm(Me.ParseTemplate(Me.OnBan, {affectedUsername, player.UserName}, {"u", "a"}), player)
        Else
            Me.Pm(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}), player)
        End If

        Return True
    End Function

End Class

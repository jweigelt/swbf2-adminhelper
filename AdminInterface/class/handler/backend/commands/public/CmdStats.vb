Public Class CmdStats
    Inherits Command

    Public Property StatsStr As String = "Points: %p, Kills: %k, Deaths: %d, K/D: %r" '"You are User %u, Slot %s, User-ID: %i, Group: %g"

    Public Property OnSyntaxError As String = "Syntax: !stats [<user>]"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"


    Sub New()
        Me.CommandAlias = "stats"
        Me.Permission = "stats"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        If params.Length > 2 Then
            Me.Say(Me.OnSyntaxError)
            Return False
        End If

        Dim affectedUser As User
        If params.Length < 2 Then
            affectedUser = player
        Else
            affectedUser = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        End If

        If affectedUser Is Nothing Then
            Me.Say(Me.ParseTemplate(Me.OnNoPlayerMatch, {params(1)}, {"e"}))
            Return False
        End If

        Me.adminIface.SQL.GetUserDetails(affectedUser)
        Dim kdr As Double
        If affectedUser.Deaths = 0 Then
            kdr = 0.0
        Else
            kdr = Math.Round(affectedUser.Kills / affectedUser.Deaths, 2)
        End If

        Me.Pm(
            Me.ParseTemplate(Me.StatsStr,
                {affectedUser.Points, affectedUser.Kills, affectedUser.Deaths, kdr},
                {"p", "k", "d", "r"}
            ), player
        )

        Return True
    End Function
End Class

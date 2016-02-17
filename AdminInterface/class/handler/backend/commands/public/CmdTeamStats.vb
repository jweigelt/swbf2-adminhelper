Public Class CmdTeamStats
    Inherits Command

    Public Property StatsStr As String = "Points: %p, Kills: %k, Deaths: %d, K/D: %r"
    Public Property StatsStrPoints As String = "Team 1: %p, Team 2: %q"
    Public Property OnSyntaxError As String = "Syntax: #stats [-p] [<user>]"
    Public Property OnNoPlayerMatch As String = "No player matching %e could be found!"

    Sub New()
        Me.CommandAlias = "teamstats"
        Me.Permission = "stats"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Dim params() As String = Split(commandStr, " ")

        Dim team1 As Int32 = 0
        Dim team2 As Int32 = 0

        For Each p As User In Me.adminIface.PHandler.PlayerList
            Dim kills As Int32 = Me.adminIface.PHandler.PlayerPointsTracker(p.KeyHash)(1)
            If p.TeamName = "Her" Then
                team1 = team1 + kills
            Else
                team2 = team2 + kills
            End If
        Next

        Me.Pm(Me.ParseTemplate(Me.StatsStrPoints, {team1, team2}, {"p", "q"}), player)
        Return True
        If params.Length > 3 Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        End If

        Dim affectedUser As User

        Dim paramsList = params.ToList
        Dim usePoints As Boolean = paramsList.Contains("-p")

        If usePoints AndAlso params.Length > 2 AndAlso params(2) <> "-p" Then
            affectedUser = Me.adminIface.PHandler.FetchUserByNameMatch(params(2))
        ElseIf usePoints AndAlso params.Length > 2 AndAlso params(2) = "-p" Then
            Me.Pm(Me.OnSyntaxError, player)
            Return False
        ElseIf usePoints Then
            affectedUser = player
        ElseIf Not usePoints AndAlso params.Length > 1 Then
            affectedUser = Me.adminIface.PHandler.FetchUserByNameMatch(params(1))
        Else
            affectedUser = player
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
            If usePoints Then
                kdr = Math.Round(affectedUser.Points / 2 / affectedUser.Deaths, 2)
            Else
                kdr = Math.Round(affectedUser.Kills / affectedUser.Deaths, 2)
            End If
        End If

        If usePoints Then
            Me.Pm(
                Me.ParseTemplate(Me.StatsStrPoints,
                    {affectedUser.Points, Math.Round(affectedUser.Points / 2, 2), affectedUser.Deaths, kdr},
                    {"p", "k", "d", "r"}
                ), player
            )
        Else
            Me.Pm(
                Me.ParseTemplate(Me.StatsStr,
                    {affectedUser.Points, affectedUser.Kills, affectedUser.Deaths, kdr},
                    {"p", "k", "d", "r"}
                ), player
            )
        End If

        Return True
    End Function
End Class

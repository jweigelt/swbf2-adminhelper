Public Class CmdNextmap
    Inherits Command

    Public Property OnCommandOK As String = "Next map: %s"

    Sub New()
        Me.CommandAlias = "nextmap"
        Me.Permission = "nextmap"
        Me.IsPublic = True
    End Sub

    Public Overrides Function Execute(ByVal commandStr As String, ByVal player As User) As Boolean
        Me.Say(Me.ParseTemplate(Me.OnCommandOK, {Me.adminIface.Config.ServerInfo.NextMap}))
        Return True
    End Function
End Class
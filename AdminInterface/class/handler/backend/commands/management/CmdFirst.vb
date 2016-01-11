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
        If Me.adminIface.MySQL.FirstUser Then
            Me.adminIface.MySQL.RegisterUser(player)
            Me.adminIface.MySQL.GetUserDetails(player)
            Me.adminIface.MySQL.PutGroup(player.UserId, Me.UserGroup)
            Me.Say(Me.ParseTemplate(OnSet, {player.UserName}, {"u"}))
            Return True
        Else
            Me.Say(OnAlreadySet)
            Return False
        End If
    End Function
End Class
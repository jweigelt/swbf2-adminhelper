Public Class User
    Public Property SlotId As Int32 = 0
    Public Property UserName As String = String.Empty
    Public Property KeyHash As String = String.Empty
    Public Property IPAddress As Net.IPAddress
    Public Property TeamId As Byte
    Public Property Ping As Int32
    Public Property Points As Int32
    Public Property TeamName As String
    Public Property Kills As Int32
    Public Property Deaths As Int32
    Public Property IsBanned As Boolean

    Public Property IsRegistered As Boolean = False
    Public Property GroupId As Int32 = -1
    Public Property GroupName As String = "-"
    Public Property UserId As Int32 = -1
    Public Property playerId As Int32 = -1
End Class

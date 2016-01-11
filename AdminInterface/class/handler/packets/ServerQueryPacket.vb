Public Class ServerQueryPacket
    Inherits TcpPacket

    Public Property ServerInfo As ServerInformation

    Sub New()
        Me.CommandAlias = "status"
    End Sub

    Public Overrides Sub FetchData(ByVal data As String)
        Me.ServerInfo = New ServerInformation()
        Dim rows() As String = Split(data, vbCrLf)
        With Me.ServerInfo
            .ServerName = FetchValue(rows, 0)
            .ServerIP = FetchValue(rows, 1)
            .Version = FetchValue(rows, 2)
            .MaxPlayers = FetchValue(rows, 3)
            .Password = FetchValue(rows, 4)
            .CurrentMap = FetchValue(rows, 5)
            .NextMap = FetchValue(rows, 6)
            .GameMode = FetchValue(rows, 7)
            .Players = FetchValue(rows, 8)
            .Scores = FetchValue(rows, 9)
            .Tickets = FetchValue(rows, 10)
            .FFEnabled = FetchValue(rows, 11)
            .Heroes = FetchValue(rows, 12)
        End With
        Me.PacketOK = True
    End Sub

    Private Function FetchValue(ByVal rows() As String, ByVal index As Int32) As String
        Dim str() As String = Split(rows(index), ": ")
        Return str(1)
    End Function

End Class
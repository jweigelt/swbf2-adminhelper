Public Enum LogLevel
    always = -1
    debug = 0
    info = 1
    warning = 2
    failure = 3
    critical = 4
End Enum

'Just a simple static Logger
Public Class Logger
    Public Shared Property MinLevel As Int32 = 1
    Public Shared Property LogToFile As Boolean = False
    Public Shared Property LogFile As String = CurDir() & "/log.txt"
    Public Shared Property ExitOnError As Boolean = False

    Public Shared Sub Log(ByVal message As String, ByVal lvl As LogLevel, ParamArray params As String())
        If Not params Is Nothing Then
            For Each str As String In params
                If message.Contains("%s") Then
                    message = Replace(message, "%s", str, 1, 1)
                End If
            Next
        End If

        Select Case lvl
            Case LogLevel.debug
                message = "DEBUG | " & message
            Case LogLevel.info
                message = "INFO  | " & message
            Case LogLevel.warning
                message = "WARN  | " & message
            Case LogLevel.failure
                message = "FAIL  | " & message
            Case LogLevel.critical
                message = "CRIT  | " & message
        End Select

        message = "[" & Now.ToString & "] " & message

        If lvl >= MinLevel Or lvl = LogLevel.always Then
            Console.WriteLine(message)
            If LogToFile Then
                IO.File.AppendAllText(LogFile, message & vbCrLf)
            End If
        End If

        If lvl = LogLevel.critical And ExitOnError Then
            End
        End If
    End Sub

End Class
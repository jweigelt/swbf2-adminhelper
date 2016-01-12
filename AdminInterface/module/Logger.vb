'This file is part of SWBF2 SADS-Administation Helper.
'
'SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SWBF2 SADS-Administation Helper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with SWBF2 SADS-Administation Helper.  If not, see <http://www.gnu.org/licenses/>.

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
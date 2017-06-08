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

Public Class RepeatingSchedulerTask
    Inherits SchedulerTask
    Private lastExecution As Int64
    Private startTime As DateTime
    Public Property Interval As Int32 = 1000

    Sub New(ByVal delegateTask As funcDelegate, Optional ByVal param As Object = Nothing)
        MyBase.New(delegateTask, param)
        startTime = New DateTime(1970, 1, 1)
    End Sub

    Public Sub Tick()
        If (Me.lastExecution < Me.GetMillis() - Me.Interval) Then
            Me.lastExecution = Me.GetMillis()
            MyBase.Run()
        End If
    End Sub
    Private Function GetMillis() As Int64

        Return (DateTime.Now - startTime).TotalMilliseconds
    End Function
End Class
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

Public Class SchedulerTask
    Public Delegate Sub funcDelegate(ByVal obj As Object) 'Default Delegate
    Private delegateTask As funcDelegate
    Public Property param As Object

    Public Sub SetTask(ByVal delegateTask As funcDelegate)
        Me.delegateTask = delegateTask
    End Sub
    Public Overridable Sub Run()
        If Not IsNothing(delegateTask) Then
            delegateTask.Invoke(param)
        End If
    End Sub
    Sub New(ByVal delegateTask As funcDelegate, Optional ByVal param As Object = Nothing)
        If Not IsNothing(param) Then Me.param = param
        Me.delegateTask = delegateTask
    End Sub
End Class

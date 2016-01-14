﻿'This file is part of SWBF2 SADS-Administation Helper.
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

Public Class LogTemplate
    Public Const CORE_PREINIT = "[CORE] Loading components..."
    Public Const CORE_INIT = "[CORE] Starting main thread..."
    Public Const CORE_POSTINIT = "[CORE]Releasing memory..."
    Public Const CORE_SHUTDOWN = "[CORE] Shutting down..."
    Public Const CORE_QUERY_INFO = "[CORE] Requesting server details..."
    Public Const CORE_QUERY_INFO_OK = "[CORE] Found server '%s', v%s, %s slots"
    Public Const CORE_IDLE = "[CORE] Start OK. Client is up."

    Public Const CORE_UPDATE_CHECK = "[CORE] Checking for updates."
    Public Const CORE_UPDATE_FAIL = "[CORE] Update failed [%s]"
    Public Const CORE_UPDATE_PULL = "[CORE] Downloading Update [%s]"
    Public Const CORE_UPDATE_DONE = "[CORE] Update done, restarting..."
    Public Const CORE_UPDATE_NO = "[CORE] Version is up to date."

    Public Const CFG_READ_ERROR = "[XMLS] Couldn't parse %s, please check syntax and file-permissions!"
    Public Const CFG_WRITE_ERROR = "[XMLS] Couldn't write to %s, please check file-permissions."
    Public Const CFG_WRITE_DEFAULT = "[XMLS] Writing default config '%s'"

    Public Const RCON_SENDING = "[RCC] Sending command %s"
    Public Const RCON_SENDING_OK = "[RCC] recv ACK"
    Public Const RCON_SENDING_FAIL = "[RCC] Network error: push failed."
    Public Const RCON_LOGIN = "[RCC] Sending login..."
    Public Const RCON_LOGIN_OK = "[RCC] Login ack'd"
    Public Const RCON_LOGIN_FAIL = "[RCC] Login failed!"
    Public Const RCON_CHAT = "[RCC] #%s: %s"
    Public Const RCON_ANSWER = "[RCC] %s"
    Public Const RCON_DISCON = "[RCC] Rcon connection lost."
    Public Const RCON_INIT = "[RCC] Connecting to %s:%s"
    Public Const RCON_INIT_OK = "[RCC] Connected."
    Public Const RCON_INIT_FAIL = "[RCC] Connection failed!"
    Public Const RCON_CLOSE = "[RCC] Connection closed."
    Public Const RCON_PACKET_TIMEOUT = "[RCC] Dropping last packet. (timeout)"

    Public Const PROC_NO_APP = "[MEMR] File '%s' does not exist!"
    Public Const PROC_NO_PROC = "[MEMR] No Process found."
    Public Const PROC_FOUND = "[MEMR] Found process: %s"
    Public Const PROC_OPEN = "[MEMR] Opening process..."
    Public Const PROC_OPENPROC_FAIL = "[MEMR] Couldn't open process (permissions ok?)."
    Public Const PROC_OPENPROC_OK = "[MEMR] Process opened."
    Public Const PROC_CLOSE = "[MEMR] Process closed."

    Public Const PROC_FETCH_PWD = "[MEMR] Fetched rcon password '%s'"
    Public Const PROC_SET_PWD = "[MEMR] No rcon password set, setting a temp one."
    Public Const PROC_RM_PWD = "[MEMR] Disabling ingame login."

    Public Const SQL_TYPE = "[SQL] SQL Type: %s"
    Public Const SQL_CONNECT_TEST = "[SQL] Testing SQL-connection"
    Public Const SQL_CONNECT_OK = "[SQL] SQL OK"
    Public Const SQL_CONNECT_FAIL = "[SQL] Couldn't connect to database"
    Public Const SQL_CLOSE = "[SQL] Closing MySQL-connection"
    Public Const SQL_CLEANUP = "[SQL] Cleaning database..."

    Public Const CMD_EXECUTED = "[CMH] '%s' issued command '%s'"
    Public Const CMD_NO_PERMISSION = "[CMH] '%s' tried to issue command '%s' "
    Public Const CMD_DYN_CUT = "[CMH] Trimming '%s' :: message is to long (max. 100 chars / line!)"
    Public Const CMD_DYN_REG = "[CMH] Registering user-defined command '%s'"

    Public Const SHEDULER_EXCEPTION = "[SYS] SynSheduler thread error: %s"

    Public Const ANNOUNCE_SKIP = "[AAH] Skipping autoannounce - no players online"
End Class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    /// <summary>
    /// 命令代碼，用于授權
    /// </summary>
    public static class CommandCode
    {
        public const string Read = "Read";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Import = "Import";
        public const string Export = "Export";
        public const string Approve = "Approve";
        public const string Disapprove = "Disapprove";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FashionStore.Areas.Admin.Models;

namespace FashionStore.Areas.Admin.Models
{
    public class Permission
    {
        public static List<string> GetPermission(int groupID)
        {
            dbAdminDataContext db = new dbAdminDataContext();
            var dbpermissions = from u in db.user_groups
                                from r in db.roles
                                from p in db.permissions
                                where u.user_group_id == p.user_group_id
                                && u.user_group_id == groupID && r.role_id == p.role_id
                                select r;
            var permissions = new List<string>();
            foreach (var permission in dbpermissions)
            {
                permissions.Add(permission.role_id);
            }
            return permissions;
        }

    }
}
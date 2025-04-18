﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonheur.BusinessObjects.Models
{
    public class ApplicationPermission(string name, string value, string groupName, string? description = null)
    {
        public string Name { get; set; } = name;
        public string Value { get; set; } = value;
        public string GroupName { get; set; } = groupName;
        public string? Description { get; set; } = description;

        public override string ToString() => Value;

        [return: NotNullIfNotNull(nameof(permission))]
        public static implicit operator string?(ApplicationPermission? permission)
        {
            return permission?.Value;
        }
    }

    public static class ApplicationPermissions
    {
        /************* USER PERMISSIONS *************/

        public const string UsersPermissionGroupName = "User Permissions";

        public static readonly ApplicationPermission ViewUsers = new(
            "View Users",
            "users.view",
            UsersPermissionGroupName,
            "Permission to view other users account details");

        public static readonly ApplicationPermission ManageUsers = new(
            "Manage Users",
            "users.manage",
            UsersPermissionGroupName,
            "Permission to create, delete and modify other users account details");

        /************* ROLE PERMISSIONS *************/

        public const string RolesPermissionGroupName = "Role Permissions";

        public static readonly ApplicationPermission ViewRoles = new(
            "View Roles",
            "roles.view",
            RolesPermissionGroupName,
            "Permission to view available roles");

        public static readonly ApplicationPermission ManageRoles = new(
            "Manage Roles",
            "roles.manage",
            RolesPermissionGroupName,
            "Permission to create, delete and modify roles");

        public static readonly ApplicationPermission AssignRoles = new(
            "Assign Roles",
            "roles.assign",
            RolesPermissionGroupName,
            "Permission to assign roles to users");

        /************* ALL PERMISSIONS *************/

        public static readonly ReadOnlyCollection<ApplicationPermission> AllPermissions =
            new List<ApplicationPermission> {
                ViewUsers, ManageUsers,
                ViewRoles, ManageRoles, AssignRoles
            }.AsReadOnly();

        /************* HELPER METHODS *************/

        public static ApplicationPermission? GetPermissionByName(string? permissionName)
        {
            return AllPermissions.SingleOrDefault(p => p.Name == permissionName);
        }

        public static ApplicationPermission? GetPermissionByValue(string? permissionValue)
        {
            return AllPermissions.SingleOrDefault(p => p.Value == permissionValue);
        }

        public static string[] GetAllPermissionValues()
        {
            return AllPermissions.Select(p => p.Value).ToArray();
        }

        public static string[] GetAdministrativePermissionValues()
        {
            return [ManageUsers, ManageRoles, AssignRoles];
        }
    }
}

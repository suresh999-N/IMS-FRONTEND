import { useEffect, useState } from "react";
import { notifyRolesUpdated } from "../../api/rolesApi";
import { PERMISSION_OPTIONS } from "../../utils/permissions";
import "./ManagePermissions.css";

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || '/api').replace(/\/+$/, '');

const PERMISSION_ROLES_API = `${API_BASE_URL}/Permissions/roles`;
const PERMISSION_ROLE_API = `${API_BASE_URL}/Permissions/role`;
const PERMISSION_UPDATE_API = `${API_BASE_URL}/Permissions/update`;
const PERMISSION_RESET_API = `${API_BASE_URL}/Permissions/reset`;

function getHeaders() {
  const rawToken = localStorage.getItem("ims-auth-token");
  let token = "";
  if (rawToken) {
    try {
      const parsed = JSON.parse(rawToken);
      token = typeof parsed === "string" ? parsed : rawToken;
    } catch {
      token = rawToken;
    }
  }
  return {
    "Content-Type": "application/json",
    "ngrok-skip-browser-warning": "true",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

const ACTIONS = ["View", "Add", "Edit", "Delete"];

const permissionText = {
  english: {
    title: "Manage Permissions",
    subtitle: "Control module access for Admin, Manager and Staff roles",
    userRoles: "User Roles",
    admin: "Admin",
    adminDesc: "Full access to all IMS modules and settings",
    manager: "Manager",
    managerDesc: "Operational access for stock, orders, purchases and reports",
    staff: "Staff",
    staffDesc: "Limited access for daily inventory and order tasks",
    permissions: "Permissions",
    desc: "Select what this role can view, add, edit or delete inside IMS.",
    module: "IMS Module",
    view: "View",
    add: "Add",
    edit: "Edit",
    delete: "Delete",
    noteTitle: "Real project behavior:",
    note:
      "These permissions can hide sidebar menus, restrict buttons like Add/Edit/Delete, and show Access Denied for unauthorized pages.",
    cancel: "Cancel",
    save: "Save Permissions",
    saved: "permissions saved successfully",
    loading: "Loading permissions...",
    saving: "Saving...",
    error: "Unable to load permissions.",
    saveError: "Unable to save permissions.",
  },
  telugu: {
    title: "పర్మిషన్స్ నిర్వహించండి",
    subtitle: "అడ్మిన్, మేనేజర్ మరియు స్టాఫ్ రోల్స్‌కు మాడ్యూల్ యాక్సెస్ నియంత్రించండి",
    userRoles: "యూజర్ రోల్స్",
    admin: "అడ్మిన్",
    adminDesc: "అన్ని IMS మాడ్యూల్స్ మరియు సెట్టింగ్స్‌కు పూర్తి యాక్సెస్",
    manager: "మేనేజర్",
    managerDesc: "స్టాక్, ఆర్డర్స్, పర్చేస్ మరియు రిపోర్ట్స్ యాక్సెస్",
    staff: "స్టాఫ్",
    staffDesc: "రోజువారీ ఇన్వెంటరీ మరియు ఆర్డర్ పనులకు పరిమిత యాక్సెస్",
    permissions: "పర్మిషన్స్",
    desc: "ఈ రోల్ IMS లో ఏమి చూడాలి, యాడ్ చేయాలి, ఎడిట్ చేయాలి, డిలీట్ చేయాలి ఎంచుకోండి.",
    module: "IMS మాడ్యూల్",
    view: "చూడండి",
    add: "యాడ్",
    edit: "ఎడిట్",
    delete: "డిలీట్",
    noteTitle: "రియల్ ప్రాజెక్ట్ బిహేవియర్:",
    note:
      "ఈ పర్మిషన్స్ సైడ్‌బార్ మెనూలను దాచవచ్చు, Add/Edit/Delete బటన్లను నియంత్రించవచ్చు, మరియు unauthorized pages కి Access Denied చూపవచ్చు.",
    cancel: "రద్దు",
    save: "పర్మిషన్స్ సేవ్ చేయండి",
    saved: "పర్మిషన్స్ విజయవంతంగా సేవ్ అయ్యాయి",
    loading: "పర్మిషన్స్ లోడ్ అవుతున్నాయి...",
    saving: "సేవ్ అవుతోంది...",
    error: "పర్మిషన్స్ లోడ్ కాలేదు.",
    saveError: "పర్మిషన్స్ సేవ్ కాలేదు.",
  },
  hindi: {
    title: "Manage Permissions",
    subtitle: "Admin, Manager और Staff roles का module access control करें",
    userRoles: "User Roles",
    admin: "Admin",
    adminDesc: "सभी IMS modules और settings का full access",
    manager: "Manager",
    managerDesc: "Stock, orders, purchases और reports access",
    staff: "Staff",
    staffDesc: "Daily inventory और order tasks के लिए limited access",
    permissions: "Permissions",
    desc: "Select करें कि यह role IMS में क्या view, add, edit या delete कर सकता है.",
    module: "IMS Module",
    view: "View",
    add: "Add",
    edit: "Edit",
    delete: "Delete",
    noteTitle: "Real project behavior:",
    note:
      "These permissions sidebar menus hide कर सकते हैं, Add/Edit/Delete buttons restrict कर सकते हैं, और unauthorized pages पर Access Denied दिखा सकते हैं.",
    cancel: "Cancel",
    save: "Save Permissions",
    saved: "permissions saved successfully",
    loading: "Loading permissions...",
    saving: "Saving...",
    error: "Unable to load permissions.",
    saveError: "Unable to save permissions.",
  },
};

function normalizeRoleName(roleName) {
  const value = String(roleName || "").trim();
  const lower = value.toLowerCase();

  if (["admin", "administrator"].includes(lower)) return "Admin";
  if (lower === "manager") return "Manager";
  if (lower === "staff") return "Staff";

  if (value) {
    return value.charAt(0).toUpperCase() + value.slice(1);
  }

  return value;
}

function normalizeRolesResponse(data) {
  const list = Array.isArray(data)
    ? data
    : Array.isArray(data?.data)
      ? data.data
      : Array.isArray(data?.roles)
        ? data.roles
        : Array.isArray(data?.items)
          ? data.items
          : [];

  if (!list.length) {
    return [];
  }

  return list.map((role, index) => {
    const rawId = role.id ?? role.roleId ?? role.RoleId ?? role.permissionRoleId ?? role.Id;
    const roleIdVal = Number(rawId) || rawId || (index + 1);
    const rawName = role.name ?? role.roleName ?? role.RoleName ?? role.role ?? role.title ?? role.displayName ?? '';
    const roleName = normalizeRoleName(rawName);

    return {
      id: roleIdVal,
      roleId: roleIdVal,
      name: roleName,
      labelKey: String(roleName).toLowerCase(),
      description:
        role.description ||
        role.roleDescription ||
        role.desc ||
        "Role access configuration",
    };
  });
}

function extractRawPermissionList(data) {
  if (Array.isArray(data)) return data;
  if (Array.isArray(data?.permissions)) return data.permissions;
  if (Array.isArray(data?.rolePermissions)) return data.rolePermissions;
  if (Array.isArray(data?.data?.permissions)) return data.data.permissions;
  if (Array.isArray(data?.data?.rolePermissions)) return data.data.rolePermissions;
  if (Array.isArray(data?.data)) return data.data;
  if (Array.isArray(data?.items)) return data.items;
  return [];
}

function normalizeRolePermissionResponse(data) {
  const rawList = extractRawPermissionList(data);
  const modules = {};

  rawList.forEach((item) => {
    const mod = item.module || item.Module || {};
    const moduleKey = String(
      item.moduleKey ?? item.ModuleKey ?? mod.moduleKey ?? mod.ModuleKey ?? ''
    ).trim();

    if (!moduleKey) return;

    const moduleName = String(
      item.moduleName ?? item.ModuleName ?? mod.moduleName ?? mod.ModuleName ?? moduleKey
    ).trim();

    const moduleDescription = String(
      item.description ?? item.Description ?? mod.description ?? mod.Description ?? ''
    ).trim();

    const category = String(
      item.category ?? item.Category ?? mod.category ?? mod.Category ?? ''
    ).trim();

    const displayOrder = Number(
      item.displayOrder ?? item.DisplayOrder ?? mod.displayOrder ?? mod.DisplayOrder ?? 999
    );

    modules[moduleKey] = {
      permissionId: Number(item.permissionId ?? item.PermissionId) || 0,
      moduleKey,
      moduleName,
      moduleDescription,
      category,
      displayOrder,
      View: Boolean(item.canView ?? item.CanView),
      Add: Boolean(item.canAdd ?? item.CanAdd),
      Edit: Boolean(item.canEdit ?? item.CanEdit),
      Delete: Boolean(item.canDelete ?? item.CanDelete),
    };
  });

  // Ensure all standard system modules exist in modules object so checkboxes are always present
  PERMISSION_OPTIONS.forEach((opt, index) => {
    if (!modules[opt.key]) {
      modules[opt.key] = {
        permissionId: 0,
        moduleKey: opt.key,
        moduleName: opt.label,
        moduleDescription: `${opt.label} management module`,
        category: 'IMS',
        displayOrder: index + 1,
        View: false,
        Add: false,
        Edit: false,
        Delete: false,
      };
    }
  });

  return modules;
}

async function getApiErrorMessage(response, fallbackMessage) {
  try {
    const data = await response.json();

    if (typeof data === "string") return data;

    const validationMessages = data?.errors
      ? Object.values(data.errors)
          .flat()
          .filter(Boolean)
          .join(" ")
      : "";

    return (
      validationMessages ||
      data?.message ||
      data?.error ||
      data?.title ||
      fallbackMessage
    );
  } catch {
    return fallbackMessage;
  }
}

function buildUpdatePayload(currentPermissions) {
  // UpdatePermissionDto requires ONLY: permissionId, canView, canAdd, canEdit, canDelete
  return Object.values(currentPermissions)
    .filter((permission) => Number(permission.permissionId) > 0)
    .map((permission) => ({
      permissionId: Number(permission.permissionId),
      canView: Boolean(permission.View),
      canAdd: Boolean(permission.Add),
      canEdit: Boolean(permission.Edit),
      canDelete: Boolean(permission.Delete),
    }));
}

function ManagePermissions({ settingsData, onClose }) {
  const [roles, setRoles] = useState([]);
  const [selectedRole, setSelectedRole] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [apiError, setApiError] = useState("");
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [search, setSearch] = useState("");

  const [permissions, setPermissions] = useState({});

  const lang = settingsData?.language || "english";
  const p = permissionText[lang] || permissionText.english;

  useEffect(() => {
    const handleEscape = (event) => {
      if (event.key === "Escape" && !saving) {
        onClose();
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [onClose, saving]);

  useEffect(() => {
    async function fetchRoles() {
      try {
        setLoading(true);
        setApiError("");

        const response = await fetch(PERMISSION_ROLES_API, {
          method: "GET",
          headers: getHeaders(),
        });

        if (!response.ok) {
          throw new Error(await getApiErrorMessage(response, p.error));
        }

        const data = await response.json();
        const mappedRoles = normalizeRolesResponse(data);

        if (!mappedRoles.length) {
          throw new Error("No roles were returned by the API.");
        }

        setRoles(mappedRoles);

        const currentUser = JSON.parse(localStorage.getItem("ims-current-user") || "null");
        const userRoleName = String(currentUser?.role || "").toLowerCase();

        const currentRole = mappedRoles.find(
          (role) => role.name.toLowerCase() === userRoleName
        ) || mappedRoles[0];

        setSelectedRole(currentRole.name);
      } catch (error) {
        console.error("Fetch roles API failed:", error);
        setRoles([]);
        setSelectedRole("");
        setApiError(error.message || p.error);
      } finally {
        setLoading(false);
      }
    }

    fetchRoles();
  }, [p.error]);

  useEffect(() => {
    async function fetchRolePermissions() {
      const selectedRoleData = roles.find((role) => role.name === selectedRole);

      if (!selectedRoleData?.id) return;

      try {
        setLoading(true);
        setApiError("");

        let response = await fetch(
          `${PERMISSION_ROLE_API}/${selectedRoleData.id}`,
          {
            method: "GET",
            headers: getHeaders(),
          }
        );

        if (!response.ok) {
          throw new Error(await getApiErrorMessage(response, p.error));
        }

        let data = await response.json();
        let rawList = extractRawPermissionList(data);

        // If newly created role has no permission rows in DB, attempt reset to initialize rows
        if (!rawList.length) {
          try {
            const resetResponse = await fetch(
              `${PERMISSION_RESET_API}/${selectedRoleData.id}`,
              {
                method: "POST",
                headers: getHeaders(),
              }
            );

            if (resetResponse.ok) {
              response = await fetch(
                `${PERMISSION_ROLE_API}/${selectedRoleData.id}`,
                {
                  method: "GET",
                  headers: getHeaders(),
                }
              );

              if (response.ok) {
                data = await response.json();
              }
            }
          } catch {
            // Ignore reset error and fallback gracefully
          }
        }

        const mappedPermissions = normalizeRolePermissionResponse(data);

        setPermissions((previousPermissions) => ({
          ...previousPermissions,
          [selectedRole]: mappedPermissions,
        }));
      } catch (error) {
        console.error("Fetch permissions API failed:", error);
        const fallbackPermissions = normalizeRolePermissionResponse([]);
        setPermissions((previousPermissions) => ({
          ...previousPermissions,
          [selectedRole]: fallbackPermissions,
        }));
        setApiError(error.message || p.error);
      } finally {
        setLoading(false);
      }
    }

    fetchRolePermissions();
  }, [selectedRole, roles, p.error]);

  const handlePermissionChange = (moduleName, actionName) => {
    if (saving) return;

    setPermissions((prevPermissions) => {
      const currentRolePerms = prevPermissions[selectedRole];
      const currentModulePerms = currentRolePerms?.[moduleName];
      if (!currentModulePerms) return prevPermissions;
      const newValue = !currentModulePerms[actionName];

      const nextModulePerms = {
        ...currentModulePerms,
        [actionName]: newValue,
      };

      if (actionName === 'View' && !newValue) {
        nextModulePerms.Add = false;
        nextModulePerms.Edit = false;
        nextModulePerms.Delete = false;
      } else if ((actionName === 'Add' || actionName === 'Edit' || actionName === 'Delete') && newValue) {
        nextModulePerms.View = true;
      }

      return {
        ...prevPermissions,
        [selectedRole]: {
          ...currentRolePerms,
          [moduleName]: nextModulePerms,
        },
      };
    });
  };

  const handleSavePermissions = async () => {
    try {
      setSaving(true);
      setSuccessMessage("");
      setApiError("");

      const selectedRoleData = roles.find((role) => role.name === selectedRole);
      let currentPermissions = permissions[selectedRole];

      if (!selectedRoleData?.id || !currentPermissions) {
        throw new Error("Select a role with permissions loaded from the API.");
      }

      // Check if any module permissions have permissionId = 0 (un-initialized in DB)
      const hasUninitializedPermissions = Object.values(currentPermissions).some(
        (perm) => Number(perm.permissionId) === 0
      );

      if (hasUninitializedPermissions) {
        try {
          const resetResponse = await fetch(
            `${PERMISSION_RESET_API}/${selectedRoleData.id}`,
            {
              method: "POST",
              headers: getHeaders(),
            }
          );

          if (resetResponse.ok) {
            const refreshResponse = await fetch(
              `${PERMISSION_ROLE_API}/${selectedRoleData.id}`,
              {
                method: "GET",
                headers: getHeaders(),
              }
            );

            if (refreshResponse.ok) {
              const refreshedData = await refreshResponse.json();
              const freshMap = normalizeRolePermissionResponse(refreshedData);

              // Merge user's checkbox state into the DB-assigned permissionId items
              const mergedPermissions = {};
              Object.keys(freshMap).forEach((moduleKey) => {
                const userState = currentPermissions[moduleKey] || {};
                const freshItem = freshMap[moduleKey];
                mergedPermissions[moduleKey] = {
                  ...freshItem,
                  View: userState.View !== undefined ? userState.View : freshItem.View,
                  Add: userState.Add !== undefined ? userState.Add : freshItem.Add,
                  Edit: userState.Edit !== undefined ? userState.Edit : freshItem.Edit,
                  Delete: userState.Delete !== undefined ? userState.Delete : freshItem.Delete,
                };
              });

              currentPermissions = mergedPermissions;
              setPermissions((prev) => ({
                ...prev,
                [selectedRole]: mergedPermissions,
              }));
            }
          }
        } catch (e) {
          console.warn("Permission reset before save notice:", e);
        }
      }

      const payload = buildUpdatePayload(currentPermissions);

      if (!payload.length) {
        throw new Error("No valid permission records to update.");
      }

      const response = await fetch(PERMISSION_UPDATE_API, {
        method: "PUT",
        headers: getHeaders(),
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error(await getApiErrorMessage(response, p.saveError));
      }

      const refreshResponse = await fetch(
        `${PERMISSION_ROLE_API}/${selectedRoleData.id}`,
        {
          method: "GET",
          headers: getHeaders(),
        }
      );

      if (refreshResponse.ok) {
        const refreshedData = await refreshResponse.json();
        const mappedPermissions = normalizeRolePermissionResponse(refreshedData);

        setPermissions((prev) => ({
          ...prev,
          [selectedRole]: mappedPermissions,
        }));
      }

      notifyRolesUpdated();

      setSuccessMessage(
        `${getRoleLabel(selectedRoleData)} ${p.saved}`
      );

      setTimeout(() => {
        setSuccessMessage("");
      }, 2500);
    } catch (error) {
      console.error("Save permissions error:", error);
      setSuccessMessage("");
      setApiError(error.message || p.saveError);
    } finally {
      setSaving(false);
    }
  };

  const roleLabels = {
    Admin: p.admin,
    Manager: p.manager,
    Staff: p.staff,
  };

  const getRoleLabel = (role) => {
    if (!role) return "";
    return roleLabels[role.name] ?? role.name;
  };

  const currentPermissions = permissions[selectedRole] ?? {};
  const selectedRoleData = roles.find((role) => role.name === selectedRole);

  return (
    <div className="permissions-overlay" onClick={!saving ? onClose : undefined}>
      <div
        className="permissions-modal"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="permissions-header">
          <div>
            <h2>{p.title}</h2>
          </div>

          <button
            className="permissions-close-btn"
            onClick={onClose}
            disabled={saving}
          >
            ×
          </button>
        </div>

        {successMessage && (
          <div className="permissions-success-message">{successMessage}</div>
        )}

        {apiError && (
          <div className="permissions-api-error page-error-banner" role="alert">{apiError}</div>
        )}

        <div className="permissions-body">
          <aside className="permissions-role-sidebar">
            <h3>{p.userRoles}</h3>

            {roles.map((role) => (
              <button
                key={role.id}
                className={selectedRole === role.name ? "active" : ""}
                onClick={() => setSelectedRole(role.name)}
                disabled={loading || saving}
              >
                <strong>{getRoleLabel(role)}</strong>
              </button>
            ))}
          </aside>

          <section className="permissions-content">
            <div className="permissions-content-header">
              <div>
                <h3>
                  {getRoleLabel(selectedRoleData)} {p.permissions}
                </h3>
              </div>

              <div className="permissions-search">
                <input
                  type="text"
                  placeholder="Search module..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                />
              </div>

              <span className="selected-role-badge">
                {getRoleLabel(selectedRoleData)}
              </span>
            </div>

            {loading && (
              <div className="permissions-loading-message">{p.loading}</div>
            )}

            {!loading && (
              <div className="permissions-table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>{p.module}</th>
                      <th>{p.view}</th>
                      <th>{p.add}</th>
                      <th>{p.edit}</th>
                      <th>{p.delete}</th>
                    </tr>
                  </thead>

                  <tbody>
                    {Object.entries(currentPermissions)
                      .filter(([moduleName, actions]) =>
                        (actions.moduleName || moduleName)
                          .toLowerCase()
                          .includes(search.toLowerCase())
                      )
                      .sort(([, a], [, b]) =>
                        (a.displayOrder ?? 999) -
                        (b.displayOrder ?? 999)
                      )
                      .map(([moduleName, actions]) => {
                        return (
                          <tr key={moduleName}>
                            <td className="module-info">
                              <div className="module-title">
                                {actions.moduleName || moduleName}
                              </div>

                              {actions.category && (
                                <div className="module-category">
                                  {actions.category}
                                </div>
                              )}

                              {actions.moduleDescription && (
                                <div className="module-description">
                                  {actions.moduleDescription}
                                </div>
                              )}
                            </td>

                            {ACTIONS.map((actionName) => (
                              <td key={actionName}>
                                <label className="permission-checkbox">
                                  <input
                                    type="checkbox"
                                    checked={Boolean(actions[actionName])}
                                    onChange={() =>
                                      handlePermissionChange(moduleName, actionName)
                                    }
                                    disabled={saving}
                                  />
                                  <span></span>
                                </label>
                              </td>
                            ))}
                          </tr>
                        );
                      })}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </div>

        <div className="permissions-footer">
          <button
            className="permissions-save-btn"
            onClick={handleSavePermissions}
            disabled={saving || loading || !selectedRoleData || !Object.keys(currentPermissions).length}
          >
            {saving ? p.saving : p.save}
          </button>

          <button
            className="permissions-cancel-btn"
            onClick={onClose}
            disabled={saving}
          >
            {p.cancel}
          </button>
        </div>
      </div>
    </div>
  );
}

export default ManagePermissions;

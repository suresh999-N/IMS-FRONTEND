import { useEffect, useRef, useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { notifyRolesUpdated } from "../../api/rolesApi";
import { createPortal } from "react-dom";
import "./AdminSettings.css";

import {
  Settings,
  Package,
  Users,
  Bell,
  ShieldCheck,
  Upload,
  X,
  Lock,
  History,
  PanelLeftClose,
  PanelLeftOpen,
} from "lucide-react";

import ChangePassword from "../ChangePassword/ChangePassword";
import LoginHistory from "../LoginHistory/LoginHistory";
import ManagePermissions from "../ManagePermissions/ManagePermissions";

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || '/api').replace(/\/+$/, '').replace(/\/api$/, '');
const FALLBACK_SETTINGS_ID = 1;

const SYSTEM_SETTINGS_API = `${API_BASE_URL}/api/SystemSettings`;
const ROLES_API = `${API_BASE_URL}/api/Roles`;

function cleanToken(token) {
  if (!token) return "";

  const value = String(token).trim();

  if (value.toLowerCase().startsWith("bearer ")) {
    return value.slice(7).trim();
  }

  return value;
}

function parseJson(value) {
  try {
    return value ? JSON.parse(value) : null;
  } catch {
    return null;
  }
}

function getStoredValue(storage, key) {
  try {
    return storage.getItem(key) || "";
  } catch {
    return "";
  }
}

function getTokenFromObject(value) {
  const parsedValue = parseJson(value);

  return cleanToken(
    parsedValue?.token ||
      parsedValue?.authToken ||
      parsedValue?.accessToken ||
      parsedValue?.jwtToken ||
      parsedValue?.bearerToken ||
      parsedValue?.data?.token ||
      parsedValue?.data?.authToken ||
      parsedValue?.data?.accessToken ||
      parsedValue?.data?.jwtToken ||
      parsedValue?.user?.token ||
      parsedValue?.user?.accessToken ||
      ""
  );
}

function getAuthToken() {
  const directKeys = [
    "token",
    "authToken",
    "accessToken",
    "jwtToken",
    "bearerToken",
    "ims-auth-token",
    "imsToken",
  ];

  const objectKeys = [
    "user",
    "authUser",
    "imsUser",
    "imsAdminUser",
    "loginUser",
    "currentUser",
    "ims-current-user",
  ];

  for (const key of directKeys) {
    const localToken = cleanToken(getStoredValue(localStorage, key));
    if (localToken) return localToken;

    const sessionToken = cleanToken(getStoredValue(sessionStorage, key));
    if (sessionToken) return sessionToken;
  }

  for (const key of objectKeys) {
    const localToken = getTokenFromObject(getStoredValue(localStorage, key));
    if (localToken) return localToken;

    const sessionToken = getTokenFromObject(getStoredValue(sessionStorage, key));
    if (sessionToken) return sessionToken;
  }

  return "";
}

function getJsonHeaders() {
  const token = getAuthToken();

  return {
    "Content-Type": "application/json",
    "ngrok-skip-browser-warning": "true",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

function getUploadHeaders() {
  const token = getAuthToken();

  return {
    "ngrok-skip-browser-warning": "true",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

function getSettingId(settings = {}) {
  return (
    settings?.id ||
    settings?.settingId ||
    settings?.systemSettingId ||
    settings?.settingsId ||
    FALLBACK_SETTINGS_ID
  );
}

function getSystemSettingsApiUrl(id) {
  return `${SYSTEM_SETTINGS_API}/${id || FALLBACK_SETTINGS_ID}`;
}

function getUploadLogoApiUrl(id) {
  return `${SYSTEM_SETTINGS_API}/upload-logo/${id || FALLBACK_SETTINGS_ID}`;
}

function getRemoveLogoApiUrl(id) {
  return `${SYSTEM_SETTINGS_API}/remove-logo/${id || FALLBACK_SETTINGS_ID}`;
}

function getRoleStatusApiUrl(id) {
  return `${ROLES_API}/${id}/status`;
}

function normalizeApiRoot(data) {
  if (Array.isArray(data)) return data[0] || {};
  if (Array.isArray(data?.data)) return data.data[0] || {};
  return data?.data || data?.settings || data?.systemSettings || data || {};
}

function normalizeLogoUrl(logoUrl = "") {
  if (!logoUrl) return "";

  if (
    logoUrl.startsWith("data:") ||
    logoUrl.startsWith("blob:") ||
    logoUrl.startsWith("http://") ||
    logoUrl.startsWith("https://")
  ) {
    return logoUrl;
  }

  if (logoUrl.startsWith("/")) {
    return `${API_BASE_URL}${logoUrl}`;
  }

  return `${API_BASE_URL}/${logoUrl}`;
}

function mapApiSettingsToUi(apiSettings, fallbackSettings = {}) {
  const settings = normalizeApiRoot(apiSettings);
  const settingId = getSettingId(settings) || getSettingId(fallbackSettings);

  const cleanVal = (val, fb = "") => {
    const s = String(val || "").trim();
    return (!s || s.toLowerCase() === "string") ? fb : s;
  };

  return {
    __apiRaw: settings,
    id: settingId,
    settingId,
    systemSettingId: settingId,

    companyName: cleanVal(
      settings?.companyName || settings?.businessName,
      fallbackSettings?.companyName || "IMS Inventory Solutions"
    ),
    companyLogo: normalizeLogoUrl(
      settings?.companyLogo ||
        settings?.logoUrl ||
        settings?.logo ||
        settings?.imageUrl ||
        fallbackSettings?.companyLogo ||
        ""
    ),
    email: cleanVal(
      settings?.email || settings?.emailAddress || settings?.companyEmail,
      fallbackSettings?.email || "support@ims.com"
    ),
    phone: cleanVal(
      settings?.phone || settings?.phoneNumber || settings?.companyPhone,
      fallbackSettings?.phone || "9876543210"
    ),
    address: cleanVal(
      settings?.address || settings?.companyAddress,
      fallbackSettings?.address || "Main Branch, Hyderabad, Telangana, India"
    ),

    allowNegativeStock:
      typeof settings?.allowNegativeStock === "boolean"
        ? settings.allowNegativeStock
        : Boolean(fallbackSettings?.allowNegativeStock),
    lowStockLimit: String(
      settings?.defaultReorderLevel ??
        settings?.lowStockAlertLimit ??
        settings?.lowStockLimit ??
        fallbackSettings?.lowStockLimit ??
        ""
    ),
    defaultReorderLevel: String(
      settings?.defaultReorderLevel ??
        settings?.lowStockAlertLimit ??
        fallbackSettings?.defaultReorderLevel ??
        fallbackSettings?.lowStockLimit ??
        ""
    ),
    stockValuationMethod:
      settings?.stockValuationMethod ||
      fallbackSettings?.stockValuationMethod ||
      "FIFO",
    defaultUnit:
      settings?.defaultUnitType ||
      settings?.defaultUnit ||
      fallbackSettings?.defaultUnit ||
      "pcs",
    defaultUnitType:
      settings?.defaultUnitType ||
      settings?.defaultUnit ||
      fallbackSettings?.defaultUnitType ||
      fallbackSettings?.defaultUnit ||
      "pcs",
    barcodeEnabled:
      typeof settings?.enableBarcode === "boolean"
        ? settings.enableBarcode
        : typeof settings?.barcodeManagement === "boolean"
          ? settings.barcodeManagement
          : Boolean(fallbackSettings?.barcodeEnabled),
    enableBarcode:
      typeof settings?.enableBarcode === "boolean"
        ? settings.enableBarcode
        : typeof settings?.barcodeManagement === "boolean"
          ? settings.barcodeManagement
          : Boolean(fallbackSettings?.enableBarcode || fallbackSettings?.barcodeEnabled),
    autoStockUpdate:
      typeof settings?.autoStockUpdate === "boolean"
        ? settings.autoStockUpdate
        : Boolean(fallbackSettings?.autoStockUpdate),
    lowStockAlert:
      typeof settings?.lowStockAlert === "boolean"
        ? settings.lowStockAlert
        : typeof settings?.lowStockNotifications === "boolean"
          ? settings.lowStockNotifications
          : Boolean(fallbackSettings?.lowStockAlert || fallbackSettings?.lowStockAlerts),
    lowStockAlerts:
      typeof settings?.lowStockAlert === "boolean"
        ? settings.lowStockAlert
        : typeof settings?.lowStockNotifications === "boolean"
          ? settings.lowStockNotifications
          : typeof settings?.lowStockAlerts === "boolean"
            ? settings.lowStockAlerts
            : Boolean(fallbackSettings?.lowStockAlerts),

    emailNotifications:
      typeof settings?.emailNotifications === "boolean"
        ? settings.emailNotifications
        : Boolean(fallbackSettings?.emailNotifications),
    lowStockNotifications:
      typeof settings?.lowStockNotifications === "boolean"
        ? settings.lowStockNotifications
        : typeof settings?.lowStockAlert === "boolean"
          ? settings.lowStockAlert
          : Boolean(fallbackSettings?.lowStockNotifications || fallbackSettings?.lowStockAlerts),
    purchaseNotifications:
      typeof settings?.purchaseNotifications === "boolean"
        ? settings.purchaseNotifications
        : Boolean(fallbackSettings?.purchaseNotifications || fallbackSettings?.paymentReminder),
    salesNotifications:
      typeof settings?.salesNotifications === "boolean"
        ? settings.salesNotifications
        : Boolean(fallbackSettings?.salesNotifications || fallbackSettings?.orderNotifications),
    systemAlerts:
      typeof settings?.systemAlerts === "boolean"
        ? settings.systemAlerts
        : Boolean(fallbackSettings?.systemAlerts),
    orderNotifications:
      typeof settings?.salesNotifications === "boolean"
        ? settings.salesNotifications
        : typeof settings?.orderNotifications === "boolean"
          ? settings.orderNotifications
          : Boolean(fallbackSettings?.orderNotifications),
    paymentReminder:
      typeof settings?.purchaseNotifications === "boolean"
        ? settings.purchaseNotifications
        : typeof settings?.supplierPaymentReminder === "boolean"
          ? settings.supplierPaymentReminder
          : Boolean(fallbackSettings?.paymentReminder),

    themeMode: settings?.themeMode || fallbackSettings?.themeMode || "light",
    language: settings?.language || fallbackSettings?.language || "english",
    collapseSidebar:
      typeof settings?.collapseSidebar === "boolean"
        ? settings.collapseSidebar
        : Boolean(fallbackSettings?.collapseSidebar),

    enableTwoFactorAuth:
      typeof settings?.enableTwoFactorAuth === "boolean"
        ? settings.enableTwoFactorAuth
        : typeof settings?.twoStepVerification === "boolean"
          ? settings.twoStepVerification
          : Boolean(fallbackSettings?.enableTwoFactorAuth || fallbackSettings?.twoStepVerification),
    twoStepVerification:
      typeof settings?.enableTwoFactorAuth === "boolean"
        ? settings.enableTwoFactorAuth
        : typeof settings?.twoStepVerification === "boolean"
          ? settings.twoStepVerification
          : Boolean(fallbackSettings?.twoStepVerification),
  };
}

function toSafeNumber(value, fallbackValue = 0) {
  const parsedValue = Number(value);
  return Number.isFinite(parsedValue) ? parsedValue : fallbackValue;
}

function mapUiSettingsToApi(formData) {
  const settingId = getSettingId(formData);
  const reorderLevel = toSafeNumber(
    formData.defaultReorderLevel || formData.lowStockLimit,
    0
  );

  /*
    IMPORTANT:
    Send only the DTO fields used by /api/SystemSettings.
    Extra alias fields like settingId/systemSettingId/barcodeManagement were causing
    backend model validation to fail on save.
  */
  return {
    id: settingId,
    companyName: String(formData.companyName || "").trim(),
    companyLogo: cleanLogoForApi(formData.companyLogo),
    emailAddress: String(formData.email || "").trim(),
    phoneNumber: String(formData.phone || "").trim(),
    address: String(formData.address || "").trim(),

    allowNegativeStock: Boolean(formData.allowNegativeStock),
    defaultReorderLevel: reorderLevel,
    stockValuationMethod: formData.stockValuationMethod || "FIFO",
    lowStockAlert: Boolean(formData.lowStockAlert),
    defaultUnitType: formData.defaultUnitType || formData.defaultUnit || "pcs",
    enableBarcode: Boolean(formData.enableBarcode ?? formData.barcodeEnabled),
    autoStockUpdate: Boolean(formData.autoStockUpdate),

    emailNotifications: Boolean(formData.emailNotifications),
    lowStockNotifications: Boolean(formData.lowStockNotifications ?? formData.lowStockAlerts),
    purchaseNotifications: Boolean(formData.purchaseNotifications ?? formData.paymentReminder),
    salesNotifications: Boolean(formData.salesNotifications ?? formData.orderNotifications),
    systemAlerts: Boolean(formData.systemAlerts),

    themeMode: formData.themeMode || "light",
    language: formData.language || "english",
    collapseSidebar: Boolean(formData.collapseSidebar),
    enableTwoFactorAuth: Boolean(formData.enableTwoFactorAuth ?? formData.twoStepVerification),
  };
}

function cleanLogoForApi(logoUrl) {
  const value = String(logoUrl || "").trim();

  if (!value || value.startsWith("data:") || value.startsWith("blob:")) {
    return "";
  }

  return value;
}

function buildSystemSettingsPayloads(formData) {
  const settingId = getSettingId(formData);
  const newPayload = mapUiSettingsToApi(formData);

  const legacyPayload = {
    settingId,
    id: settingId,
    companyName: newPayload.companyName,
    companyLogo: cleanLogoForApi(formData.companyLogo),
    emailAddress: newPayload.emailAddress,
    phoneNumber: newPayload.phoneNumber,
    address: newPayload.address,
    lowStockAlertLimit: newPayload.defaultReorderLevel,
    defaultReorderLevel: newPayload.defaultReorderLevel,
    stockValuationMethod: newPayload.stockValuationMethod,
    defaultUnitType: newPayload.defaultUnitType,
    barcodeManagement: newPayload.enableBarcode,
    enableBarcode: newPayload.enableBarcode,
    allowNegativeStock: newPayload.allowNegativeStock,
    autoStockUpdate: newPayload.autoStockUpdate,
    lowStockAlerts: newPayload.lowStockAlert,
    lowStockAlert: newPayload.lowStockAlert,
    emailNotifications: newPayload.emailNotifications,
    lowStockNotifications: newPayload.lowStockNotifications,
    purchaseNotifications: newPayload.purchaseNotifications,
    salesNotifications: newPayload.salesNotifications,
    systemAlerts: newPayload.systemAlerts,
    orderNotifications: newPayload.salesNotifications,
    supplierPaymentReminder: newPayload.purchaseNotifications,
    paymentReminder: newPayload.purchaseNotifications,
    themeMode: newPayload.themeMode,
    language: newPayload.language,
    collapseSidebar: newPayload.collapseSidebar,
    twoStepVerification: newPayload.enableTwoFactorAuth,
    enableTwoFactorAuth: newPayload.enableTwoFactorAuth,
  };

  const mergedPayload = {
    ...(formData.__apiRaw && typeof formData.__apiRaw === "object"
      ? formData.__apiRaw
      : {}),
    ...legacyPayload,
    ...newPayload,
    id: settingId,
    settingId,
    companyLogo: cleanLogoForApi(formData.companyLogo),
  };

  return [newPayload, legacyPayload, mergedPayload];
}

function normalizeCompareValue(value) {
  if (typeof value === "boolean") return value;
  if (typeof value === "number") return Number.isFinite(value) ? String(value) : "";

  const textValue = String(value ?? "").trim();

  if (textValue.toLowerCase() === "true") return true;
  if (textValue.toLowerCase() === "false") return false;

  return textValue;
}

function compareValues(expectedValue, savedValue) {
  const expected = normalizeCompareValue(expectedValue);
  const saved = normalizeCompareValue(savedValue);

  if (typeof expected === "boolean" || typeof saved === "boolean") {
    return Boolean(expected) === Boolean(saved);
  }

  return String(expected) === String(saved);
}

function getSavedValue(settings, keys) {
  for (const key of keys) {
    const value = settings?.[key];

    if (value !== undefined && value !== null && value !== "") {
      return value;
    }
  }

  return "";
}

function getVerificationRules(activeTab, formData) {
  if (activeTab === "general") {
    return [
      { label: "Company name", expected: formData.companyName, keys: ["companyName"] },
      { label: "Email", expected: formData.email, keys: ["email", "emailAddress"] },
      { label: "Phone", expected: formData.phone, keys: ["phone", "phoneNumber"] },
      { label: "Address", expected: formData.address, keys: ["address"] },
    ];
  }

  if (activeTab === "inventory") {
    return [
      { label: "Allow negative stock", expected: Boolean(formData.allowNegativeStock), keys: ["allowNegativeStock"] },
      {
        label: "Default reorder level",
        expected: String(formData.defaultReorderLevel || formData.lowStockLimit || ""),
        keys: ["defaultReorderLevel", "lowStockLimit"],
      },
      { label: "Stock valuation method", expected: formData.stockValuationMethod || "FIFO", keys: ["stockValuationMethod"] },
      {
        label: "Low stock alert",
        expected: Boolean(formData.lowStockAlert || formData.lowStockAlerts || formData.lowStockNotifications),
        keys: ["lowStockAlert", "lowStockAlerts", "lowStockNotifications"],
      },
      { label: "Default unit", expected: formData.defaultUnitType || formData.defaultUnit || "pcs", keys: ["defaultUnitType", "defaultUnit"] },
      { label: "Barcode", expected: Boolean(formData.enableBarcode || formData.barcodeEnabled), keys: ["enableBarcode", "barcodeEnabled"] },
      { label: "Auto stock update", expected: Boolean(formData.autoStockUpdate), keys: ["autoStockUpdate"] },
    ];
  }

  if (activeTab === "notifications") {
    return [
      { label: "Email notifications", expected: Boolean(formData.emailNotifications), keys: ["emailNotifications"] },
      {
        label: "Low stock notifications",
        expected: Boolean(formData.lowStockNotifications || formData.lowStockAlerts || formData.lowStockAlert),
        keys: ["lowStockNotifications", "lowStockAlerts", "lowStockAlert"],
      },
      {
        label: "Purchase notifications",
        expected: Boolean(formData.purchaseNotifications || formData.paymentReminder),
        keys: ["purchaseNotifications", "paymentReminder"],
      },
      {
        label: "Sales notifications",
        expected: Boolean(formData.salesNotifications || formData.orderNotifications),
        keys: ["salesNotifications", "orderNotifications"],
      },
      { label: "System alerts", expected: Boolean(formData.systemAlerts), keys: ["systemAlerts"] },
    ];
  }



  if (activeTab === "security") {
    return [
      {
        label: "Two factor authentication",
        expected: Boolean(formData.enableTwoFactorAuth || formData.twoStepVerification),
        keys: ["enableTwoFactorAuth", "twoStepVerification"],
      },
    ];
  }

  return [];
}

function getNotUpdatedFields(activeTab, expectedSettings, savedSettings) {
  const rules = getVerificationRules(activeTab, expectedSettings);

  return rules.filter((rule) => {
    const savedValue = getSavedValue(savedSettings, rule.keys);
    return !compareValues(rule.expected, savedValue);
  });
}

async function fetchConfirmedSystemSettings(formData) {
  const data = await fetchJsonFromAvailableUrls(
    [SYSTEM_SETTINGS_API],
    {
      method: "GET",
      headers: getJsonHeaders(),
    },
    "Unable to reload settings after save."
  );

  return mapApiSettingsToUi(data, formData);
}

async function saveSystemSettingsWithRetry(settingId, formData) {
  const payloads = buildSystemSettingsPayloads(formData);
  let lastError = null;

  for (const payload of payloads) {
    try {
      await sendJsonToAvailableUrls(
        [getSystemSettingsApiUrl(settingId)],
        {
          method: "PUT",
          headers: getJsonHeaders(),
          body: JSON.stringify(payload),
        },
        "Unable to save settings."
      );

      return {
        confirmedSettings: mapApiSettingsToUi(
          {
            ...(formData.__apiRaw && typeof formData.__apiRaw === "object" ? formData.__apiRaw : {}),
            ...payload,
          },
          formData
        ),
        payload,
      };
    } catch (error) {
      lastError = error;
    }
  }

  throw lastError || new Error("Unable to save settings.");
}


function getCleanRoleName(roleName) {
  const value = String(roleName || "").trim();
  const lower = value.toLowerCase();

  if (["admin", "administrator"].includes(lower)) return "Admin";
  if (lower === "manager") return "Manager";
  if (lower === "staff") return "Staff";

  if (value) {
    return value.charAt(0).toUpperCase() + value.slice(1);
  }

  return "";
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

  const roleMap = new Map();

  list.forEach((role, index) => {
    const roleName = getCleanRoleName(
      role.name || role.roleName || role.role || role.title || role.displayName
    );

    if (!roleName) return;

    const existingRole = roleMap.get(roleName);

    if (existingRole) {
      roleMap.set(roleName, {
        ...existingRole,
        id: existingRole.id || role.id || role.roleId || role.permissionRoleId || index + 1,
        isActive: existingRole.isActive || role.isActive === true || role.active === true || String(role.status || "").toLowerCase() === "active",
      });
      return;
    }

    roleMap.set(roleName, {
      id: role.id || role.roleId || role.permissionRoleId || index + 1,
      name: roleName,
      description: role.description || role.roleDescription || role.desc || "Role access configuration",
      isActive:
        typeof role.isActive === "boolean"
          ? role.isActive
          : typeof role.active === "boolean"
            ? role.active
            : String(role.status || "Active").toLowerCase() !== "inactive",
      raw: role,
    });
  });

  const orderedRoles = [];
  const standardNames = ["Admin", "Manager", "Staff"];
  
  standardNames.forEach(name => {
    const r = roleMap.get(name);
    if (r) {
      orderedRoles.push(r);
      roleMap.delete(name);
    }
  });

  roleMap.forEach(role => {
    orderedRoles.push(role);
  });

  return orderedRoles;
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

async function fetchJsonFromAvailableUrls(urls, options, fallbackMessage) {
  let lastError = null;

  for (const url of urls) {
    try {
      const response = await fetch(url, options);

      if (!response.ok) {
        const errorMessage = await getApiErrorMessage(response, fallbackMessage);
        lastError = new Error(errorMessage);
        continue;
      }

      return await response.json();
    } catch (error) {
      lastError = error;
    }
  }

  throw lastError || new Error(fallbackMessage);
}

async function sendJsonToAvailableUrls(urls, options, fallbackMessage) {
  let lastError = null;

  for (const url of urls) {
    try {
      const response = await fetch(url, options);

      if (!response.ok) {
        const errorMessage = await getApiErrorMessage(response, fallbackMessage);
        lastError = new Error(errorMessage);
        continue;
      }

      try {
        return await response.json();
      } catch {
        return null;
      }
    } catch (error) {
      lastError = error;
    }
  }

  throw lastError || new Error(fallbackMessage);
}

async function uploadToAvailableUrls(urls, options, fallbackMessage) {
  let lastError = null;

  for (const url of urls) {
    try {
      const response = await fetch(url, options);

      if (!response.ok) {
        const errorMessage = await getApiErrorMessage(response, fallbackMessage);
        lastError = new Error(errorMessage);
        continue;
      }

      try {
        return await response.json();
      } catch {
        return null;
      }
    } catch (error) {
      lastError = error;
    }
  }

  throw lastError || new Error(fallbackMessage);
}

function AdminSettings({ settingsData, t, onUpdateSettings, onClose }) {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState("general");
  const [isSettingsSidebarCollapsed, setIsSettingsSidebarCollapsed] =
    useState(settingsData?.collapseSidebar || false);

  const [showChangePassword, setShowChangePassword] = useState(false);
  const [showLoginHistory, setShowLoginHistory] = useState(false);
  const [showManagePermissions, setShowManagePermissions] = useState(false);

  const [formData, setFormData] = useState({
    companyLogo: "",
    ...settingsData,
  });

  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState("");
  const [apiError, setApiError] = useState("");
  const [loadingSettings, setLoadingSettings] = useState(false);
  const [savingSettings, setSavingSettings] = useState(false);
  const [uploadingLogo, setUploadingLogo] = useState(false);
  const [roles, setRoles] = useState([]);
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [roleActionId, setRoleActionId] = useState("");
  const [rolesError, setRolesError] = useState("");


  const logoInputRef = useRef(null);
  const st = t.settingsPage;

  useEffect(() => {
    setFormData({
      companyLogo: "",
      ...settingsData,
    });

    setIsSettingsSidebarCollapsed(settingsData?.collapseSidebar || false);
  }, [settingsData]);

  useEffect(() => {
    async function fetchSettings() {
      try {
        setLoadingSettings(true);
        setApiError("");

        const data = await fetchJsonFromAvailableUrls(
          [SYSTEM_SETTINGS_API],
          {
            method: "GET",
            headers: getJsonHeaders(),
          },
          "Unable to load settings."
        );

        const mappedSettings = mapApiSettingsToUi(data, settingsData);

        setFormData(mappedSettings);
        setIsSettingsSidebarCollapsed(mappedSettings.collapseSidebar);

        if (typeof onUpdateSettings === "function") {
          onUpdateSettings(mappedSettings);
        }
      } catch (error) {
        console.error("Settings fetch error:", error);

        const hasFallbackSettings =
          Boolean(settingsData) && Object.keys(settingsData).length > 0;

        if (!hasFallbackSettings) {
          setApiError(error.message || "Unable to load settings.");
        }
      } finally {
        setLoadingSettings(false);
      }
    }

    fetchSettings();
  }, []);

  useEffect(() => {
    async function fetchRoles() {
      try {
        setLoadingRoles(true);
        setRolesError("");

        const data = await fetchJsonFromAvailableUrls(
          [ROLES_API],
          {
            method: "GET",
            headers: getJsonHeaders(),
          },
          "Unable to load roles."
        );

        setRoles(normalizeRolesResponse(data));
      } catch (error) {
        console.error("Roles fetch error:", error);
        setRoles([]);
        setRolesError(error.message || "Unable to load roles.");
      } finally {
        setLoadingRoles(false);
      }
    }

    fetchRoles();
  }, []);

  useEffect(() => {
    const handleEscape = (event) => {
      if (
        event.key === "Escape" &&
        !savingSettings &&
        !uploadingLogo &&
        !loadingSettings
      ) {
        onClose();
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [onClose, savingSettings, uploadingLogo, loadingSettings]);

  const tabs = [
    { id: "general", label: st.tabs.general, icon: Settings },
    { id: "inventory", label: st.tabs.inventory, icon: Package },
    { id: "roles", label: st.tabs.roles, icon: Users },
    { id: "notifications", label: st.tabs.notifications, icon: Bell },
    { id: "security", label: st.tabs.security, icon: ShieldCheck },
  ];

  const handleInputChange = (event) => {
    const { name, value, type, checked } = event.target;

    if (name === "phone") {
      setFormData({
        ...formData,
        phone: value.replace(/\D/g, "").slice(0, 10),
      });
      return;
    }

    if (name === "lowStockLimit") {
      setFormData({
        ...formData,
        lowStockLimit: value.replace(/\D/g, ""),
      });
      return;
    }

    if (type === "checkbox") {
      const booleanValue = Boolean(checked);

      if (name === "collapseSidebar") {
        setFormData((previousData) => ({
          ...previousData,
          collapseSidebar: booleanValue,
        }));
        setIsSettingsSidebarCollapsed(booleanValue);
        return;
      }

      if (name === "lowStockAlert") {
        setFormData((previousData) => ({
          ...previousData,
          lowStockAlert: booleanValue,
        }));
        return;
      }

      if (name === "barcodeEnabled") {
        setFormData((previousData) => ({
          ...previousData,
          barcodeEnabled: booleanValue,
          enableBarcode: booleanValue,
        }));
        return;
      }

      if (name === "lowStockAlerts") {
        setFormData((previousData) => ({
          ...previousData,
          lowStockAlerts: booleanValue,
          lowStockNotifications: booleanValue,
        }));
        return;
      }

      if (name === "orderNotifications") {
        setFormData((previousData) => ({
          ...previousData,
          orderNotifications: booleanValue,
          salesNotifications: booleanValue,
        }));
        return;
      }

      if (name === "paymentReminder") {
        setFormData((previousData) => ({
          ...previousData,
          paymentReminder: booleanValue,
          purchaseNotifications: booleanValue,
        }));
        return;
      }

      if (name === "twoStepVerification") {
        setFormData((previousData) => ({
          ...previousData,
          twoStepVerification: booleanValue,
          enableTwoFactorAuth: booleanValue,
        }));
        return;
      }

      setFormData((previousData) => ({
        ...previousData,
        [name]: booleanValue,
      }));
      return;
    }

    setFormData({
      ...formData,
      [name]: value,
    });
  };

  const handleLogoUpload = async (event) => {
    const file = event.target.files?.[0];

    if (!file) return;

    const isImage = file.type.startsWith("image/");
    const isValidSize = file.size <= 2 * 1024 * 1024;

    if (!isImage) {
      setErrors({
        ...errors,
        companyLogo: "Please upload only image files",
      });
      return;
    }

    if (!isValidSize) {
      setErrors({
        ...errors,
        companyLogo: "Logo size must be below 2MB",
      });
      return;
    }

    try {
      setUploadingLogo(true);
      setApiError("");

      const uploadData = new FormData();
      uploadData.append("file", file);

      const responseData = await uploadToAvailableUrls(
        [getUploadLogoApiUrl(formData.settingId)],
        {
          method: "POST",
          headers: getUploadHeaders(),
          body: uploadData,
        },
        "Logo upload failed."
      );

      const uploadedLogo = normalizeLogoUrl(
        responseData?.companyLogo ||
          responseData?.logoUrl ||
          responseData?.imageUrl ||
          responseData?.url ||
          responseData?.fileUrl ||
          responseData?.path ||
          responseData?.data?.companyLogo ||
          responseData?.data?.logoUrl ||
          responseData?.data?.imageUrl ||
          responseData?.data?.url ||
          responseData?.data?.fileUrl ||
          responseData?.data?.path ||
          ""
      );

      if (uploadedLogo) {
        setFormData({
          ...formData,
          companyLogo: uploadedLogo,
        });
      } else {
        const reader = new FileReader();

        reader.onload = () => {
          setFormData({
            ...formData,
            companyLogo: reader.result,
          });
        };

        reader.readAsDataURL(file);
      }

      setErrors({
        ...errors,
        companyLogo: "",
      });
    } catch (error) {
      console.error("Logo upload error:", error);
      setApiError(error.message || "Logo upload failed.");
    } finally {
      setUploadingLogo(false);

      if (logoInputRef.current) {
        logoInputRef.current.value = "";
      }
    }
  };

  const handleRemoveLogo = async () => {
    try {
      setUploadingLogo(true);
      setApiError("");

      await sendJsonToAvailableUrls(
        [getRemoveLogoApiUrl(formData.settingId)],
        {
          method: "DELETE",
          headers: getJsonHeaders(),
        },
        "Logo remove failed."
      );

      setFormData({
        ...formData,
        companyLogo: "",
      });

      if (logoInputRef.current) {
        logoInputRef.current.value = "";
      }
    } catch (error) {
      console.error("Logo remove error:", error);
      setApiError(error.message || "Logo remove failed.");
    } finally {
      setUploadingLogo(false);
    }
  };

  const handleToggleSettingsSidebar = () => {
    const nextValue = !isSettingsSidebarCollapsed;

    setIsSettingsSidebarCollapsed(nextValue);
    setFormData({
      ...formData,
      collapseSidebar: nextValue,
    });
  };

  const handleToggleRoleStatus = async (role) => {
    if (!role?.id || roleActionId) return;

    try {
      setRoleActionId(role.id);
      setRolesError("");
      setSuccessMessage("");

      const responseData = await sendJsonToAvailableUrls(
        [getRoleStatusApiUrl(role.id)],
        {
          method: "PUT",
          headers: getJsonHeaders(),
          body: JSON.stringify({
            id: role.id,
            roleId: role.id,
            isActive: !role.isActive,
            status: role.isActive ? "Inactive" : "Active",
          }),
        },
        "Unable to update role status."
      );

      // Resolve status update dynamically
      let isNewActive = !role.isActive;
      if (responseData) {
        if (typeof responseData.isActive === "boolean") {
          isNewActive = responseData.isActive;
        } else if (responseData.status) {
          isNewActive = String(responseData.status).toLowerCase() === "active";
        }
      }

      setRoles((currentRoles) =>
        currentRoles.map((item) =>
          item.id === role.id
            ? {
                ...item,
                isActive: isNewActive,
              }
            : item
        )
      );

      notifyRolesUpdated();

      setSuccessMessage("Role status updated successfully.");
      setTimeout(() => {
        setSuccessMessage("");
      }, 2500);
    } catch (error) {
      console.error("Role status update error:", error);
      setRolesError(error.message || "Unable to update role status.");
      setSuccessMessage("");
    } finally {
      setRoleActionId("");
    }
  };

  const validateSettings = () => {
    const newErrors = {};

    if (activeTab === "general") {
      if (!String(formData.companyName || "").trim()) {
        newErrors.companyName = st.validation.companyName;
      }

      if (!String(formData.email || "").trim()) {
        newErrors.email = st.validation.emailRequired;
      } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
        newErrors.email = st.validation.emailInvalid;
      }

      if (!String(formData.phone || "").trim()) {
        newErrors.phone = st.validation.phoneRequired;
      } else if (!/^\d{10}$/.test(formData.phone)) {
        newErrors.phone = st.validation.phoneInvalid;
      }

      if (!String(formData.address || "").trim()) {
        newErrors.address = st.validation.address;
      }
    }

    if (activeTab === "inventory") {
      if (!String(formData.lowStockLimit || formData.defaultReorderLevel || "").trim()) {
        newErrors.lowStockLimit = st.validation.lowStockRequired;
      } else if (Number(formData.lowStockLimit || formData.defaultReorderLevel) <= 0) {
        newErrors.lowStockLimit = st.validation.lowStockInvalid;
      }
    }

    setErrors(newErrors);

    if (Object.keys(newErrors).length > 0) {
      setApiError("Please correct the highlighted fields before saving.");
      return false;
    }

    setApiError("");
    return true;
  };

  const handleSaveSettings = async () => {
    if (activeTab === "roles") {
      setApiError("");
      setSuccessMessage("User & role settings saved successfully.");
      setTimeout(() => setSuccessMessage(""), 2500);
      return;
    }

    if (!validateSettings()) return;

    try {
      setSavingSettings(true);
      setSuccessMessage("");
      setApiError("");

      const settingId = getSettingId(formData);
      const { confirmedSettings } = await saveSystemSettingsWithRetry(
        settingId,
        formData
      );

      const finalSettings = {
        ...formData,
        ...confirmedSettings,
        companyName: formData.companyName,
        email: formData.email,
        phone: formData.phone,
        address: formData.address,
        allowNegativeStock: Boolean(formData.allowNegativeStock),
        defaultReorderLevel: formData.defaultReorderLevel || formData.lowStockLimit,
        lowStockLimit: formData.lowStockLimit || formData.defaultReorderLevel,
        stockValuationMethod: formData.stockValuationMethod || "FIFO",
        lowStockAlert: Boolean(formData.lowStockAlert),
        lowStockAlerts: Boolean(formData.lowStockAlerts),
        lowStockNotifications: Boolean(formData.lowStockNotifications ?? formData.lowStockAlerts),
        defaultUnitType: formData.defaultUnitType || formData.defaultUnit || "pcs",
        defaultUnit: formData.defaultUnit || formData.defaultUnitType || "pcs",
        enableBarcode: Boolean(formData.enableBarcode ?? formData.barcodeEnabled),
        barcodeEnabled: Boolean(formData.barcodeEnabled ?? formData.enableBarcode),
        autoStockUpdate: Boolean(formData.autoStockUpdate),
        emailNotifications: Boolean(formData.emailNotifications),
        purchaseNotifications: Boolean(formData.purchaseNotifications ?? formData.paymentReminder),
        paymentReminder: Boolean(formData.paymentReminder ?? formData.purchaseNotifications),
        salesNotifications: Boolean(formData.salesNotifications ?? formData.orderNotifications),
        orderNotifications: Boolean(formData.orderNotifications ?? formData.salesNotifications),
        systemAlerts: Boolean(formData.systemAlerts),
        themeMode: formData.themeMode || "light",
        language: formData.language || "english",
        collapseSidebar: Boolean(formData.collapseSidebar),
        enableTwoFactorAuth: Boolean(formData.enableTwoFactorAuth ?? formData.twoStepVerification),
        twoStepVerification: Boolean(formData.twoStepVerification ?? formData.enableTwoFactorAuth),
      };

      setFormData(finalSettings);
      setIsSettingsSidebarCollapsed(finalSettings.collapseSidebar);

      if (typeof onUpdateSettings === "function") {
        onUpdateSettings(finalSettings);
      }

      localStorage.setItem("imsAdminSettings", JSON.stringify(finalSettings));
      window.dispatchEvent(new CustomEvent('ims-settings-updated', { detail: finalSettings }));

      setSuccessMessage(st.validation.success || "Settings saved successfully.");
      setTimeout(() => {
        setSuccessMessage("");
      }, 2500);
    } catch (error) {
      console.error("Settings save error:", error);
      setApiError(error.message || "Unable to save settings.");
    } finally {
      setSavingSettings(false);
    }
  };

  return (
    <>
      <div
        className="settings-overlay"
        onClick={
          !savingSettings && !uploadingLogo && !loadingSettings
            ? onClose
            : undefined
        }
      >
        <div
          className={`settings-modal ${
            isSettingsSidebarCollapsed
              ? "settings-modal--sidebar-collapsed"
              : ""
          }`}
          onClick={(event) => event.stopPropagation()}
        >
          <div className="settings-header">
            <div>
              <h2>{st.title}</h2>
            </div>

            <div className="settings-header-actions">
              {successMessage && (
                <div className="settings-header-success-message">
                  {successMessage}
                </div>
              )}
              <button
                className="settings-close-btn"
                onClick={onClose}
                disabled={savingSettings || uploadingLogo || loadingSettings}
              >
                ×
              </button>
            </div>
          </div>

          {loadingSettings && (
            <div className="settings-success-message">Loading settings...</div>
          )}

          {apiError && <div className="settings-api-error page-error-banner" role="alert">{apiError}</div>}

          <div className="settings-body">
            <aside className="settings-sidebar">
              <div className="settings-sidebar-tabs">
                {tabs.map((tab) => {
                  const Icon = tab.icon;

                  return (
                    <button
                      key={tab.id}
                      className={activeTab === tab.id ? "active" : ""}
                      onClick={() => setActiveTab(tab.id)}
                      title={tab.label}
                      disabled={loadingSettings}
                    >
                      <span className="settings-tab-icon">
                        <Icon size={18} strokeWidth={2.2} />
                      </span>
                      <span className="settings-tab-label">{tab.label}</span>
                    </button>
                  );
                })}
              </div>
            </aside>

            <section className="settings-content">
              {activeTab === "general" && (
                <div className="settings-panel">
                  <h3>{st.general.title}</h3>

                  <div className="settings-form-group">
                    <label>{st.general.companyName}</label>
                    <input
                      type="text"
                      name="companyName"
                      value={formData.companyName}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                    {errors.companyName && <small>{errors.companyName}</small>}
                  </div>



                  <div className="settings-form-grid">
                    <div className="settings-form-group">
                      <label>{st.general.emailAddress}</label>
                      <input
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleInputChange}
                        disabled={loadingSettings || savingSettings}
                      />
                      {errors.email && <small>{errors.email}</small>}
                    </div>

                    <div className="settings-form-group">
                      <label>{st.general.phoneNumber}</label>
                      <input
                        type="text"
                        name="phone"
                        value={formData.phone}
                        onChange={handleInputChange}
                        maxLength="10"
                        disabled={loadingSettings || savingSettings}
                      />
                      {errors.phone && <small>{errors.phone}</small>}
                    </div>
                  </div>

                  <div className="settings-form-group">
                    <label>{st.general.address}</label>
                    <textarea
                      name="address"
                      value={formData.address}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    ></textarea>
                    {errors.address && <small>{errors.address}</small>}
                  </div>
                </div>
              )}

              {activeTab === "inventory" && (
                <div className="settings-panel">
                  <h3>{st.inventory.title}</h3>

                  <div className="settings-form-grid">
                    <div className="settings-form-group">
                      <label>{st.inventory.lowStockLimit}</label>
                      <input
                        type="text"
                        name="lowStockLimit"
                        value={formData.lowStockLimit}
                        onChange={handleInputChange}
                        disabled={loadingSettings || savingSettings}
                      />
                      {errors.lowStockLimit && (
                        <small>{errors.lowStockLimit}</small>
                      )}
                    </div>

                    <div className="settings-form-group">
                      <label>{st.inventory.defaultUnit}</label>
                      <select
                        name="defaultUnit"
                        value={formData.defaultUnit}
                        onChange={handleInputChange}
                        disabled={loadingSettings || savingSettings}
                      >
                        <option value="pcs">{st.inventory.pieces}</option>
                        <option value="kg">{st.inventory.kilograms}</option>
                        <option value="box">{st.inventory.boxes}</option>
                        <option value="ltr">{st.inventory.liters}</option>
                      </select>
                    </div>
                  </div>

                  <div className="settings-form-grid">
                    <div className="settings-form-group">
                      <label>Stock Valuation Method</label>
                      <select
                        name="stockValuationMethod"
                        value={formData.stockValuationMethod}
                        onChange={handleInputChange}
                        disabled={loadingSettings || savingSettings}
                      >
                        <option value="FIFO">FIFO</option>
                        <option value="LIFO">LIFO</option>
                        <option value="Weighted Average">Weighted Average</option>
                      </select>
                    </div>
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>Allow Negative Stock</h4>
                      <p>Allow billing or stock movement even when available stock is below zero.</p>
                    </div>

                    <input
                      type="checkbox"
                      name="allowNegativeStock"
                      checked={Boolean(formData.allowNegativeStock)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>Low Stock Alert</h4>
                      <p>Enable warnings when product quantity reaches reorder level.</p>
                    </div>

                    <input
                      type="checkbox"
                      name="lowStockAlert"
                      checked={Boolean(formData.lowStockAlert)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.inventory.barcode}</h4>
                      <p>{st.inventory.barcodeDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="barcodeEnabled"
                      checked={Boolean(formData.barcodeEnabled)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.inventory.autoStock}</h4>
                      <p>{st.inventory.autoStockDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="autoStockUpdate"
                      checked={Boolean(formData.autoStockUpdate)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>
                </div>
              )}

              {activeTab === "roles" && (
                <div className="settings-panel">
                  <h3>{st.roles.title}</h3>

                  {loadingRoles && (
                    <div className="settings-success-message">Loading roles...</div>
                  )}

                  {rolesError && (
                    <div className="settings-api-error page-error-banner" role="alert">{rolesError}</div>
                  )}

                  {!loadingRoles &&
                    roles.map((role) => (
                      <div className="role-card" key={role.id}>
                        <div>
                          <h4>
                            {role.name === "Admin"
                              ? st.roles.adminRole
                              : role.name === "Manager"
                                ? st.roles.managerRole
                                : role.name === "Staff"
                                  ? st.roles.staffRole
                                  : role.name}
                          </h4>
                          <p>
                            {role.description ||
                              (role.name === "Admin"
                                ? st.roles.adminDesc
                                : role.name === "Manager"
                                  ? st.roles.managerDesc
                                  : role.name === "Staff"
                                    ? st.roles.staffDesc
                                    : "Role access configuration")}
                          </p>
                        </div>
                        <button
                          type="button"
                          className="settings-outline-btn"
                          onClick={() => handleToggleRoleStatus(role)}
                          disabled={loadingSettings || savingSettings || loadingRoles || Boolean(roleActionId)}
                        >
                          {roleActionId === role.id
                            ? "Updating..."
                            : role.isActive
                              ? st.roles.active || "Active"
                              : "Inactive"}
                        </button>
                      </div>
                    ))}

                  {user?.role === "Admin" && (
                    <button
                      className="settings-outline-btn"
                      onClick={() => setShowManagePermissions(true)}
                      disabled={loadingSettings || savingSettings}
                    >
                      {st.roles.managePermissions}
                    </button>
                  )}
                </div>
              )}

              {activeTab === "notifications" && (
                <div className="settings-panel">
                  <h3>{st.notifications.title}</h3>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>Email Notifications</h4>
                      <p>Send important IMS notifications through email.</p>
                    </div>

                    <input
                      type="checkbox"
                      name="emailNotifications"
                      checked={Boolean(formData.emailNotifications)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.notifications.lowStockAlerts}</h4>
                      <p>{st.notifications.lowStockAlertsDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="lowStockAlerts"
                      checked={Boolean(formData.lowStockAlerts)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.notifications.orderNotifications}</h4>
                      <p>{st.notifications.orderNotificationsDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="orderNotifications"
                      checked={Boolean(formData.orderNotifications)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.notifications.supplierPayment}</h4>
                      <p>{st.notifications.supplierPaymentDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="paymentReminder"
                      checked={Boolean(formData.paymentReminder)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>System Alerts</h4>
                      <p>Show important system and security alerts inside IMS.</p>
                    </div>

                    <input
                      type="checkbox"
                      name="systemAlerts"
                      checked={Boolean(formData.systemAlerts)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>
                </div>
              )}



              {activeTab === "security" && (
                <div className="settings-panel">
                  <h3>{st.security.title}</h3>

                  <button
                    className="security-action-btn"
                    onClick={() => setShowChangePassword(true)}
                    disabled={loadingSettings || savingSettings}
                  >
                    <Lock size={17} />
                    {st.security.changePassword}
                  </button>

                  <button
                    className="security-action-btn"
                    onClick={() => setShowLoginHistory(true)}
                    disabled={loadingSettings || savingSettings}
                  >
                    <History size={17} />
                    {st.security.viewLoginHistory}
                  </button>

                  <div className="settings-toggle-row">
                    <div>
                      <h4>{st.security.twoStep}</h4>
                      <p>{st.security.twoStepDesc}</p>
                    </div>

                    <input
                      type="checkbox"
                      name="twoStepVerification"
                      checked={Boolean(formData.twoStepVerification)}
                      onChange={handleInputChange}
                      disabled={loadingSettings || savingSettings}
                    />
                  </div>
                </div>
              )}
            </section>
          </div>

          <div className="settings-footer">
            <div className="settings-footer-sidebar">
              <button
                type="button"
                className="settings-sidebar-bottom-toggle"
                onClick={handleToggleSettingsSidebar}
                title={
                  isSettingsSidebarCollapsed
                    ? "Expand settings sidebar"
                    : "Collapse settings sidebar"
                }
                disabled={loadingSettings || savingSettings}
              >
                {isSettingsSidebarCollapsed ? (
                  <PanelLeftOpen size={18} />
                ) : (
                  <PanelLeftClose size={18} />
                )}

                <span className="settings-tab-label">
                  {isSettingsSidebarCollapsed ? "Expand" : "Collapse"}
                </span>
              </button>
            </div>

            <div className="settings-footer-actions">
              <button
                className="settings-final-save-btn"
                onClick={handleSaveSettings}
                disabled={savingSettings || uploadingLogo || loadingSettings}
              >
                {savingSettings ? "Saving..." : st.saveChanges}
              </button>

              <button
                className="settings-cancel-btn"
                onClick={onClose}
                disabled={savingSettings || uploadingLogo || loadingSettings}
              >
                {st.cancel}
              </button>
            </div>
          </div>
        </div>
      </div>

      {createPortal(
        <>
          {showChangePassword && (
            <ChangePassword
              settingsData={formData}
              t={t}
              onClose={() => setShowChangePassword(false)}
            />
          )}

          {showLoginHistory && (
            <LoginHistory
              settingsData={formData}
              t={t}
              onClose={() => setShowLoginHistory(false)}
            />
          )}

          {showManagePermissions && (
            <ManagePermissions
              settingsData={formData}
              t={t}
              onClose={() => setShowManagePermissions(false)}
            />
          )}
        </>,
        document.body
      )}
    </>
  );
}

export default AdminSettings;

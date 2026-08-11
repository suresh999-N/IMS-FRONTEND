import { useEffect, useState } from "react";
import { LogOut, MonitorOff, X } from "lucide-react";
import { logoutAllDevices, logoutCurrentSession } from "../../api/authApi";
import loginLeftPanel from "../../assets/auth/login-left-panel.png";
import "./LogoutConfirm.css";

const logoutText = {
  english: {
    title: "Logout Confirmation",
    message: "Are you sure you want to logout from IMS Admin?",
    sessionTitle: "IMS Admin Session",
    sessionDesc: "Your current login session will be closed securely.",
    cancel: "Cancel",
    logout: "Logout",
    logoutAll: "Logout All Devices",
    loggingOut: "Logging out...",
    loggingOutAll: "Logging out all devices...",
    success: "Logged out successfully.",
    allSuccess: "Logged out from all devices successfully.",
    failed: "Logout failed. Please try again.",
    allFailed: "Logout all devices failed. Please try again.",
    unauthorized: "Session expired. Logging out locally.",
  },
  telugu: {
    title: "లాగౌట్ నిర్ధారణ",
    message: "మీరు IMS Admin నుండి లాగౌట్ కావాలనుకుంటున్నారా?",
    sessionTitle: "IMS అడ్మిన్ సెషన్",
    sessionDesc: "మీ ప్రస్తుత లాగిన్ సెషన్ సురక్షితంగా మూసివేయబడుతుంది.",
    cancel: "రద్దు",
    logout: "లాగౌట్",
    logoutAll: "అన్ని డివైస్‌ల నుండి లాగౌట్",
    loggingOut: "లాగౌట్ అవుతోంది...",
    loggingOutAll: "అన్ని డివైస్‌ల నుండి లాగౌట్ అవుతోంది...",
    success: "విజయవంతంగా లాగౌట్ అయ్యారు.",
    allSuccess: "అన్ని డివైస్‌ల నుండి విజయవంతంగా లాగౌట్ అయ్యారు.",
    failed: "లాగౌట్ కాలేదు. మళ్లీ ప్రయత్నించండి.",
    allFailed: "అన్ని డివైస్‌ల లాగౌట్ కాలేదు. మళ్లీ ప్రయత్నించండి.",
    unauthorized: "సెషన్ ముగిసింది. లోకల్‌గా లాగౌట్ అవుతోంది.",
  },
  hindi: {
    title: "Logout Confirmation",
    message: "क्या आप IMS Admin से logout करना चाहते हैं?",
    sessionTitle: "IMS Admin Session",
    sessionDesc: "आपका current login session securely close हो जाएगा.",
    cancel: "Cancel",
    logout: "Logout",
    logoutAll: "Logout All Devices",
    loggingOut: "Logging out...",
    loggingOutAll: "Logging out all devices...",
    success: "Logged out successfully.",
    allSuccess: "Logged out from all devices successfully.",
    failed: "Logout failed. Please try again.",
    allFailed: "Logout all devices failed. Please try again.",
    unauthorized: "Session expired. Logging out locally.",
  },
};

function LogoutConfirm({ settingsData, user, onCancel, onLogout }) {
  const [loadingType, setLoadingType] = useState("");
  const [apiError, setApiError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const lang = settingsData?.language || "english";
  const l = logoutText[lang] || logoutText.english;

  const userId = user?.id ?? user?.userId;
  const loading = Boolean(loadingType);

  useEffect(() => {
    const handleEscape = (event) => {
      if (event.key === "Escape" && !loading) {
        onCancel();
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [onCancel, loading]);

  const finishLogout = () => {
    setTimeout(() => {
      if (typeof onLogout === "function") {
        onLogout();
      }
    }, 500);
  };

  const callLogoutApi = async ({ type }) => {
    const isAllDevices = type === "all";

    const failedMessage = isAllDevices ? l.allFailed : l.failed;
    const successText = isAllDevices ? l.allSuccess : l.success;

    try {
      setLoadingType(type);
      setApiError("");
      setSuccessMessage("");

      const result = isAllDevices
        ? await logoutAllDevices(userId)
        : await logoutCurrentSession(userId);

      if (!result.success) {
        setApiError(result.error || failedMessage);
        return;
      }

      setSuccessMessage(result.message || successText);
      finishLogout();
    } catch (error) {
      console.error("Logout network/fetch error:", error);
      setApiError(error instanceof Error ? error.message : failedMessage);
    } finally {
      setLoadingType("");
    }
  };

  const handleLogoutCurrentSession = () => {
    callLogoutApi({ type: "current" });
  };

  const handleLogoutAllDevices = () => {
    callLogoutApi({ type: "all" });
  };

  return (
    <div className="logout-overlay" onClick={!loading ? onCancel : undefined}>
      <div
        className="logout-modal"
        onClick={(event) => event.stopPropagation()}
      >
        <div
          className="logout-auth-art"
          style={{ backgroundImage: `url(${loginLeftPanel})` }}
          aria-hidden="true"
        />

        <section className="logout-auth-panel">
          <div className="logout-auth-card">
            <div className="logout-icon">
              <LogOut size={29} />
            </div>

            <h2>{l.title}</h2>
            <p>{l.message}</p>

            {successMessage && (
              <div className="logout-api-success">{successMessage}</div>
            )}

            {apiError && <div className="logout-api-error page-error-banner" role="alert">{apiError}</div>}

            <div className="logout-info-box">
              <strong>{l.sessionTitle}</strong>
              <span>{l.sessionDesc}</span>
            </div>

            <div className="logout-actions">
              <button
                className="logout-confirm-btn"
                type="button"
                onClick={handleLogoutCurrentSession}
                disabled={loading}
              >
                <LogOut size={18} />
                {loadingType === "current" ? l.loggingOut : l.logout}
              </button>

              <button
                className="logout-confirm-all-btn"
                type="button"
                onClick={handleLogoutAllDevices}
                disabled={loading}
              >
                <MonitorOff size={18} />
                {loadingType === "all" ? l.loggingOutAll : l.logoutAll}
              </button>

              <button
                className="logout-cancel-btn"
                type="button"
                onClick={onCancel}
                disabled={loading}
              >
                <X size={18} />
                {l.cancel}
              </button>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}

export default LogoutConfirm;

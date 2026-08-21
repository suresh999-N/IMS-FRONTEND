import {
  BarChart3,
  Boxes,
  Check,
  Eye,
  EyeOff,
  LineChart,
  LockKeyhole,
  Mail,
  PackageCheck,
  Phone,
  ShieldCheck,
  ShoppingCart,
  User,
  UserPlus,
  X,
} from "lucide-react";
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { apiRequest } from "../../api/apiClient";
import { API_ENDPOINTS } from "../../api/endpoints";
import loginLeftPanel from "../../assets/auth/login-left-panel.png";
import {
  emailInputProps,
  getEmailError,
  sanitizeEmailInput,
} from "../../validators/emailValidator";
import {
  getNameError,
  nameInputProps,
  sanitizeNameInput,
} from "../../validators/nameValidator";
import {
  getPhoneError,
  phoneInputProps,
  sanitizePhoneInput,
} from "../../validators/phoneValidator";
import {
  checkPasswordRequirements,
  getPasswordError,
} from "../../validators/passwordValidator";
import { useAuth } from "../../hooks/useAuth";
import { getAuthErrorMessage } from "./authCopy";
import "./Auth.css";

export function getConfirmPasswordError(password, confirmPassword) {
  if (!confirmPassword) return "Confirm password is required.";
  if (password !== confirmPassword) return "Passwords do not match.";
  return "";
}

export default function Register() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
  });
  const [touched, setTouched] = useState({
    name: false,
    email: false,
    phoneNumber: false,
    password: false,
    confirmPassword: false,
  });
  const [serverFieldErrors, setServerFieldErrors] = useState({});
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [passwordFocused, setPasswordFocused] = useState(false);

  // Compute field errors dynamically
  const nameError = getNameError(formData.name, { required: true, label: "Full Name" });
  const emailError = getEmailError(formData.email, { required: true });
  const phoneError = getPhoneError(formData.phoneNumber, "Mobile number");
  const passwordError = getPasswordError(formData.password);
  const confirmPasswordError = getConfirmPasswordError(formData.password, formData.confirmPassword);
  const passwordChecks = checkPasswordRequirements(formData.password);

  const isFormValid =
    formData.name.trim() !== "" &&
    formData.email.trim() !== "" &&
    formData.phoneNumber.trim() !== "" &&
    formData.password !== "" &&
    formData.confirmPassword !== "" &&
    !nameError &&
    !emailError &&
    !phoneError &&
    !passwordError &&
    !confirmPasswordError;

  function handleChange(event) {
    const { name, value } = event.target;
    setError("");
    setServerFieldErrors((prev) => ({ ...prev, [name]: "" }));
    setTouched((prev) => ({ ...prev, [name]: true }));
    setFormData((prev) => ({
      ...prev,
      [name]:
        name === "email"
          ? sanitizeEmailInput(value)
          : name === "phoneNumber"
            ? sanitizePhoneInput(value)
            : name === "name"
              ? sanitizeNameInput(value)
              : value,
    }));
  }

  function handleBlur(event) {
    const { name } = event.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    if (loading) return;
    setError("");

    setTouched({
      name: true,
      email: true,
      phoneNumber: true,
      password: true,
      confirmPassword: true,
    });

    const clientErrors = [
      { field: 'Full Name', key: 'name', error: nameError },
      { field: 'Email Address', key: 'email', error: emailError },
      { field: 'Mobile Number', key: 'phoneNumber', error: phoneError },
      { field: 'Password', key: 'password', error: passwordError },
      { field: 'Confirm Password', key: 'confirmPassword', error: confirmPasswordError },
    ].filter(item => Boolean(item.error));

    if (clientErrors.length > 0) {
      const fieldNames = clientErrors.map(item => item.field);
      const detailedMessages = clientErrors.map(item => `${item.field}: ${item.error}`);

      setError(
        clientErrors.length === 1
          ? `Please fix ${clientErrors[0].field}: ${clientErrors[0].error}`
          : `Please check the highlighted fields: ${fieldNames.join(", ")}.`
      );
      return;
    }

    try {
      setLoading(true);
      const result = await apiRequest(API_ENDPOINTS.auth.register, {
        method: "POST",
        body: {
          name: formData.name.trim(),
          email: sanitizeEmailInput(formData.email),
          phoneNumber: sanitizePhoneInput(formData.phoneNumber),
          password: formData.password,
          confirmPassword: formData.confirmPassword,
        },
      });

      if (!result.success) {
        const rawErrors = result.errors || {};
        const fieldErrorMap = {};
        const messages = [];

        if (typeof rawErrors === "object" && rawErrors !== null) {
          Object.entries(rawErrors).forEach(([key, val]) => {
            const msg = Array.isArray(val) ? val.join(" ") : String(val);
            if (!msg) return;

            const lowerKey = key.toLowerCase();
            let fieldLabel = key;
            if (lowerKey.includes("name")) { fieldErrorMap.name = msg; fieldLabel = "Full Name"; }
            else if (lowerKey.includes("email")) { fieldErrorMap.email = msg; fieldLabel = "Email Address"; }
            else if (lowerKey.includes("phone") || lowerKey.includes("mobile")) { fieldErrorMap.phoneNumber = msg; fieldLabel = "Mobile Number"; }
            else if (lowerKey.includes("confirmpassword")) { fieldErrorMap.confirmPassword = msg; fieldLabel = "Confirm Password"; }
            else if (lowerKey.includes("password")) { fieldErrorMap.password = msg; fieldLabel = "Password"; }

            messages.push(`${fieldLabel}: ${msg}`);
          });
        }

        setServerFieldErrors(fieldErrorMap);

        const rawErrStr = String(result.error || result.message || '');
        let specificError = messages.join(" | ");

        if (!specificError) {
          if (/email/i.test(rawErrStr)) {
            specificError = "Email Address: Email address is already registered or invalid.";
            setServerFieldErrors(prev => ({ ...prev, email: "Email address is already registered or invalid." }));
          } else if (/phone|mobile/i.test(rawErrStr)) {
            specificError = "Mobile Number: Mobile number is already registered or invalid.";
            setServerFieldErrors(prev => ({ ...prev, phoneNumber: "Mobile number is already registered or invalid." }));
          } else if (/password/i.test(rawErrStr)) {
            specificError = "Password: Password does not meet security requirements.";
            setServerFieldErrors(prev => ({ ...prev, password: "Password does not meet security requirements." }));
          } else {
            specificError = rawErrStr ? `Registration Error: ${rawErrStr}` : "Account creation failed. Please check the highlighted fields.";
          }
        }

        setError(specificError);
        return;
      }

      const userEmail = sanitizeEmailInput(formData.email);
      try {
        localStorage.removeItem("ims-email-verification-completed");
      } catch {}

      navigate(
        `/verify-email?email=${encodeURIComponent(userEmail)}`,
        { replace: true },
      );
    } catch (err) {
      const msg = err?.message || ''
      if (/network|fetch|timeout|aborted|ERR_/i.test(msg)) {
        setError("Unable to connect to the server. Please check your internet connection and try again.")
      } else {
        setError("Something went wrong while creating your account. Please try again.")
      }
    } finally {
      setLoading(false);
    }
  }

  const nameDisplayError = serverFieldErrors.name || (touched.name && nameError);
  const emailDisplayError = serverFieldErrors.email || (touched.email && emailError);
  const phoneDisplayError = serverFieldErrors.phoneNumber || (touched.phoneNumber && phoneError);
  const passwordDisplayError = serverFieldErrors.password || (touched.password && passwordError);
  const confirmPasswordDisplayError =
    (touched.confirmPassword || Boolean(formData.confirmPassword)) && confirmPasswordError;

  return (
    <div
      className="auth-wrapper auth-wrapper--login auth-wrapper--register"
      style={{ "--auth-login-left-panel": `url(${loginLeftPanel})` }}
    >
      <div className="auth-left-panel">
        <div className="auth-login-brand">
          <span className="auth-login-brand__mark" aria-hidden="true"><Boxes size={30} /></span>
          <strong>IMS</strong>
          <span className="auth-login-brand__divider" aria-hidden="true" />
          <span>Inventory<br /><em>Management System</em></span>
        </div>

        <div className="auth-login-message">
          <h1>Smart <span>Inventory.</span><br />Better Control.<br />Stronger Business.</h1>
          <i aria-hidden="true" />
          <p>IMS helps you manage stock, track transactions, and streamline operations efficiently in one place.</p>
        </div>

        <div className="auth-login-visual" aria-hidden="true">
          <div className="auth-login-dashboard">
            <div className="auth-login-dashboard__bar"><span /><span /><span /></div>
            <div className="auth-login-dashboard__stats"><b>Stock</b><b>Sales</b><b>Orders</b></div>
            <LineChart size={104} />
          </div>
          <div className="auth-login-visual__tile auth-login-visual__tile--stock"><PackageCheck /><span>Stock<br />Management</span></div>
          <div className="auth-login-visual__tile auth-login-visual__tile--sales"><BarChart3 /><span>Sales<br />Management</span></div>
          <div className="auth-login-visual__tile auth-login-visual__tile--purchase"><ShoppingCart /><span>Purchase<br />Management</span></div>
        </div>

        <div className="features auth-login-features">
          <div><PackageCheck size={17} /> Real-time Tracking</div>
          <div><ShieldCheck size={17} /> Secure &amp; Reliable</div>
          <div><BarChart3 size={17} /> Insightful Reports</div>
        </div>
      </div>

      <div className="auth-right-panel">
        <div className="login-card">
          <div className="auth-login-lock" aria-hidden="true">
            <UserPlus size={29} />
          </div>
          <h2>Create Account</h2>
          <p className="sub">Create your IMS workspace account</p>

          {error && (
            <div className="error-box auth-register-error" role="alert">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} noValidate className="auth-register-form">
            <div className="register-field-group">
              <label className="auth-login-label" htmlFor="register-name">
                Full Name
              </label>
              <div className={`input-box ${nameDisplayError ? "input-box--error" : ""}`}>
                <User size={16} />
                <input
                  id="register-name"
                  {...nameInputProps}
                  name="name"
                  placeholder="Full name"
                  value={formData.name}
                  onChange={handleChange}
                  maxLength={50}
                  onBlur={handleBlur}
                  autoComplete="name"
                />
              </div>
              {nameDisplayError && (
                <span className="field-error-text">{nameDisplayError}</span>
              )}
            </div>

            <div className="register-field-group">
              <label className="auth-login-label" htmlFor="register-email">
                Email Address
              </label>
              <div className={`input-box ${emailDisplayError ? "input-box--error" : ""}`}>
                <Mail size={16} />
                <input
                  id="register-email"
                  {...emailInputProps}
                  name="email"
                  placeholder="Email address"
                  value={formData.email}
                  onChange={handleChange}
                  onBlur={handleBlur}
                />
              </div>
              {emailDisplayError && (
                <span className="field-error-text">{emailDisplayError}</span>
              )}
            </div>

            <div className="register-field-group">
              <label className="auth-login-label" htmlFor="register-phone">
                Mobile Number
              </label>
              <div className={`input-box ${phoneDisplayError ? "input-box--error" : ""}`}>
                <Phone size={16} />
                <input
                  id="register-phone"
                  {...phoneInputProps}
                  name="phoneNumber"
                  placeholder="10-digit mobile number"
                  value={formData.phoneNumber}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  autoComplete="tel"
                />
              </div>
              {phoneDisplayError && (
                <span className="field-error-text">{phoneDisplayError}</span>
              )}
            </div>

            <div className="register-field-group">
              <label className="auth-login-label" htmlFor="register-password">
                Password
              </label>
              <div className={`input-box ${passwordDisplayError ? "input-box--error" : ""}`}>
                <LockKeyhole size={16} />
                <input
                  id="register-password"
                  type={showPassword ? "text" : "password"}
                  name="password"
                  placeholder="Password"
                  value={formData.password}
                  onChange={handleChange}
                  onFocus={() => setPasswordFocused(true)}
                  onBlur={(e) => {
                    handleBlur(e);
                    setPasswordFocused(false);
                  }}
                  autoComplete="new-password"
                />
                {formData.password ? (
                  <button
                    type="button"
                    className="auth-login-password-toggle"
                    onClick={(e) => {
                      e.preventDefault();
                      e.stopPropagation();
                      setShowPassword((current) => !current);
                    }}
                    aria-label={showPassword ? "Hide password" : "Show password"}
                    tabIndex={-1}
                  >
                    {showPassword ? <Eye size={17} /> : <EyeOff size={17} />}
                  </button>
                ) : null}
              </div>
              {(passwordFocused || formData.password.length > 0) && (
                <div className="password-requirements-panel" aria-live="polite">
                  <span className="password-requirements-title">Password must contain:</span>
                  <ul className="password-requirements-list">
                    <li className={`password-requirement-item ${passwordChecks.minLength ? "is-valid" : "is-invalid"}`}>
                      {passwordChecks.minLength ? <Check size={13} /> : <X size={13} />}
                      <span>At least 8 characters</span>
                    </li>
                    <li className={`password-requirement-item ${passwordChecks.uppercase ? "is-valid" : "is-invalid"}`}>
                      {passwordChecks.uppercase ? <Check size={13} /> : <X size={13} />}
                      <span>At least one uppercase letter (A-Z)</span>
                    </li>
                    <li className={`password-requirement-item ${passwordChecks.lowercase ? "is-valid" : "is-invalid"}`}>
                      {passwordChecks.lowercase ? <Check size={13} /> : <X size={13} />}
                      <span>At least one lowercase letter (a-z)</span>
                    </li>
                    <li className={`password-requirement-item ${passwordChecks.number ? "is-valid" : "is-invalid"}`}>
                      {passwordChecks.number ? <Check size={13} /> : <X size={13} />}
                      <span>At least one number (0-9)</span>
                    </li>
                    <li className={`password-requirement-item ${passwordChecks.special ? "is-valid" : "is-invalid"}`}>
                      {passwordChecks.special ? <Check size={13} /> : <X size={13} />}
                      <span>At least one special character (!@#$%^&amp;*)</span>
                    </li>
                  </ul>
                </div>
              )}
              {passwordDisplayError && (
                <span className="field-error-text">{passwordDisplayError}</span>
              )}
            </div>

            <div className="register-field-group">
              <label
                className="auth-login-label"
                htmlFor="register-confirm-password"
              >
                Confirm Password
              </label>
              <div className={`input-box ${confirmPasswordDisplayError ? "input-box--error" : ""}`}>
                <LockKeyhole size={16} />
                <input
                  id="register-confirm-password"
                  type={showConfirmPassword ? "text" : "password"}
                  name="confirmPassword"
                  placeholder="Confirm password"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  autoComplete="new-password"
                />
                {formData.confirmPassword ? (
                  <button
                    type="button"
                    className="auth-login-password-toggle"
                    onClick={(e) => {
                      e.preventDefault();
                      e.stopPropagation();
                      setShowConfirmPassword((current) => !current);
                    }}
                    aria-label={
                      showConfirmPassword ? "Hide password" : "Show password"
                    }
                    tabIndex={-1}
                  >
                    {showConfirmPassword ? <Eye size={17} /> : <EyeOff size={17} />}
                  </button>
                ) : null}
              </div>
              {confirmPasswordDisplayError && (
                <span className="field-error-text">{confirmPasswordDisplayError}</span>
              )}
            </div>

            <div className="links auth-register-login-link">
              <span>Already have an account?</span>
              <Link to="/login">Login</Link>
            </div>

            <button type="submit" disabled={loading || (Object.values(touched).every(Boolean) && !isFormValid)}>
              <UserPlus size={18} />
              {loading ? "Creating..." : "Create account"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

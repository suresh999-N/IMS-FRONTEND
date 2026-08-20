import {
  BarChart3,
  Boxes,
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
import { useAuth } from "../../hooks/useAuth";
import { getAuthErrorMessage } from "./authCopy";
import "./Auth.css";

export function getPasswordError(password) {
  if (!password) return "Password is required.";
  if (password.length < 8) return "Password must be at least 8 characters.";
  if (!/[A-Z]/.test(password)) return "Password must include at least one uppercase letter.";
  if (!/[a-z]/.test(password)) return "Password must include at least one lowercase letter.";
  if (!/[0-9!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) return "Password must include at least one number or symbol.";
  return "";
}

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

  // Compute field errors dynamically
  const nameError = getNameError(formData.name, { required: true, label: "Full Name" });
  const emailError = getEmailError(formData.email, { required: true });
  const phoneError = getPhoneError(formData.phoneNumber, "Mobile number");
  const passwordError = getPasswordError(formData.password);
  const confirmPasswordError = getConfirmPasswordError(formData.password, formData.confirmPassword);

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

  const isFormCompletelyEmpty = Object.values(formData).every(
    (value) => value.trim() === "",
  );

  function handleChange(event) {
    const { name, value } = event.target;
    setError("");
    setServerFieldErrors((prev) => ({ ...prev, [name]: "" }));
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

    if (!isFormValid) {
      const validationErrors = [
        nameError,
        emailError,
        phoneError,
        passwordError,
        confirmPasswordError,
      ].filter(Boolean);
      setError(
        isFormCompletelyEmpty
          ? "Please fill in all required fields."
          : validationErrors.length === 1
            ? validationErrors[0]
            : "Please correct the highlighted fields.",
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
            messages.push(msg);
            const lowerKey = key.toLowerCase();
            if (lowerKey.includes("name")) fieldErrorMap.name = msg;
            if (lowerKey.includes("email")) fieldErrorMap.email = msg;
            if (lowerKey.includes("phone")) fieldErrorMap.phoneNumber = msg;
            if (lowerKey.includes("password")) fieldErrorMap.password = msg;
          });
        }

        setServerFieldErrors(fieldErrorMap);

        const specificError = messages.length > 0
          ? messages.join(" ")
          : getAuthErrorMessage(
              result.error || result.message,
              "We could not create the account. Review the form and try again.",
            );

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

  const nameFieldError = serverFieldErrors.name || (touched.name && nameError);
  const emailFieldError = serverFieldErrors.email || (touched.email && emailError);
  const phoneFieldError = serverFieldErrors.phoneNumber || (touched.phoneNumber && phoneError);
  const passwordFieldError = serverFieldErrors.password || (touched.password && passwordError);
  const confirmPasswordFieldError = touched.confirmPassword && confirmPasswordError;

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
          <h2>Create Account</h2>

          <p
            className={`auth-register-error ${error ? "is-visible" : ""}`}
            role={error ? "alert" : undefined}
            aria-hidden={!error}
          >
            {error || "\u00A0"}
          </p>

          <form onSubmit={handleSubmit} noValidate>
            <div className="auth-register-field">
              <label className="auth-login-label" htmlFor="register-name">
                Full Name
              </label>
              <div className={`input-box ${nameFieldError ? "input-box--error" : ""}`}>
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
            </div>

            <div className="auth-register-field">
              <label className="auth-login-label" htmlFor="register-email">
                Email Address
              </label>
              <div className={`input-box ${emailFieldError ? "input-box--error" : ""}`}>
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
            </div>

            <div className="auth-register-field">
              <label className="auth-login-label" htmlFor="register-phone">
                Mobile Number
              </label>
              <div className={`input-box ${phoneFieldError ? "input-box--error" : ""}`}>
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
            </div>

            <div className="auth-register-field">
              <label className="auth-login-label" htmlFor="register-password">
                Password
              </label>
              <div className={`input-box ${passwordFieldError ? "input-box--error" : ""}`}>
                <LockKeyhole size={16} />
                <input
                  id="register-password"
                  type={showPassword ? "text" : "password"}
                  name="password"
                  placeholder="Password"
                  value={formData.password}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  autoComplete="new-password"
                />
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
                  {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
                </button>
              </div>
            </div>

            <div className="auth-register-field">
              <label
                className="auth-login-label"
                htmlFor="register-confirm-password"
              >
                Confirm Password
              </label>
              <div className={`input-box ${confirmPasswordFieldError ? "input-box--error" : ""}`}>
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
                  {showConfirmPassword ? <EyeOff size={17} /> : <Eye size={17} />}
                </button>
              </div>
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

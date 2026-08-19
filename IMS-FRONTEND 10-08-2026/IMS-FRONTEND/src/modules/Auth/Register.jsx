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
import { getAuthErrorMessage } from "./authCopy";
import "./Auth.css";

export default function Register() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  function handleChange(event) {
    const { name, value } = event.target;
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

  async function handleSubmit(event) {
    event.preventDefault();
    if (loading) return;
    setError("");

    if (
      !formData.name.trim() ||
      !formData.email.trim() ||
      !formData.phoneNumber ||
      !formData.password
    ) {
      setError("Complete the required fields.");
      return;
    }

    const nameError = getNameError(formData.name, { required: true, label: "Full Name" });
    if (nameError) {
      setError(nameError);
      return;
    }

    const emailError = getEmailError(formData.email, { required: true });
    if (emailError) {
      setError(emailError);
      return;
    }

    const phoneError = getPhoneError(formData.phoneNumber, "Mobile number");
    if (phoneError) {
      setError(phoneError);
      return;
    }

    if (formData.password.length < 8) {
      setError("Use at least 8 characters.");
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match.");
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
        setError(
          getAuthErrorMessage(
            result.error,
            "We could not create the account. Try again.",
          ),
        );
        return;
      }

      navigate(
        `/verify-email?email=${encodeURIComponent(sanitizeEmailInput(formData.email))}`,
        { replace: true },
      );
    } catch {
      setError("Unable to connect to the server.");
    } finally {
      setLoading(false);
    }
  }

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

          <div
            className={`error-box auth-register-error ${error ? "is-visible" : ""}`}
            role={error ? "alert" : undefined}
            aria-hidden={!error}
          >
            {error || "\u00A0"}
          </div>

          <form onSubmit={handleSubmit}>
            <label className="auth-login-label" htmlFor="register-name">
              Full Name
            </label>
            <div className="input-box">
              <User size={16} />
              <input
                id="register-name"
                type="text"
                name="name"
                placeholder="Full name"
                value={formData.name}
                onChange={handleChange}
                maxLength={50}
                autoComplete="name"
              />
            </div>

            <label className="auth-login-label" htmlFor="register-email">
              Email Address
            </label>
            <div className="input-box">
              <Mail size={16} />
              <input
                id="register-email"
                {...emailInputProps}
                name="email"
                placeholder="Email address"
                value={formData.email}
                onChange={handleChange}
              />
            </div>

            <label className="auth-login-label" htmlFor="register-phone">
              Mobile Number
            </label>
            <div className="input-box">
              <Phone size={16} />
              <input
                id="register-phone"
                {...phoneInputProps}
                name="phoneNumber"
                placeholder="10-digit mobile number"
                value={formData.phoneNumber}
                onChange={handleChange}
                autoComplete="tel"
                required
              />
            </div>

            <label className="auth-login-label" htmlFor="register-password">
              Password
            </label>
            <div className="input-box">
              <LockKeyhole size={16} />
              <input
                id="register-password"
                type={showPassword ? "text" : "password"}
                name="password"
                placeholder="Password"
                value={formData.password}
                onChange={handleChange}
                autoComplete="new-password"
                required
              />
              <button
                type="button"
                className="auth-login-password-toggle"
                onClick={() => setShowPassword((current) => !current)}
                aria-label={showPassword ? "Hide password" : "Show password"}
              >
                {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>

            <label
              className="auth-login-label"
              htmlFor="register-confirm-password"
            >
              Confirm Password
            </label>
            <div className="input-box">
              <LockKeyhole size={16} />
              <input
                id="register-confirm-password"
                type={showConfirmPassword ? "text" : "password"}
                name="confirmPassword"
                placeholder="Confirm password"
                value={formData.confirmPassword}
                onChange={handleChange}
                autoComplete="new-password"
                required
              />
              <button
                type="button"
                className="auth-login-password-toggle"
                onClick={() => setShowConfirmPassword((current) => !current)}
                aria-label={
                  showConfirmPassword ? "Hide password" : "Show password"
                }
              >
                {showConfirmPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>

            <div className="links auth-register-login-link">
              <span>Already have an account?</span>
              <Link to="/login">Login</Link>
            </div>

            <button type="submit" disabled={loading}>
              <UserPlus size={18} />
              {loading ? "Creating..." : "Create account"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

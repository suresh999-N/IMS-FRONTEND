import {
  BarChart3,
  Boxes,
  Eye,
  EyeOff,
  LineChart,
  LockKeyhole,
  LogIn,
  Mail,
  PackageCheck,
  Phone,
  ShieldCheck,
  ShoppingCart,
} from 'lucide-react'
import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import loginLeftPanel from '../../assets/auth/login-left-panel.png'
import {
  getEmailError,
  sanitizeEmailInput,
} from '../../validators/emailValidator'
import { getAuthErrorMessage } from './authCopy'
import './Auth.css'
 
export default function Login() {
  const navigate = useNavigate()
  const location = useLocation()
  const { login } = useAuth()
  const verificationMessage = location.state?.verificationMessage || ''
  const verificationNotice = location.state?.verificationNotice || ''
 
  const [formData, setFormData] = useState({
    email: location.state?.email || '',
    password: '',
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
 
  function handleChange(event) {
    const { name, value } = event.target
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'email' ? (value.includes('@') ? sanitizeEmailInput(value) : value) : value,
    }))
  }
 
  async function handleSubmit(event) {
    event.preventDefault()
    if (loading) return
    setError('')
 
    const identifier = formData.email.trim()
    const isPhone = /^[0-9+\s-()]+$/.test(identifier) && identifier.replace(/\D/g, '').length >= 7
 
    if (!identifier || !formData.password) {
      setError('Enter your email or phone number, and password.')
      return
    }
 
    if (!isPhone) {
      const emailError = getEmailError(identifier, { required: true })
      if (emailError) {
        setError(emailError)
        return
      }
    } else {
      const digitsOnly = identifier.replace(/\D/g, '')
      if (digitsOnly.length < 10) {
        setError('Please enter a valid 10-digit phone number.')
        return
      }
    }
 
    try {
      setLoading(true)
     const result = await login({
    emailOrPhone: identifier,
    password: formData.password,
})
 
      if (!result?.success) {
        setError(getAuthErrorMessage(result?.message, 'Check your credentials and password.'))
        return
      }
 
      navigate('/dashboard', { replace: true })
    } catch {
      setError('Unable to connect to the server.')
    } finally {
      setLoading(false)
    }
  }
 
  return (
    <div
      className="auth-wrapper auth-wrapper--login"
      style={{ '--auth-login-left-panel': `url(${loginLeftPanel})` }}
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
          <div className="auth-login-lock" aria-hidden="true"><LockKeyhole size={30} /></div>
          <h2>Welcome Back!</h2>
          <p className="sub">Login to access your IMS dashboard</p>
 
          {verificationMessage ? (
            <div className="success-box" role="status">
              {verificationMessage}
            </div>
          ) : null}
          {verificationNotice ? (
            <div className="notice-box" role="status">
              {verificationNotice}
            </div>
          ) : null}
          {error ? <div className="error-box">{error}</div> : null}
 
          <form onSubmit={handleSubmit}>
            <label className="auth-login-label" htmlFor="login-identifier">Email Address or Phone Number</label>
            <div className="input-box">
              {/^[0-9+\s-()]+$/.test(formData.email) ? (
                <Phone size={16} />
              ) : (
                <Mail size={16} />
              )}
              <input
                id="login-identifier"
                type="text"
                name="email"
                placeholder="Email or Phone Number"
                value={formData.email}
                onChange={handleChange}
                autoComplete="username"
                required
              />
            </div>
 
            <label className="auth-login-label" htmlFor="login-password">Password</label>
            <div className="input-box">
              <LockKeyhole size={16} />
              <input
                id="login-password"
                type={showPassword ? 'text' : 'password'}
                name="password"
                placeholder="Password"
                value={formData.password}
                onChange={handleChange}
                autoComplete="current-password"
                required
              />
              <button
                type="button"
                className="auth-login-password-toggle"
                onClick={(e) => {
                  e.preventDefault()
                  e.stopPropagation()
                  setShowPassword((current) => !current)
                }}
                aria-label={showPassword ? 'Hide password' : 'Show password'}
                tabIndex={-1}
              >
                {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>
 
            <div className="links">
              <Link to="/register">Create account</Link>
              <Link to="/forgot-password">Forgot password?</Link>
            </div>
 
            <button type="submit" disabled={loading}>
              <LogIn size={18} />
              {loading ? 'Logging in...' : 'Login'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}

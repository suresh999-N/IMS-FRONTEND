import {
  BarChart3,
  Boxes,
  Eye,
  EyeOff,
  LineChart,
  LockKeyhole,
  PackageCheck,
  ShieldCheck,
  ShoppingCart,
} from 'lucide-react'
import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { apiRequest } from '../../api/apiClient'
import { API_ENDPOINTS } from '../../api/endpoints'
import loginLeftPanel from '../../assets/auth/login-left-panel.png'
import { getAuthErrorMessage } from './authCopy'
import './Auth.css'

export default function ResetPassword() {
  const navigate = useNavigate()
  const location = useLocation()
  const email = location.state?.email || ''
  const [otp, setOtp] = useState(location.state?.otp || '')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!email) {
      setError('Your reset session expired. Request a new code.')
      return
    }

    if (!otp || !password || !confirmPassword) {
      setError('Complete all fields.')
      return
    }

    if (!/^\d{6}$/.test(otp)) {
      setError('Enter the 6-digit code.')
      return
    }

    if (password.length < 8) {
      setError('Use at least 8 characters.')
      return
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    try {
      setLoading(true)
      const result = await apiRequest(API_ENDPOINTS.auth.resetPassword, {
        method: 'POST',
        body: {
          email,
          otp,
          newPassword: password,
        },
      })

      if (!result.success) {
        setError(getAuthErrorMessage(result.error, 'We could not update your password. Try again.'))
        return
      }

      navigate('/login', { replace: true })
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
          <h2>Reset Password</h2>
          <p className="sub">Enter your verification code and new password</p>

          {error ? <div className="error-box">{error}</div> : null}

          <form onSubmit={handleSubmit} autoComplete="off">
            <label className="auth-login-label" htmlFor="reset-otp">Verification Code</label>
            <div className="input-box">
              <ShieldCheck size={16} />
              <input
                id="reset-otp"
                type="text"
                placeholder="6-digit code"
                value={otp}
                onChange={(event) => setOtp(event.target.value.replace(/\D/g, ''))}
                inputMode="numeric"
                maxLength={6}
                autoComplete="one-time-code"
              />
            </div>

            <label className="auth-login-label" htmlFor="reset-password">New Password</label>
            <div className="input-box">
              <LockKeyhole size={16} />
              <input
                id="reset-password"
                type={showPassword ? 'text' : 'password'}
                placeholder="New password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                autoComplete="new-password"
              />
              <button
                type="button"
                className="auth-login-password-toggle"
                onClick={(e) => {
                  e.preventDefault()
                  e.stopPropagation()
                  setShowPassword((prev) => !prev)
                }}
                aria-label={showPassword ? 'Hide password' : 'Show password'}
                tabIndex={-1}
              >
                {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>

            <label className="auth-login-label" htmlFor="reset-confirm-password">Confirm Password</label>
            <div className="input-box">
              <LockKeyhole size={16} />
              <input
                id="reset-confirm-password"
                type={showConfirmPassword ? 'text' : 'password'}
                placeholder="Confirm password"
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
                autoComplete="new-password"
              />
              <button
                type="button"
                className="auth-login-password-toggle"
                onClick={(e) => {
                  e.preventDefault()
                  e.stopPropagation()
                  setShowConfirmPassword((prev) => !prev)
                }}
                aria-label={showConfirmPassword ? 'Hide password' : 'Show password'}
                tabIndex={-1}
              >
                {showConfirmPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>

            <div className="links" style={{ marginBottom: '1rem' }}>
              <Link to="/login">Back to login</Link>
            </div>

            <button type="submit" disabled={loading}>
              <LockKeyhole size={18} />
              {loading ? 'Updating...' : 'Update password'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}

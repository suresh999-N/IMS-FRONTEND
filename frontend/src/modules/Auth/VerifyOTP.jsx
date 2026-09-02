import {
  BarChart3,
  Boxes,
  LineChart,
  LoaderCircle,
  Mail,
  PackageCheck,
  RefreshCw,
  ShieldCheck,
  ShoppingCart,
} from 'lucide-react'
import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { resendLoginOtp, verifyOtp } from '../../api/authApi'
import loginLeftPanel from '../../assets/auth/login-left-panel.png'
import { getAuthErrorMessage } from './authCopy'
import './Auth.css'

function cleanSuccessMessage(rawMessage, fallback = 'A verification code has been sent to your email.') {
  const text = String(rawMessage || '').trim()
  if (!text) return fallback
  if (/registered|exist|if the email/i.test(text)) {
    return 'A verification code has been sent to your email.'
  }
  return text
}

export default function VerifyOTP() {
  const navigate = useNavigate()
  const location = useLocation()
  const email = location.state?.email || ''
  const initialMessage = cleanSuccessMessage(location.state?.message)
  const [otp, setOtp] = useState('')
  const [error, setError] = useState('')
  const [message, setMessage] = useState(initialMessage)
  const [loading, setLoading] = useState(false)
  const [resendLoading, setResendLoading] = useState(false)

  if (!email) {
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
            <div className="auth-login-lock" aria-hidden="true"><ShieldCheck size={30} /></div>
            <h2>Session Expired</h2>
            <p className="sub">Request a new code to continue</p>
            <div className="error-box">No active reset session was found.</div>
            <div className="links">
              <Link to="/forgot-password">Request code</Link>
              <Link to="/login">Sign in</Link>
            </div>
          </div>
        </div>
      </div>
    )
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!/^\d{6}$/.test(otp)) {
      setError('Enter the 6-digit verification code.')
      return
    }

    try {
      setLoading(true)
      const response = await verifyOtp(email, otp)
      if (!response.success && response.status !== 404 && response.status !== 0) {
        setError(getAuthErrorMessage(response.error || response.message, 'Invalid verification code.'))
        return
      }

      navigate('/reset-password', {
        replace: true,
        state: { email, otp },
      })
    } catch {
      navigate('/reset-password', {
        replace: true,
        state: { email, otp },
      })
    } finally {
      setLoading(false)
    }
  }

  async function handleResendCode() {
    setError('')
    setMessage('')
    try {
      setResendLoading(true)
      const response = await resendLoginOtp(email)
      if (response.success) {
        setMessage('A new verification code has been sent to your email.')
      } else {
        setError(response.error || 'Unable to resend verification code.')
      }
    } catch {
      setError('Unable to resend verification code. Please try again.')
    } finally {
      setResendLoading(false)
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
          <div className="auth-login-lock" aria-hidden="true">
            {loading ? <LoaderCircle className="animate-spin" size={30} /> : <ShieldCheck size={30} />}
          </div>
          <h2>Enter Code</h2>
          <p className="sub">Use the 6-digit code sent to your email</p>

          {message ? <div className="success-box">{message}</div> : null}
          {error ? <div className="error-box">{error}</div> : null}

          <form onSubmit={handleSubmit} autoComplete="off">
            <label className="auth-login-label" htmlFor="verify-email">Email Address</label>
            <div className="input-box">
              <Mail size={16} />
              <input id="verify-email" value={email} readOnly disabled />
            </div>

            <label className="auth-login-label" htmlFor="verify-otp">Verification Code</label>
            <div className="input-box">
              <ShieldCheck size={16} />
              <input
                id="verify-otp"
                type="text"
                name="otp-code"
                placeholder="Enter 6-digit code"
                value={otp}
                autoComplete="one-time-code"
                inputMode="numeric"
                onChange={(event) => {
                  setError('')
                  setOtp(event.target.value.replace(/\D/g, ''))
                }}
                maxLength={6}
                autoFocus
              />
            </div>

            <div className="links" style={{ marginBottom: '1rem', justifyContent: 'space-between' }}>
              <Link to="/forgot-password">Back</Link>
              <button
                type="button"
                onClick={handleResendCode}
                disabled={resendLoading}
                style={{ background: 'none', border: 'none', color: 'var(--ims-accent-color, #0284c7)', cursor: 'pointer', fontSize: '0.875rem', padding: 0 }}
              >
                {resendLoading ? 'Resending...' : 'Resend Code'}
              </button>
            </div>

            <button type="submit" disabled={loading || !otp || otp.length < 6}>
              {loading ? <LoaderCircle className="animate-spin" size={18} /> : <ShieldCheck size={18} />}
              {loading ? 'Verifying...' : 'Continue'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}

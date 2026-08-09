import { Link } from 'react-router-dom'
import StateBlock from './StateBlock'

export default function AccessDenied() {
  return (
    <main className="app-fallback" role="alert">
      <div className="app-fallback__panel">
        <StateBlock
          type="permission"
          title="Access unavailable"
          message="Your role does not include this page. Ask an administrator to update your access."
        />
        <Link className="button button-primary" to="/dashboard">
          Back to dashboard
        </Link>
      </div>
    </main>
  )
}

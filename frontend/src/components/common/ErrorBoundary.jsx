import { Component } from 'react'
import StateBlock from './StateBlock'

export default class ErrorBoundary extends Component {
  constructor(props) {
    super(props)
    this.state = { hasError: false, error: null, errorInfo: null }
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error }
  }

  componentDidCatch(error, errorInfo) {
    console.error('Application error boundary caught an error:', error, errorInfo)
    this.setState({ errorInfo })
  }

  handleReload = () => {
    window.location.reload()
  }

  render() {
    if (this.state.hasError) {
      return (
        <main className="app-fallback" role="alert">
          <div className="app-fallback__panel">
            <StateBlock
              type="error"
              title="This view could not load"
              message="Refresh the page and try again. Your workspace data is still safe."
              actionLabel="Refresh"
              onAction={this.handleReload}
            />
            {this.state.error ? (
              <details style={{ marginTop: '1rem', textAlign: 'left', fontSize: '12px', color: '#666', background: '#f8f9fa', padding: '10px', borderRadius: '6px', border: '1px solid #e9ecef', maxWidth: '600px', margin: '1rem auto 0 auto' }}>
                <summary style={{ cursor: 'pointer', fontWeight: 'bold', color: '#dc3545' }}>Error Details</summary>
                <p style={{ color: '#d32f2f', fontWeight: 'bold', margin: '8px 0 4px 0', wordBreak: 'break-word' }}>
                  {String(this.state.error?.message || this.state.error)}
                </p>
                {this.state.error?.stack ? (
                  <pre style={{ fontSize: '11px', whiteSpace: 'pre-wrap', wordBreak: 'break-all', maxHeight: '200px', overflowY: 'auto', background: '#fff', padding: '8px', border: '1px solid #dee2e6', borderRadius: '4px' }}>
                    {this.state.error.stack}
                  </pre>
                ) : null}
              </details>
            ) : null}
          </div>
        </main>
      )
    }

    return this.props.children
  }
}

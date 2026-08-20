import { AlertCircle, CheckCircle2, Info, TriangleAlert, X } from 'lucide-react'
import { useEffect, useState } from 'react'
import { createPortal } from 'react-dom'
import { IMS_TOAST_EVENT } from './toast'
import './ToastViewport.css'

const toastIcons = {
  success: CheckCircle2,
  error: AlertCircle,
  warning: TriangleAlert,
  info: Info,
}

export default function ToastViewport() {
  const [toasts, setToasts] = useState([])

  useEffect(() => {
    function handleToast(event) {
      const nextToast = event.detail

      setToasts((currentValue) => [nextToast, ...currentValue].slice(0, 5))

      window.setTimeout(() => {
        setToasts((currentValue) =>
          currentValue.filter((toast) => toast.id !== nextToast.id),
        )
      }, nextToast.duration ?? 3600)
    }

    window.addEventListener(IMS_TOAST_EVENT, handleToast)

    return () => {
      window.removeEventListener(IMS_TOAST_EVENT, handleToast)
    }
  }, [])

  if (toasts.length === 0) {
    return null
  }

  return createPortal(
    <div className="toast-viewport" aria-live="polite" aria-atomic="true">
      {toasts.map((toast) => {
        const Icon = toastIcons[toast.type] ?? toastIcons.info

        return (
          <div
            key={toast.id}
            className={`toast-viewport__item toast-viewport__item--${toast.type}`}
          >
            <div className="toast-viewport__icon">
              <Icon size={18} />
            </div>

            <div className="toast-viewport__body">
              <p className="toast-viewport__title">
                {toast.title || (toast.type === 'success' ? 'Success' : toast.type === 'error' ? 'Error' : toast.type === 'warning' ? 'Warning' : 'Notification')}
              </p>
              <p className="toast-viewport__message">{toast.message}</p>
              {toast.action?.label && typeof toast.action?.onClick === 'function' ? (
                <button
                  type="button"
                  className="toast-viewport__action"
                  onClick={() => {
                    toast.action.onClick()
                    setToasts((currentValue) =>
                      currentValue.filter((item) => item.id !== toast.id),
                    )
                  }}
                >
                  {toast.action.label}
                </button>
              ) : null}
            </div>

            <button
              type="button"
              className="toast-viewport__close"
              onClick={() =>
                setToasts((currentValue) =>
                  currentValue.filter((item) => item.id !== toast.id),
                )
              }
              aria-label="Dismiss notification"
            >
              <X size={16} />
            </button>
          </div>
        )
      })}
    </div>,
    document.body
  )
}

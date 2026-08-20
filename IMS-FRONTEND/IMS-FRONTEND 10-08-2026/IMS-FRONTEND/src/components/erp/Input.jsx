import './ERPComponents.css'

export default function Input({ className = '', ...props }) {
  return <input className={`erp-input ${className}`.trim()} {...props} />
}

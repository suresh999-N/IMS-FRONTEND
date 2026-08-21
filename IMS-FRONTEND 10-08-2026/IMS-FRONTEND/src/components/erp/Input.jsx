import './ERPComponents.css'
import { shouldAutoCapitalizeField } from '../../validators/nameValidator'

export default function Input({ className = '', name, type, autoCapitalize, ...props }) {
  const computedAutoCap = autoCapitalize ?? (shouldAutoCapitalizeField(name, type) ? 'words' : undefined)
  return <input className={`erp-input ${className}`.trim()} name={name} type={type} autoCapitalize={computedAutoCap} {...props} />
}

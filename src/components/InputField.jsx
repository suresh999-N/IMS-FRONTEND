export default function InputField({
  id,
  label,
  icon: Icon,
  prefix,
  type = 'text',
  name,
  value,
  onChange,
  onBlur,
  placeholder,
  error,
  helperText,
  textarea,
  rows = 4,
  className = '',
  trailingAction,
  onIconClick,
  iconLabel,
  ...props
}) {
  const isTextarea = Boolean(textarea)
  const describedBy = [
    helperText ? `${id}-help` : '',
    error ? `${id}-error` : '',
  ].filter(Boolean).join(' ') || undefined

  return (
    <div className={`field ${error ? 'field--error' : ''} ${className}`.trim()}>
      <label htmlFor={id}>{label}</label>
      <div
        className={`input-with-icon ${isTextarea ? 'input-with-icon--textarea' : ''} ${
          error ? 'field--error' : ''
        }`.trim()}
      >
        {Icon ? (
          onIconClick ? (
            <button
              type="button"
              className="input-icon-button"
              onClick={onIconClick}
              aria-label={iconLabel || `${label || 'Field'} action`}
            >
              <Icon size={18} />
            </button>
          ) : (
            <Icon size={18} />
          )
        ) : null}
        {prefix ? <span className="input-prefix">{prefix}</span> : null}
        {textarea ? (
          <textarea
            id={id}
            name={name}
            value={value}
            rows={rows}
            placeholder={placeholder}
            onChange={onChange}
            onBlur={onBlur}
            aria-invalid={Boolean(error)}
            aria-describedby={describedBy}
            autoComplete="off"
            {...props}
          />
        ) : (
          <input
            id={id}
            name={name}
            type={type}
            value={value}
            placeholder={placeholder}
            onChange={onChange}
            onBlur={onBlur}
            aria-invalid={Boolean(error)}
            aria-describedby={describedBy}
            autoComplete="off"
            {...props}
          />
        )}
        {!textarea && trailingAction ? trailingAction : null}
      </div>
      {helperText && !error ? (
        <span id={`${id}-help`} className="field-help">
          {helperText}
        </span>
      ) : null}
      {error ? (
        <span id={`${id}-error`} className="field-error" role="alert">
          {error}
        </span>
      ) : null}
    </div>
  )
}

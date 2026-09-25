import React from 'react'

export function renderFormLabel(label, required = false) {
  if (!label) return label
  const labelText = typeof label === 'string' ? label : String(label)
  const isRequired = Boolean(required || labelText.includes('*'))
  if (!isRequired) {
    return label
  }

  const cleanText = labelText.replace(/\s*\*+$/, '')
  if (cleanText.includes('(Today)')) {
    const [before, after] = cleanText.split('(Today)')
    return (
      <>
        {before}
        <span
          className="label-tag-today"
          style={{
            fontSize: '11px',
            color: '#047857',
            backgroundColor: '#ecfdf5',
            padding: '1px 6px',
            borderRadius: '4px',
            fontWeight: 600,
            border: '1px solid #a7f3d0',
            verticalAlign: 'middle',
            marginRight: '2px',
          }}
        >
          Today
        </span>
        {after}
        <span className="required-asterisk" style={{ color: '#dc2626', fontWeight: 'bold', marginLeft: '2px' }}>
          *
        </span>
      </>
    )
  }

  return (
    <>
      {cleanText}
      <span className="required-asterisk" style={{ color: '#dc2626', fontWeight: 'bold', marginLeft: '2px' }}>
        *
      </span>
    </>
  )
}

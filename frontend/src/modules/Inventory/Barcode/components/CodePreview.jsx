import { getBarcodeBars, getQrCells } from '../utils/preview'

function BarcodeVisual({ value, compact = false }) {
  const bars = getBarcodeBars(value)

  return (
    <div
      className={`barcode-visual ${compact ? 'barcode-visual--compact' : ''}`.trim()}
      aria-label={`Barcode preview for ${value}`}
    >
      <div className="barcode-visual__bars">
        {bars.map((bar) => (
          <span
            key={bar.key}
            className="barcode-visual__bar"
            style={{
              width: compact ? `${Math.max(2, Math.round(bar.width * 1.5))}px` : `${bar.width}px`,
              height: compact ? `${Math.max(18, Math.round(bar.height * 0.45))}px` : `${bar.height}px`,
            }}
          />
        ))}
      </div>
      {!compact && <span className="barcode-visual__label">{value}</span>}
    </div>
  )
}

function QrVisual({ value, compact = false }) {
  const cells = getQrCells(value)

  return (
    <div
      className={`qr-visual ${compact ? 'qr-visual--compact' : ''}`.trim()}
      aria-label={`QR preview for ${value}`}
    >
      <div className="qr-visual__grid">
        {cells.map((cell) => (
          <span
            key={cell.key}
            className={`qr-visual__cell ${cell.dark ? 'is-dark' : ''}`}
          />
        ))}
      </div>
      {!compact && <span className="qr-visual__label">{value}</span>}
    </div>
  )
}

export default function CodePreview({ codeType, value, compact = false }) {
  if (codeType === 'QR Code') {
    return <QrVisual value={value} compact={compact} />
  }

  return <BarcodeVisual value={value} compact={compact} />
}
